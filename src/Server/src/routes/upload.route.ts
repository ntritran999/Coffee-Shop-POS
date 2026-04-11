import "dotenv/config";
import { Router, type Request, type Response, type NextFunction } from "express";
import jwt from "jsonwebtoken";
import multer from "multer";
import { uploadImage } from "../services/cloudinary.service.js";
import { ROLES } from "../constants/roles.js";

const router = Router();

const secretKey = process.env.SECRET_KEY || "";

const storage = multer.memoryStorage();
const upload = multer({
  storage,
  limits: {
    fileSize: 5 * 1024 * 1024, // 5MB
  },
  fileFilter: (_req, file, cb) => {
    const allowedTypes = ["image/jpeg", "image/png", "image/gif", "image/webp"];
    if (allowedTypes.includes(file.mimetype)) {
      cb(null, true);
    } else {
      cb(new Error("Only image files (jpg, png, gif, webp) are allowed"));
    }
  },
});

interface AuthRequest extends Request {
  account?: {
    Username: string;
    Role: string;
  };
}

function authenticateToken(req: AuthRequest, res: Response, next: NextFunction) {
  const authHeader = req.headers.authorization;
  const token = authHeader?.startsWith("Bearer ")
    ? authHeader.split(" ")[1]
    : null;

  if (!token) {
    res.status(401).json({ error: "Authentication required" });
    return;
  }

  try {
    const decoded = jwt.verify(token, secretKey) as { Username: string; Role: string };
    req.account = decoded;
    next();
  } catch {
    res.status(401).json({ error: "Invalid token" });
  }
}

function requireRole(req: AuthRequest, res: Response, next: NextFunction) {
  if (!req.account) {
    res.status(401).json({ error: "Authentication required" });
    return;
  }

  const allowedRoles = [ROLES.MANAGER, ROLES.ADMIN];
  if (!allowedRoles.includes(req.account.Role as typeof ROLES.MANAGER)) {
    res.status(403).json({ error: "Access denied. Manager role required" });
    return;
  }

  next();
}

router.post(
  "/upload",
  authenticateToken,
  requireRole,
  upload.single("image"),
  async (req: AuthRequest, res: Response, next: NextFunction) => {
    try {
      if (!req.file) {
        res.status(400).json({ error: "No image file provided" });
        return;
      }

      const fileName = `${Date.now()}-${req.account?.Username || "unknown"}`;
      const result = await uploadImage(req.file.buffer, fileName);

      res.json({
        url: result.url,
        publicId: result.publicId,
      });
    } catch (error) {
      next(error);
    }
  }
);

export default router;