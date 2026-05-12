using ApiGateway.Infrastructure.Dependency;
using ApiGateway.Application.Dependency;
using ApiGateway.Infrastructure.Persistence.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ApiGateway.Application.Auth;
using APP.API.Filters;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.JsonWebTokens;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMemberClient(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddScoped<TokenBucketFilter>();

builder.Services
    .AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = false,
            ValidateLifetime = false,
            RequireSignedTokens = false,
            SignatureValidator = (token, parameters) =>
            {
                return new JsonWebToken(token);
            }
        };



        options.Events = new JwtBearerEvents
        {

            OnTokenValidated = context =>
            {
                var jwtUtil = context.HttpContext
                    .RequestServices
                    .GetRequiredService<JwtUtil>();

                var rawToken = context.Request.Headers["Authorization"]
                    .ToString()
                    .Replace("Bearer ", "");

                var principal = jwtUtil.ValidateToken(
                    rawToken,
                    validIssuer: builder.Configuration["GatewayName"]
                );

                if (principal == null)
                {
                    context.Fail("Custom JWT validation failed");
                    return Task.CompletedTask;
                }

                context.Principal = principal;

                return Task.CompletedTask;
            },

        };
    });



builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5005);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // ← 自動執行所有未套用的 Migration
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
