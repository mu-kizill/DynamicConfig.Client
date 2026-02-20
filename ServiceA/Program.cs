using DynamicConfig.Client;
using DynamicConfig.Client.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConfigurationReader>(sp =>
{
    var connectionString =
        builder.Configuration.GetConnectionString("ConfigDb");

    var provider = new DbConfigProvider(connectionString);

    return new ConfigurationReader("SERVICE-A", provider, 5000);
});



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
