using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanThuocNhuomToc.Models
{
    [Table("DonHang")]
    public class DonHang
    {
        [Key]
        public int MaDonHang { get; set; }

        public int MaNguoiDung { get; set; }
        [ForeignKey("MaNguoiDung")]
        public NguoiDung NguoiDung { get; set; } = null!;

        public int? MaVoucher { get; set; }
        [ForeignKey("MaVoucher")]
        public MaGiamGia? MaGiamGia { get; set; }

        public DateTime NgayDatHang { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        [StringLength(50)]
        public string TrangThaiDonHang { get; set; } = "Chờ xác nhận";

        [Required]
        [StringLength(255)]
        public string DiaChiGiaoHang { get; set; } = string.Empty; // Đã sửa từ get: set thành get; set

        [Required]
        [StringLength(15)]
        public string SoDienThoaiNhan { get; set; } = string.Empty;

        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();
    }
}