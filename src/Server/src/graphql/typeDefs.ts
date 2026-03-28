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

  type BillInfo {
    BillInfoID: Int!
    BillID: Int!
    ProductID: Int!
    Count: Int!
    Price: Float!
    Note: String
  }

  type Bill {
    BillID: Int!
    DateCheckIn: String!
    DateCheckOut: String
    TableID: Int
    Status: Int!
    TotalAmount: Float!
    Discount: Float!
    BillInfo: [BillInfo!]!
  }

  type Query {
    health: HealthResponse!
    currentAccount: Account
    accounts: [Account!]
    account(Username: String!): Account
    bills(Status: Int, TableID: Int): [Bill!]!
    bill(BillID: Int!): Bill
    billInfo(BillID: Int!): [BillInfo!]!
  }

  input UpdateAccountInput {
    DisplayName: String
    Role: String
    Password: String
  }

  input CreateBillInput {
    TableID: Int
    Discount: Float
  }

  input AddBillItemInput {
    BillID: Int!
    ProductID: Int!
    Count: Int!
    Price: Float
    Note: String
  }

  input UpdateBillInput {
    TableID: Int
    Discount: Float
    Status: Int
    DateCheckOut: String
  }

  input UpdateBillItemInput {
    Count: Int
    Price: Float
    Note: String
  }

  type DeleteResponse {
    success: Boolean!
  }

  type Mutation {
    login(Username: String!, Password: String!): AuthPayload!
    createAccount(Username: String!, Password: String!, DisplayName: String!, Role: String!): Account!
    updateAccount(Username: String!, updataData: UpdateAccountInput): Account!
    deleteAccount(Username: String!): DeleteResponse!
    createBill(data: CreateBillInput): Bill!
    updateBill(BillID: Int!, data: UpdateBillInput): Bill!
    deleteBill(BillID: Int!): DeleteResponse!
    addBillItem(data: AddBillItemInput!): BillInfo!
    updateBillItem(BillInfoID: Int!, data: UpdateBillItemInput!): BillInfo!
    deleteBillItem(BillInfoID: Int!): DeleteResponse!
  }
`;
