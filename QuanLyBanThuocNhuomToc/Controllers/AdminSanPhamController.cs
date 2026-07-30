using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyBanThuocNhuomToc.Data;
using QuanLyBanThuocNhuomToc.Models;

namespace QuanLyBanThuocNhuomToc.Controllers
{
    [Authorize(Roles = "Quản trị")] // Bắt buộc phải đăng nhập và là Quản trị viên mới được vào
    public class AdminSanPhamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminSanPhamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH SẢN PHẨM ADMIN
        public async Task<IActionResult> Index()
        {
            var sanPhams = await _context.SanPhams.Include(s => s.DanhMucSanPham).ToListAsync();
            return View(sanPhams);
        }

        // 2. THÊM SẢN PHẨM (GET)
        public IActionResult Them()
        {
            ViewBag.MaDanhMuc = new SelectList(_context.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc");
            return View();
        }

        // 2. THÊM SẢN PHẨM (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them(SanPham sanPham)
        {
            if (ModelState.IsValid)
            {
                _context.SanPhams.Add(sanPham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MaDanhMuc = new SelectList(_context.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }

        // 3. SỬA SẢN PHẨM (GET)
        public async Task<IActionResult> Sua(int? id)
        {
            if (id == null) return NotFound();

            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham == null) return NotFound();

            ViewBag.MaDanhMuc = new SelectList(_context.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }

        // 3. SỬA SẢN PHẨM (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sua(int id, SanPham sanPham)
        {
            if (id != sanPham.MaSanPham) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanPham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.SanPhams.Any(e => e.MaSanPham == sanPham.MaSanPham))
                        return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.MaDanhMuc = new SelectList(_context.DanhMucSanPhams, "MaDanhMuc", "TenDanhMuc", sanPham.MaDanhMuc);
            return View(sanPham);
        }

        // 4. XÓA SẢN PHẨM
        public async Task<IActionResult> Xoa(int id)
        {
            var sanPham = await _context.SanPhams.FindAsync(id);
            if (sanPham != null)
            {
                _context.SanPhams.Remove(sanPham);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}