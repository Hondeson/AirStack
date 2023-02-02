using AirStack.API.Helper;
using AirStack.API.Service;
using AirStack.Core.Connection;
using AirStack.Core.Model;
using AirStack.Core.Service;
using AirStack.Core.Service.Mssql;
using AirStack.Core.Service.Validation;
using System.Reflection;

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
builder.Services.AddSingleton<ISettingsProvider, SettingsMssqlProvider>();
builder.Services.AddSingleton<IItemValidationService, ItemValidationService>();
builder.Services.AddSingleton<IItemDTOProvider, ItemDTOMssqlProvider>();
builder.Services.AddSingleton<IItemHistoryQueueProvider, ItemHistoryQueueMssqlProvider>();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddHostedService<ProcessItemHistoryQueueService>();

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

CheckBeforeStart();

app.Run();

void CheckBeforeStart()
{
    //check zda filtrovací (flag) enumy odpovídají enumum
    if (FilterEnumHelper.ValidateEnumAgainstMain(typeof(StatusEnum), typeof(StatusFilterEnum)) == false)
        throw new Exception($"Enum {nameof(FilterEnumHelper)} neodpovídá {nameof(StatusEnum)}");

    //Check na enumy vs enum v DB
    var statusSvc = app.Services.GetRequiredService<IStatusProvider>();
    var dbEnums = statusSvc.GetAll();

    if (dbEnums.Count != Enum.GetValues(typeof(StatusEnum)).Length)
        throw new Exception("");
}