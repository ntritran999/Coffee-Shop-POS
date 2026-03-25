import { GraphQLError } from "graphql";

export function requireAuth(context: any) {
    if (!context.account) {
        throw new GraphQLError("Authentication required");
    }
    return context.account;
}

export function requireRole(context: any, requiredRole: string) {
    const account = requireAuth(context);
    if (account.Role !== requiredRole) {
        throw new GraphQLError(`Access denied. Required role: ${requiredRole}`);
    }
    return account;
}