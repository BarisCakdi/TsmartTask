using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TsmartTask.Data;
using TsmartTask.Services;

namespace TsmartTask
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] ?? string.Empty)),
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return context.Response.WriteAsJsonAsync(new { message = "Yetkisiz erişim! Lütfen giriş yapınız." });
                        }
                    };
                });

            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // CORS ayarları
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.Use(async (context, next) =>
            {
                if (context.Request.Method == "POST" || context.Request.Method == "PATCH")
                {
                    if (!context.Request.Headers.ContainsKey("Content-Type"))
                    {
                        context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                        context.Response.ContentType = "application/json";
                        var errorResponse = new
                        {
                            message = "Lütfen geçerli bir JSON formatında veri gönderin.",
                            example = new
                            {
                                Name = "Yeni Ürün Adı",
                                Price = 150.75,
                                Stock = 100
                            },
                            details = "Lütfen Content-Type başlığını 'application/json' olarak ayarladığınızdan emin olun."
                        };
                        await context.Response.WriteAsJsonAsync(errorResponse);
                        return;
                    }

                    if (!context.Request.Headers["Content-Type"].ToString().Contains("application/json"))
                    {
                        context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
                        context.Response.ContentType = "application/json";
                        var errorResponse = new
                        {
                            message = "Lütfen geçerli bir JSON formatında veri gönderin.",
                            example = new
                            {
                                Name = "Yeni Ürün Adı",
                                Price = 150.75,
                                Stock = 100
                            },
                            details = "Lütfen Content-Type başlığını 'application/json' olarak ayarladığınızdan emin olun."
                        };
                        await context.Response.WriteAsJsonAsync(errorResponse);
                        return;
                    }
                }

                await next();
            });

            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}