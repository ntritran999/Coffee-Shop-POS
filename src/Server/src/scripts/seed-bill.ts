import prisma from "../db/prisma.js";

async function main() {
  console.log("Fetching products...");
  const products = await prisma.product.findMany();
  if (products.length === 0) {
    console.error("No products found! Please run seed-b3.ts first.");
    process.exit(1);
  }

  console.log("Checking tables...");
  let tables = await prisma.table.findMany();
  if (tables.length === 0) {
    console.log("No tables found, creating tables...");
    const tablesData = [];
    for (let i = 1; i <= 20; i++) {
      tablesData.push({ TableName: `Bàn ${i}` });
    }
    await prisma.table.createMany({ data: tablesData });
    tables = await prisma.table.findMany();
  }

  console.log("Creating 100 bills...");
  const billsToCreate = 100;
  
  for (let i = 0; i < billsToCreate; i++) {
    // Generate CheckIn time (within last 30 days)
    const checkInDate = new Date();
    checkInDate.setDate(checkInDate.getDate() - Math.floor(Math.random() * 30));
    checkInDate.setHours(Math.floor(Math.random() * 12) + 8); // 8 AM to 8 PM
    checkInDate.setMinutes(Math.floor(Math.random() * 60));

    // Status: 0 (Unpaid), 1 (Paid), 2 (Cancelled)
    const status = Math.floor(Math.random() * 3);
    
    let checkOutDate = null;
    // Always provide DateCheckOut to fill all fields if possible.
    // If unpaid, it can be null, but requirement says "Điền đầy đủ field của bill".
    // I will fill DateCheckOut for all statuses except 0 maybe? Wait, "đầy đủ field" might imply DateCheckOut should have a value.
    // But logically an unpaid bill doesn't have a check out date.
    // Let's stick to: if status !== 0, fill DateCheckOut. If status === 0, it should be null.
    // Actually, to be safe, I'll fill it if status is 1 or 2.
    if (status !== 0) { 
      checkOutDate = new Date(checkInDate);
      checkOutDate.setMinutes(checkOutDate.getMinutes() + Math.floor(Math.random() * 120) + 10); // 10 to 130 minutes later
    }

    const table = tables[Math.floor(Math.random() * tables.length)];
    const discount = Math.random() > 0.8 ? (Math.floor(Math.random() * 5) * 10) : 0; // 0, 10, 20, 30, 40

    // Random number of items (1 to 5)
    const numItems = Math.floor(Math.random() * 5) + 1;
    let totalAmount = 0;
    const billInfos = [];
    
    // Pick unique products
    const shuffledProducts = [...products].sort(() => 0.5 - Math.random());
    const selectedProducts = shuffledProducts.slice(0, numItems);

    for (const product of selectedProducts) {
      const count = Math.floor(Math.random() * 3) + 1;
      const price = product.Price;
      const amount = count * price;
      totalAmount += amount;
      
      const noteOptions = ["Không đá", "Ít đường", "Nhiều sữa", "Đem về", "Làm nhanh", ""];
      const note = noteOptions[Math.floor(Math.random() * noteOptions.length)];

      billInfos.push({
        ProductID: product.ProductID,
        Count: count,
        Price: price,
        Note: note
      });
    }

    // Apply discount
    if (discount > 0) {
      totalAmount = totalAmount * (1 - discount / 100);
    }

    await prisma.bill.create({
      data: {
        DateCheckIn: checkInDate,
        DateCheckOut: checkOutDate,
        TableID: table.TableID,
        Status: status,
        TotalAmount: totalAmount,
        Discount: discount,
        BillInfo: {
          create: billInfos
        }
      }
    });
  }

  console.log(`Successfully created ${billsToCreate} bills with their bill infos.`);
}

main()
  .catch((e) => {
    console.error(e);
    process.exit(1);
  })
  .finally(async () => {
    await prisma.$disconnect();
  });
