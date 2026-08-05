using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanThuocNhuomToc.Data;

namespace QuanLyBanThuocNhuomToc.Controllers
{
    public class SanPhamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SanPhamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách sản phẩm (có thể lọc theo danh mục hoặc từ khóa)
        public async Task<IActionResult> Index(int? maDanhMuc, string? keyword)
        {
            var query = _context.SanPhams
                .Include(s => s.DanhMucSanPham)
                .AsQueryable();

            if (maDanhMuc.HasValue)
            {
                query = query.Where(s => s.MaDanhMuc == maDanhMuc.Value);
                var danhMuc = await _context.DanhMucSanPhams.FindAsync(maDanhMuc.Value);
                ViewBag.TenDanhMuc = danhMuc?.TenDanhMuc;
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(s => s.TenSanPham.Contains(keyword));
            }

            var danhSachSanPham = await query.ToListAsync();
            return View(danhSachSanPham);
        }

        // Chi tiết sản phẩm
        public async Task<IActionResult> ChiTiet(int id)
        {
            var sanPham = await _context.SanPhams
                .Include(s => s.DanhMucSanPham)
                .Include(s => s.DanhGias)
                    .ThenInclude(d => d.NguoiDung)
                .FirstOrDefaultAsync(s => s.MaSanPham == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }
    }
}