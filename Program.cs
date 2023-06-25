using System.Net;
using App.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using App.ExtendMethods;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSingleton<ProductService, ProductService>();
builder.Services.AddSingleton<PlanetService, PlanetService>();

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


// custom response page from error 404 -> 599
app.AddStatusCodePage(); // phương thức tự Extend 

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


// url: /xemsanpham/id(1,4) hoặc /viewproduct/id(1,4)
app.MapControllerRoute(
    name: "first",
    pattern: "{url:regex(^((viewproduct)|(xemsanpham))$)}/{id:range(1, 4)}",
    defaults: new
    {
        controller = "First",
        action = "ViewProduct"
    }
);

app.MapAreaControllerRoute(
    name: "defaultArea",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    areaName: "ProductManage"
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

/**
    Tạo controller
    dotnet aspnet-codegenerator controller -name ControllerName -namespace ControllerNameSpace -outDir ControllersDir

    Tạo Area
    dotnet aspnet-codegenerator area ProductManage
*/
