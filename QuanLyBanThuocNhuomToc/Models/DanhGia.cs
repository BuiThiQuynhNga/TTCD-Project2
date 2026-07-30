using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanThuocNhuomToc.Models
{
    [Table("DanhGia")]
    public class DanhGia
    {
        [Key]
        public int MaDanhGia { get; set; }

        public int MaSanPham { get; set; }
        [ForeignKey("MaSanPham")]
        public SanPham SanPham { get; set; } = null!;

        public int MaNguoiDung { get; set; }
        [ForeignKey("MaNguoiDung")]
        public NguoiDung NguoiDung { get; set; } = null!;

        public int SoSao { get; set; }

        public string? BinhLuan { get; set; }

        public DateTime NgayDanhGia { get; set; } = DateTime.Now;
    }
}