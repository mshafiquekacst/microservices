using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "PlatformService", Version = "v1" });
});


if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("InMemory"));
}
else
{
    Console.WriteLine($"--> Using SqlServer Db", builder.Environment.EnvironmentName);
    builder.Services.AddDbContext<AppDbContext>(options =>
       options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformConn")));

}
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseHttpsRedirection();
app.MapControllers();
app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/protos/platforms.proto", async context =>
{
    var file = Path.Combine(builder.Environment.ContentRootPath, "Protos", "platforms.proto");
    if (File.Exists(file))
        await context.Response.WriteAsync(await File.ReadAllTextAsync(file));
    else
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("Not Found");
    }

});
Console.WriteLine("selected environment", builder.Configuration["CommandService"]);
PrepDb.PrepPopulation(app, app.Environment.IsProduction());
app.Run();

