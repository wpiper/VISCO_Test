using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var oauth = builder.Configuration.GetSection("OAuth");
var issuer = oauth["Issuer"] ?? "https://auth.example.com";
var audience = oauth["Audience"] ?? "visco-api";
var signingKey = oauth["SigningKey"] ?? "replace-this-with-a-long-random-secret";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/xml", ([FromBody] XmlRequest request) =>
    {
        if (string.IsNullOrWhiteSpace(request.Key1))
        {
            return Results.BadRequest("key1 is required.");
        }

        var xml = new XElement("response",
            new XElement("key1", request.Key1),
            new XElement("generatedAtUtc", DateTime.UtcNow.ToString("O"))
        );

        return Results.Text(xml.ToString(SaveOptions.DisableFormatting), "application/xml");
    })
    .RequireAuthorization()
    .WithName("GetXmlByKey1")
    .WithOpenApi();

app.Run();

public sealed record XmlRequest(string Key1);
