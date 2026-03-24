import { checkHealth } from "../services/health.service.js";

export const resolvers = {
  Query: {
    health: async () => {
      return await checkHealth();
    },
  },
};
