using ChannelEngine.Business.Clients;
using ChannelEngine.Business.Models.Options;
using ChannelEngine.Business.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ChannelEngineOptions>(
    builder.Configuration.GetSection("ChannelEngine"));

builder.Services.AddHttpClient<IChannelEngineClient, ChannelEngineClient>();

// Register ProductService by interface
builder.Services.AddTransient<IProductService, ProductService>();


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}");

app.Run();
