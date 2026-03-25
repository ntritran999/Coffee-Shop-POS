import prisma from "../db/prisma";
import { GraphQLError } from "graphql";
import * as AccountValidator from "../validators/account.validator";

export async function createAccount(Username: string, Password: string, DisplayName: string, Role: string) {
	AccountValidator.validateCreateAccount(Username, Password, DisplayName, Role);

	const existing = await prisma.account.findUnique({ where: { Username } });
	if (existing) {
		throw new GraphQLError("Username already exists", { extensions: { code: "BAD_USER_INPUT" } });
	}

	const newAccount = await prisma.account.create({
		data: {
			Username,
			Password,
			DisplayName,
			Role
		}
	});

	return {
		Username: newAccount.Username,
		DisplayName: newAccount.DisplayName,
		Role: newAccount.Role
	};
}