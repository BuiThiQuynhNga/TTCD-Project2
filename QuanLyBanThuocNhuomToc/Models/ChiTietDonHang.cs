using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanThuocNhuomToc.Models
{
    [Table("ChiTietDonHang")]
    public class ChiTietDonHang
    {
        [Key]
        public int MaChiTietDonHang { get; set; }

        public int MaDonHang { get; set; }
        [ForeignKey("MaDonHang")]
        public DonHang DonHang { get; set; } = null!;

        public int MaSanPham { get; set; }
        [ForeignKey("MaSanPham")]
        public SanPham SanPham { get; set; } = null!;

        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }
    }
}