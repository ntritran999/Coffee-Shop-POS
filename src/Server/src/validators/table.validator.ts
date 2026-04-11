import { GraphQLError } from "graphql";

type UpdateTableData = {
  TableName?: string | null;
  Status?: number | null;
};

export function validateTableId(TableID: number) {
  if (!Number.isInteger(TableID) || TableID <= 0) {
    throw new GraphQLError("TableID must be a positive integer", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}

export function validateOptionalTableName(TableName: string | null | undefined) {
  if (TableName !== undefined && (TableName === null || TableName.trim().length === 0)) {
    throw new GraphQLError("TableName is required", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}

export function validateOptionalStatus(Status: number | undefined) {
  if (Status !== undefined && ![0, 1, 2].includes(Status)) {
    throw new GraphQLError("Status must be 0, 1, or 2", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}

export function validateCreateTable(TableName: string, Status?: number | null) {
  validateOptionalTableName(TableName);
  validateOptionalStatus(Status ?? undefined);
}

export function validateUpdateTable(TableID: number, data: UpdateTableData) {
  validateTableId(TableID);

  if (data.TableName === undefined && data.Status === undefined) {
    throw new GraphQLError("At least one field must be provided to update table", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }

  if (data.Status === null) {
    throw new GraphQLError("Status cannot be null", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }

  validateOptionalTableName(data.TableName);
  validateOptionalStatus(data.Status);
}
