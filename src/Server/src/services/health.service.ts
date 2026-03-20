import prisma from "../db/prisma.js";

export async function checkHealth() {
  await prisma.$queryRaw`SELECT 1`;

  return {
    status: "ok",
    database: "connected",
  };
}
