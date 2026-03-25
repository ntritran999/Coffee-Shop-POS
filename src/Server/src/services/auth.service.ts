import "dotenv/config";
import prisma from "../db/prisma";
import { GraphQLError } from "graphql";
import jwt from "jsonwebtoken";
import * as AccountValidator from "../validators/account.validator";

const secretKey = process.env.SECRET_KEY || "";

export async function login(Username: string, Password: string) {
  AccountValidator.validateLogin(Username, Password);

  const account = await prisma.account.findFirst({
    where: { Username }
  });

  if (!account) throw new GraphQLError("Username does not exist");
  if (Password !== account.Password) throw new GraphQLError("Password is incorrect");

  const token = jwt.sign(
    {
      Username: account.Username,
      Role: account.Role,
      DisplayName: account.DisplayName
    },
    secretKey,
    { expiresIn: "1h" }
  );

  return {
    token,
    account: {
      Username: account.Username,
      DisplayName: account.DisplayName,
      Role: account.Role
    }
  };
}