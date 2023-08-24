using InventarizationApi.Models.Config;
using InventarizationApi.Models.Repository.Warehouse;
using InventarizationApi.Models.Repository.WarehouseGeometry;
using InventarizationApi.Models.Service;
using InventarizationApi.Models.Service.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConnectorCreator, PostgresConnectorCreator>()
    .AddHostedService<UpdateDbService>()
    .AddScoped<IWarehouseRepository, WarehouseRepository>()
    .AddScoped<IWarehouseGeometryObjectsRepository, WarehouseGeometryObjectRepository>()
    .AddScoped<WarehouseService>()
    .Configure<DataSource>(builder.Configuration.GetSection("DataSource"))
    .AddControllersWithViews()
    .AddJsonOptions(
        options =>
        {
            options.JsonSerializerOptions.Converters.Add(
                new NetTopologySuite.IO.Converters.GeoJsonConverterFactory());
        }
    );

var app = builder
    .Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();