using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanThuocNhuomToc.Data;

namespace QuanLyBanThuocNhuomToc.Controllers
{
    [Authorize]
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetMaNguoiDung()
        {
            var claim = User.FindFirst("MaNguoiDung");
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        // GET: /DonHang (Admin xem toàn bộ đơn hàng)
        [Authorize(Roles = "Quản trị")]
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.DonHangs
                .Include(d => d.NguoiDung)
                .Where(d => d.TrangThaiDonHang != "Giỏ hàng")
                .OrderByDescending(d => d.NgayDatHang)
                .ToListAsync();

            return View(danhSach);
        }

        // POST: /DonHang/CapNhatTrangThai (Admin đổi trạng thái đơn hàng)
        [Authorize(Roles = "Quản trị")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(int id, string trangThai)
        {
            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang == null)
            {
                return NotFound();
            }

            var trangThaiHopLe = new[] { "Chờ xác nhận", "Đang giao", "Đã giao", "Đã hủy" };
            if (!trangThaiHopLe.Contains(trangThai))
            {
                return BadRequest();
            }

            donHang.TrangThaiDonHang = trangThai;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã cập nhật đơn hàng #{donHang.MaDonHang} sang trạng thái \"{trangThai}\".";
            return RedirectToAction("Index");
        }

        // GET: /DonHang/LichSu (Khách xem đơn hàng của chính mình)
        public async Task<IActionResult> LichSu()
        {
            int maNguoiDung = GetMaNguoiDung();

            var danhSach = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPham)
                .Where(d => d.MaNguoiDung == maNguoiDung && d.TrangThaiDonHang != "Giỏ hàng")
                .OrderByDescending(d => d.NgayDatHang)
                .ToListAsync();

            return View(danhSach);
        }

        // GET: /DonHang/ChiTiet/5
        public async Task<IActionResult> ChiTiet(int id)
        {
            var donHang = await _context.DonHangs
                .Include(d => d.NguoiDung)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefaultAsync(d => d.MaDonHang == id);

            if (donHang == null)
            {
                return NotFound();
            }

            bool laAdmin = User.IsInRole("Quản trị");
            if (!laAdmin && donHang.MaNguoiDung != GetMaNguoiDung())
            {
                return Forbid();
            }

            return View(donHang);
        }
    }
}