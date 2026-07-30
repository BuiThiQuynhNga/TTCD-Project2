using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanThuocNhuomToc.Data;
using QuanLyBanThuocNhuomToc.Models;
using System.Security.Claims;

namespace QuanLyBanThuocNhuomToc.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. ĐĂNG KÝ (GET)
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }

        // 1. ĐĂNG KÝ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(NguoiDung model, string XacNhanMatKhau)
        {
            if (model.MatKhau != XacNhanMatKhau)
            {
                ModelState.AddModelError("", "Mật khẩu xác nhận không khớp.");
                return View(model);
            }

            var existingUser = await _context.NguoiDungs.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Email này đã được sử dụng.");
                return View(model);
            }

            model.VaiTro = "Khách hàng"; // Mặc định là khách hàng
            _context.NguoiDungs.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("DangNhap");
        }

        // 2. ĐĂNG NHẬP (GET)
        [HttpGet]
        public IActionResult DangNhap()
        {
            return View();
        }

        // 2. ĐĂNG NHẬP (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(string email, string matKhau)
        {
            var user = await _context.NguoiDungs
                .FirstOrDefaultAsync(u => u.Email == email && u.MatKhau == matKhau);

            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác.");
                return View();
            }

            // Tạo danh sách thông tin người dùng lưu vào Cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.HoTen),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.VaiTro), // Phân quyền: "Quản trị" hoặc "Khách hàng"
                new Claim("MaNguoiDung", user.MaNguoiDung.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // Phân hướng dựa theo vai trò
            if (user.VaiTro == "Quản trị")
            {
                return RedirectToAction("Index", "SanPham"); // Hoặc trang Admin dashboard
            }
            return RedirectToAction("Index", "Home");
        }

        // 3. ĐĂNG XUẤT
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // 4. TRANG BÁO LỖI KHÔNG CÓ QUYỀN
        public IActionResult KhongCoQuyen()
        {
            return View();
        }
    }
}