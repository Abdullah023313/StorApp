using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using StorApp.Model;
using StorApp.Services;
using StorApp.Services.StorApi.Services;




var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File(@"D:\LogFiles\Log.txt",rollingInterval:RollingInterval.Day)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StorDbContext>(options => options.UseSqlServer(connectionString));


builder.Services.AddControllers(Options =>
{
    Options.ReturnHttpNotAcceptable = true;
})
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog();

builder.Services.AddScoped< IProductsService,ProductsService>();

#if DEBUG
builder.Services.AddTransient<IMailServices, MockMailServises>();
#else
builder.Services.AddTransient<IMailServices, StorMailServices>();
#endif

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(x => x
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .SetIsOriginAllowed(origin => true) // allow any origin
                    .AllowCredentials()); // allow credentials
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();

