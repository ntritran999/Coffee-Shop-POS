import "dotenv/config";
import { ApolloServer } from "@apollo/server";
import { expressMiddleware } from "@as-integrations/express5";
import app from "./app.js";
import { typeDefs } from "./graphql/typeDefs.js";
import { resolvers } from "./graphql/resolvers.js";

const port = Number(process.env.PORT) || 5000;

async function bootstrap() {
  const server = new ApolloServer({
    typeDefs,
    resolvers,
  });

  await server.start();

  app.use("/graphql", expressMiddleware(server));

  app.listen(port, () => {
    console.log(`Server is listening on http://localhost:${port}/graphql`);
  });
}

bootstrap();
