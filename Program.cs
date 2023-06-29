using System.Net;
using App.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using App.ExtendMethods;
using App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using App.Data;

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
builder.Services.AddDbContext<AppDbContext>(option =>
{
    var connStr = builder.Configuration.GetConnectionString("AppMvcConnectionString");
    option.UseSqlServer(connStr);
});

/* ================ Send Mail Service ================ */
// Lấy và đăng ký cấu hình send mail
builder.Services.AddOptions();
var mailsettings = builder.Configuration.GetSection("MailSettings");
builder.Services.Configure<MailSettings>(mailsettings);
// Đăng ký dịch vụ IEmailSender với đối tượng cụ thể là SendMailService để Identity gửi email xác thực
builder.Services.AddSingleton<IEmailSender, SendMailService>();


// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại, xác thực rồi mới cho login)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;  // Yêu cầu xác thực email trước khi login, xem trang register để rõ hơn

});
// Đăng ký Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

/* ================ Authorization option ================ */
builder.Services.ConfigureApplicationCookie(option =>
{
    option.LoginPath = "/login/";
    option.LogoutPath = "/logout/";
    option.AccessDeniedPath = "/khongduoctruycap.html";
});

/* ================ Thêm các Authentication provider ================ */
// Từ google, facebook
builder.Services.AddAuthentication()
                .AddGoogle(option =>
                {
                    var ggConfig = builder.Configuration.GetSection("Authentication:Google");
                    option.ClientId = ggConfig["ClientId"];
                    option.ClientSecret = ggConfig["ClientSecret"];
                    // http://localhost:5253/signin-google => Callbackpath mặc định
                    option.CallbackPath = "/dang-nhap-tu-google";
                })
                .AddFacebook(option =>
                {
                    var fbConfig = builder.Configuration.GetSection("Authentication:Facebook");
                    option.AppId = fbConfig["ClientId"];
                    option.AppSecret = fbConfig["ClientSecret"];
                    // còn error khi login
                    // phải xóa &scope=email trong url để fix
                });
/* ================ Tùy biến thông báo lỗi của Identity ================ */
builder.Services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

/* ================ Policy ================ */
builder.Services.AddAuthorization(options => {
    options.AddPolicy("ViewManageMenu", pBuilder => {
        pBuilder.RequireAuthenticatedUser();
        pBuilder.RequireRole(RoleName.Administrator);
    });
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

/* ================ Thêm vào đoạn này để fix bug khi dùng external login ================ */
app.UseCookiePolicy(new CookiePolicyOptions()
{
    MinimumSameSitePolicy = SameSiteMode.Lax
});

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
