using ChannelEngine.Business.Clients;
using ChannelEngine.Business.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<IChannelEngineClient, ChannelEngineClient>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
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
