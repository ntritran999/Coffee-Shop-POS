import prisma from "../db/prisma.js";
import { GraphQLError } from "graphql";
import * as ProductValidator from "../validators/product.validator.js";

type CreateProductData = {
  Name: string;
  Price: number;
  Unit: number;
  CategoryID: number;
  Image?: string | null;
};

type UpdateProductData = {
  Name?: string | null;
  Price?: number | null;
  Unit?: number | null;
  CategoryID?: number | null;
  Image?: string | null;
};

async function ensureProductExists(ProductID: number) {
  const product = await prisma.product.findUnique({
    where: { ProductID }
  });

  if (!product) {
    throw new GraphQLError("Product not found", {
      extensions: { code: "NOT_FOUND" }
    });
  }

  return product;
}

async function ensureCategoryExists(CategoryID: number) {
  const category = await prisma.category.findUnique({
    where: { CategoryID }
  });

  if (!category) {
    throw new GraphQLError("Category not found", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }

  return category;
}

export async function getProducts(CategoryID?: number | null, Name?: string | null) {
  const normalizedCategoryId = CategoryID === null ? undefined : CategoryID;
  ProductValidator.validateOptionalCategoryId(normalizedCategoryId);

  const normalizedName = Name?.trim();
  const where: Record<string, unknown> = {};

  if (normalizedCategoryId !== undefined) {
    where.CategoryID = normalizedCategoryId;
  }

  if (normalizedName) {
    where.Name = {
      contains: normalizedName,
      mode: "insensitive"
    };
  }

  return prisma.product.findMany({
    where,
    orderBy: {
      ProductID: "asc"
    }
  });
}

export async function getProductById(ProductID: number) {
  ProductValidator.validateProductId(ProductID);
  return prisma.product.findUnique({
    where: { ProductID }
  });
}

export async function createProduct(data: CreateProductData) {
  ProductValidator.validateCreateProduct(data.Name, data.Price, data.Unit, data.CategoryID, data.Image);
  await ensureCategoryExists(data.CategoryID);

  const normalizedImage =
    data.Image === undefined || data.Image === null
      ? null
      : data.Image.trim();

  return prisma.product.create({
    data: {
      Name: data.Name.trim(),
      Price: data.Price,
      Unit: data.Unit,
      CategoryID: data.CategoryID,
      Image: normalizedImage
    }
  });
}

export async function updateProduct(ProductID: number, data: UpdateProductData) {
  ProductValidator.validateUpdateProduct(ProductID, data);
  await ensureProductExists(ProductID);

  if (data.CategoryID !== undefined && data.CategoryID !== null) {
    await ensureCategoryExists(data.CategoryID);
  }

  const updateData: Record<string, unknown> = {};

  if (data.Name !== undefined) {
    updateData.Name = data.Name!.trim();
  }

  if (data.Price !== undefined) {
    updateData.Price = data.Price;
  }

  if (data.Unit !== undefined) {
    updateData.Unit = data.Unit;
  }

  if (data.CategoryID !== undefined) {
    updateData.CategoryID = data.CategoryID as number;
  }

  if (data.Image !== undefined) {
    updateData.Image = data.Image === null ? null : data.Image.trim();
  }

  return prisma.product.update({
    where: { ProductID },
    data: updateData
  });
}

export async function deleteProduct(ProductID: number) {
  ProductValidator.validateProductId(ProductID);
  await ensureProductExists(ProductID);

  const billInfoCount = await prisma.billInfo.count({
    where: { ProductID }
  });

  if (billInfoCount > 0) {
    throw new GraphQLError("Cannot delete product that is used in bills", {
      extensions: { code: "BAD_USER_INPUT" }
    });
  }

  await prisma.product.delete({
    where: { ProductID }
  });

  return { success: true };
}
