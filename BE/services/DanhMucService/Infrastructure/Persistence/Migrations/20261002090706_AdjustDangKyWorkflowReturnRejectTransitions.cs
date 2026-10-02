using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DanhMucService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AdjustDangKyWorkflowReturnRejectTransitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "dm",
                table: "DanhMucChuyenBuocQuyTrinhs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333324"),
                column: "TuBuocId",
                value: new Guid("33333333-3333-3333-3333-333333333312"));

            migrationBuilder.UpdateData(
                schema: "dm",
                table: "DanhMucChuyenBuocQuyTrinhs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333325"),
                column: "TuBuocId",
                value: new Guid("33333333-3333-3333-3333-333333333312"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "dm",
                table: "DanhMucChuyenBuocQuyTrinhs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333324"),
                column: "TuBuocId",
                value: new Guid("33333333-3333-3333-3333-333333333313"));

            migrationBuilder.UpdateData(
                schema: "dm",
                table: "DanhMucChuyenBuocQuyTrinhs",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333325"),
                column: "TuBuocId",
                value: new Guid("33333333-3333-3333-3333-333333333313"));
        }
    }
}
