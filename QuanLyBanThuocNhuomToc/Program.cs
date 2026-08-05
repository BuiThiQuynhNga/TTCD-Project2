using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QuanLyBanThuocNhuomToc.Data;
using QuanLyBanThuocNhuomToc.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký ApplicationDbContext kết nối SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Cấu hình Cookie Authentication (Xác thực người dùng)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/TaiKhoan/DangNhap";
        options.AccessDeniedPath = "/TaiKhoan/KhongCoQuyen";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });
builder.Services.AddControllersWithViews();
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ===== SEED DỮ LIỆU MẪU (chạy 1 lần, xóa đoạn này sau khi dùng xong) =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    async Task<int> LayHoacTaoDanhMuc(string ten)
    {
        var dm = await db.DanhMucSanPhams.FirstOrDefaultAsync(d => d.TenDanhMuc == ten);
        if (dm != null) return dm.MaDanhMuc;

        dm = new DanhMucSanPham { TenDanhMuc = ten };
        db.DanhMucSanPhams.Add(dm);
        await db.SaveChangesAsync();
        return dm.MaDanhMuc;
    }

    if (!db.SanPhams.Any())
    {
        int idThoiTrang = await LayHoacTaoDanhMuc("Thuốc nhuộm thời trang");
        int idPhuBac = await LayHoacTaoDanhMuc("Thuốc nhuộm phủ bạc tự nhiên");
        int idOxy = await LayHoacTaoDanhMuc("Oxy trợ dưỡng & Tẩy tóc");
        int idDungCu = await LayHoacTaoDanhMuc("Dụng cụ nhuộm tóc tại nhà");

        var sanPhamMau = new List<SanPham>
        {
            new() { TenSanPham = "Thuốc nhuộm tóc màu Nâu Trà Sữa", MaMau = "7/77", GiaBan = 150000, SoLuongTon = 120, MaDanhMuc = idThoiTrang, MoTaSanPham = "Tông màu hot trend, không cần tẩy tóc, tôn da." },
            new() { TenSanPham = "Thuốc nhuộm tóc màu Xám Khói", MaMau = "0/11", GiaBan = 180000, SoLuongTon = 85, MaDanhMuc = idThoiTrang, MoTaSanPham = "Cần nâng tông hoặc tẩy nhẹ trước khi nhuộm tại nhà." },
            new() { TenSanPham = "Thuốc nhuộm tóc màu Hồng Baby", MaMau = "8/26", GiaBan = 160000, SoLuongTon = 60, MaDanhMuc = idThoiTrang, MoTaSanPham = "Màu hồng nhẹ nhàng, phù hợp tóc đã tẩy sáng." },
            new() { TenSanPham = "Thuốc nhuộm tóc màu Nâu Đồng", MaMau = "6/34", GiaBan = 145000, SoLuongTon = 95, MaDanhMuc = idThoiTrang, MoTaSanPham = "Tông ấm, dễ lên màu trên tóc Việt." },
            new() { TenSanPham = "Thuốc nhuộm tóc màu Tím Khói", MaMau = "5/22", GiaBan = 170000, SoLuongTon = 40, MaDanhMuc = idThoiTrang, MoTaSanPham = "Màu cá tính, giữ màu lâu." },
            new() { TenSanPham = "Thuốc nhuộm tóc màu Đỏ Rượu Vang", MaMau = "4/65", GiaBan = 165000, SoLuongTon = 70, MaDanhMuc = idThoiTrang, MoTaSanPham = "Màu sang trọng, hợp da trắng." },

            new() { TenSanPham = "Thuốc nhuộm phủ bạc thảo dược Thiên Nhiên", MaMau = "3/0", GiaBan = 120000, SoLuongTon = 200, MaDanhMuc = idPhuBac, MoTaSanPham = "Chiết xuất nhân sâm, phủ bạc 100%, an toàn da đầu." },
            new() { TenSanPham = "Thuốc nhuộm phủ bạc nhanh 5 phút", MaMau = "2/0", GiaBan = 110000, SoLuongTon = 150, MaDanhMuc = idPhuBac, MoTaSanPham = "Công thức nhanh, tiện lợi cho người bận rộn." },
            new() { TenSanPham = "Dầu gội phủ bạc đen tự nhiên", MaMau = "1/0", GiaBan = 135000, SoLuongTon = 90, MaDanhMuc = idPhuBac, MoTaSanPham = "Dùng thay dầu gội hàng ngày, lên màu dần." },

            new() { TenSanPham = "Oxy trợ nhuộm 20 Vol", MaMau = null, GiaBan = 45000, SoLuongTon = 300, MaDanhMuc = idOxy, MoTaSanPham = "Dùng kèm thuốc nhuộm, tỉ lệ 1:1." },
            new() { TenSanPham = "Bột tẩy tóc siêu tốc", MaMau = null, GiaBan = 55000, SoLuongTon = 180, MaDanhMuc = idOxy, MoTaSanPham = "Tẩy tóc lên tông nhanh trong 20-30 phút." },
            new() { TenSanPham = "Kem dưỡng phục hồi sau nhuộm", MaMau = null, GiaBan = 89000, SoLuongTon = 110, MaDanhMuc = idOxy, MoTaSanPham = "Phục hồi tóc hư tổn sau khi nhuộm/tẩy." },

            new() { TenSanPham = "Găng tay nhuộm tóc dùng 1 lần (hộp 10)", MaMau = null, GiaBan = 15000, SoLuongTon = 500, MaDanhMuc = idDungCu, MoTaSanPham = "Bảo vệ tay khi tự nhuộm tại nhà." },
            new() { TenSanPham = "Chén trộn thuốc nhuộm + cọ chải", MaMau = null, GiaBan = 25000, SoLuongTon = 250, MaDanhMuc = idDungCu, MoTaSanPham = "Bộ dụng cụ trộn và thoa thuốc nhuộm tiện lợi." },
            new() { TenSanPham = "Mũ trùm tóc nhuộm silicon", MaMau = null, GiaBan = 35000, SoLuongTon = 130, MaDanhMuc = idDungCu, MoTaSanPham = "Giữ nhiệt, giúp màu lên đều và nhanh hơn." },
        };

        db.SanPhams.AddRange(sanPhamMau);
        await db.SaveChangesAsync();
    }
}
// ===== HẾT PHẦN SEED =====

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

// 4. Định nghĩa đường dẫn mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Run();