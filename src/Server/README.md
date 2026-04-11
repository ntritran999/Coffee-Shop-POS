# Server

ExpressJS API for the Coffee Shop POS backend with Prisma 7 and PostgreSQL/Supabase.

## Quick start

```bash
cd src/Server
npm install
npx prisma db pull
npx prisma generate
npm run dev
```

Server runs at `http://localhost:5000` by default.

## Environment

Set `.env` like this:

```env
DATABASE_URL="postgresql://pooled-supabase-url:6543/postgres?pgbouncer=true"
PORT=5000
SECRET_KEY="replace-with-a-strong-secret"
```

`DATABASE_URL` is used for both app runtime and Prisma CLI commands in this setup.

## Development

Run the server with nodemon:

```bash
npm run dev
```

Run the server with `tsx watch` directly:

```bash
npm run dev:tsx
```

Run once without watch mode:

```bash
npm run start
```

## Prisma

Preferred workflow:

```bash
npx prisma db pull
npx prisma generate
npx prisma migrate dev
npx prisma studio
```


## API

- `GET /api/health`

GraphQL endpoint:

- `POST /graphql`

## Notes

- This setup follows Prisma 7's `prisma-client` generator with a custom output directory.
- The server runtime uses `tsx` because Prisma 7's generated client is TypeScript-first with this setup.
- After you add your Supabase URL, run `npx prisma db pull` and then `npx prisma generate`.
- If `db pull` is slow or unreachable through the pooler on your network, temporarily replace `DATABASE_URL` with the direct connection string from Supabase and run the pull again.
- Once your schema is pulled, we can add domain routes for products, tables, bills, and accounts on top of the generated client.
