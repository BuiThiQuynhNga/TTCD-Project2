using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanThuocNhuomToc.Data;
using QuanLyBanThuocNhuomToc.Models;

namespace QuanLyBanThuocNhuomToc.Controllers
{
    public class AdminSanPhamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminSanPhamController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /AdminSanPham
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.SanPhams
                .Include(s => s.DanhMucSanPham)
                .OrderByDescending(s => s.MaSanPham)
                .ToListAsync();

            return View(danhSach);
        }

        // GET: /AdminSanPham/Them
        public async Task<IActionResult> Them()
        {
            ViewBag.DanhMucList = await _context.DanhMucSanPhams.ToListAsync();
            return View();
        }

        // POST: /AdminSanPham/Them
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them(SanPham sanPham, IFormFile? anhUpload)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DanhMucList = await _context.DanhMucSanPhams.ToListAsync();
                return View(sanPham);
            }

            if (anhUpload != null && anhUpload.Length > 0)
            {
                sanPham.HinhAnh = await LuuAnh(anhUpload);
            }

            _context.SanPhams.Add(sanPham);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Thêm sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /AdminSanPham/Sua/5
        public async Task<IActionResult> Sua(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }

            ViewBag.DanhMucList = await _context.DanhMucSanPhams.ToListAsync();
            return View(sanPham);
        }

        // POST: /AdminSanPham/Sua/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sua(int id, SanPham sanPham, IFormFile? anhUpload)
        {
            if (id != sanPham.MaSanPham)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.DanhMucList = await _context.DanhMucSanPhams.ToListAsync();
                return View(sanPham);
            }

            var spCu = await _context.SanPhams.AsNoTracking()
                .FirstOrDefaultAsync(s => s.MaSanPham == id);

            if (spCu == null)
            {
                return NotFound();
            }

            if (anhUpload != null && anhUpload.Length > 0)
            {
                sanPham.HinhAnh = await LuuAnh(anhUpload);
            }
            else
            {
                sanPham.HinhAnh = spCu.HinhAnh;
            }

            try
            {
                _context.Update(sanPham);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.SanPhams.AnyAsync(s => s.MaSanPham == id))
                    return NotFound();
                throw;
            }

            TempData["ThongBao"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /AdminSanPham/Xoa/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null)
            {
                return NotFound();
            }

            bool daBanRa = await _context.ChiTietDonHangs
                .AnyAsync(ct => ct.MaSanPham == id);

            if (daBanRa)
            {
                TempData["LoiXoa"] = $"Không thể xóa \"{sanPham.TenSanPham}\" vì sản phẩm đã nằm trong đơn hàng. Hãy ẩn sản phẩm thay vì xóa.";
                return RedirectToAction(nameof(Index));
            }

            _context.SanPhams.Remove(sanPham);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Xóa sản phẩm thành công!";
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> LuuAnh(IFormFile file)
        {
            var tenFile = Guid.NewGuid().ToString("N") + Path.GetExtension(file.FileName);
            var duongDan = Path.Combine(_env.WebRootPath, "images", tenFile);

            using (var stream = new FileStream(duongDan, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return tenFile;
        }
    }
}