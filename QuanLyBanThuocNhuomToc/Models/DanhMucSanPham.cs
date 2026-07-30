using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanThuocNhuomToc.Models
{
    [Table("DanhMucSanPham")]
    public class DanhMucSanPham
    {
        [Key]
        public int MaDanhMuc { get; set; }

        [Required]
        [StringLength(100)]
        public string TenDanhMuc { get; set; } = string.Empty;

        [StringLength(500)]
        public string? MoTa { get; set; }

        public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
    }
}