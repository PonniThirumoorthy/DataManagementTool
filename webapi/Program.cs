using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using webapi.EF;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
//{
//    builder.WithOrigins("http://localhost:3000")
//           .AllowAnyMethod()
//           .AllowAnyHeader();
//}));



// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Azure-DB");
builder.Services.AddDbContext<CustomerContext>(options => options.UseSqlServer(connectionString));


builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("CorsPolicy");
}

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("*"));


app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Data Management API V1");
   // c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

