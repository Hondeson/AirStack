using AirStack.Core.Connection;
using AirStack.Core.Services;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
        //.AllowCredentials();
    });
});

var db = builder.Configuration.GetConnectionString("MSSQL");
builder.Services.AddSingleton<ISqlAdapter, MssqlConnectionProvider>((ioc) =>
{
    return new MssqlConnectionProvider(db);
});

builder.Services.AddSingleton<IItemProvider, ItemMssqlProvider>();
builder.Services.AddSingleton<IItemHistoryProvider, ItemHistoryMssqlProvider>();
builder.Services.AddSingleton<IStatusProvider, StatusMssqlProvider>();

var app = builder.Build();
app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();