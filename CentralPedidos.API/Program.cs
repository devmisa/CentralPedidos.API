using CentralPedidos.API.Extensions;
using CentralPedidos.Infrastructure.Context;
using CentralPedidos.Infrastructure.DapperHandlers;
using Dapper;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

SqlMapper.AddTypeHandler(new GuidTypeHandler());

builder.Host.UseSerilog((ctx, config) =>
    config
        .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.RegisterServices(builder.Configuration);

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    IServiceProvider services = scope.ServiceProvider;
    try
    {
        AppDbContext context = services.GetRequiredService<AppDbContext>();
        _ = await context.Database.EnsureCreatedAsync();
    }
    catch (Exception ex)
    {
        ILogger<Program> logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao criar ou migrar o banco de dados.");
    }
}

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
