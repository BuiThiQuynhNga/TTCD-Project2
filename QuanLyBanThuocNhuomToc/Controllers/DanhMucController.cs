using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanThuocNhuomToc.Data;
using QuanLyBanThuocNhuomToc.Models;

namespace QuanLyBanThuocNhuomToc.Controllers
{
    public class DanhMucController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DanhMucController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /DanhMuc — Ai cũng xem được, không cần đăng nhập
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.DanhMucSanPhams
                .Include(d => d.SanPhams)
                .ToListAsync();

            return View(danhSach);
        }

        // GET: /DanhMuc/Them — Chỉ Admin
        [Authorize(Roles = "Quản trị")]
        public IActionResult Them()
        {
            return View();
        }

        // POST: /DanhMuc/Them — Chỉ Admin
        [Authorize(Roles = "Quản trị")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Them(DanhMucSanPham danhMuc)
        {
            if (!ModelState.IsValid)
            {
                return View(danhMuc);
            }

            bool trung = await _context.DanhMucSanPhams
                .AnyAsync(d => d.TenDanhMuc.ToLower() == danhMuc.TenDanhMuc.ToLower());

            if (trung)
            {
                ModelState.AddModelError("TenDanhMuc", "Tên danh mục này đã tồn tại.");
                return View(danhMuc);
            }

            _context.DanhMucSanPhams.Add(danhMuc);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Thêm danh mục thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /DanhMuc/Sua/5 — Chỉ Admin
        [Authorize(Roles = "Quản trị")]
        public async Task<IActionResult> Sua(int id)
        {
            var danhMuc = await _context.DanhMucSanPhams.FindAsync(id);
            if (danhMuc == null)
            {
                return NotFound();
            }

            return View(danhMuc);
        }

        // POST: /DanhMuc/Sua/5 — Chỉ Admin
        [Authorize(Roles = "Quản trị")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sua(int id, DanhMucSanPham danhMuc)
        {
            if (id != danhMuc.MaDanhMuc)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(danhMuc);
            }

            bool trung = await _context.DanhMucSanPhams
                .AnyAsync(d => d.TenDanhMuc.ToLower() == danhMuc.TenDanhMuc.ToLower()
                            && d.MaDanhMuc != id);

            if (trung)
            {
                ModelState.AddModelError("TenDanhMuc", "Tên danh mục này đã tồn tại.");
                return View(danhMuc);
            }

            try
            {
                _context.Update(danhMuc);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.DanhMucSanPhams.AnyAsync(d => d.MaDanhMuc == id))
                    return NotFound();
                throw;
            }

            TempData["ThongBao"] = "Cập nhật danh mục thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /DanhMuc/Xoa/5 — Chỉ Admin
        [Authorize(Roles = "Quản trị")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Xoa(int id)
        {
            var danhMuc = await _context.DanhMucSanPhams
                .Include(d => d.SanPhams)
                .FirstOrDefaultAsync(d => d.MaDanhMuc == id);

            if (danhMuc == null)
            {
                return NotFound();
            }

            if (danhMuc.SanPhams != null && danhMuc.SanPhams.Any())
            {
                TempData["LoiXoa"] = $"Không thể xóa \"{danhMuc.TenDanhMuc}\" vì còn {danhMuc.SanPhams.Count} sản phẩm thuộc danh mục này.";
                return RedirectToAction(nameof(Index));
            }

            _context.DanhMucSanPhams.Remove(danhMuc);
            await _context.SaveChangesAsync();

            TempData["ThongBao"] = "Xóa danh mục thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}