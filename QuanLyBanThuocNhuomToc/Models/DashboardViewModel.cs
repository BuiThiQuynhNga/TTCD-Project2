namespace QuanLyBanThuocNhuomToc.Models
{
    public class DashboardViewModel
    {
        public int TongSanPham { get; set; }
        public int TongDanhMuc { get; set; }
        public int TongKhachHang { get; set; }

        public int TongDonHang { get; set; }
        public int DonChoXacNhan { get; set; }
        public int DonDangGiao { get; set; }

        public decimal DoanhThu { get; set; }

        public List<SanPham> SanPhamSapHet { get; set; } = new();
        public List<DonHang> DonHangGanDay { get; set; } = new();
    }
}