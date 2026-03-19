import type { NextFunction, Request, Response } from "express";

import { checkHealth } from "../services/health.service.js";

export async function getHealth(_request: Request, response: Response, next: NextFunction): Promise<void> {
  try {
    const health = await checkHealth();

    response.json(health);
  } catch (error) {
    next(error);
  }
}
