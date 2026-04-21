import "dotenv/config";
import prisma from "../db/prisma.js";
import { uploadImage } from "../services/cloudinary.service.js";
import { readFileSync } from "fs";

const products = [
  { id: 1, imagePath: "../Client/Client/Assets/ProductSource/1.png", name: "Espresso" },
  { id: 2, imagePath: "../Client/Client/Assets/ProductSource/2.png", name: "Americano" },
  { id: 3, imagePath: "../Client/Client/Assets/ProductSource/3.png", name: "Cà phê đen đá" },
  { id: 4, imagePath: "../Client/Client/Assets/ProductSource/4.png", name: "Cà phê sữa đá" },
  { id: 5, imagePath: "../Client/Client/Assets/ProductSource/5.png", name: "Trà đào cam sả" },
  { id: 6, imagePath: "../Client/Client/Assets/ProductSource/4.png", name: "Trà vải" }, // Reuse ảnh tạm
  { id: 7, imagePath: "../Client/Client/Assets/ProductSource/5.png", name: "Trà sữa" },
  { id: 8, imagePath: "../Client/Client/Assets/ProductSource/1.png", name: "Matcha" },
  { id: 9, imagePath: "../Client/Client/Assets/ProductSource/2.png", name: "Sinh tố bơ" },
  { id: 10, imagePath: "../Client/Client/Assets/ProductSource/3.png", name: "Nước ép" },
  { id: 11, imagePath: "../Client/Client/Assets/ProductSource/4.png", name: "Coca" },
  { id: 12, imagePath: "../Client/Client/Assets/ProductSource/5.png", name: "Tiramisu" },
  { id: 13, imagePath: "../Client/Client/Assets/ProductSource/1.png", name: "Mousse" },
  { id: 14, imagePath: "../Client/Client/Assets/ProductSource/2.png", name: "Hướng dương" },
  { id: 15, imagePath: "../Client/Client/Assets/ProductSource/3.png", name: "Cà phê rang mộc" },
];

async function main() {
  console.log("Starting upload products to Cloudinary and update DB...");

  for (const product of products) {
    try {
      console.log(`\n📤 Uploading ${product.name}...`);
      
      const fileBuffer = readFileSync(product.imagePath);
      const fileName = `product-${product.id}-${Date.now()}`;
      
      const result = await uploadImage(fileBuffer, fileName);
      
      console.log(`✅ Uploaded: ${result.url}`);
      
      console.log(`📝 Updating DB for product ID ${product.id}...`);
      await prisma.product.update({
        where: { ProductID: product.id },
        data: { Image: result.url },
      });
      
      console.log(`✅ Updated product ${product.id} with image URL`);
    } catch (error) {
      console.error(`❌ Error processing ${product.name}:`, error.message);
    }
  }

  console.log("\n✅ All products uploaded and updated!");
  
  const updatedProducts = await prisma.product.findMany();
  console.log("\n📋 Products in DB:");
  for (const p of updatedProducts) {
    console.log(`  ID: ${p.ProductID}, Name: ${p.Name}, Image: ${p.Image}`);
  }
}

main()
  .catch(console.error)
  .finally(() => prisma.$disconnect());