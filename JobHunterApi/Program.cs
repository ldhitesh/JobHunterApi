using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JobHunterApi.Database;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);



// Add CORS support and configure the policy to allow any origin, method, and header
builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAllOrigins", builder =>
        {
            builder.AllowAnyOrigin()  // Allow all origins (including Angular app, other apps)
                   .AllowAnyMethod()  // Allow any HTTP method (GET, POST, PUT, DELETE, etc.)
                   .AllowAnyHeader(); // Allow any headers
        });
    });

builder.Services.AddDbContext<CompaniesDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("LocalHostConnection"), 
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("LocalHostConnection"))));
    
// Add services to the container.
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("LocalHostConnection"), 
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("LocalHostConnection"))));

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();


    builder.Services.AddAuthentication(options =>
{
    // Default scheme for requests
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    // This configuration is for your custom JWT (Symmetric key)
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"]
    };
})
.AddJwtBearer("Auth0", options =>
{
    // This configuration is for the Auth0 JWT (RS256)
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = "https://dev-23jl6hcwap6zsa4n.us.auth0.com/",
        ValidAudience = "0wWmE5XJ8RNcCzPaha3XeWUBZ46WdeaD",
        ValidateIssuerSigningKey = true,

        // Resolving the signing key via JWKS (Auth0)
        IssuerSigningKeyResolver = (token, securityToken, keyIdentifier, validationParameters) =>
        {
            using var client = new HttpClient();
            var jwks = client.GetStringAsync("https://dev-23jl6hcwap6zsa4n.us.auth0.com/.well-known/jwks.json").Result;
            var jwksDocument = JsonSerializer.Deserialize<JsonElement>(jwks);
            
            var keys = jwksDocument.GetProperty("keys").EnumerateArray()
                .Where(key => key.GetProperty("kid").GetString() == keyIdentifier)
                .Select(key => new JsonWebKey(key.GetRawText()))
                .ToList();
            return keys;
        }
    };
});

// // Add JWT Authentication

// //Uses my database and my own JWT 

// // builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
// // .AddJwtBearer("Bearer",options =>
// // {
// //     options.TokenValidationParameters = new TokenValidationParameters
// //     {
// //         ValidateIssuerSigningKey = true,
// //         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
// //         ValidateIssuer = true,
// //         ValidateAudience = true,
// //         ValidIssuer = builder.Configuration["Jwt:Issuer"],
// //         ValidAudience = builder.Configuration["Jwt:Audience"]
// //     };
// // });


// //Uses Auth0 
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer("Bearer", options =>
//     {
//         // Token validation parameters (for RS256 signature algorithm)
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidIssuer = "https://dev-23jl6hcwap6zsa4n.us.auth0.com/", // Ensure the issuer is correct
//             ValidAudience = "0wWmE5XJ8RNcCzPaha3XeWUBZ46WdeaD", // Ensure the audience is correct
//             ValidateIssuerSigningKey = true,  // Ensure the key is being validated

//             // Resolving the signing key via JWKS
//             IssuerSigningKeyResolver = (token, securityToken, keyIdentifier, validationParameters) =>
//             {
//                 using var client = new HttpClient();
//                 var jwks = client.GetStringAsync("https://dev-23jl6hcwap6zsa4n.us.auth0.com/.well-known/jwks.json").Result;
//                 var jwksDocument = JsonSerializer.Deserialize<JsonElement>(jwks);
//                 // Extract keys from the JWKS response
//                 var keys = jwksDocument.GetProperty("keys").EnumerateArray()
//                     .Where(key => key.GetProperty("kid").GetString() == keyIdentifier)
//                     .Select(key => new JsonWebKey(key.GetRawText()))
//                     .ToList();
//                 return keys;
//             },
            
//         };

//     });


// Authorization policies (based on specific claims in the token)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireClaim("jobhunter-roles", "admin"));
    
    options.AddPolicy("UserPolicy", policy =>
        policy.RequireClaim("jobhunter-roles", "user"));
});


builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
await SeedRolesAsync(app.Services);

app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

async Task SeedRolesAsync(IServiceProvider services)
{
    using (var scope = services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var roles = new[] { "Admin", "User" };

        foreach (var role in roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(role);
            if (!roleExist)
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}



