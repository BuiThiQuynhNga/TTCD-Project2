using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanThuocNhuomToc.Data;
using QuanLyBanThuocNhuomToc.Models;

namespace QuanLyBanThuocNhuomToc.Controllers
{
    public class MaGiamGiaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaGiamGiaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /MaGiamGia  — CÔNG KHAI, ai cũng xem được, chỉ thấy tên/%/còn-hết
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var danhSach = await _context.Set<MaGiamGia>()
                .Where(v => v.TrangThai && v.NgayHetHan >= DateTime.Now)
                .OrderByDescending(v => v.MaVoucher)
                .ToListAsync();
            return View(danhSach);
        }

        // GET: /MaGiamGia/QuanLy — CHỈ QUẢN TRỊ, xem đầy đủ + thao tác
        [Authorize(Roles = "Quản trị")]
        public async Task<IActionResult> QuanLy()
        {
            var danhSach = await _context.Set<MaGiamGia>()
                .OrderByDescending(v => v.MaVoucher)
                .ToListAsync();
            return View(danhSach);
        }

        // GET: /MaGiamGia/Create
        [Authorize(Roles = "Quản trị")]
        public IActionResult Create()
        {
            return View(new MaGiamGia
            {
                NgayHetHan = DateTime.Now.AddDays(7),
                TrangThai = true,
                SoLuong = 100
            });
        }

        // POST: /MaGiamGia/Create
        [Authorize(Roles = "Quản trị")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaGiamGia model)
        {
            if (string.IsNullOrWhiteSpace(model.TenVoucher))
            {
                ModelState.AddModelError(nameof(model.TenVoucher), "Vui lòng nhập mã voucher.");
            }
            else
            {
                bool trung = await _context.Set<MaGiamGia>()
                    .AnyAsync(v => v.TenVoucher.ToLower() == model.TenVoucher.Trim().ToLower());
                if (trung)
                {
                    ModelState.AddModelError(nameof(model.TenVoucher), "Mã voucher này đã tồn tại.");
                }
            }

            if (model.PhanTramGiam <= 0 || model.PhanTramGiam > 100)
            {
                ModelState.AddModelError(nameof(model.PhanTramGiam), "Phần trăm giảm phải trong khoảng 1-100.");
            }

            if (model.GiaTriToiDa.HasValue && model.GiaTriToiDa <= 0)
            {
                ModelState.AddModelError(nameof(model.GiaTriToiDa), "Giá trị giảm tối đa phải lớn hơn 0.");
            }

            if (model.SoLuong <= 0)
            {
                ModelState.AddModelError(nameof(model.SoLuong), "Số lượng mã phải lớn hơn 0.");
            }

            if (model.NgayHetHan <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(model.NgayHetHan), "Ngày hết hạn phải ở tương lai.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.TenVoucher = model.TenVoucher.Trim().ToUpper();
            model.SoLuongDaDung = 0;
            _context.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã tạo mã giảm giá \"{model.TenVoucher}\".";
            return RedirectToAction(nameof(QuanLy));
        }

        // GET: /MaGiamGia/Edit/5
        [Authorize(Roles = "Quản trị")]
        public async Task<IActionResult> Edit(int id)
        {
            var voucher = await _context.Set<MaGiamGia>().FindAsync(id);
            if (voucher == null) return NotFound();
            return View(voucher);
        }

        // POST: /MaGiamGia/Edit/5
        [Authorize(Roles = "Quản trị")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MaGiamGia model)
        {
            if (id != model.MaVoucher) return NotFound();

            var voucher = await _context.Set<MaGiamGia>().FindAsync(id);
            if (voucher == null) return NotFound();

            if (model.PhanTramGiam <= 0 || model.PhanTramGiam > 100)
            {
                ModelState.AddModelError(nameof(model.PhanTramGiam), "Phần trăm giảm phải trong khoảng 1-100.");
            }

            if (model.SoLuong < voucher.SoLuongDaDung)
            {
                ModelState.AddModelError(nameof(model.SoLuong), $"Số lượng mới không được nhỏ hơn số đã dùng ({voucher.SoLuongDaDung}).");
            }

            if (!ModelState.IsValid)
            {
                model.TenVoucher = voucher.TenVoucher;
                model.SoLuongDaDung = voucher.SoLuongDaDung;
                return View(model);
            }

            voucher.PhanTramGiam = model.PhanTramGiam;
            voucher.GiaTriToiDa = model.GiaTriToiDa;
            voucher.NgayHetHan = model.NgayHetHan;
            voucher.TrangThai = model.TrangThai;
            voucher.SoLuong = model.SoLuong; // Admin điều chỉnh số lượng mã tại đây

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã cập nhật mã giảm giá.";
            return RedirectToAction(nameof(QuanLy));
        }

        // POST: /MaGiamGia/ToggleTrangThai/5
        [Authorize(Roles = "Quản trị")]
        [HttpPost]
        public async Task<IActionResult> ToggleTrangThai(int id)
        {
            var voucher = await _context.Set<MaGiamGia>().FindAsync(id);
            if (voucher == null) return NotFound();

            voucher.TrangThai = !voucher.TrangThai;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(QuanLy));
        }

        // POST: /MaGiamGia/Delete/5
        [Authorize(Roles = "Quản trị")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var voucher = await _context.Set<MaGiamGia>().FindAsync(id);
            if (voucher == null) return NotFound();

            bool dangDuocDung = await _context.DonHangs.AnyAsync(d => d.MaVoucher == id);
            if (dangDuocDung)
            {
                TempData["ErrorMessage"] = "Không thể xóa: mã này đang được gắn với đơn hàng/giỏ hàng. Hãy tắt trạng thái thay vì xóa.";
                return RedirectToAction(nameof(QuanLy));
            }

            _context.Remove(voucher);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã xóa mã giảm giá.";
            return RedirectToAction(nameof(QuanLy));
        }
    }
}