using Microsoft.EntityFrameworkCore;
using PromoEngine.Data;
using PromoEngine.Models;
using PromoEngine.Services;
using PromoEngine.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0))
    )
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<SubmissionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

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
    }
}

app.Run();
