using Squora.Forms.Hub.Api.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// MongoDB
var connectionString = builder.Configuration["MongoDB:ConnectionString"] ?? "mongodb://localhost:27017";
var databaseName = builder.Configuration["MongoDB:DatabaseName"] ?? "squora_forms_hub";

builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));
builder.Services.AddSingleton<FormDefinitionService>();

var app = builder.Build();

app.MapControllers();
app.Run();
