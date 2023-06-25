using App.Services;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSingleton<ProductService, ProductService>();

/* ================ Cấu hình thư mục tìm kiếm các view  ================ */
builder.Services.Configure<RazorViewEngineOptions>(option =>
{
    /**
        Thứ tự tìm kiếm:
        1. /Views/Controller/Action.cshtml
        2. /Views/Shared/Action.cshtml
        3. /Pages/Shared/Action.cshtml (nếu có sử dụng UseRazorPages())
        4. tự thiết lập

        {0}: Action
        {1}: Controller
        {2}: Area
    */

    option.ViewLocationFormats.Add("/MyView/{1}/{0}" + RazorViewEngine.ViewExtension);
});







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

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
