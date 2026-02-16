using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlertHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Identifier = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Sender = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Sent = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    MessageType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Scope = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Source = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Restriction = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Note = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Addresses = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Codes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    References = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    Incidents = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RawPayload = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    IngestedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "alert_infos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlertId = table.Column<Guid>(type: "uuid", nullable: false),
                    Event = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Urgency = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Severity = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Certainty = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Language = table.Column<string>(type: "character varying(35)", maxLength: 35, nullable: true),
                    Audience = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Effective = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Onset = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Expires = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    SenderName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Headline = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Instruction = table.Column<string>(type: "text", nullable: true),
                    Web = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Contact = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alert_infos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alert_infos_alerts_AlertId",
                        column: x => x.AlertId,
                        principalTable: "alerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alert_areas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlertInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    AreaDescription = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Altitude = table.Column<double>(type: "double precision", nullable: true),
                    Ceiling = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alert_areas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alert_areas_alert_infos_AlertInfoId",
                        column: x => x.AlertInfoId,
                        principalTable: "alert_infos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alert_info_categories",
                columns: table => new
                {
                    AlertInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alert_info_categories", x => new { x.AlertInfoId, x.Category });
                    table.ForeignKey(
                        name: "FK_alert_info_categories_alert_infos_AlertInfoId",
                        column: x => x.AlertInfoId,
                        principalTable: "alert_infos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alert_info_event_codes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlertInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValueName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alert_info_event_codes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alert_info_event_codes_alert_infos_AlertInfoId",
                        column: x => x.AlertInfoId,
                        principalTable: "alert_infos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alert_info_parameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlertInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValueName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Value = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alert_info_parameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alert_info_parameters_alert_infos_AlertInfoId",
                        column: x => x.AlertInfoId,
                        principalTable: "alert_infos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alert_info_resources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlertInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResourceDescription = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    MimeType = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Size = table.Column<long>(type: "bigint", nullable: true),
                    Uri = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Digest = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alert_info_resources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alert_info_resources_alert_infos_AlertInfoId",
                        column: x => x.AlertInfoId,
                        principalTable: "alert_infos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alert_info_response_types",
                columns: table => new
                {
                    AlertInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResponseType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alert_info_response_types", x => new { x.AlertInfoId, x.ResponseType });
                    table.ForeignKey(
                        name: "FK_alert_info_response_types_alert_infos_AlertInfoId",
                        column: x => x.AlertInfoId,
                        principalTable: "alert_infos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alert_area_circles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlertAreaId = table.Column<Guid>(type: "uuid", nullable: false),
                    CenterLatitude = table.Column<double>(type: "double precision", nullable: false),
                    CenterLongitude = table.Column<double>(type: "double precision", nullable: false),
                    Radius = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alert_area_circles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alert_area_circles_alert_areas_AlertAreaId",
                        column: x => x.AlertAreaId,
                        principalTable: "alert_areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alert_area_geocodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlertAreaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValueName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alert_area_geocodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alert_area_geocodes_alert_areas_AlertAreaId",
                        column: x => x.AlertAreaId,
                        principalTable: "alert_areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alert_area_polygons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AlertAreaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Points = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alert_area_polygons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alert_area_polygons_alert_areas_AlertAreaId",
                        column: x => x.AlertAreaId,
                        principalTable: "alert_areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alert_area_circles_AlertAreaId",
                table: "alert_area_circles",
                column: "AlertAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_alert_area_geocodes_AlertAreaId",
                table: "alert_area_geocodes",
                column: "AlertAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_alert_area_geocodes_ValueName_Value",
                table: "alert_area_geocodes",
                columns: new[] { "ValueName", "Value" });

            migrationBuilder.CreateIndex(
                name: "IX_alert_area_polygons_AlertAreaId",
                table: "alert_area_polygons",
                column: "AlertAreaId");

            migrationBuilder.CreateIndex(
                name: "IX_alert_areas_AlertInfoId",
                table: "alert_areas",
                column: "AlertInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_alert_info_categories_Category",
                table: "alert_info_categories",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_alert_info_event_codes_AlertInfoId",
                table: "alert_info_event_codes",
                column: "AlertInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_alert_info_parameters_AlertInfoId",
                table: "alert_info_parameters",
                column: "AlertInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_alert_info_resources_AlertInfoId",
                table: "alert_info_resources",
                column: "AlertInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_alert_infos_AlertId",
                table: "alert_infos",
                column: "AlertId");

            migrationBuilder.CreateIndex(
                name: "IX_alert_infos_Certainty",
                table: "alert_infos",
                column: "Certainty");

            migrationBuilder.CreateIndex(
                name: "IX_alert_infos_Event",
                table: "alert_infos",
                column: "Event");

            migrationBuilder.CreateIndex(
                name: "IX_alert_infos_Expires",
                table: "alert_infos",
                column: "Expires");

            migrationBuilder.CreateIndex(
                name: "IX_alert_infos_Severity",
                table: "alert_infos",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_alert_infos_Urgency",
                table: "alert_infos",
                column: "Urgency");

            migrationBuilder.CreateIndex(
                name: "IX_alerts_Sender_Identifier",
                table: "alerts",
                columns: new[] { "Sender", "Identifier" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alert_area_circles");

            migrationBuilder.DropTable(
                name: "alert_area_geocodes");

            migrationBuilder.DropTable(
                name: "alert_area_polygons");

            migrationBuilder.DropTable(
                name: "alert_info_categories");

            migrationBuilder.DropTable(
                name: "alert_info_event_codes");

            migrationBuilder.DropTable(
                name: "alert_info_parameters");

            migrationBuilder.DropTable(
                name: "alert_info_resources");

            migrationBuilder.DropTable(
                name: "alert_info_response_types");

            migrationBuilder.DropTable(
                name: "alert_areas");

            migrationBuilder.DropTable(
                name: "alert_infos");

            migrationBuilder.DropTable(
                name: "alerts");
        }
    }
}
