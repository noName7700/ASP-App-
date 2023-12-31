using ASP_App_���.Helpers;
using ASP_App_���.Services;
using ASP_App_���.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication("Cookies").AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();
builder.Services.AddSingleton<ISort, SortByProp>();

var port = builder.Configuration.GetValue("ServerPort", "44370");

builder.Services.AddHttpClient<IWebService, WebService>(c => c.BaseAddress = new Uri($"https://localhost:{port}/"));
//builder.Services.AddHttpClient<IWebService, WebSocketService>(c => c.BaseAddress = new Uri($"https://localhost:{port}/muns"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
