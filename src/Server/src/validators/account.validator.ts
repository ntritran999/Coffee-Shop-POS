import { GraphQLError } from "graphql";

export function validateCreateAccount(Username: string, Password: string, DisplayName: string, Role: string) {
    if (!Username || Username.trim().length < 3) {
        throw new GraphQLError("Username must be at least 3 characters", { extensions: { code: "BAD_USER_INPUT" } });
    }

    if (!Password || Password.length < 6) {
        throw new GraphQLError("Password must be at least 6 characters", { extensions: { code: "BAD_USER_INPUT" } });
    }

    if (!DisplayName || DisplayName.trim().length === 0) {
        throw new GraphQLError("DisplayName is required", { extensions: { code: "BAD_USER_INPUT" } });
    }

    const allowedRoles = ["Manager", "Cashier"];
    if (!Role || !allowedRoles.includes(Role)) {
        throw new GraphQLError(`Role must be one of: ${allowedRoles.join(", ")}`, { extensions: { code: "BAD_USER_INPUT" } });
    }

}

export function validateLogin(Username: string, Password: string) {
    if (!Username || Username.trim().length < 3) {
        throw new GraphQLError("Username must be at least 3 characters", { extensions: { code: "BAD_USER_INPUT" } });
    }

    if (!Password || Password.length < 6) {
        throw new GraphQLError("Password must be at least 6 characters", { extensions: { code: "BAD_USER_INPUT" } });
    }

}