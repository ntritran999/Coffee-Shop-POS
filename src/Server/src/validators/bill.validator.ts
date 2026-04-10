import { GraphQLError } from "graphql";

export function validateBillId(BillID: number) {
  if (!Number.isInteger(BillID) || BillID <= 0) {
    throw new GraphQLError("BillID must be a positive integer", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}

export function validateBillInfoId(BillInfoID: number) {
  if (!Number.isInteger(BillInfoID) || BillInfoID <= 0) {
    throw new GraphQLError("BillInfoID must be a positive integer", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}

export function validateOptionalTableId(TableID: number | null | undefined) {
  if (TableID !== undefined && TableID !== null && (!Number.isInteger(TableID) || TableID <= 0)) {
    throw new GraphQLError("TableID must be a positive integer", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}

export function validateOptionalDiscount(Discount: number | null | undefined) {
  if (Discount !== undefined && Discount !== null && Discount < 0) {
    throw new GraphQLError("Discount must be greater than or equal to 0", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}

export function validateOptionalStatus(Status: number | null | undefined) {
  if (Status !== undefined && Status !== null && ![0, 1].includes(Status)) {
    throw new GraphQLError("Status must be 0 or 1", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}

export function validateCreateBill(TableID?: number | null, Discount?: number | null) {
  validateOptionalTableId(TableID);
  validateOptionalDiscount(Discount);
}

export function validateUpdateBill(
  BillID: number,
  TableID?: number | null,
  Discount?: number | null,
  Status?: number | null,
  DateCheckOut?: string | null
) {
  validateBillId(BillID);
  validateOptionalTableId(TableID);
  validateOptionalDiscount(Discount);
  validateOptionalStatus(Status);

  if (DateCheckOut !== undefined && DateCheckOut !== null && Number.isNaN(Date.parse(DateCheckOut))) {
    throw new GraphQLError("DateCheckOut must be a valid ISO date string", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}

export function validateCreateBillItem(BillID: number, ProductID: number, Count: number, Price?: number | null) {
  validateBillId(BillID);

  if (!Number.isInteger(ProductID) || ProductID <= 0) {
    throw new GraphQLError("ProductID must be a positive integer", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }

  if (!Number.isInteger(Count) || Count <= 0) {
    throw new GraphQLError("Count must be a positive integer", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }

  if (Price !== undefined && Price !== null && Price < 0) {
    throw new GraphQLError("Price must be greater than or equal to 0", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}

export function validateUpdateBillItem(BillInfoID: number, Count?: number | null, Price?: number | null) {
  validateBillInfoId(BillInfoID);

  if (Count !== undefined && Count !== null && (!Number.isInteger(Count) || Count <= 0)) {
    throw new GraphQLError("Count must be a positive integer", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }

  if (Price !== undefined && Price !== null && Price < 0) {
    throw new GraphQLError("Price must be greater than or equal to 0", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }

  if (Count === undefined && Price === undefined) {
    throw new GraphQLError("At least one field must be provided to update bill item", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}
