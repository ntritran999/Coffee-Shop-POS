import { checkHealth } from "../services/health.service.js";
import * as AuthService from "../services/auth.service.js";

export const resolvers = {
  Query: {
    health: async () => {
      return await checkHealth();
    },
  },
  Mutation: {
    login: async (parent: any, args: any, context: any, info: any) => {
      return await AuthService.login(args.Username, args.Password);
    }
  }
};
