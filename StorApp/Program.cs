using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using StorApp.Model;
using StorApp.Services;
using StorApp.Services.StorApi.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File(@"D:\LogFiles\Log.txt", rollingInterval: RollingInterval.Day)
        .ReadFrom.Configuration(configuration)
        .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();


builder.Services.AddDbContext<StorDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddControllers(Options =>
{
    Options.ReturnHttpNotAcceptable = true;
})
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "StorApp.xml"));
    options.AddSecurityDefinition("StorAuthentication", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Enter a valid JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
    {
        new OpenApiSecurityScheme {
            Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = "StorAuthentication"
            } },
        new List<string>()
    }});
});



builder.Host.UseSerilog();

builder.Services.AddAuthentication("Bearer").AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Authentication:Issuer"],
            ValidAudience = builder.Configuration["Authentication:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Authentication:Secret"]))
        };
        options.Validate();
    }
    );

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("SuperAdmin", policy =>
//    {
//        policy.RequireAuthenticatedUser();
//        policy.RequireRole("");
//        policy.RequireClaim("", "");
//    });
//});



builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IProductsRepository, ProductsRepository>();

#if DEBUG
builder.Services.AddTransient<IMailServices, MockMailServises>();
#else
builder.Services.AddTransient<IMailServices, StorMailServices>();
#endif

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(x => x.AllowAnyMethod()
                  .AllowAnyHeader()
                  .SetIsOriginAllowed(origin => true) // allow any origin
                  .AllowCredentials()// allow credentials
                  );

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.Run();

