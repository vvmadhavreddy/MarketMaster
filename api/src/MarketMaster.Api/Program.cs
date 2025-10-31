using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// CORS for React dev server
const string DevCors = "DevCors";
builder.Services.AddCors(o =>
{
    o.AddPolicy(DevCors, p =>
        p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod());
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Note: Module registration has to be done here

var app = builder.Build();

app.UseCors(DevCors);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapGet("/api/hello", () => Results.Ok(new { message = "Hello from MarketMaster API" }));

app.MapGet("/api/db-ping", async (IConfiguration cfg) =>
{
    await using var conn = new NpgsqlConnection(cfg.GetConnectionString("Postgres"));
    await conn.OpenAsync();
    await using var cmd = new NpgsqlCommand("select 1", conn);
    var x = await cmd.ExecuteScalarAsync();
    return Results.Ok(new { db = "ok", result = x });
});

app.Run();


