import prisma from "../db/prisma";
import { GraphQLError } from "graphql";
import * as HashUtil from "../utils/hash.util";
import * as AccountValidator from "../validators/account.validator";

export async function createAccount(Username: string, Password: string, DisplayName: string, Role: string) {
	AccountValidator.validateCreateAccount(Username, Password, DisplayName, Role);

	const existing = await prisma.account.findUnique({ where: { Username } });
	if (existing) {
		throw new GraphQLError("Username already exists", { extensions: { code: "BAD_USER_INPUT" } });
	}

	const hashedPassword = await HashUtil.hashPassword(Password);

	const newAccount = await prisma.account.create({
		data: {
			Username,
			Password: hashedPassword,
			DisplayName,
			Role
		}
	});

	return {
		Username: newAccount.Username,
		DisplayName: newAccount.DisplayName,
		Password: newAccount.Password,
		Role: newAccount.Role
	};
}

export async function getAll() {
	const accounts = await prisma.account.findMany({
		select: {
			Username: true,
			DisplayName: true,
			Role: true,
			Password: true
		}
	});

	return accounts;
}

// async function hashPasswordAllUser(accounts: any) {
// 	for (const account of accounts) {
// 		// KIỂM TRA QUAN TRỌNG: 
// 		// Bcrypt hash luôn bắt đầu bằng "$2b$" hoặc "$2a$". 
// 		// Nếu đã có tiền tố này, bỏ qua để không băm đè lên cái đã băm.
// 		const isAlreadyHashed = account.Password.startsWith('$2');

// 		if (!isAlreadyHashed) {
// 			const newHashedPassword = await HashUtil.hashPassword(account.Password);

// 			await prisma.account.update({
// 				where: { Username: account.Username },
// 				data: { Password: newHashedPassword }
// 			});
// 			console.log(`Updated: ${account.Username}`);
// 		}
// 	}
// }

export async function getById(Username: string) {
	const account = await prisma.account.findFirst({
		where: { Username },
		select: {
			Username: true,
			DisplayName: true,
			Role: true
		}
	})

	return account;
}

export async function updateAccount(Username: string, Password: string | undefined, DisplayName: string | undefined, Role: string | undefined) {
	AccountValidator.validateUpdateAccount(Username, Password, DisplayName, Role);

	try {
		const hashedPassword = Password ? await HashUtil.hashPassword(Password) : Password;

		const updatedAccount = await prisma.account.update({
			where: { Username },
			data: {
				Password: hashedPassword,
				DisplayName: DisplayName,
				Role: Role
			}
		})

		return updatedAccount;
	} catch (err) {
		throw new GraphQLError("Account not found or update failed");
	}
}

export async function deleteAccount(Username: string) {
	try {
		const deletedAccount = await prisma.account.delete({
			where: { Username }
		});

		return { success: true };
	} catch (error) {
		throw new GraphQLError("Account not found or delete failed");
	}
}