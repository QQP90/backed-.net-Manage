// Web API 启动程序
var builder = WebApplication.CreateBuilder(args);

// 添加控制器
builder.Services.AddControllers();

// 添加 Swagger
builder.Services.AddSwaggerGen();

// 添加 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();

// Swagger UI（开发环境）
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Database API v1");
    });
}

app.UseCors("AllowAll");
app.MapControllers();

Console.WriteLine("========================================");
Console.WriteLine("  Database API 已启动!");
Console.WriteLine("========================================");
Console.WriteLine("  Swagger UI: http://localhost:5000/swagger");
Console.WriteLine("  API 地址：http://localhost:5000/api/database/test");
Console.WriteLine("========================================");
Console.WriteLine("按 Ctrl+C 停止服务...");

app.Run("http://localhost:5000");
