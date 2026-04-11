import prisma from "../db/prisma.js";
import { GraphQLError } from "graphql";
import * as TableValidator from "../validators/table.validator.js";

type UpdateTableData = {
  TableName?: string | null;
  Status?: number | null;
};

async function ensureTableExists(TableID: number) {
  const table = await prisma.table.findUnique({
    where: { TableID }
  });

  if (!table) {
    throw new GraphQLError("Table not found", {
      extensions: { code: "NOT_FOUND" }
    });
  }

  return table;
}

export async function getTables(Status?: number | null) {
  const normalizedStatus = Status === null ? undefined : Status;
  TableValidator.validateOptionalStatus(normalizedStatus);

  return prisma.table.findMany({
    where: normalizedStatus !== undefined ? { Status: normalizedStatus } : {},
    orderBy: {
      TableID: "asc"
    }
  });
}

export async function getTableById(TableID: number) {
  TableValidator.validateTableId(TableID);
  return prisma.table.findUnique({
    where: { TableID }
  });
}

export async function createTable(TableName: string, Status?: number | null) {
  const normalizedStatus = Status === null ? undefined : Status;
  TableValidator.validateCreateTable(TableName, normalizedStatus);

  return prisma.table.create({
    data: {
      TableName: TableName.trim(),
      Status: normalizedStatus ?? 0
    }
  });
}

export async function updateTable(TableID: number, data: UpdateTableData) {
  TableValidator.validateUpdateTable(TableID, data);
  await ensureTableExists(TableID);

  const updateData: Record<string, unknown> = {};

  if (data.TableName !== undefined) {
    updateData.TableName = data.TableName!.trim();
  }

  if (data.Status !== undefined) {
    updateData.Status = data.Status;
  }

  return prisma.table.update({
    where: { TableID },
    data: updateData
  });
}

export async function deleteTable(TableID: number) {
  TableValidator.validateTableId(TableID);
  await ensureTableExists(TableID);

  const billCount = await prisma.bill.count({
    where: { TableID }
  });

  if (billCount > 0) {
    throw new GraphQLError("Cannot delete table that is used in bills", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }

  await prisma.table.delete({
    where: { TableID }
  });

  return { success: true };
}
