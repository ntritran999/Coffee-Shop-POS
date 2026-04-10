import { GraphQLError } from "graphql";

type UpdateProductData = {
  Name?: string | null;
  Price?: number | null;
  Unit?: number | null;
  CategoryID?: number | null;
  Image?: string | null;
};

export function validateProductId(ProductID: number) {
  if (!Number.isInteger(ProductID) || ProductID <= 0) {
    throw new GraphQLError("ProductID must be a positive integer", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}

export function validateOptionalCategoryId(CategoryID: number | undefined) {
  if (CategoryID !== undefined && (!Number.isInteger(CategoryID) || CategoryID <= 0)) {
    throw new GraphQLError("CategoryID must be a positive integer", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}

export function validateOptionalName(Name: string | null | undefined) {
  if (Name !== undefined && (Name === null || Name.trim().length === 0)) {
    throw new GraphQLError("Name is required", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}

export function validateOptionalPrice(Price: number | null | undefined) {
  if (
    Price !== undefined &&
    (Price === null || !Number.isFinite(Price) || Price < 0)
  ) {
    throw new GraphQLError("Price must be greater than or equal to 0", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}

export function validateOptionalUnit(Unit: number | null | undefined) {
  if (Unit !== undefined && (Unit === null || !Number.isInteger(Unit) || Unit <= 0)) {
    throw new GraphQLError("Unit must be a positive integer", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}

export function validateOptionalImage(Image: string | null | undefined) {
  if (Image !== undefined && Image !== null && Image.trim().length === 0) {
    throw new GraphQLError("Image cannot be an empty string", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }
}

export function validateCreateProduct(
  Name: string,
  Price: number,
  Unit: number,
  CategoryID: number,
  Image?: string | null
) {
  validateOptionalName(Name);
  validateOptionalPrice(Price);
  validateOptionalUnit(Unit);
  validateOptionalCategoryId(CategoryID);
  validateOptionalImage(Image);
}

export function validateUpdateProduct(ProductID: number, data: UpdateProductData) {
  validateProductId(ProductID);

  if (
    data.Name === undefined &&
    data.Price === undefined &&
    data.Unit === undefined &&
    data.CategoryID === undefined &&
    data.Image === undefined
  ) {
    throw new GraphQLError("At least one field must be provided to update product", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }

  if (data.CategoryID === null) {
    throw new GraphQLError("CategoryID cannot be null", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }

  validateOptionalName(data.Name);
  validateOptionalPrice(data.Price);
  validateOptionalUnit(data.Unit);
  validateOptionalCategoryId(data.CategoryID);
  validateOptionalImage(data.Image);
}
