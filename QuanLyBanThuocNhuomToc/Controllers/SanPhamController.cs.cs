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

        // Danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var danhSachSanPham = await _context.SanPhams
                .Include(s => s.DanhMucSanPham)
                .ToListAsync();

            return View(danhSachSanPham);
        }

        // Chi tiết sản phẩm
        public async Task<IActionResult> Details(int id)
        {
            var sanPham = await _context.SanPhams
                .Include(s => s.DanhMucSanPham)
                .FirstOrDefaultAsync(s => s.MaSanPham == id);

            if (sanPham == null)
            {
                return NotFound();
            }

            return View(sanPham);
        }
    }
}