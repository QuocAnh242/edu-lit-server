using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Load Ocelot configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Register Ocelot services
builder.Services.AddOcelot(builder.Configuration);

// Add controllers & Swagger (optional)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger for debugging Gateway only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Do NOT use HTTPS redirect if you're using HTTP between containers
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// THIS LINE IS REQUIRED
await app.UseOcelot();

app.Run();
