using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanThuocNhuomToc.Models
{
    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key]
        public int MaNguoiDung { get; set; }

        [Required]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string MatKhau { get; set; } = string.Empty;

        [StringLength(15)]
        public string? SoDienThoai { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        [StringLength(20)]
        public string VaiTro { get; set; } = "Khách hàng";

        public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
        public ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
    }
}