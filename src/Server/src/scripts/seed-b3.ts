import prisma from "../db/prisma.js";
import * as fs from "fs";

const categoriesData = [
  { CategoryName: "Cà phê" },
  { CategoryName: "Trà" },
  { CategoryName: "Nước trái cây" },
  { CategoryName: "Bánh ngọt" },
  { CategoryName: "Đồ ăn vặt" },
];

function generateImages(categoryIndex: number, productIndex: number): string {
  const baseUrl = "https://picsum.photos/seed";
  return `${baseUrl}/cat${categoryIndex}prod${productIndex}_1/300/300;${baseUrl}/cat${categoryIndex}prod${productIndex}_2/300/300;${baseUrl}/cat${categoryIndex}prod${productIndex}_3/300/300`;
}

function generateProductName(categoryName: string, index: number): string {
  const prefixes = {
    "Cà phê": ["Espresso", "Americano", "Cappuccino", "Latte", "Mocha", "Cold Brew", "Macchiato", "Flat White", "Cà phê đen đá", "Cà phê sữa đá", "Bạc xỉu", "Cà phê trứng", "Cà phê muối", "Cà phê dừa", "Phin đen", "Phin sữa", "Phin đá", "Caramel Macchiato", "Hazelnut Latte", "Vanilla Latte", "Irish Coffee", "Affogato", "Cortado", "Piccolo", "Ristretto"],
    "Trà": ["Trà đào cam sả", "Trà vải", "Trà sen vàng", "Trà sữa trân châu", "Trà ô long", "Trà lài", "Trà xanh", "Trà đen", "Trà Earl Grey", "Trà hoa cúc", "Trà bạc hà", "Trà dâu tây", "Trà chanh leo", "Trà táo", "Trà dưa lưới", "Trà sữa khoai môn", "Trà sữa matcha", "Trà sữa socola", "Trà sữa hokkaido", "Trà sữa okinawa", "Trà mãng cầu", "Trà măng cụt", "Trà dâu tằm", "Trà bưởi hồng", "Trà xoài"],
    "Nước trái cây": ["Nước ép cam", "Nước ép dưa hấu", "Nước ép táo", "Nước ép thơm", "Nước ép cà rốt", "Nước ép ổi", "Nước ép nho", "Nước ép lựu", "Nước ép bưởi", "Nước chanh", "Sinh tố bơ", "Sinh tố dâu", "Sinh tố xoài", "Sinh tố chuối", "Sinh tố mãng cầu", "Sinh tố sapoche", "Sinh tố việt quất", "Sinh tố mâm xôi", "Sinh tố sầu riêng", "Sinh tố đu đủ", "Sinh tố dừa", "Nước dừa tươi", "Cam vắt", "Chanh dây", "Cóc ép"],
    "Bánh ngọt": ["Tiramisu", "Mousse đào", "Mousse chanh dây", "Mousse trà xanh", "Mousse socola", "Bánh phô mai", "Cheesecake dâu", "Cheesecake việt quất", "Bánh su kem", "Bánh flan", "Croissant", "Pain au chocolat", "Bánh mì hoa cúc", "Bánh mì bơ tỏi", "Bánh cookie", "Bánh macaron", "Bánh brownie", "Bánh tart trứng", "Bánh tart táo", "Bánh crepe sầu riêng", "Bánh waffle", "Bánh pancake", "Bánh bông lan trứng muối", "Bánh cuộn", "Donut"],
    "Đồ ăn vặt": ["Hạt hướng dương", "Hạt dưa", "Hạt dẻ cười", "Hạt điều", "Đậu phộng rang", "Khô gà lá chanh", "Khô bò", "Khô mực", "Da heo mắm tỏi", "Bánh tráng trộn", "Bánh tráng nướng", "Khoai tây chiên", "Khoai lang lắc phô mai", "Cá viên chiên", "Bò viên chiên", "Xúc xích chiên", "Phô mai que", "Nem chua rán", "Trái cây dĩa", "Sữa chua nếp cẩm", "Sữa chua trái cây", "Rong biển cháy tỏi", "Đậu hà lan rang", "Bắp rang bơ", "Thịt xiên nướng"]
  };
  
  const list = prefixes[categoryName as keyof typeof prefixes] || [];
  if (index < list.length) {
    return list[index];
  }
  return `${categoryName} đặc biệt số ${index + 1}`;
}

async function main() {
  console.log("Bắt đầu xóa dữ liệu cũ...");
  
  await prisma.billInfo.deleteMany();
  await prisma.bill.deleteMany();
  await prisma.product.deleteMany();
  await prisma.category.deleteMany();

  console.log("Xóa dữ liệu cũ thành công!");
  console.log("Bắt đầu seed dữ liệu mới...");

  let totalProducts = 0;
  const csvData: string[] = ["Name,Price,Unit,CategoryName,Image"]; // Header cho file CSV

  for (let i = 0; i < categoriesData.length; i++) {
    const categoryName = categoriesData[i].CategoryName;
    const category = await prisma.category.create({
      data: { CategoryName: categoryName }
    });

    console.log(`Đã tạo Category: ${category.CategoryName} (ID: ${category.CategoryID})`);

    const productsToCreate = [];
    for (let j = 0; j < 22; j++) {
      const name = generateProductName(categoryName, j);
      const price = Math.floor(Math.random() * 50 + 20) * 1000;
      // Trong project này trường Unit lại được dùng như Số lượng tồn kho (dựa theo code Dashboard lấy LowStock < 5)
      // Nên random Unit từ 10 đến 100 cho hợp lý.
      const unit = Math.floor(Math.random() * 90) + 10; 
      const image = generateImages(i, j);

      productsToCreate.push({
        Name: name,
        Price: price,
        Unit: unit,
        CategoryID: category.CategoryID,
        Image: image
      });

      // Lưu lại dữ liệu cho file CSV (để Hùng có dữ liệu import)
      csvData.push(`"${name}",${price},${unit},"${categoryName}","${image}"`);
    }

    const createdProducts = await prisma.product.createMany({
      data: productsToCreate
    });

    totalProducts += createdProducts.count;
    console.log(` -> Đã thêm ${createdProducts.count} sản phẩm cho ${categoryName}`);
  }

  // Ghi file CSV ra ngoài thư mục dự án
  fs.writeFileSync("../../B3_Sample_Products.csv", csvData.join("\n"), "utf8");
  console.log(`\nĐã xuất file dữ liệu mẫu ra: B3_Sample_Products.csv (Hùng có thể dùng file này để test Import)`);

  console.log(`\nSeed hoàn tất! Đã tạo tổng cộng ${totalProducts} sản phẩm cho ${categoriesData.length} loại.`);
}

main()
  .catch((e) => {
    console.error(e);
    process.exit(1);
  })
  .finally(async () => {
    await prisma.$disconnect();
  });