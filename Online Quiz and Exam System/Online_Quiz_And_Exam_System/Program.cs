using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Online_Quiz_And_Exam_System.DAL;
using Online_Quiz_API.DAL;
using System.Text;

namespace Online_Quiz_And_Exam_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ================= CONTROLLERS =================
            builder.Services.AddControllers();

            // ================= CORS =================
            builder.Services.AddCors(o =>
                o.AddPolicy("react", p =>
                    p.AllowAnyOrigin()
                     .AllowAnyHeader()
                     .AllowAnyMethod())
            );

            // ================= JWT AUTHENTICATION =================
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
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

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
                    ),

                    ClockSkew = TimeSpan.Zero
                };
            });

            // ================= DEPENDENCY INJECTION =================
            builder.Services.AddSingleton<DbConnection>();
            builder.Services.AddScoped<UserDAL>();
            builder.Services.AddScoped<ModuleDAL>();
            builder.Services.AddScoped<QuestionDAL>();
            builder.Services.AddScoped<ResultDAL>();
            builder.Services.AddScoped<MockDAL>();

            var app = builder.Build();

            // ================= MIDDLEWARE PIPELINE =================
            app.UseRouting();

            app.UseCors("react");

            // 🔑 JWT MIDDLEWARE (ORDER MATTERS)
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
