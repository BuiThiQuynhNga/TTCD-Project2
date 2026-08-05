using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanThuocNhuomToc.Data;
using QuanLyBanThuocNhuomToc.Models;

namespace QuanLyBanThuocNhuomToc.Controllers
{
    [Authorize]
    public class GioHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        private const decimal NguongMienPhiShip = 300000m;
        private const decimal PhiShipCoDinh = 30000m;
        public GioHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetMaNguoiDung()
        {
            var claim = User.FindFirst("MaNguoiDung");
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        private async Task<DonHang> LayHoacTaoGioHang()
        {
            int maNguoiDung = GetMaNguoiDung();

            var gioHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPham)
                .Include(d => d.MaGiamGia)
                .FirstOrDefaultAsync(d => d.MaNguoiDung == maNguoiDung && d.TrangThaiDonHang == "Giỏ hàng");

            if (gioHang == null)
            {
                gioHang = new DonHang
                {
                    MaNguoiDung = maNguoiDung,
                    NgayDatHang = DateTime.Now,
                    TongTien = 0,
                    TrangThaiDonHang = "Giỏ hàng",
                    DiaChiGiaoHang = "",
                    SoDienThoaiNhan = ""
                };
                _context.DonHangs.Add(gioHang);
                await _context.SaveChangesAsync();
            }

            return gioHang;
        }

        // Tính lại tổng tiền: áp dụng % giảm giá (nếu có voucher hợp lệ), giới hạn bởi GiaTriToiDa
        private async Task CapNhatTongTien(DonHang gioHang)
        {
            decimal tongTruocGiam = gioHang.ChiTietDonHangs.Sum(ct => ct.SoLuong * ct.DonGia);
            decimal soTienGiam = 0;

            if (gioHang.MaGiamGia != null)
            {
                bool conHan = gioHang.MaGiamGia.TrangThai && gioHang.MaGiamGia.NgayHetHan >= DateTime.Now;

                if (conHan)
                {
                    soTienGiam = tongTruocGiam * gioHang.MaGiamGia.PhanTramGiam / 100m;

                    if (gioHang.MaGiamGia.GiaTriToiDa.HasValue && soTienGiam > gioHang.MaGiamGia.GiaTriToiDa.Value)
                    {
                        soTienGiam = gioHang.MaGiamGia.GiaTriToiDa.Value;
                    }
                }
                else
                {
                    gioHang.MaVoucher = null;
                }
            }

            decimal tongSauGiam = tongTruocGiam - soTienGiam;

            // Tính phí ship: miễn phí nếu tổng sau giảm đạt ngưỡng, ngược lại tính phí cố định
            gioHang.PhiShip = (tongSauGiam >= NguongMienPhiShip || tongSauGiam <= 0) ? 0 : PhiShipCoDinh;

            gioHang.TongTien = tongSauGiam + gioHang.PhiShip;
            _context.DonHangs.Update(gioHang);
            await _context.SaveChangesAsync();
        }
        // GET: /GioHang
        public async Task<IActionResult> Index()
        {
            var gioHang = await LayHoacTaoGioHang();
            return View(gioHang);
        }

        // POST: /GioHang/ThemVaoGio
        [HttpPost]
        public async Task<IActionResult> ThemVaoGio(int maSanPham, int soLuong = 1)
        {
            var sanPham = await _context.SanPhams.FindAsync(maSanPham);
            if (sanPham == null)
            {
                return NotFound();
            }

            var gioHang = await LayHoacTaoGioHang();

            var chiTiet = gioHang.ChiTietDonHangs.FirstOrDefault(ct => ct.MaSanPham == maSanPham);
            if (chiTiet != null)
            {
                chiTiet.SoLuong += soLuong;
            }
            else
            {
                chiTiet = new ChiTietDonHang
                {
                    MaDonHang = gioHang.MaDonHang,
                    MaSanPham = maSanPham,
                    SoLuong = soLuong,
                    DonGia = sanPham.GiaBan
                };
                _context.ChiTietDonHangs.Add(chiTiet);
            }

            await _context.SaveChangesAsync();

            gioHang.ChiTietDonHangs = await _context.ChiTietDonHangs
                .Where(ct => ct.MaDonHang == gioHang.MaDonHang)
                .ToListAsync();
            await CapNhatTongTien(gioHang);

            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng.";
            return RedirectToAction("Index", "SanPham");
        }

        // POST: /GioHang/CapNhatSoLuong
        [HttpPost]
        public async Task<IActionResult> CapNhatSoLuong(int maChiTiet, int soLuong)
        {
            var chiTiet = await _context.ChiTietDonHangs
                .Include(ct => ct.DonHang)
                .FirstOrDefaultAsync(ct => ct.MaChiTietDonHang == maChiTiet);

            if (chiTiet == null || chiTiet.DonHang.MaNguoiDung != GetMaNguoiDung())
            {
                return NotFound();
            }

            if (soLuong <= 0)
            {
                _context.ChiTietDonHangs.Remove(chiTiet);
            }
            else
            {
                chiTiet.SoLuong = soLuong;
            }
            await _context.SaveChangesAsync();

            var gioHang = await LayHoacTaoGioHang();
            await CapNhatTongTien(gioHang);

            return RedirectToAction("Index");
        }

        // POST: /GioHang/XoaSanPham
        [HttpPost]
        public async Task<IActionResult> XoaSanPham(int maChiTiet)
        {
            var chiTiet = await _context.ChiTietDonHangs
                .Include(ct => ct.DonHang)
                .FirstOrDefaultAsync(ct => ct.MaChiTietDonHang == maChiTiet);

            if (chiTiet == null || chiTiet.DonHang.MaNguoiDung != GetMaNguoiDung())
            {
                return NotFound();
            }

            _context.ChiTietDonHangs.Remove(chiTiet);
            await _context.SaveChangesAsync();

            var gioHang = await LayHoacTaoGioHang();
            await CapNhatTongTien(gioHang);

            return RedirectToAction("Index");
        }

        // POST: /GioHang/ApDungMaGiamGia
        [HttpPost]
        public async Task<IActionResult> ApDungMaGiamGia(string tenVoucher)
        {
            var gioHang = await LayHoacTaoGioHang();

            if (string.IsNullOrWhiteSpace(tenVoucher))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập mã giảm giá.";
                return RedirectToAction("Index");
            }

            var voucher = await _context.Set<MaGiamGia>()
                .FirstOrDefaultAsync(v => v.TenVoucher.ToLower() == tenVoucher.Trim().ToLower());

            if (voucher == null)
            {
                TempData["ErrorMessage"] = "Mã giảm giá không tồn tại.";
                return RedirectToAction("Index");
            }

            if (!voucher.TrangThai)
            {
                TempData["ErrorMessage"] = "Mã giảm giá này đã ngừng hoạt động.";
                return RedirectToAction("Index");
            }

            if (voucher.NgayHetHan < DateTime.Now)
            {
                TempData["ErrorMessage"] = "Mã giảm giá này đã hết hạn.";
                return RedirectToAction("Index");
            }
            if (voucher.SoLuongConLai <= 0)
            {
                TempData["ErrorMessage"] = "Mã giảm giá này đã hết lượt sử dụng.";
                return RedirectToAction("Index");
            }

            gioHang.MaVoucher = voucher.MaVoucher;
            gioHang.MaGiamGia = voucher;
            await CapNhatTongTien(gioHang);

            TempData["SuccessMessage"] = $"Áp dụng mã \"{voucher.TenVoucher}\" thành công! Giảm {voucher.PhanTramGiam}%.";
            return RedirectToAction("Index");
        }

        // POST: /GioHang/GoMaGiamGia
        [HttpPost]
        public async Task<IActionResult> GoMaGiamGia()
        {
            var gioHang = await LayHoacTaoGioHang();
            gioHang.MaVoucher = null;
            gioHang.MaGiamGia = null;
            await CapNhatTongTien(gioHang);

            TempData["SuccessMessage"] = "Đã gỡ mã giảm giá.";
            return RedirectToAction("Index");
        }
        // GET: /GioHang/ThanhToan
        public async Task<IActionResult> ThanhToan()
        {
            var gioHang = await LayHoacTaoGioHang();
            if (!gioHang.ChiTietDonHangs.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng đang trống.";
                return RedirectToAction("Index");
            }

            var thieuHang = gioHang.ChiTietDonHangs
                .Where(ct => ct.SoLuong > ct.SanPham.SoLuongTon)
                .ToList();

            if (thieuHang.Any())
            {
                var tenSp = string.Join(", ", thieuHang.Select(ct => ct.SanPham.TenSanPham));
                TempData["ErrorMessage"] = $"Sản phẩm sau không đủ hàng trong kho: {tenSp}. Vui lòng cập nhật lại số lượng.";
                return RedirectToAction("Index");
            }

            return View(gioHang);
        }

        // POST: /GioHang/ThanhToan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToan(string diaChiGiaoHang, string soDienThoaiNhan)
        {
            var gioHang = await LayHoacTaoGioHang();

            if (string.IsNullOrWhiteSpace(diaChiGiaoHang) || string.IsNullOrWhiteSpace(soDienThoaiNhan))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ địa chỉ và số điện thoại nhận hàng.");
                return View(gioHang);
            }

            // Kiểm tra lại tồn kho lần cuối trước khi chốt đơn (phòng trường hợp thay đổi giữa lúc xem và lúc đặt)
            var thieuHang = gioHang.ChiTietDonHangs
                .Where(ct => ct.SoLuong > ct.SanPham.SoLuongTon)
                .ToList();

            if (thieuHang.Any())
            {
                var tenSp = string.Join(", ", thieuHang.Select(ct => ct.SanPham.TenSanPham));
                TempData["ErrorMessage"] = $"Sản phẩm sau không đủ hàng trong kho: {tenSp}. Vui lòng cập nhật lại số lượng.";
                return RedirectToAction("Index");
            }

            // Trừ tồn kho cho từng sản phẩm trong đơn
            foreach (var ct in gioHang.ChiTietDonHangs)
            {
                ct.SanPham.SoLuongTon -= ct.SoLuong;
            }

            gioHang.DiaChiGiaoHang = diaChiGiaoHang;
            gioHang.SoDienThoaiNhan = soDienThoaiNhan;
            gioHang.TrangThaiDonHang = "Chờ xác nhận";
            gioHang.NgayDatHang = DateTime.Now;

            // Trừ lượt sử dụng của mã giảm giá (nếu có)
            if (gioHang.MaGiamGia != null)
            {
                gioHang.MaGiamGia.SoLuongDaDung += 1;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đặt hàng thành công!";
            return RedirectToAction("Index", "Home");
        }
    }
}