using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionEnvios.Api.Migrations
{
    /// <inheritdoc />
    public partial class CreacionInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Envios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumeroGuia = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Transportadora = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreRemitente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CalleRemitente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CiudadRemitente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoRemitente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoPostalRemitente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelefonoRemitente = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreDestinatario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CalleDestinatario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CiudadDestinatario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstadoDestinatario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodigoPostalDestinatario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelefonoDestinatario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PesoKilos = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    LargoCentimetros = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    AnchoCentimetros = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    AltoCentimetros = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    EtiquetaPdfBytes = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    CodigoConfirmacionRecoleccion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEntregaTransportadora = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Envios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HistorialEnvios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnvioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialEnvios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialEnvios_Envios_EnvioId",
                        column: x => x.EnvioId,
                        principalTable: "Envios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Envios_NumeroGuia",
                table: "Envios",
                column: "NumeroGuia",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEnvios_EnvioId",
                table: "HistorialEnvios",
                column: "EnvioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialEnvios");

            migrationBuilder.DropTable(
                name: "Envios");
        }
    }
}
