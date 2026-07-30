using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanThuocNhuomToc.Data;

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
            // Lấy danh mục và sản phẩm nổi bật ra trang chủ
            var danhMucList = await _context.DanhMucSanPhams.ToListAsync();
            var sanPhamList = await _context.SanPhams.Take(6).ToListAsync(); // Lấy 6 sản phẩm đầu tiên

            ViewBag.DanhMucList = danhMucList;
            return View(sanPhamList);
        }
    }
}