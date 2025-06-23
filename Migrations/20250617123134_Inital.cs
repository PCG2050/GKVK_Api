using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GKVK_Api.Migrations
{
    /// <inheritdoc />
    public partial class Inital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrgUnits",
                columns: table => new
                {
                    OrgUnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    TemplateSourceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgUnits", x => x.OrgUnitId);
                    table.ForeignKey(
                        name: "FK_OrgUnits_OrgUnits_ParentId",
                        column: x => x.ParentId,
                        principalTable: "OrgUnits",
                        principalColumn: "OrgUnitId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrgUnits_OrgUnits_TemplateSourceId",
                        column: x => x.TemplateSourceId,
                        principalTable: "OrgUnits",
                        principalColumn: "OrgUnitId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "TableDefinitions",
                columns: table => new
                {
                    TableDefinitionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    OrgUnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableDefinitions", x => x.TableDefinitionId);
                    table.ForeignKey(
                        name: "FK_TableDefinitions_OrgUnits_OrgUnitId",
                        column: x => x.OrgUnitId,
                        principalTable: "OrgUnits",
                        principalColumn: "OrgUnitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    RefreshTokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FK_UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.RefreshTokenId);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_FK_UserId",
                        column: x => x.FK_UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrainerUnitAssignments",
                columns: table => new
                {
                    TrainerUnitAssignmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OrgUnitId = table.Column<int>(type: "int", nullable: false),
                    AssignedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnassignedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerUnitAssignments", x => x.TrainerUnitAssignmentId);
                    table.ForeignKey(
                        name: "FK_TrainerUnitAssignments_OrgUnits_OrgUnitId",
                        column: x => x.OrgUnitId,
                        principalTable: "OrgUnits",
                        principalColumn: "OrgUnitId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrainerUnitAssignments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableColumns",
                columns: table => new
                {
                    TableColumnId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    TableDefinitionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableColumns", x => x.TableColumnId);
                    table.ForeignKey(
                        name: "FK_TableColumns_TableDefinitions_TableDefinitionId",
                        column: x => x.TableDefinitionId,
                        principalTable: "TableDefinitions",
                        principalColumn: "TableDefinitionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableDataRows",
                columns: table => new
                {
                    TableDataRowId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableDefinitionId = table.Column<int>(type: "int", nullable: false),
                    OrgUnitId = table.Column<int>(type: "int", nullable: false),
                    TrainerId = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableDataRows", x => x.TableDataRowId);
                    table.ForeignKey(
                        name: "FK_TableDataRows_OrgUnits_OrgUnitId",
                        column: x => x.OrgUnitId,
                        principalTable: "OrgUnits",
                        principalColumn: "OrgUnitId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TableDataRows_TableDefinitions_TableDefinitionId",
                        column: x => x.TableDefinitionId,
                        principalTable: "TableDefinitions",
                        principalColumn: "TableDefinitionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableDataRows_Users_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TableDataCells",
                columns: table => new
                {
                    TableDataCellId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TableDataRowId = table.Column<int>(type: "int", nullable: false),
                    TableColumnId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableDataCells", x => x.TableDataCellId);
                    table.ForeignKey(
                        name: "FK_TableDataCells_TableColumns_TableColumnId",
                        column: x => x.TableColumnId,
                        principalTable: "TableColumns",
                        principalColumn: "TableColumnId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TableDataCells_TableDataRows_TableDataRowId",
                        column: x => x.TableDataRowId,
                        principalTable: "TableDataRows",
                        principalColumn: "TableDataRowId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "OrgUnits",
                columns: new[] { "OrgUnitId", "Name", "ParentId", "TemplateSourceId" },
                values: new object[,]
                {
                    { 1000, "KVK Template", null, null },
                    { 2000, "Farmers Training Institute (FTI)", null, null },
                    { 3000, "Staff Training Unit (STU)", null, null },
                    { 4000, "Farm Information Unit (FIU)", null, null },
                    { 5000, "Institute of Baking Technology & Value Addition (IBT&VA)", null, null },
                    { 6000, "Agricultural Technology Information Centre (ATIC)", null, null },
                    { 7000, "Distance Education Unit (DEU)", null, null },
                    { 8000, "Agricultural Sciences Museum (ASM)", null, null },
                    { 9000, "National Agriculture Extension Project (NAEP)", null, null },
                    { 9500, "Extension Education Unit (EEU)", null, null },
                    { 3001, "SAMETI – DESI Programme", 3000, null }
                });

            migrationBuilder.InsertData(
                table: "TableDefinitions",
                columns: new[] { "TableDefinitionId", "DisplayOrder", "OrgUnitId", "Title" },
                values: new object[,]
                {
                    { 1101, 1, 1000, "1. OFT" },
                    { 1102, 2, 1000, "2. FLD" },
                    { 1103, 3, 1000, "3. On-Campus Training Programs Conducted" },
                    { 1104, 4, 1000, "4. Off-Campus Training Programs Conducted" },
                    { 1321, 321, 1000, "32.1 Short Messages" },
                    { 1322, 322, 1000, "32.2 Expert Center" },
                    { 1323, 323, 1000, "32.3 Other E‑KVK Activities" },
                    { 1400, 400, 1000, "40. Any Other Story" },
                    { 2101, 1, 2000, "Training Programmes Organized" },
                    { 2102, 2, 2000, "Any Other Activities" },
                    { 3101, 1, 3000, "Training Programmes Organized" },
                    { 3102, 2, 3000, "Sponsored Training Programmes Organized" },
                    { 3103, 3, 3000, "Any Other Activities" },
                    { 4101, 1, 4000, "Activities / Programmes" },
                    { 5101, 1, 5000, "Programmes Organized" },
                    { 5102, 2, 5000, "Any Other Activities" },
                    { 6101, 1, 6000, "Advisory Services" },
                    { 6102, 2, 6000, "Sale of Inputs / Plant Materials" },
                    { 6103, 3, 6000, "Any Other Activities" },
                    { 7101, 1, 7000, "Diploma Courses" },
                    { 7102, 2, 7000, "Certificate Courses" },
                    { 7103, 3, 7000, "Any Other Activities" },
                    { 8101, 1, 8000, "Museum Visits" },
                    { 9101, 1, 9000, "Extension Programmes" },
                    { 9511, 1, 9500, "OFT (On‑Farm Trials)" },
                    { 9512, 2, 9500, "FLD (Frontline Demonstrations)" },
                    { 9513, 3, 9500, "Training Programmes Organized" }
                });

            migrationBuilder.InsertData(
                table: "TableColumns",
                columns: new[] { "TableColumnId", "DataType", "Name", "Position", "TableDefinitionId" },
                values: new object[,]
                {
                    { 110101, 0, "Title", 1, 1101 },
                    { 110102, 0, "Crop", 2, 1101 },
                    { 110103, 5, "Area(ha)", 3, 1101 },
                    { 110104, 1, "NoOfTrials_Male_SC/ST", 4, 1101 },
                    { 110105, 1, "NoOfTrials_Male_Gen", 5, 1101 },
                    { 110106, 1, "NoOfTrials_Female_SC/ST", 6, 1101 },
                    { 110107, 1, "NoOfTrials_Female_Gen", 7, 1101 },
                    { 110108, 5, "Yield(q/ha)_Y1", 8, 1101 },
                    { 110109, 5, "Yield(q/ha)_Y2", 9, 1101 },
                    { 110110, 5, "%IncreaseOverT2", 10, 1101 },
                    { 110201, 0, "Title", 1, 1102 },
                    { 110202, 0, "Crop", 2, 1102 },
                    { 110203, 5, "Area(ha)", 3, 1102 },
                    { 110204, 1, "NoOfTrials_Male_SC/ST", 4, 1102 },
                    { 110205, 1, "NoOfTrials_Male_Gen", 5, 1102 },
                    { 110206, 1, "NoOfTrials_Female_SC/ST", 6, 1102 },
                    { 110207, 1, "NoOfTrials_Female_Gen", 7, 1102 },
                    { 110208, 5, "Yield(q/ha)_Y1", 8, 1102 },
                    { 110209, 5, "Yield(q/ha)_Y2", 9, 1102 },
                    { 110210, 5, "%IncreaseOverT2", 10, 1102 },
                    { 110301, 3, "Date", 1, 1103 },
                    { 110302, 0, "Title", 2, 1103 },
                    { 110303, 0, "Thematic area*", 3, 1103 },
                    { 110304, 0, "Type**", 4, 1103 },
                    { 110305, 1, "NoOfTrials_Male_SC/ST", 5, 1103 },
                    { 110306, 1, "NoOfTrials_Male_Gen", 6, 1103 },
                    { 110307, 1, "NoOfTrials_Female_SC/ST", 7, 1103 },
                    { 110308, 1, "NoOfTrials_Female_Gen", 8, 1103 },
                    { 110401, 3, "Date", 1, 1104 },
                    { 110402, 0, "Title", 2, 1104 },
                    { 110403, 0, "Thematic area*", 3, 1104 },
                    { 110404, 0, "Type**", 4, 1104 },
                    { 110405, 0, "Village/Taluk", 5, 1104 },
                    { 110406, 1, "NoOfTrials_Male_SC/ST", 6, 1104 },
                    { 110407, 1, "NoOfTrials_Male_Gen", 7, 1104 },
                    { 110408, 1, "NoOfTrials_Female_SC/ST", 8, 1104 },
                    { 110409, 1, "NoOfTrials_Female_Gen", 9, 1104 },
                    { 132101, 1, "SlNo", 1, 1321 },
                    { 132102, 0, "Message", 2, 1321 },
                    { 132103, 0, "Category", 3, 1321 },
                    { 132201, 3, "Date", 1, 1322 },
                    { 132202, 0, "ParticipatingCenter", 2, 1322 },
                    { 132203, 0, "Purpose", 3, 1322 },
                    { 132301, 0, "Details", 1, 1323 },
                    { 140001, 0, "Story", 1, 1400 },
                    { 210101, 1, "SlNo", 1, 2101 },
                    { 210102, 0, "SponsoredOrganization", 2, 2101 },
                    { 210103, 0, "TrainingTitle", 3, 2101 },
                    { 210104, 3, "Date", 4, 2101 },
                    { 210105, 0, "Duration", 5, 2101 },
                    { 210106, 1, "NoOfTrainings", 6, 2101 },
                    { 210107, 1, "NoOfParticipants", 7, 2101 },
                    { 210201, 0, "Details", 1, 2102 },
                    { 310101, 1, "SlNo", 1, 3101 },
                    { 310102, 0, "TrainingTitle", 2, 3101 },
                    { 310103, 0, "DateAndPlace", 3, 3101 },
                    { 310104, 0, "Duration", 4, 3101 },
                    { 310105, 1, "NoOfTrainings", 5, 3101 },
                    { 310106, 1, "NoOfParticipants", 6, 3101 },
                    { 310201, 1, "SlNo", 1, 3102 },
                    { 310202, 0, "SponsoredOrganization", 2, 3102 },
                    { 310203, 0, "TrainingTitle", 3, 3102 },
                    { 310204, 0, "DateAndPlace", 4, 3102 },
                    { 310205, 0, "Duration", 5, 3102 },
                    { 310206, 1, "NoOfTrainings", 6, 3102 },
                    { 310207, 1, "NoOfParticipants", 7, 3102 },
                    { 310301, 0, "Details", 1, 3103 },
                    { 410101, 1, "SlNo", 1, 4101 },
                    { 410102, 0, "ActivityType", 2, 4101 },
                    { 410103, 1, "Number", 3, 4101 },
                    { 510101, 1, "SlNo", 1, 5101 },
                    { 510102, 0, "TrainingDetails", 2, 5101 },
                    { 510103, 1, "Number", 3, 5101 },
                    { 510104, 1, "NoOfProgramme", 4, 5101 },
                    { 510105, 0, "DateAndPlace", 5, 5101 },
                    { 510106, 0, "Duration", 6, 5101 },
                    { 510107, 1, "NoOfParticipants", 7, 5101 },
                    { 510201, 0, "Details", 1, 5102 },
                    { 610101, 1, "SlNo", 1, 6101 },
                    { 610102, 0, "ServiceType", 2, 6101 },
                    { 610103, 1, "Nos", 3, 6101 },
                    { 610104, 1, "NoOfBeneficiaries", 4, 6101 },
                    { 610201, 1, "SlNo", 1, 6102 },
                    { 610202, 0, "InputDetails", 2, 6102 },
                    { 610203, 0, "Quantity", 3, 6102 },
                    { 610204, 2, "Amount", 4, 6102 },
                    { 610301, 0, "Details", 1, 6103 },
                    { 710101, 0, "CourseName", 1, 7101 },
                    { 710102, 1, "CandidatesAdmitted", 2, 7101 },
                    { 710103, 1, "AttendedExam", 3, 7101 },
                    { 710104, 1, "Passed", 4, 7101 },
                    { 710201, 0, "CourseName", 1, 7102 },
                    { 710202, 1, "CandidatesAdmitted", 2, 7102 },
                    { 710203, 1, "AttendedExam", 3, 7102 },
                    { 710204, 1, "Passed", 4, 7102 },
                    { 710301, 0, "Details", 1, 7103 },
                    { 810101, 0, "InstitutionName", 1, 8101 },
                    { 810102, 3, "DateOfVisit", 2, 8101 },
                    { 810103, 1, "NoOfVisitors", 3, 8101 },
                    { 910101, 0, "Particulars", 1, 9101 },
                    { 910102, 3, "Date", 2, 9101 },
                    { 910103, 0, "Place", 3, 9101 },
                    { 910104, 1, "NoOfProgrammes", 4, 9101 },
                    { 910105, 1, "NoOfParticipants", 5, 9101 },
                    { 951101, 0, "Title", 1, 9511 },
                    { 951102, 0, "Crop", 2, 9511 },
                    { 951103, 0, "Area", 3, 9511 },
                    { 951104, 1, "Trials_SC_Male", 4, 9511 },
                    { 951105, 1, "Trials_SC_Female", 5, 9511 },
                    { 951106, 1, "Trials_ST_Male", 6, 9511 },
                    { 951107, 1, "Trials_ST_Female", 7, 9511 },
                    { 951108, 1, "Trials_General_Male", 8, 9511 },
                    { 951109, 1, "Trials_General_Female", 9, 9511 },
                    { 951110, 2, "Yield_T1", 10, 9511 },
                    { 951111, 2, "Yield_T2", 11, 9511 },
                    { 951112, 2, "%IncreaseOverT2", 12, 9511 },
                    { 951201, 0, "Title", 1, 9512 },
                    { 951202, 0, "Crop", 2, 9512 },
                    { 951203, 0, "Area", 3, 9512 },
                    { 951204, 1, "Trials_SC_Male", 4, 9512 },
                    { 951205, 1, "Trials_SC_Female", 5, 9512 },
                    { 951206, 1, "Trials_ST_Male", 6, 9512 },
                    { 951207, 1, "Trials_ST_Female", 7, 9512 },
                    { 951208, 1, "Trials_General_Male", 8, 9512 },
                    { 951209, 1, "Trials_General_Female", 9, 9512 },
                    { 951210, 2, "Yield_Demo", 10, 9512 },
                    { 951211, 2, "Yield_Check", 11, 9512 },
                    { 951212, 2, "%IncreaseOverCheck", 12, 9512 },
                    { 951301, 1, "SlNo", 1, 9513 },
                    { 951302, 0, "Title", 2, 9513 },
                    { 951303, 3, "Date", 3, 9513 },
                    { 951304, 0, "Duration", 4, 9513 },
                    { 951305, 1, "NoOfProgrammes", 5, 9513 },
                    { 951306, 1, "NoOfParticipants", 6, 9513 }
                });

            migrationBuilder.InsertData(
                table: "TableDefinitions",
                columns: new[] { "TableDefinitionId", "DisplayOrder", "OrgUnitId", "Title" },
                values: new object[,]
                {
                    { 3201, 1, 3001, "DESI Programme" },
                    { 3202, 2, 3001, "Any Other Activities" }
                });

            migrationBuilder.InsertData(
                table: "TableColumns",
                columns: new[] { "TableColumnId", "DataType", "Name", "Position", "TableDefinitionId" },
                values: new object[,]
                {
                    { 320101, 1, "SlNo", 1, 3201 },
                    { 320102, 0, "Place", 2, 3201 },
                    { 320103, 0, "NodalTrainingInstitute", 3, 3201 },
                    { 320104, 1, "NoOfBatch", 4, 3201 },
                    { 320105, 1, "NoOfInputDealers", 5, 3201 },
                    { 320201, 0, "Details", 1, 3202 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrgUnits_ParentId",
                table: "OrgUnits",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgUnits_TemplateSourceId",
                table: "OrgUnits",
                column: "TemplateSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_FK_UserId",
                table: "RefreshTokens",
                column: "FK_UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TableColumns_TableDefinitionId",
                table: "TableColumns",
                column: "TableDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TableDataCells_TableColumnId",
                table: "TableDataCells",
                column: "TableColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_TableDataCells_TableDataRowId",
                table: "TableDataCells",
                column: "TableDataRowId");

            migrationBuilder.CreateIndex(
                name: "IX_TableDataRows_OrgUnitId",
                table: "TableDataRows",
                column: "OrgUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_TableDataRows_TableDefinitionId",
                table: "TableDataRows",
                column: "TableDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_TableDataRows_TrainerId",
                table: "TableDataRows",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_TableDefinitions_OrgUnitId",
                table: "TableDefinitions",
                column: "OrgUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignment_Active",
                table: "TrainerUnitAssignments",
                columns: new[] { "UserId", "OrgUnitId", "UnassignedOn" });

            migrationBuilder.CreateIndex(
                name: "IX_TrainerUnitAssignments_OrgUnitId",
                table: "TrainerUnitAssignments",
                column: "OrgUnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "TableDataCells");

            migrationBuilder.DropTable(
                name: "TrainerUnitAssignments");

            migrationBuilder.DropTable(
                name: "TableColumns");

            migrationBuilder.DropTable(
                name: "TableDataRows");

            migrationBuilder.DropTable(
                name: "TableDefinitions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "OrgUnits");
        }
    }
}
