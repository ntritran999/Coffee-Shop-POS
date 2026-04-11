import "dotenv/config";
import { v2 as cloudinary } from "cloudinary";

cloudinary.config({
  cloud_name: process.env.CLOUDINARY_CLOUD_NAME || "",
  api_key: process.env.CLOUDINARY_API_KEY || "",
  api_secret: process.env.CLOUDINARY_API_SECRET || "",
});

export interface UploadResult {
  url: string;
  publicId: string;
}

export async function uploadImage(
  fileBuffer: Buffer,
  fileName: string
): Promise<UploadResult> {
  return new Promise((resolve, reject) => {
    const uploadStream = cloudinary.uploader.upload_stream(
      {
        folder: "coffee-shop-products",
        public_id: fileName.replace(/\.[^/.]+$/, ""),
        resource_type: "image" as const,
      },
      (error, result) => {
        if (error) {
          reject(error);
          return;
        }
        if (!result) {
          reject(new Error("Upload failed - no result"));
          return;
        }
        resolve({
          url: result.secure_url,
          publicId: result.public_id,
        });
      }
    );

    uploadStream.end(fileBuffer);
  });
}

export async function deleteImage(publicId: string): Promise<boolean> {
  try {
    const result = await cloudinary.uploader.destroy(publicId);
    return result.result === "ok";
  } catch (error) {
    return false;
  }
}

export function getPublicIdFromUrl(url: string): string | null {
  const match = url.match(/coffee-shop-products\/(.+)$/);
  return match ? match[1] : null;
}