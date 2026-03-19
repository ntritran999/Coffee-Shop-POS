import express, { type NextFunction, type Request, type Response } from "express";
import cors from "cors";

import healthRouter from "./routes/health.route.js";

const app = express();

app.use(cors());
app.use(express.json());

app.use("/api/health", healthRouter);

app.use((error: Error & { statusCode?: number }, _request: Request, response: Response, _next: NextFunction) => {
  console.error(error);

  response.status(error.statusCode || 500).json({
    message: error.message || "Internal server error.",
  });
});

export default app;
