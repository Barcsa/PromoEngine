using Microsoft.EntityFrameworkCore;
using PromoEngine.Data;
using PromoEngine.Models;
using PromoEngine.Repositories;
using PromoEngine.Repositories.Interfaces;
using PromoEngine.Services;
using PromoEngine.Services.Interfaces;
using PromoEngine.Utils;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 0)),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    )
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://promo-engine-barcsa.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IPromoCodeRepository, PromoCodeRepository>();
builder.Services.AddScoped<ISubmissionRepository, SubmissionRepository>();
builder.Services.AddScoped<IWinningTimestampRepository, WinningTimestampRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

// test seeds - promo codes
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

    if (!db.PromoCodes.Any())
    {
        var codes = PromoCodeGenerator.GenerateUniqueCodes(10)
            .Select(c => new PromoCode { Code = c })
            .ToList();

        db.PromoCodes.AddRange(codes);
        db.SaveChanges();

        Console.WriteLine("Seeded promo codes:");
        foreach (var code in codes)
            Console.WriteLine($" - {code.Code}");
    }

    if (!db.WinningTimestamps.Any())
    {
        var now = DateTime.UtcNow;

// test seeds for daily 10 random winner timestaps
        var dailyTimestamps = Enumerable.Range(1, 10)
            .Select(i => new WinningTimestamp
            {
                TargetTime = now.AddDays(i).AddHours(new Random().Next(0, 23)).AddMinutes(new Random().Next(0, 59)),
                Type = "Daily",
                IsClaimed = false
            })
            .ToList();

// test seeds for weekly 5 winner timestamps for 5 weeks
        var weeklyTimestamps = Enumerable.Range(1, 5)
            .Select(i => new WinningTimestamp
            {
                TargetTime = now.AddDays(i * 7).Date.AddHours(12).AddMinutes(new Random().Next(0, 59)),
                Type = "Weekly",
                IsClaimed = false
            })
            .ToList();

        db.WinningTimestamps.AddRange(dailyTimestamps);
        db.WinningTimestamps.AddRange(weeklyTimestamps);
        db.SaveChanges();
    }
}

app.Run();
