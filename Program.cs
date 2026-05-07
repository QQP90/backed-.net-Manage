using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using WebApplication1.Infrastructure;
using WebApplication1.Repositories;
using WebApplication1.Services;
using WebApplication1.Repositories.Order;
var builder = WebApplication.CreateBuilder(args);


// 第 1 步：读取连接字符串（来源：appsettings.json 的 ConnectionStrings:DefaultConnection）
// 目的：让整个应用（EF Core + ADO.NET）都用同一份数据库配置
// 如果不做：可能连到错误的库/错误的服务器，或者在运行时才报连接错误（更难定位）
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Missing connection string: ConnectionStrings:DefaultConnection");
}

// 第 2 步：注册 EF Core 的 DbContext（这里只是“注册”，不会立刻打开数据库连接）
// 目的：让 Repository 可以通过依赖注入拿到 AppDbContext
// 如果不做：凡是依赖 AppDbContext 的地方（User/Order）都会启动失败或运行时报错
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

// 第 3 步：注册 MVC 控制器（让 /api/... 路由能工作）
// 如果不做：Swagger 能打开但接口 404，或者路由找不到
builder.Services.AddControllers();

// 全局 ProblemDetails 配置：把所有错误响应统一成标准结构（RFC7807）
// 这里额外塞入 traceId，方便你在日志里按 traceId 快速定位一次请求的全链路
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] =
            Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
    };
});

// 第 4 步：注册 Order 模块（Repository + Service）
// 目的：Controller -> Service -> Repository -> DbContext 的调用链条能完整组装起来
// 如果不做：调用 /api/order 会报 500（DI 无法解析 IOrderService / IOrderRepository）
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
// 添加 Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Student API",
        Version = "v1",
        Description = "学生管理 API - 提供学生的增删改查功能"
    });
});

// 第 5 步：配置 CORS（让浏览器前端可以跨域调用）
// 如果不做：浏览器会被 CORS 拦截（Postman/curl 一般不受影响）
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 注册 Repository
builder.Services.AddSingleton<IStudentRepository>(provider => 
    new StudentRepository(connectionString));
builder.Services.AddScoped<IUserRepository, UserRepository>();

// 注册 Service
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IUserService, UserService>();

// 注册AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// 异常 -> HTTP 状态码映射：
// 目的：让常见“参数错误/找不到/未授权”返回更合理的状态码，而不是全部 500
static int MapExceptionToStatusCode(Exception ex)
{
    return ex switch
    {
        ArgumentException => StatusCodes.Status400BadRequest,
        KeyNotFoundException => StatusCodes.Status404NotFound,
        UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status500InternalServerError
    };
}

// 全局异常处理：
// 捕获所有未处理异常，返回 ProblemDetails（application/problem+json）
// 开发环境返回 Detail=完整堆栈；生产环境不返回堆栈避免泄露敏感信息
app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;
        if (ex == null)
        {
            // 理论上不会发生：没有异常对象但进入了异常处理管道
            // 兜底返回 500，避免卡住请求
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return;
        }

        var statusCode = MapExceptionToStatusCode(ex);
        context.Response.StatusCode = statusCode;

        // 统一错误输出结构（前端只要处理一种格式即可）
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode == StatusCodes.Status500InternalServerError ? "服务器内部错误" : "请求处理失败",
            Detail = app.Environment.IsDevelopment() ? ex.ToString() : null,
            Instance = context.Request.Path
        };

        var problemDetailsService = context.RequestServices.GetRequiredService<IProblemDetailsService>();
        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = problem,
            Exception = ex
        });
    });
});

// 非异常类错误（例如 404/405）默认可能没有响应体：
// 这里把它们也统一包装成 ProblemDetails，保证前端永远拿到同一套错误结构
app.UseStatusCodePages(async statusCodeContext =>
{
    var context = statusCodeContext.HttpContext;
    if (context.Response.HasStarted)
    {
        return;
    }

    if (context.Response.StatusCode < 400)
    {
        return;
    }

    var problem = new ProblemDetails
    {
        Status = context.Response.StatusCode,
        Title = "请求失败",
        Instance = context.Request.Path
    };

    var problemDetailsService = context.RequestServices.GetRequiredService<IProblemDetailsService>();
    await problemDetailsService.WriteAsync(new ProblemDetailsContext
    {
        HttpContext = context,
        ProblemDetails = problem
    });
});

// Swagger UI（开发环境）
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student API v1");
    });
}

// 添加根路径处理
app.MapGet("/", async context =>
{
    context.Response.ContentType = "application/json";
    await context.Response.WriteAsJsonAsync(new { status = "ok", message = "Server is running" });
});

// 第 6 步：把 CORS / Controller 路由挂到 HTTP 管道
// 如果中间件顺序不对：会出现 CORS 头没加上、路由不生效等问题
app.UseCors("AllowAll");
app.MapControllers();

app.Run();
