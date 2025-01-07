using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Test.Abstractions;
using Test.Data;
using Test.Data.Models;
using Test.Data.Repositories;
using Test.Data.Validators;

var MyAllowSpecificOrigins = "frontend";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IContactRepository, ContactRepository>();

builder.Services.AddControllers();

builder.Services.AddScoped<IValidator<Contact>, ContactValidator>();

var app = builder.Build();

app.MapControllers();
app.UseRouting();
app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.MapGet("/", () => "Hello World!");

if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
