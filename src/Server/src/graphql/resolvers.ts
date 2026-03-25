import { checkHealth } from "../services/health.service.js";
import * as AuthMiddleware from "../middlewares/auth.middleware.js";
import * as AuthService from "../services/auth.service.js";
import * as AccountService from "../services/account.service.js";

export const resolvers = {
  Query: {
    health: async () => {
      return await checkHealth();
    },
    currentAccount: async (parent: any, args: any, context: any, info: any) => {
      const account = AuthMiddleware.requireAuth(context);
      return account;
    },
    accounts: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireAuth(context);
      return await AccountService.getAll();
    },
    account: async (parent: any, args: any, context: any, info: any) => {
      return await AccountService.getById(args.Username);
    }
  },
  Mutation: {
    login: async (parent: any, args: any, context: any, info: any) => {
      return await AuthService.login(args.Username, args.Password);
    },
    createAccount: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireRole(context, "Manager"); // use enum in future
      return await AccountService.createAccount(args.Username, args.Password, args.DisplayName, args.Role);
    },
    updateAccount: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireRole(context, "Manager");
      return await AccountService.updateAccount(args.Username, args.Password, args.DisplayName, args.Role);
    }
  }
};
