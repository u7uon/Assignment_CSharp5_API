using Assignment_Backend.Data;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Assignment_Backend.Repository;
using Assignment_Backend.Services;
using Assingment_Backend.Interfaces;
using Assingment_Backend.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var blazorAccept = "AllowBlazor";

var config = builder.Configuration; 

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Cho phép frontend truy cập API
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    options.AddPolicy(name: blazorAccept,
        policy => policy.WithOrigins("https://localhost:7299") // cổng Blazor WebAssembly
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
});



builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Defaultconnection")));
builder.Services.Configure<FormOptions>(options =>
{
    // Tăng giới hạn kích thước multipart form-data
    options.MultipartBodyLengthLimit = 52428800; // 50 MB
});


//Cấu hình jwt
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var secretKey = jwtSettings["Key"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}
).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
}
).AddGoogle(options =>
{
    options.ClientId = "your-google-client-id";
    options.ClientSecret = "your-google-client-secret";
})
.AddFacebook(options =>
{
    options.AppId = "your-facebook-app-id";
    options.AppSecret = "your-facebook-app-secret";
});

builder.Services.AddAuthorization();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 52428800; // 50 MB
});



builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
}
).AddEntityFrameworkStores<ApplicationContext>()
.AddDefaultTokenProviders();

//DI Config 
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();



builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IBrandService, BrandService>();

builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();


builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();




builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
var app = builder.Build();


app.UseCors(blazorAccept);

app.UseCors(MyAllowSpecificOrigins);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", "https://localhost:7299");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
        context.Response.StatusCode = 200;
        return;
    }
    await next();
});

app.MapControllers();

app.Run();
