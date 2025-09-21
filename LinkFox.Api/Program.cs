using LinkFox.Api.Middleware;
using LinkFox.Application.Interface;
using LinkFox.Application.Services;
using LinkFox.Infrastructure.Persistence;
using LinkFox.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure Serilog (reads from appsettings Serilog section)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add DbContext (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//DI: Repository and Service
builder.Services.AddScoped<IUrlRepository, UrlRepository>();
builder.Services.AddScoped<IUrlService, UrlService>();

builder.Services.AddControllers();

//Add API Versioning
builder.Services.AddApiVersioning(options =>
{
	options.DefaultApiVersion = new ApiVersion(1, 0);
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // e.g. v1, v1.1
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//CORS Policy
builder.Services.AddCors(options =>
{
	options.AddPolicy("linkfox-webapp",
		policy =>
		{
			policy.WithOrigins("http://localhost:4200")
			.AllowAnyHeader()
			.AllowAnyMethod();
		});
});

var app = builder.Build();

//Custom middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

//Create a DB if not exists and apply initialization

using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

	try
	{
		// Create a DB if not exists. 
		// This check and create a table as per the EF Model
		db.Database.EnsureCreated();
		Log.Information("Database Ensured/Created");
	}
	catch (Exception ex)
	{
		Log.Fatal(ex, "An occured while creating the database");
		throw;
	}
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("swagger/v1/swagger.json", "LinkFox-API-Version-1.0");
		c.RoutePrefix = string.Empty;
	});
    //app.MapOpenApi();
}

app.UseSerilogRequestLogging();

app.MapControllers();

app.UseHttpsRedirection();

//Enable CORS
app.UseCors("linkfox-webapp");

app.UseAuthorization();
app.Run();
