using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace QuanLyBanThuocNhuomToc.Models
{
    [Table("SanPham")]
    public class SanPham
    {
        [Key]
        public int MaSanPham { get; set; }

        public int MaDanhMuc { get; set; }

        [ForeignKey("MaDanhMuc")]
        [ValidateNever]
        public DanhMucSanPham DanhMucSanPham { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string TenSanPham { get; set; } = string.Empty;

        [StringLength(50)]
        public string? MaMau { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaBan { get; set; }

        public int SoLuongTon { get; set; }

        [StringLength(255)]
        public string? HinhAnh { get; set; }

        public string? MoTaSanPham { get; set; }

        [ValidateNever]
        public ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

        [ValidateNever]
        public ICollection<DanhGia> DanhGias { get; set; } = new List<DanhGia>();
    }
}