using System.Text;
using FribergCarRentalsAPI.Constants;
using FribergCarRentalsAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services
    .AddIdentityCore<ApiUser>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true; 
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT
var jwt = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme   = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // options.RequireHttpsMetadata = false; // endast om du måste köra HTTP i dev
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime         = true,
        ClockSkew                = TimeSpan.Zero,
        ValidIssuer   = jwt["Issuer"],
        ValidAudience = jwt["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
    };
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FribergCarRentalsAPI", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Ex: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
    options.AddPolicy("AllowAll", b => b
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()
    ));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); 
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FribergCarRentalsAPI v1"));
}

using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = sp.GetRequiredService<UserManager<ApiUser>>();
    var db      = sp.GetRequiredService<AppDbContext>();
    var logger  = sp.GetRequiredService<ILogger<Program>>();

    // 1) Roller
    foreach (var roleName in new[] { ApiRoles.Admin, ApiRoles.User })
    {
        if (!await roleMgr.RoleExistsAsync(roleName))
        {
            var r = await roleMgr.CreateAsync(new IdentityRole
            {
                Name = roleName,
                NormalizedName = roleName.ToUpper()
            });
            if (!r.Succeeded)
            {
                logger.LogError("Kunde inte skapa roll {Role}: {Errors}",
                    roleName, string.Join("; ", r.Errors.Select(e => $"{e.Code}:{e.Description}")));
                throw new Exception("Roll-seeding misslyckades");
            }
        }
    }

    // 2) Admin-användare
    const string adminEmail = "admin@friberg.com";
    const string adminPass  = "Test1234";

    var admin = await userMgr.FindByEmailAsync(adminEmail);
    if (admin is null)
    {
        admin = new ApiUser
        {
            UserName = adminEmail,
            NormalizedUserName = adminEmail.ToUpper(),
            Email = adminEmail,
            NormalizedEmail = adminEmail.ToUpper(),
            EmailConfirmed = true,
            FirstName = "System",
            LastName = "Administrator"
        };
        var createRes = await userMgr.CreateAsync(admin, adminPass);
        if (!createRes.Succeeded)
        {
            logger.LogError("Kunde inte skapa admin {Email}: {Errors}",
                adminEmail, string.Join("; ", createRes.Errors.Select(e => $"{e.Code}:{e.Description}")));
            throw new Exception("Admin-seeding misslyckades");
        }
    }

    // 3) Lägg till Admin-roll
    if (!await userMgr.IsInRoleAsync(admin, ApiRoles.Admin))
    {
        var addRoleRes = await userMgr.AddToRoleAsync(admin, ApiRoles.Admin);
        if (!addRoleRes.Succeeded)
        {
            logger.LogError("Kunde inte lägga admin i roll {Role}: {Errors}",
                ApiRoles.Admin, string.Join("; ", addRoleRes.Errors.Select(e => $"{e.Code}:{e.Description}")));
            throw new Exception("Roll-assignment misslyckades");
        }
    }

    // 4) (Valfritt) rad i Admins-tabellen
    if (!await db.Admins.AnyAsync(a => a.ApiUserId == admin.Id))
    {
        db.Admins.Add(new FribergCarRentalsAPI.Data.Models.Admin
        {
            Email = admin.Email!,
            ApiUserId = admin.Id
        });
        await db.SaveChangesAsync();
    }

    logger.LogInformation("Admin seeding klar: {Email}", adminEmail);
}



app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
