import { checkHealth } from "../services/health.service.js";
import * as AuthMiddleware from "../middlewares/auth.middleware.js";
import * as AuthService from "../services/auth.service.js";
import * as AccountService from "../services/account.service.js";
import * as CategoryService from "../services/category.service.js";
import * as ProductService from "../services/product.service.js";
import * as TableService from "../services/table.service.js";
import * as BillService from "../services/bill.service.js";

export const resolvers = {
  Bill: {
    DateCheckIn: (parent: any) => {
      return new Date(parent.DateCheckIn).toISOString();
    },
    DateCheckOut: (parent: any) => {
      return parent.DateCheckOut ? new Date(parent.DateCheckOut).toISOString() : null;
    }
  },
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
    },
    categories: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireAuth(context);
      return await CategoryService.getCategories();
    },
    products: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireAuth(context);
      return await ProductService.getProducts(args.CategoryID, args.Name);
    },
    product: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireAuth(context);
      return await ProductService.getProductById(args.ProductID);
    },
    tables: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireAuth(context);
      return await TableService.getTables(args.Status);
    },
    table: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireAuth(context);
      return await TableService.getTableById(args.TableID);
    },
    bills: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireAuth(context);
      return await BillService.getBills(args.Status, args.TableID);
    },
    bill: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireAuth(context);
      return await BillService.getBillById(args.BillID);
    },
    billInfo: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireAuth(context);
      return await BillService.getBillInfoByBillId(args.BillID);
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
      return await AccountService.updateAccount(args.Username, args.updataData.Password, args.updataData.DisplayName, args.updataData.Role);
    },
    deleteAccount: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireRole(context, "Manager");
      return await AccountService.deleteAccount(args.Username);
    },
    createProduct: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireRole(context, "Manager");
      return await ProductService.createProduct(args.data);
    },
    updateProduct: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireRole(context, "Manager");
      return await ProductService.updateProduct(args.ProductID, args.data ?? {});
    },
    deleteProduct: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireRole(context, "Manager");
      return await ProductService.deleteProduct(args.ProductID);
    },
    createTable: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireRole(context, "Manager");
      return await TableService.createTable(args.data.TableName, args.data.Status);
    },
    updateTable: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireRole(context, "Manager");
      return await TableService.updateTable(args.TableID, args.data ?? {});
    },
    deleteTable: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireRole(context, "Manager");
      return await TableService.deleteTable(args.TableID);
    },
    createBill: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireAuth(context);
      return await BillService.createBill(args.data?.TableID, args.data?.Discount);
    },
    updateBill: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireAuth(context);
      return await BillService.updateBill(args.BillID, args.data ?? {});
    },
    deleteBill: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireAuth(context);
      return await BillService.deleteBill(args.BillID);
    },
    addBillItem: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireAuth(context);
      return await BillService.addBillItem(
        args.data.BillID,
        args.data.ProductID,
        args.data.Count,
        args.data.Price,
        args.data.Note
      );
    },
    updateBillItem: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireAuth(context);
      return await BillService.updateBillItem(args.BillInfoID, args.data);
    },
    deleteBillItem: async (parent: any, args: any, context: any, info: any) => {
      AuthMiddleware.requireAuth(context);
      return await BillService.deleteBillItem(args.BillInfoID);
    }
  }
};
