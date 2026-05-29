using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AeroSoilAI.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_PROPRIEDADE",
                columns: table => new
                {
                    ID_PROPRIEDADE = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_PROPRIEDADE = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    DS_LOCALIZACAO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    NR_HECTARES = table.Column<decimal>(type: "NUMBER(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PROPRIEDADE", x => x.ID_PROPRIEDADE);
                });

            migrationBuilder.CreateTable(
                name: "TB_SENSOR",
                columns: table => new
                {
                    ID_SENSOR = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TP_SENSOR = table.Column<string>(type: "VARCHAR2(20)", maxLength: 20, nullable: false),
                    VL_ULTIMA_LEITURA = table.Column<decimal>(type: "NUMBER(10,2)", precision: 10, scale: 2, nullable: false),
                    DT_ATUALIZACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ID_PROPRIEDADE = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_SENSOR", x => x.ID_SENSOR);
                    table.ForeignKey(
                        name: "FK_TB_SENSOR_TB_PROPRIEDADE_ID_PROPRIEDADE",
                        column: x => x.ID_PROPRIEDADE,
                        principalTable: "TB_PROPRIEDADE",
                        principalColumn: "ID_PROPRIEDADE",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_SENSOR_ID_PROPRIEDADE",
                table: "TB_SENSOR",
                column: "ID_PROPRIEDADE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_SENSOR");

            migrationBuilder.DropTable(
                name: "TB_PROPRIEDADE");
        }
    }
}
