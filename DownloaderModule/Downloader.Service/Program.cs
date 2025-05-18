using Downloader.Business;
using Downloader.Business.Interfaces;
using Downloader.Model.Model;

var builder = WebApplication.CreateBuilder(args);

//Add configuration to the container

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.local.json");
}

builder.Services.Configure<BaseAppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddScoped<IVideoDownloader, VideoDownloader>();

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
