using tarea2.Data;
using tarea2.Endpoints;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var mongoSettings = builder.Configuration.GetSection("MongoDbSettings");
var connectionString = mongoSettings["ConnectionString"];
builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));
builder.Services.AddSingleton<MongoDbContext>();


builder.Services.AddCors();

var app = builder.Build();


app.UseDefaultFiles();
app.UseStaticFiles();


app.UseHttpsRedirection();


app.MapVentaEndpoints();

app.Run();