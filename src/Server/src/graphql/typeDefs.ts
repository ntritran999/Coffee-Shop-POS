export const typeDefs = `#graphql
  type HealthResponse {
    status: String!
    database: String!
  }

  type Account {
    Username: String!
    Password: String
    DisplayName: String!
    Role: String!
  }

  type AuthPayload {
    token: String!
    account: Account!
  }

  type Query {
    health: HealthResponse!
    currentAccount: Account

  }

  type Mutation {
    login(Username: String!, Password: String!): AuthPayload!
    createAccount(Username: String!, Password: String!, DisplayName: String!, Role: String!): Account!
  }
`;
