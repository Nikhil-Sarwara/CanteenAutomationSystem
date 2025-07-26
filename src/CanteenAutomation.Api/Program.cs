using System.Text;
using CanteenAutomation.Infrastructure.Validation;
using FluentValidation;
using FluentValidation.AspNetCore;
// using Hangfire; // Temporarily disabled
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

// --- Add services to the container. ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- Hangfire Temporarily Disabled ---
// builder.Services.AddHangfire(config => config
//     .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("HangfireConnection")));
// builder.Services.AddHangfireServer();
// ------------------------------------

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddSignalR();
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddValidatorsFromAssemblyContaining<OrderRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("WebAppPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173") // The address of your React app
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- Configure the HTTP request pipeline. ---
app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --- Hangfire Temporarily Disabled ---
// app.UseHangfireDashboard();
// ------------------------------------

app.MapHub<OrderHub>("/orderHub");

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRolesAsync(roleManager);

    // --- Hangfire Temporarily Disabled ---
    // RecurringJob.AddOrUpdate<DailySalesJob>(
    //     "daily-sales-report",
    //     job => job.Execute(),
    //     Cron.Daily);
    // ------------------------------------
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("WebAppPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }