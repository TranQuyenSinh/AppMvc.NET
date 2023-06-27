using System.Net;
using App.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using App.ExtendMethods;
using App.Models;
using Microsoft.EntityFrameworkCore;

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

/* ================ Đăng ký DbContext ================ */
builder.Services.AddDbContext<AppDbContext>(option => {
    var connStr = builder.Configuration.GetConnectionString("AppMvcConnectionString");
    option.UseSqlServer(connStr);
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

    Sử dụng Gulp
    - Cài: 
    npm init
    npm install --global gulp-cli
    npm install gulp
    npm install node-sass postcss sass
    npm install gulp-sass gulp-less gulp-concat gulp-cssmin gulp-uglify rimraf gulp-postcss gulp-rename 

    Tạo folder lưu SCSS assets/scss/site.scss
    Thêm vào thư mục gốc dự án file gulpfile.js
    chạy: gulp để watch thay đổi scss thành css
    nếu có error ps1 cannot be loaded because running scripts is disabled on this system
    => Chạy Terminal: Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy Unrestricted
*/
