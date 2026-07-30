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

        public ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
    }
}