using Fourjuris.Integracao.Api.Endpoints;
using Fourjuris.Integracao.Api.Services;
using Fourjuris.Integracao.Configurations;
using Fourjuris.Integracao.Helpers;
using Fourjuris.Integracao.WhatsApp;
using Fourjuris.Integracao.WhatsApp.Abstractions;
using Fourjuris.Integracao.WhatsApp.Evolution.V2;
using Fourjuris.Integracao.WhatsApp.Evolution.V2.Services;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

Batteries.Init();
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("PoliticaIntegracao", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();

        //policy.WithOrigins("https://localhost")
        //      .AllowAnyMethod()
        //      .AllowAnyHeader();
    });

});

// Implementa autentica��o JWT
//builder.Services.AddAuthentication("Bearer")
//    .AddJwtBearer(options =>
//    {
//        options.Authority = "https://auth-server";
//        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//        {
//            ValidateAudience = false
//        };
//    });



// Adicionar servi�os necess�rios para API Minimal
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "FourJuris Integra��o API", Version = "v1" });
});


// Adicionar DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("Fourjuris.Integracao.Configurations"));
    }
    else
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("Fourjuris.Integracao.Configurations"));
    }
});


// Adicionar servi�os de configura��o
builder.Services.AddMemoryCache();
// HostedService faz a conex�o com a API Evolution e publica eventos de mensagens recebidas
//builder.Services.AddHostedService<EvolutionWebSocketService>();
// ITenantConfigurationService e TenantConfigurationRepository fazem a gest�o das configura��es por empresa
builder.Services.AddScoped<ITenantConfigurationRepository, TenantConfigurationRepository>();
builder.Services.AddScoped<ITenantConfigurationService, TenantConfigurationService>();


//builder.Services.AddHttpClient("WhatsAppEvolution");

// este servi�o � usado para fazer chamadas HTTP para a API Evolution
builder.Services.AddHttpClient("WhatsAppEvolution", client =>
{
    client.BaseAddress = new Uri("http://168.231.88.109:8080/");
    client.Timeout = TimeSpan.FromSeconds(30);
});


// IEventBus com Redis, usando configura��o do appsettings.json

builder.Services.AddSingleton<IEventBus>(provider =>
{
    var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    return new RedisEventBus(redisConnectionString);
});

// Adicionar WhatsApp services
builder.Services.AddScoped<IWhatsAppClientFactory, WhatsAppClientFactory>();
builder.Services.AddScoped<IWhatsAppMensagensService, WhatsAppMensagensService>();

// Configurar servi�os
builder.Services.AddScoped<IWhatsAppClientResolver, WhatsAppClientResolver>();
builder.Services.AddScoped<IWhatsAppClientFactory, WhatsAppClientFactory>();
builder.Services.AddScoped<IWhatsAppMensagensService, WhatsAppMensagensService>();
builder.Services.AddScoped<ChatService>();
builder.Services.AddScoped<MessageService>();
// Configurar o WhatsAppClientResolver com os tipos de cliente suportados
builder.Services.AddSingleton(s =>
{
    var resolver = new WhatsAppClientResolver(s.GetRequiredService<IServiceProvider>());
    resolver.RegisterClientType(TipoIntegracao.Evolution, typeof(EvolutionWhatsAppClient));
    // Adicionar outros tipos de integra��o aqui, se necess�rio (ex.: Meta)
    return resolver;
});
var app = builder.Build();

// Configurar o pipeline de tratamento de erros
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\": \"Ocorreu um erro no servidor.\"}");
    });
});

// Aplicar migra��es do banco de dados 
//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//    dbContext.Database.Migrate();
//}

// Configure o pipeline de HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FourJuris Integra��o API v1"));
}
app.UseCors("PoliticaIntegracao");
app.UseRouting();
// Implementar se necess�rio
//app.UseAuthentication();
//app.UseAuthorization();
app.UseHttpsRedirection();
// Mapear todos os endpoints da API Minimal
app.MapAllEndpoints();
//app.MapControllers();
app.Run();