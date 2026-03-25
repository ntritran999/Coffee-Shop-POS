import { checkHealth } from "../services/health.service.js";
import * as AuthService from "../services/auth.service.js";
import * as AuthMiddleware from "../middlewares/auth.middleware.js";

export const resolvers = {
  Query: {
    health: async () => {
      return await checkHealth();
    },
    currentAccount: async (parent: any, args: any, context: any, info: any) => {
      const account = AuthMiddleware.requireAuth(context);
      return account;
    }
  },
  Mutation: {
    login: async (parent: any, args: any, context: any, info: any) => {
      return await AuthService.login(args.Username, args.Password);
    }
  }
};
