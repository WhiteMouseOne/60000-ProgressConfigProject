using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Progress.Common;
using Progress.IService;
using Progress.IService.Business;
using Progress.IService.SystemManagement;
using Progress.Repository;
using Progress.Service.Business;
using Progress.Service.SystemManagement;
using Progress.WebApi;
using Progress.WebApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JWTTokenOptions>(builder.Configuration.GetSection("JWTTokenOptions"));
builder.Services.Configure<PoValidationOptions>(builder.Configuration.GetSection(PoValidationOptions.SectionName));
builder.Services.Configure<EmailOptions>(builder.Configuration.GetSection(EmailOptions.SectionName));

var conn = builder.Configuration.GetConnectionString("MySqlConnectStr");
if (string.IsNullOrWhiteSpace(conn))
    throw new InvalidOperationException("ConnectionStrings:MySqlConnectStr is required.");

var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));
builder.Services.AddDbContext<ProgressDbContext>(o =>
    o.UseMySql(conn, serverVersion));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
builder.Services.AddScoped<IDataScopeService, DataScopeService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthLoginService, AuthLoginService>();
builder.Services.AddScoped<IMetaService, MetaService>();
builder.Services.AddScoped<IAlertSettingService, AlertSettingService>();
builder.Services.AddScoped<ISupplierAdminService, SupplierAdminService>();
builder.Services.AddScoped<ISupplierProfileService, SupplierProfileService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICraftAdminService, CraftAdminService>();
builder.Services.AddScoped<ICraftRecipeAdminService, CraftRecipeAdminService>();
builder.Services.AddScoped<IOrderLineService, OrderLineService>();
builder.Services.AddScoped<IRepairService, RepairService>();
builder.Services.AddScoped<IHomeStatsService, HomeStatsService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IAlertScanService, AlertScanService>();
builder.Services.AddHostedService<AlertBackgroundService>();

var jwt = builder.Configuration.GetSection("JWTTokenOptions").Get<JWTTokenOptions>()
          ?? throw new InvalidOperationException("JWTTokenOptions missing");
if (string.IsNullOrWhiteSpace(jwt.SecurityKey) || jwt.SecurityKey.Length < 32)
    throw new InvalidOperationException("JWTTokenOptions:SecurityKey must be at least 32 characters.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecurityKey))
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Progress API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "在下方输入框填入 JWT：Bearer {token}（登录接口 GetToken 返回的 token）",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddCors(p => p.AddPolicy("cors", c =>
    c.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

var wwwroot = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
Directory.CreateDirectory(Path.Combine(wwwroot, "uploads", "avatars"));

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProgressDbContext>();
    await DbSeed.EnsureSeedAsync(db);
}

app.UseCors("cors");
app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(o =>
{
    o.SwaggerEndpoint("/swagger/v1/swagger.json", "Progress API v1");
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
