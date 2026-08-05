using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanThuocNhuomToc.Data;
using QuanLyBanThuocNhuomToc.Models;

namespace QuanLyBanThuocNhuomToc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Nếu là Admin đã đăng nhập -> hiện Dashboard quản trị
            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("Quản trị"))
            {
                var thongKe = new DashboardViewModel
                {
                    TongSanPham = await _context.SanPhams.CountAsync(),
                    TongDanhMuc = await _context.DanhMucSanPhams.CountAsync(),
                    TongKhachHang = await _context.NguoiDungs.CountAsync(n => n.VaiTro == "Khách hàng"),

                    TongDonHang = await _context.DonHangs.CountAsync(d => d.TrangThaiDonHang != "Giỏ hàng"),
                    DonChoXacNhan = await _context.DonHangs.CountAsync(d => d.TrangThaiDonHang == "Chờ xác nhận"),
                    DonDangGiao = await _context.DonHangs.CountAsync(d => d.TrangThaiDonHang == "Đang giao"),

                    DoanhThu = await _context.DonHangs
                        .Where(d => d.TrangThaiDonHang == "Đã giao")
                        .SumAsync(d => (decimal?)d.TongTien) ?? 0,

                    SanPhamSapHet = await _context.SanPhams
                        .Where(s => s.SoLuongTon <= 10)
                        .OrderBy(s => s.SoLuongTon)
                        .Take(5)
                        .ToListAsync(),

                    DonHangGanDay = await _context.DonHangs
                        .Include(d => d.NguoiDung)
                        .Where(d => d.TrangThaiDonHang != "Giỏ hàng")
                        .OrderByDescending(d => d.NgayDatHang)
                        .Take(5)
                        .ToListAsync()
                };

                return View("Dashboard", thongKe);
            }

            // Khách hàng / khách vãng lai -> trang chủ bán hàng như cũ
            var danhMucList = await _context.DanhMucSanPhams.ToListAsync();
            var sanPhamList = await _context.SanPhams.Take(6).ToListAsync();

            ViewBag.DanhMucList = danhMucList;
            return View(sanPhamList);
        }
    }
}