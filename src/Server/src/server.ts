import "dotenv/config";
import { ApolloServer } from "@apollo/server";
import { expressMiddleware } from "@as-integrations/express5";
import jwt from "jsonwebtoken";
import app from "./app.js";
import { typeDefs } from "./graphql/typeDefs.js";
import { resolvers } from "./graphql/resolvers.js";

const port = Number(process.env.PORT) || 5000;
const secretKey = process.env.SECRET_KEY || "";

async function bootstrap() {
  const server = new ApolloServer({
    typeDefs,
    resolvers,
  });

  await server.start();

  app.use("/graphql", expressMiddleware(server, {
    context: async ({ req }) => {
      const authHeader = req.headers.authorization || '';
      const token = authHeader.startsWith('Bearer ')
        ? authHeader.split(' ')[1]
        : '';

      try {
        const decoded = jwt.verify(token, secretKey);
        return { account: decoded };
      } catch (err) {
        return { account: null };
      }
    }
  }));

  app.listen(port, () => {
    console.log(`Server is listening on http://localhost:${port}/graphql`);
  });
}

bootstrap();
