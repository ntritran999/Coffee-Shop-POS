import prisma from "../db/prisma.js";

export async function getCategories() {
  return prisma.category.findMany({
    orderBy: {
      CategoryID: "asc"
    }
  });
}

export async function createCategory(categoryName: string) {
  return prisma.category.create({
    data: {
      CategoryName: categoryName
    }
  });
}
