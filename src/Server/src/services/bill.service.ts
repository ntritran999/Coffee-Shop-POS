import prisma from "../db/prisma.js";
import { GraphQLError } from "graphql";
import * as BillValidator from "../validators/bill.validator.js";

const billInclude = {
  BillInfo: {
    orderBy: {
      BillInfoID: "asc" as const
    }
  }
};

async function recalculateBillTotal(tx: Pick<typeof prisma, "bill">, BillID: number) {
  const bill = await tx.bill.findUnique({
    where: { BillID },
    include: billInclude
  });

  if (!bill) {
    throw new GraphQLError("Bill not found", {
      extensions: { code: "NOT_FOUND" }
    });
  }

  const subTotal = bill.BillInfo.reduce((sum, item) => sum + item.Price * item.Count, 0);
  const totalAmount = Math.max(subTotal - bill.Discount, 0);

  return tx.bill.update({
    where: { BillID },
    data: { TotalAmount: totalAmount },
    include: billInclude
  });
}

async function ensureBillExists(BillID: number) {
  const bill = await prisma.bill.findUnique({
    where: { BillID },
    include: billInclude
  });

  if (!bill) {
    throw new GraphQLError("Bill not found", {
      extensions: { code: "NOT_FOUND" }
    });
  }

  return bill;
}

export async function getBills(Status?: number, TableID?: number) {
  if (Status !== undefined) {
    BillValidator.validateOptionalStatus(Status);
  }

  if (TableID !== undefined) {
    BillValidator.validateOptionalTableId(TableID);
  }

  return prisma.bill.findMany({
    where: {
      ...(Status !== undefined ? { Status } : {}),
      ...(TableID !== undefined ? { TableID } : {})
    },
    include: billInclude,
    orderBy: {
      BillID: "desc"
    }
  });
}

export async function getBillById(BillID: number) {
  BillValidator.validateBillId(BillID);
  return prisma.bill.findUnique({
    where: { BillID },
    include: billInclude
  });
}

export async function getBillInfoByBillId(BillID: number) {
  BillValidator.validateBillId(BillID);
  await ensureBillExists(BillID);

  return prisma.billInfo.findMany({
    where: { BillID },
    orderBy: {
      BillInfoID: "asc"
    }
  });
}

export async function createBill(TableID?: number | null, Discount?: number | null) {
  BillValidator.validateCreateBill(TableID, Discount);

  const bill = await prisma.bill.create({
    data: {
      TableID: TableID ?? null,
      Discount: Discount ?? 0,
      Status: 0
    },
    include: billInclude
  });

  return recalculateBillTotal(prisma, bill.BillID);
}

export async function updateBill(
  BillID: number,
  data: {
    TableID?: number | null;
    Discount?: number | null;
    Status?: number | null;
    DateCheckOut?: string | null;
  }
) {
  BillValidator.validateUpdateBill(BillID, data.TableID, data.Discount, data.Status, data.DateCheckOut);
  await ensureBillExists(BillID);

  const updateData: Record<string, unknown> = {};

  if (data.TableID !== undefined) {
    updateData.TableID = data.TableID;
  }

  if (data.Discount !== undefined) {
    updateData.Discount = data.Discount ?? 0;
  }

  if (data.Status !== undefined) {
    updateData.Status = data.Status;
  }

  if (data.DateCheckOut !== undefined) {
    updateData.DateCheckOut = data.DateCheckOut ? new Date(data.DateCheckOut) : null;
  }

  await prisma.bill.update({
    where: { BillID },
    data: updateData
  });

  return recalculateBillTotal(prisma, BillID);
}

export async function deleteBill(BillID: number) {
  BillValidator.validateBillId(BillID);

  try {
    await prisma.bill.delete({
      where: { BillID }
    });

    return { success: true };
  } catch {
    throw new GraphQLError("Bill not found or delete failed", {
      extensions: { code: "NOT_FOUND" }
    });
  }
}

export async function addBillItem(
  BillID: number,
  ProductID: number,
  Count: number,
  Price?: number | null,
  Note?: string | null
) {
  BillValidator.validateCreateBillItem(BillID, ProductID, Count, Price);

  const bill = await ensureBillExists(BillID);

  const product = await prisma.product.findUnique({
    where: { ProductID }
  });

  if (!product) {
    throw new GraphQLError("Product not found", {
      extensions: { code: "NOT_FOUND" }
    });
  }

  const billItem = await prisma.$transaction(async (tx) => {
    const created = await tx.billInfo.create({
      data: {
        BillID: bill.BillID,
        ProductID,
        Count,
        Price: Price ?? product.Price,
        Note: Note ?? null
      }
    });

    await recalculateBillTotal(tx, BillID);
    return created;
  });

  return billItem;
}

export async function updateBillItem(
  BillInfoID: number,
  data: {
    Count?: number | null;
    Price?: number | null;
    Note?: string | null;
  }
) {
  BillValidator.validateUpdateBillItem(BillInfoID, data.Count, data.Price);

  const billItem = await prisma.billInfo.findUnique({
    where: { BillInfoID }
  });

  if (!billItem) {
    throw new GraphQLError("Bill item not found", {
      extensions: { code: "NOT_FOUND" }
    });
  }

  return prisma.$transaction(async (tx) => {
    const updateData: Record<string, unknown> = {};

    if (data.Count !== undefined) {
      updateData.Count = data.Count;
    }

    if (data.Price !== undefined) {
      updateData.Price = data.Price;
    }

    if (data.Note !== undefined) {
      updateData.Note = data.Note;
    }

    const updated = await tx.billInfo.update({
      where: { BillInfoID },
      data: updateData
    });

    await recalculateBillTotal(tx, billItem.BillID);
    return updated;
  });
}

export async function deleteBillItem(BillInfoID: number) {
  BillValidator.validateBillInfoId(BillInfoID);

  const billItem = await prisma.billInfo.findUnique({
    where: { BillInfoID }
  });

  if (!billItem) {
    throw new GraphQLError("Bill item not found", {
      extensions: { code: "NOT_FOUND" }
    });
  }

  await prisma.$transaction(async (tx) => {
    await tx.billInfo.delete({
      where: { BillInfoID }
    });

    await recalculateBillTotal(tx, billItem.BillID);
  });

  return { success: true };
}
