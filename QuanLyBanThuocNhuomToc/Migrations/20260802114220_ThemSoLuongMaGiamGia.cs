using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyBanThuocNhuomToc.Migrations
{
    /// <inheritdoc />
    public partial class ThemSoLuongMaGiamGia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SoLuong",
                table: "MaGiamGia",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SoLuongDaDung",
                table: "MaGiamGia",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoLuong",
                table: "MaGiamGia");

            migrationBuilder.DropColumn(
                name: "SoLuongDaDung",
                table: "MaGiamGia");
        }
    }
}
