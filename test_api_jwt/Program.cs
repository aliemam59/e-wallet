using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using test_api_jwt.Data;
using test_api_jwt.Security;
using test_api_jwt.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.RateLimiting;

using MassTransit;
using test_api_jwt.Consumers;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRateLimiter(options =>
{
    // بنعمل قاعدة سميناها "BankRule"
    options.AddFixedWindowLimiter("BankRule", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10); // المدة الزمنية
        opt.PermitLimit = 5;                   // عدد الطلبات المسموحة
        opt.QueueLimit = 0;                    // لو زاد، ارفض فوراً (متحطوش في طابور انتظار)
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // لو العميل اتخطى الحد، نرجعله Status Code 429 (Too Many Requests)
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});


// 1. إعدادات قاعدة البيانات
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddMassTransit(x =>
{
    // بنسجل الموظف بتاعنا
    x.AddConsumer<DepositCompletedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("guest");
            h.Password("guest");
        });

        // السطر ده بيخلي الرابيت يجهز الطوابير (Queues) أوتوماتيك
        cfg.ConfigureEndpoints(context);
    });
});

// 2. تسجيل الخدمات (Dependency Injection)
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

builder.Services.AddControllers();

// 3. إعدادات Swagger عشان يقبل الـ Token
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter JWT Bearer token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// 4. إعدادات الـ JWT Authentication
var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "EWallet_";
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// تفعيل التحقق من الهوية والصلاحيات
app.UseAuthentication();
app.UseAuthorization();
app.UseRouting();

// لازم السطر ده يكون هنا (قبل الـ Authorization)
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();