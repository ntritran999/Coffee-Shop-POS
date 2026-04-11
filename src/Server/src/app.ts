import "dotenv/config";
import express, { type NextFunction, type Request, type Response } from "express";
import cors from "cors";
import uploadRouter from "./routes/upload.route.js";

const app = express();

app.use(cors());
app.use(express.json());

app.use("/api", uploadRouter);

app.use((error: Error & { statusCode?: number }, _request: Request, response: Response, _next: NextFunction) => {
  console.error(error);

  if (error.message === "Only image files (jpg, png, gif, webp) are allowed") {
    response.status(400).json({ error: error.message });
    return;
  }

  if (error.message === "File too large") {
    response.status(413).json({ error: "File size exceeds 5MB limit" });
    return;
  }

  response.status(error.statusCode || 500).json({
    message: error.message || "Internal server error.",
  });
});

export default app;
