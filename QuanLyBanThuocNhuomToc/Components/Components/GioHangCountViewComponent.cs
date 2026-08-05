using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyBanThuocNhuomToc.Data;

namespace QuanLyBanThuocNhuomToc.Components
{
    public class GioHangCountViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public GioHangCountViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int soLuong = 0;

            if (UserClaimsPrincipal.Identity != null && UserClaimsPrincipal.Identity.IsAuthenticated)
            {
                var claim = UserClaimsPrincipal.FindFirst("MaNguoiDung");
                if (claim != null)
                {
                    int maNguoiDung = int.Parse(claim.Value);

                    soLuong = await _context.ChiTietDonHangs
                        .Where(ct => ct.DonHang.MaNguoiDung == maNguoiDung
                                  && ct.DonHang.TrangThaiDonHang == "Giỏ hàng")
                        .SumAsync(ct => (int?)ct.SoLuong) ?? 0;
                }
            }

            return View(soLuong);
        }
    }
}