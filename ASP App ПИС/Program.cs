using ASP_App_ПИС.Services;
using ASP_App_ПИС.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication("Cookies").AddCookie(options => options.LoginPath = "/login");
builder.Services.AddAuthorization();

builder.Services.AddHttpClient<IWebService, WebService>(c => c.BaseAddress = new Uri("https://localhost:7022/"));

var app = builder.Build();

// получение данных

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
