using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QuanLyBanThuocNhuomToc.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký ApplicationDbContext kết nối SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Cấu hình Cookie Authentication (Xác thực người dùng)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/TaiKhoan/DangNhap"; // Đường dẫn chuyển hướng khi chưa đăng nhập
        options.AccessDeniedPath = "/TaiKhoan/KhongCoQuyen"; // Đường dẫn khi không đủ quyền
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 3. QUAN TRỌNG: Phải đặt UseAuthentication() TRƯỚC UseAuthorization()
app.UseAuthentication();
app.UseAuthorization();

// 4. Định nghĩa đường dẫn mặc định (Bạn có thể đổi về Home/Index hoặc giữ nguyên SanPham/Index)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();