import prisma from "../db/prisma.js";

export async function getCategories() {
  return prisma.category.findMany({
    orderBy: {
      CategoryID: "asc"
    }
  });
}
