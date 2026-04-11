export const ROLES = {
  MANAGER: "Manager",
  ADMIN: "Admin",
  SALE: "Sale",
} as const;

export type Role = (typeof ROLES)[keyof typeof ROLES];