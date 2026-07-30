using Microsoft.EntityFrameworkCore;
using QuanLyBanThuocNhuomToc.Models;

namespace QuanLyBanThuocNhuomToc.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<DanhMucSanPham> DanhMucSanPhams { get; set; }
        public DbSet<SanPham> SanPhams { get; set; }
        public DbSet<MaGiamGia> MaGiamGias { get; set; }
        public DbSet<DonHang> DonHangs { get; set; }
        public DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public DbSet<DanhGia> DanhGias { get; set; }
    }
}