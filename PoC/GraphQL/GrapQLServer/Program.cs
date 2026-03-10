using GrapQLServer.Data;
using GrapQLServer.GraphQL;

var builder = WebApplication.CreateBuilder(args);

// Ensure the server listens on http://localhost:5000
builder.WebHost.UseUrls("http://localhost:5000");

builder.Services.AddSingleton<BillRepository>();
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>();

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/graphql"));
app.MapGraphQL("/graphql");

app.Run();
