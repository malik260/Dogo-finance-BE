using DogoFinance.Api.Extensions;
using DogoFinance.DataAccess.Layer.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// ── JWT Authentication ─────────────────────────────────────────────
var jwtSecret = builder.Configuration["SystemConfig:JWTSecret"];
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("--- JWT Auth Failed ---");
            Console.WriteLine($"Message: {context.Exception.Message}");
            Console.WriteLine($"Auth Header: {context.Request.Headers["Authorization"]}");
            return Task.CompletedTask;
        },
        OnTokenValidated = async context =>
        {
            var uow = context.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
            var userIdStr = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (long.TryParse(userIdStr, out long userId))
            {
                var user = await uow.Users.GetById(userId);
                
                // Properly extract iat from the token
                var jwtToken = context.SecurityToken as JwtSecurityToken;
                var iatStr = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Iat)?.Value;
                
                if (user != null && user.LastLogoutDate.HasValue && long.TryParse(iatStr, out long iat))
                {
                    var iatDateTime = DateTimeOffset.FromUnixTimeSeconds(iat).UtcDateTime;
                    if (iatDateTime <= user.LastLogoutDate.Value.AddSeconds(-1))
                    {
                        // context.Fail("Token has been revoked by logout.");
                    }
                }
            }
        }
    };
});

// ── DogoFinance DAL Registration ───────────────────────────────────
builder.Services.AddDogoFinanceServices();

// CORS for Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

var app = builder.Build();

app.UseCors("AllowFrontend");

// ── DogoFinance Global Configuration ───────────────────────────────
app.ConfigureDogoFinance();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
