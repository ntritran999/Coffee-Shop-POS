export const typeDefs = `#graphql
  type HealthResponse {
    status: String!
    database: String!
  }

  type Account {
    Username: String!
    DisplayName: String!
    Password: String
    Role: String!
  }

  type AuthPayload {
    token: String!
    account: Account!
  }

  type Query {
    health: HealthResponse!
    currentAccount: Account
    accounts: [Account!]
    account(Username: String!): Account
  }

  input UpdateAccountInput {
    DisplayName: String
    Role: String
    Password: String
  }

  type DeleteResponse {
    success: Boolean!
  }

  type Mutation {
    login(Username: String!, Password: String!): AuthPayload!
    createAccount(Username: String!, Password: String!, DisplayName: String!, Role: String!): Account!
    updateAccount(Username: String!, updataData: UpdateAccountInput): Account!
    deleteAccount(Username: String!): DeleteResponse!
  }
`;
