using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using PubQuizMediaServer.Util;
using PubQuizMediaServer.Util.Handler;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = new[] { "https://192.168.0.187", "https://localhost:7147", "https://localhost:7148" };

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

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
            ValidAudiences = new[]
            {
                builder.Configuration["Jwt:Audience:Admin"],
                builder.Configuration["Jwt:Audience:Organizer"],
                builder.Configuration["Jwt:Audience:Attendee"]
            },
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

//builder.Services.AddAuthorization(options =>
//{
//    // Example: policy for media admins or owners
//    options.AddPolicy("MediaAccessPolicy", policy =>
//        policy.RequireAuthenticatedUser()
//    // You can add roles or claims checks here
//    );
//});

var app = builder.Build();

MediaPaths.EnsureDirectoriesExist();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(MediaPaths.Public.Base),
    RequestPath = "/media"
});

app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
