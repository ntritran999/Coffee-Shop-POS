export const typeDefs = `#graphql
  type HealthResponse {
    status: String!
    database: String!
  }

  type Query {
    health: HealthResponse!
  }
`;
