using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanThuocNhuomToc.Models
{
    [Table("MaGiamGia")]
    public class MaGiamGia
    {
        [Key]
        public int MaVoucher { get; set; }

        [Required]
        [StringLength(50)]
        public string TenVoucher { get; set; } = string.Empty;

        public int PhanTramGiam { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? GiaTriToiDa { get; set; }

        public DateTime NgayHetHan { get; set; }

        public bool TrangThai { get; set; } = true;

        // Tổng số mã được phát hành
        public int SoLuong { get; set; }

        // Số mã đã được sử dụng (tăng lên mỗi khi có đơn hàng thanh toán thành công dùng mã này)
        public int SoLuongDaDung { get; set; } = 0;

        [NotMapped]
        public int SoLuongConLai => SoLuong - SoLuongDaDung;

        [NotMapped]
        public bool ConLuot => SoLuongConLai > 0;

        public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
    }
}