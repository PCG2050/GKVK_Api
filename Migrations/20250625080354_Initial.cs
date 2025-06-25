using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GKVK_Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Institutes",
                columns: table => new
                {
                    InstituteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstituteName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    InstituteLogo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Institutes", x => x.InstituteId);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    StateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StateName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.StateId);
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
                name: "Districts",
                columns: table => new
                {
                    DistrictId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DistrictName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FK_StateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.DistrictId);
                    table.ForeignKey(
                        name: "FK_Districts_States_FK_StateId",
                        column: x => x.FK_StateId,
                        principalTable: "States",
                        principalColumn: "StateId",
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
                name: "OrgUnits",
                columns: table => new
                {
                    OrgUnitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InstituteId = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    DistrictId = table.Column<int>(type: "int", nullable: true),
                    TemplateSourceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgUnits", x => x.OrgUnitId);
                    table.ForeignKey(
                        name: "FK_OrgUnits_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "DistrictId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrgUnits_Institutes_InstituteId",
                        column: x => x.InstituteId,
                        principalTable: "Institutes",
                        principalColumn: "InstituteId",
                        onDelete: ReferentialAction.Restrict);
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
                table: "Institutes",
                columns: new[] { "InstituteId", "Address", "ContactEmail", "InstituteLogo", "InstituteName" },
                values: new object[,]
                {
                    { 1, "GKVK, Bengaluru, Karnataka, India", "gkvk@uasb.edu.in", "logo1.png", "University of Agricultural Sciences, GKVK" },
                    { 2, "Krishi Bhawan, New Delhi, India", "icar@nic.in", "logo2.png", "Indian Council of Agricultural Research (ICAR)" },
                    { 3, "Hesaraghatta, Bengaluru, India", "nihr@example.com", "logo3.png", "National Institute of Horticultural Research" }
                });

            migrationBuilder.InsertData(
                table: "States",
                columns: new[] { "StateId", "StateName" },
                values: new object[,]
                {
                    { 1, "Andhra Pradesh" },
                    { 2, "Arunachal Pradesh" },
                    { 3, "Assam" },
                    { 4, "Bihar" },
                    { 5, "Chhattisgarh" },
                    { 6, "Goa" },
                    { 7, "Gujarat" },
                    { 8, "Haryana" },
                    { 9, "Himachal Pradesh" },
                    { 10, "Jharkhand" },
                    { 11, "Karnataka" },
                    { 12, "Kerala" },
                    { 13, "Madhya Pradesh" },
                    { 14, "Maharashtra" },
                    { 15, "Manipur" },
                    { 16, "Meghalaya" },
                    { 17, "Mizoram" },
                    { 18, "Nagaland" },
                    { 19, "Odisha" },
                    { 20, "Punjab" },
                    { 21, "Rajasthan" },
                    { 22, "Sikkim" },
                    { 23, "Tamil Nadu" },
                    { 24, "Telangana" },
                    { 25, "Tripura" },
                    { 26, "Uttar Pradesh" },
                    { 27, "Uttarakhand" },
                    { 28, "West Bengal" },
                    { 29, "Andaman and Nicobar Islands" },
                    { 30, "Chandigarh" },
                    { 31, "Dadra and Nagar Haveli and Daman & Diu" },
                    { 32, "Lakshadweep" },
                    { 33, "Delhi" },
                    { 34, "Puducherry" }
                });

            migrationBuilder.InsertData(
                table: "Districts",
                columns: new[] { "DistrictId", "DistrictName", "FK_StateId" },
                values: new object[,]
                {
                    { 1001, "Anantapur", 1 },
                    { 1002, "Chittoor", 1 },
                    { 1003, "East Godavari", 1 },
                    { 1004, "Guntur", 1 },
                    { 1005, "Krishna", 1 },
                    { 1006, "Kurnool", 1 },
                    { 1007, "Nellore", 1 },
                    { 1008, "Prakasam", 1 },
                    { 1009, "Srikakulam", 1 },
                    { 1010, "Visakhapatnam", 1 },
                    { 1011, "Vizianagaram", 1 },
                    { 1012, "West Godavari", 1 },
                    { 1013, "YSR Kadapa", 1 },
                    { 2001, "Anjaw", 2 },
                    { 2002, "Changlang", 2 },
                    { 2003, "Dibang Valley", 2 },
                    { 2004, "East Kameng", 2 },
                    { 2005, "East Siang", 2 },
                    { 2006, "Kra Daadi", 2 },
                    { 2007, "Kurung Kumey", 2 },
                    { 2008, "Lohit", 2 },
                    { 2009, "Longding", 2 },
                    { 2010, "Lower Dibang Valley", 2 },
                    { 2011, "Lower Siang", 2 },
                    { 2012, "Lower Subansiri", 2 },
                    { 2013, "Namsai", 2 },
                    { 2014, "Papum Pare", 2 },
                    { 2015, "Shi Yomi", 2 },
                    { 2016, "Siang", 2 },
                    { 2017, "Tawang", 2 },
                    { 2018, "Tirap", 2 },
                    { 2019, "Upper Siang", 2 },
                    { 2020, "Upper Subansiri", 2 },
                    { 2021, "West Kameng", 2 },
                    { 2022, "West Siang", 2 },
                    { 3001, "Baksa", 3 },
                    { 3002, "Barpeta", 3 },
                    { 3003, "Biswanath", 3 },
                    { 3004, "Bongaigaon", 3 },
                    { 3005, "Cachar", 3 },
                    { 3006, "Charaideo", 3 },
                    { 3007, "Chirang", 3 },
                    { 3008, "Darrang", 3 },
                    { 3009, "Dhemaji", 3 },
                    { 3010, "Dhubri", 3 },
                    { 3011, "Dibrugarh", 3 },
                    { 3012, "Dima Hasao", 3 },
                    { 3013, "Goalpara", 3 },
                    { 3014, "Golaghat", 3 },
                    { 3015, "Hailakandi", 3 },
                    { 3016, "Hojai", 3 },
                    { 3017, "Jorhat", 3 },
                    { 3018, "Kamrup Metropolitan", 3 },
                    { 3019, "Kamrup", 3 },
                    { 3020, "Karbi Anglong", 3 },
                    { 3021, "Karimganj", 3 },
                    { 3022, "Kokrajhar", 3 },
                    { 3023, "Lakhimpur", 3 },
                    { 3024, "Majuli", 3 },
                    { 3025, "Morigaon", 3 },
                    { 3026, "Nagaon", 3 },
                    { 3027, "Nalbari", 3 },
                    { 3028, "Sivasagar", 3 },
                    { 3029, "Sonitpur", 3 },
                    { 3030, "South Salmara-Mankachar", 3 },
                    { 3031, "Tinsukia", 3 },
                    { 3032, "Udalguri", 3 },
                    { 3033, "West Karbi Anglong", 3 },
                    { 4001, "Araria", 4 },
                    { 4002, "Arwal", 4 },
                    { 4003, "Aurangabad", 4 },
                    { 4004, "Banka", 4 },
                    { 4005, "Begusarai", 4 },
                    { 4006, "Bhagalpur", 4 },
                    { 4007, "Bhojpur", 4 },
                    { 4008, "Buxar", 4 },
                    { 4009, "Darbhanga", 4 },
                    { 4010, "East Champaran", 4 },
                    { 4011, "Gaya", 4 },
                    { 4012, "Gopalganj", 4 },
                    { 4013, "Jamui", 4 },
                    { 4014, "Jehanabad", 4 },
                    { 4015, "Kaimur", 4 },
                    { 4016, "Katihar", 4 },
                    { 4017, "Khagaria", 4 },
                    { 4018, "Kishanganj", 4 },
                    { 4019, "Lakhisarai", 4 },
                    { 4020, "Madhepura", 4 },
                    { 4021, "Madhubani", 4 },
                    { 4022, "Munger", 4 },
                    { 4023, "Muzaffarpur", 4 },
                    { 4024, "Nalanda", 4 },
                    { 4025, "Nawada", 4 },
                    { 4026, "Patna", 4 },
                    { 4027, "Purnia", 4 },
                    { 4028, "Rohtas", 4 },
                    { 4029, "Saharsa", 4 },
                    { 4030, "Samastipur", 4 },
                    { 4031, "Saran", 4 },
                    { 4032, "Sheikhpura", 4 },
                    { 4033, "Sheohar", 4 },
                    { 4034, "Sitamarhi", 4 },
                    { 4035, "Siwan", 4 },
                    { 4036, "Supaul", 4 },
                    { 4037, "Vaishali", 4 },
                    { 4038, "West Champaran", 4 },
                    { 5001, "Balod", 5 },
                    { 5002, "Baloda Bazar", 5 },
                    { 5003, "Balrampur", 5 },
                    { 5004, "Bastar", 5 },
                    { 5005, "Bemetara", 5 },
                    { 5006, "Bijapur", 5 },
                    { 5007, "Bilaspur", 5 },
                    { 5008, "Dantewada", 5 },
                    { 5009, "Dhamtari", 5 },
                    { 5010, "Durg", 5 },
                    { 5011, "Gariaband", 5 },
                    { 5012, "Gaurela-Pendra-Marwahi", 5 },
                    { 5013, "Janjgir-Champa", 5 },
                    { 5014, "Jashpur", 5 },
                    { 5015, "Kabirdham", 5 },
                    { 5016, "Kanker", 5 },
                    { 5017, "Kondagaon", 5 },
                    { 5018, "Korba", 5 },
                    { 5019, "Koriya", 5 },
                    { 5020, "Mahasamund", 5 },
                    { 5021, "Mungeli", 5 },
                    { 5022, "Narayanpur", 5 },
                    { 5023, "Raigarh", 5 },
                    { 5024, "Raipur", 5 },
                    { 5025, "Rajnandgaon", 5 },
                    { 5026, "Sukma", 5 },
                    { 5027, "Surajpur", 5 },
                    { 5028, "Surguja", 5 },
                    { 6001, "North Goa", 6 },
                    { 6002, "South Goa", 6 },
                    { 7001, "Ahmedabad", 7 },
                    { 7002, "Amreli", 7 },
                    { 7003, "Anand", 7 },
                    { 7004, "Aravalli", 7 },
                    { 7005, "Banaskantha", 7 },
                    { 7006, "Bharuch", 7 },
                    { 7007, "Bhavnagar", 7 },
                    { 7008, "Botad", 7 },
                    { 7009, "Chhota Udaipur", 7 },
                    { 7010, "Dahod", 7 },
                    { 7011, "Dang", 7 },
                    { 7012, "Devbhoomi Dwarka", 7 },
                    { 7013, "Gandhinagar", 7 },
                    { 7014, "Gir Somnath", 7 },
                    { 7015, "Jamnagar", 7 },
                    { 7016, "Junagadh", 7 },
                    { 7017, "Kheda", 7 },
                    { 7018, "Kutch", 7 },
                    { 7019, "Mahisagar", 7 },
                    { 7020, "Mehsana", 7 },
                    { 7021, "Morbi", 7 },
                    { 7022, "Narmada", 7 },
                    { 7023, "Navsari", 7 },
                    { 7024, "Panchmahal", 7 },
                    { 7025, "Patan", 7 },
                    { 7026, "Porbandar", 7 },
                    { 7027, "Rajkot", 7 },
                    { 7028, "Sabarkantha", 7 },
                    { 7029, "Surat", 7 },
                    { 7030, "Surendranagar", 7 },
                    { 7031, "Tapi", 7 },
                    { 7032, "Vadodara", 7 },
                    { 7033, "Valsad", 7 },
                    { 8001, "Ambala", 8 },
                    { 8002, "Bhiwani", 8 },
                    { 8003, "Charkhi Dadri", 8 },
                    { 8004, "Faridabad", 8 },
                    { 8005, "Fatehabad", 8 },
                    { 8006, "Gurugram", 8 },
                    { 8007, "Hisar", 8 },
                    { 8008, "Jhajjar", 8 },
                    { 8009, "Jind", 8 },
                    { 8010, "Kaithal", 8 },
                    { 8011, "Karnal", 8 },
                    { 8012, "Kurukshetra", 8 },
                    { 8013, "Mahendragarh", 8 },
                    { 8014, "Nuh", 8 },
                    { 8015, "Palwal", 8 },
                    { 8016, "Panchkula", 8 },
                    { 8017, "Panipat", 8 },
                    { 8018, "Rewari", 8 },
                    { 8019, "Rohtak", 8 },
                    { 8020, "Sirsa", 8 },
                    { 8021, "Sonipat", 8 },
                    { 8022, "Yamunanagar", 8 },
                    { 9001, "Bilaspur", 9 },
                    { 9002, "Chamba", 9 },
                    { 9003, "Hamirpur", 9 },
                    { 9004, "Kangra", 9 },
                    { 9005, "Kinnaur", 9 },
                    { 9006, "Kullu", 9 },
                    { 9007, "Lahaul and Spiti", 9 },
                    { 9008, "Mandi", 9 },
                    { 9009, "Shimla", 9 },
                    { 9010, "Sirmaur", 9 },
                    { 9011, "Solan", 9 },
                    { 9012, "Una", 9 },
                    { 10001, "Bokaro", 10 },
                    { 10002, "Chatra", 10 },
                    { 10003, "Deoghar", 10 },
                    { 10004, "Dhanbad", 10 },
                    { 10005, "Dumka", 10 },
                    { 10006, "East Singhbhum", 10 },
                    { 10007, "Garhwa", 10 },
                    { 10008, "Giridih", 10 },
                    { 10009, "Godda", 10 },
                    { 10010, "Gumla", 10 },
                    { 10011, "Hazaribagh", 10 },
                    { 10012, "Jamtara", 10 },
                    { 10013, "Khunti", 10 },
                    { 10014, "Koderma", 10 },
                    { 10015, "Latehar", 10 },
                    { 10016, "Lohardaga", 10 },
                    { 10017, "Pakur", 10 },
                    { 10018, "Palamu", 10 },
                    { 10019, "Ramgarh", 10 },
                    { 10020, "Ranchi", 10 },
                    { 10021, "Sahebganj", 10 },
                    { 10022, "Seraikela Kharsawan", 10 },
                    { 10023, "Simdega", 10 },
                    { 10024, "West Singhbhum", 10 },
                    { 11001, "Bagalkot", 11 },
                    { 11002, "Ballari (Bellary)", 11 },
                    { 11003, "Belagavi (Belgaum)", 11 },
                    { 11004, "Bengaluru Rural", 11 },
                    { 11005, "Bengaluru Urban", 11 },
                    { 11006, "Bidar", 11 },
                    { 11007, "Chamarajanagar", 11 },
                    { 11008, "Chikkaballapur", 11 },
                    { 11009, "Chikkamagaluru", 11 },
                    { 11010, "Chitradurga", 11 },
                    { 11011, "Dakshina Kannada", 11 },
                    { 11012, "Davangere", 11 },
                    { 11013, "Dharwad", 11 },
                    { 11014, "Gadag", 11 },
                    { 11015, "Kalaburagi (Gulbarga)", 11 },
                    { 11016, "Hassan", 11 },
                    { 11017, "Haveri", 11 },
                    { 11018, "Kodagu", 11 },
                    { 11019, "Kolar", 11 },
                    { 11020, "Koppal", 11 },
                    { 11021, "Mandya", 11 },
                    { 11022, "Mysuru (Mysore)", 11 },
                    { 11023, "Raichur", 11 },
                    { 11024, "Ramanagara", 11 },
                    { 11025, "Shivamogga (Shimoga)", 11 },
                    { 11026, "Tumakuru (Tumkur)", 11 },
                    { 11027, "Udupi", 11 },
                    { 11028, "Uttara Kannada", 11 },
                    { 11029, "Vijayapura (Bijapur)", 11 },
                    { 11030, "Yadgir", 11 },
                    { 12001, "Alappuzha", 12 },
                    { 12002, "Ernakulam", 12 },
                    { 12003, "Idukki", 12 },
                    { 12004, "Kannur", 12 },
                    { 12005, "Kasaragod", 12 },
                    { 12006, "Kollam", 12 },
                    { 12007, "Kottayam", 12 },
                    { 12008, "Kozhikode", 12 },
                    { 12009, "Malappuram", 12 },
                    { 12010, "Palakkad", 12 },
                    { 12011, "Pathanamthitta", 12 },
                    { 12012, "Thiruvananthapuram", 12 },
                    { 12013, "Thrissur", 12 },
                    { 12014, "Wayanad", 12 },
                    { 13001, "Agar Malwa", 13 },
                    { 13002, "Alirajpur", 13 },
                    { 13003, "Anuppur", 13 },
                    { 13004, "Ashoknagar", 13 },
                    { 13005, "Balaghat", 13 },
                    { 13006, "Barwani", 13 },
                    { 13007, "Betul", 13 },
                    { 13008, "Bhind", 13 },
                    { 13009, "Bhopal", 13 },
                    { 13010, "Burhanpur", 13 },
                    { 13011, "Chhatarpur", 13 },
                    { 13012, "Chhindwara", 13 },
                    { 13013, "Damoh", 13 },
                    { 13014, "Datia", 13 },
                    { 13015, "Dewas", 13 },
                    { 13016, "Dhar", 13 },
                    { 13017, "Dindori", 13 },
                    { 13018, "Guna", 13 },
                    { 13019, "Gwalior", 13 },
                    { 13020, "Harda", 13 },
                    { 13021, "Hoshangabad", 13 },
                    { 13022, "Indore", 13 },
                    { 13023, "Jabalpur", 13 },
                    { 13024, "Jhabua", 13 },
                    { 13025, "Katni", 13 },
                    { 13026, "Khandwa", 13 },
                    { 13027, "Khargone", 13 },
                    { 13028, "Mandla", 13 },
                    { 13029, "Mandsaur", 13 },
                    { 13030, "Morena", 13 },
                    { 13031, "Narsinghpur", 13 },
                    { 13032, "Neemuch", 13 },
                    { 13033, "Niwari", 13 },
                    { 13034, "Panna", 13 },
                    { 13035, "Raisen", 13 },
                    { 13036, "Rajgarh", 13 },
                    { 13037, "Ratlam", 13 },
                    { 13038, "Rewa", 13 },
                    { 13039, "Sagar", 13 },
                    { 13040, "Satna", 13 },
                    { 13041, "Sehore", 13 },
                    { 13042, "Seoni", 13 },
                    { 13043, "Shahdol", 13 },
                    { 13044, "Shajapur", 13 },
                    { 13045, "Shivpuri", 13 },
                    { 13046, "Sidhi", 13 },
                    { 13047, "Singrauli", 13 },
                    { 13048, "Tikamgarh", 13 },
                    { 13049, "Ujjain", 13 },
                    { 13050, "Umaria", 13 },
                    { 13051, "Vidisha", 13 },
                    { 14001, "Ahmednagar", 14 },
                    { 14002, "Akola", 14 },
                    { 14003, "Amravati", 14 },
                    { 14004, "Aurangabad", 14 },
                    { 14005, "Beed", 14 },
                    { 14006, "Bhandara", 14 },
                    { 14007, "Buldhana", 14 },
                    { 14008, "Chandrapur", 14 },
                    { 14009, "Dhule", 14 },
                    { 14010, "Gadchiroli", 14 },
                    { 14011, "Gondia", 14 },
                    { 14012, "Hingoli", 14 },
                    { 14013, "Jalgaon", 14 },
                    { 14014, "Jalna", 14 },
                    { 14015, "Kolhapur", 14 },
                    { 14016, "Latur", 14 },
                    { 14017, "Mumbai City", 14 },
                    { 14018, "Mumbai Suburban", 14 },
                    { 14019, "Nagpur", 14 },
                    { 14020, "Nanded", 14 },
                    { 14021, "Nandurbar", 14 },
                    { 14022, "Nashik", 14 },
                    { 14023, "Osmanabad", 14 },
                    { 14024, "Palghar", 14 },
                    { 14025, "Parbhani", 14 },
                    { 14026, "Pune", 14 },
                    { 14027, "Raigad", 14 },
                    { 14028, "Ratnagiri", 14 },
                    { 14029, "Sangli", 14 },
                    { 14030, "Satara", 14 },
                    { 14031, "Sindhudurg", 14 },
                    { 14032, "Solapur", 14 },
                    { 14033, "Thane", 14 },
                    { 14034, "Wardha", 14 },
                    { 14035, "Washim", 14 },
                    { 14036, "Yavatmal", 14 },
                    { 15001, "Bishnupur", 15 },
                    { 15002, "Chandel", 15 },
                    { 15003, "Churachandpur", 15 },
                    { 15004, "Imphal East", 15 },
                    { 15005, "Imphal West", 15 },
                    { 15006, "Jiribam", 15 },
                    { 15007, "Kakching", 15 },
                    { 15008, "Kamjong", 15 },
                    { 15009, "Kangpokpi", 15 },
                    { 15010, "Noney", 15 },
                    { 15011, "Pherzawl", 15 },
                    { 15012, "Senapati", 15 },
                    { 15013, "Tamenglong", 15 },
                    { 15014, "Tengnoupal", 15 },
                    { 15015, "Thoubal", 15 },
                    { 15016, "Ukhrul", 15 },
                    { 16001, "East Garo Hills", 16 },
                    { 16002, "East Jaintia Hills", 16 },
                    { 16003, "East Khasi Hills", 16 },
                    { 16004, "North Garo Hills", 16 },
                    { 16005, "Ri Bhoi", 16 },
                    { 16006, "South Garo Hills", 16 },
                    { 16007, "South West Garo Hills", 16 },
                    { 16008, "South West Khasi Hills", 16 },
                    { 16009, "West Garo Hills", 16 },
                    { 16010, "West Jaintia Hills", 16 },
                    { 16011, "West Khasi Hills", 16 },
                    { 17001, "Aizawl", 17 },
                    { 17002, "Champhai", 17 },
                    { 17003, "Hnahthial", 17 },
                    { 17004, "Khawzawl", 17 },
                    { 17005, "Kolasib", 17 },
                    { 17006, "Lawngtlai", 17 },
                    { 17007, "Lunglei", 17 },
                    { 17008, "Mamit", 17 },
                    { 17009, "Saiha", 17 },
                    { 17010, "Serchhip", 17 },
                    { 18001, "Chumukedima", 18 },
                    { 18002, "Dimapur", 18 },
                    { 18003, "Kiphire", 18 },
                    { 18004, "Kohima", 18 },
                    { 18005, "Longleng", 18 },
                    { 18006, "Mokokchung", 18 },
                    { 18007, "Mon", 18 },
                    { 18008, "Niuland", 18 },
                    { 18009, "Noklak", 18 },
                    { 18010, "Peren", 18 },
                    { 18011, "Phek", 18 },
                    { 18012, "Shamator", 18 },
                    { 18013, "Tseminyu", 18 },
                    { 18014, "Tuensang", 18 },
                    { 18015, "Wokha", 18 },
                    { 18016, "Zunheboto", 18 },
                    { 19001, "Angul", 19 },
                    { 19002, "Balangir", 19 },
                    { 19003, "Balasore", 19 },
                    { 19004, "Bargarh", 19 },
                    { 19005, "Bhadrak", 19 },
                    { 19006, "Boudh", 19 },
                    { 19007, "Cuttack", 19 },
                    { 19008, "Debagarh (Deogarh)", 19 },
                    { 19009, "Dhenkanal", 19 },
                    { 19010, "Gajapati", 19 },
                    { 19011, "Ganjam", 19 },
                    { 19012, "Jagatsinghpur", 19 },
                    { 19013, "Jajpur", 19 },
                    { 19014, "Jharsuguda", 19 },
                    { 19015, "Kalahandi", 19 },
                    { 19016, "Kandhamal", 19 },
                    { 19017, "Kendrapara", 19 },
                    { 19018, "Kendujhar (Keonjhar)", 19 },
                    { 19019, "Khordha", 19 },
                    { 19020, "Koraput", 19 },
                    { 19021, "Malkangiri", 19 },
                    { 19022, "Mayurbhanj", 19 },
                    { 19023, "Nabarangpur", 19 },
                    { 19024, "Nayagarh", 19 },
                    { 19025, "Nuapada", 19 },
                    { 19026, "Puri", 19 },
                    { 19027, "Rayagada", 19 },
                    { 19028, "Sambalpur", 19 },
                    { 19029, "Subarnapur (Sonepur)", 19 },
                    { 19030, "Sundargarh", 19 },
                    { 20001, "Amritsar", 20 },
                    { 20002, "Barnala", 20 },
                    { 20003, "Bathinda", 20 },
                    { 20004, "Faridkot", 20 },
                    { 20005, "Fatehgarh Sahib", 20 },
                    { 20006, "Fazilka", 20 },
                    { 20007, "Ferozepur", 20 },
                    { 20008, "Gurdaspur", 20 },
                    { 20009, "Hoshiarpur", 20 },
                    { 20010, "Jalandhar", 20 },
                    { 20011, "Kapurthala", 20 },
                    { 20012, "Ludhiana", 20 },
                    { 20013, "Mansa", 20 },
                    { 20014, "Moga", 20 },
                    { 20015, "Muktsar", 20 },
                    { 20016, "Pathankot", 20 },
                    { 20017, "Patiala", 20 },
                    { 20018, "Rupnagar", 20 },
                    { 20019, "Sangrur", 20 },
                    { 20020, "SAS Nagar (Mohali)", 20 },
                    { 20021, "Shaheed Bhagat Singh Nagar", 20 },
                    { 20022, "Sri Muktsar Sahib", 20 },
                    { 20023, "Tarn Taran", 20 },
                    { 21001, "Ajmer", 21 },
                    { 21002, "Alwar", 21 },
                    { 21003, "Banswara", 21 },
                    { 21004, "Baran", 21 },
                    { 21005, "Barmer", 21 },
                    { 21006, "Bharatpur", 21 },
                    { 21007, "Bhilwara", 21 },
                    { 21008, "Bikaner", 21 },
                    { 21009, "Bundi", 21 },
                    { 21010, "Chittorgarh", 21 },
                    { 21011, "Churu", 21 },
                    { 21012, "Dausa", 21 },
                    { 21013, "Dholpur", 21 },
                    { 21014, "Dungarpur", 21 },
                    { 21015, "Hanumangarh", 21 },
                    { 21016, "Jaipur", 21 },
                    { 21017, "Jaisalmer", 21 },
                    { 21018, "Jalore", 21 },
                    { 21019, "Jhalawar", 21 },
                    { 21020, "Jhunjhunu", 21 },
                    { 21021, "Jodhpur", 21 },
                    { 21022, "Karauli", 21 },
                    { 21023, "Kota", 21 },
                    { 21024, "Nagaur", 21 },
                    { 21025, "Pali", 21 },
                    { 21026, "Pratapgarh", 21 },
                    { 21027, "Rajsamand", 21 },
                    { 21028, "Sawai Madhopur", 21 },
                    { 21029, "Sikar", 21 },
                    { 21030, "Sirohi", 21 },
                    { 21031, "Sri Ganganagar", 21 },
                    { 21032, "Tonk", 21 },
                    { 21033, "Udaipur", 21 },
                    { 22001, "East Sikkim", 22 },
                    { 22002, "North Sikkim", 22 },
                    { 22003, "South Sikkim", 22 },
                    { 22004, "West Sikkim", 22 },
                    { 23001, "Ariyalur", 23 },
                    { 23002, "Chengalpattu", 23 },
                    { 23003, "Chennai", 23 },
                    { 23004, "Coimbatore", 23 },
                    { 23005, "Cuddalore", 23 },
                    { 23006, "Dharmapuri", 23 },
                    { 23007, "Dindigul", 23 },
                    { 23008, "Erode", 23 },
                    { 23009, "Kallakurichi", 23 },
                    { 23010, "Kanchipuram", 23 },
                    { 23011, "Kanyakumari", 23 },
                    { 23012, "Karur", 23 },
                    { 23013, "Krishnagiri", 23 },
                    { 23014, "Madurai", 23 },
                    { 23015, "Mayiladuthurai", 23 },
                    { 23016, "Nagapattinam", 23 },
                    { 23017, "Namakkal", 23 },
                    { 23018, "Nilgiris", 23 },
                    { 23019, "Perambalur", 23 },
                    { 23020, "Pudukkottai", 23 },
                    { 23021, "Ramanathapuram", 23 },
                    { 23022, "Ranipet", 23 },
                    { 23023, "Salem", 23 },
                    { 23024, "Sivaganga", 23 },
                    { 23025, "Tenkasi", 23 },
                    { 23026, "Thanjavur", 23 },
                    { 23027, "Theni", 23 },
                    { 23028, "Thoothukudi (Tuticorin)", 23 },
                    { 23029, "Tiruchirappalli", 23 },
                    { 23030, "Tirunelveli", 23 },
                    { 23031, "Tirupattur", 23 },
                    { 23032, "Tiruppur", 23 },
                    { 23033, "Tiruvallur", 23 },
                    { 23034, "Tiruvannamalai", 23 },
                    { 23035, "Tiruvarur", 23 },
                    { 23036, "Vellore", 23 },
                    { 23037, "Viluppuram", 23 },
                    { 23038, "Virudhunagar", 23 },
                    { 24001, "Adilabad", 24 },
                    { 24002, "Bhadradri Kothagudem", 24 },
                    { 24003, "Hyderabad", 24 },
                    { 24004, "Jagtial", 24 },
                    { 24005, "Jangaon", 24 },
                    { 24006, "Jayashankar Bhupalpally", 24 },
                    { 24007, "Jogulamba Gadwal", 24 },
                    { 24008, "Kamareddy", 24 },
                    { 24009, "Karimnagar", 24 },
                    { 24010, "Khammam", 24 },
                    { 24011, "Komaram Bheem Asifabad", 24 },
                    { 24012, "Mahabubabad", 24 },
                    { 24013, "Mahabubnagar", 24 },
                    { 24014, "Mancherial", 24 },
                    { 24015, "Medak", 24 },
                    { 24016, "Medchal-Malkajgiri", 24 },
                    { 24017, "Mulugu", 24 },
                    { 24018, "Nagarkurnool", 24 },
                    { 24019, "Nalgonda", 24 },
                    { 24020, "Narayanpet", 24 },
                    { 24021, "Nirmal", 24 },
                    { 24022, "Nizamabad", 24 },
                    { 24023, "Peddapalli", 24 },
                    { 24024, "Rajanna Sircilla", 24 },
                    { 24025, "Rangareddy", 24 },
                    { 24026, "Sangareddy", 24 },
                    { 24027, "Siddipet", 24 },
                    { 24028, "Suryapet", 24 },
                    { 24029, "Vikarabad", 24 },
                    { 24030, "Wanaparthy", 24 },
                    { 24031, "Warangal Rural", 24 },
                    { 24032, "Warangal Urban", 24 },
                    { 24033, "Yadadri Bhuvanagiri", 24 },
                    { 25001, "Dhalai", 25 },
                    { 25002, "Gomati", 25 },
                    { 25003, "Khowai", 25 },
                    { 25004, "North Tripura", 25 },
                    { 25005, "Sepahijala", 25 },
                    { 25006, "South Tripura", 25 },
                    { 25007, "Unakoti", 25 },
                    { 25008, "West Tripura", 25 },
                    { 26001, "Agra", 26 },
                    { 26002, "Aligarh", 26 },
                    { 26003, "Ambedkar Nagar", 26 },
                    { 26004, "Amethi", 26 },
                    { 26005, "Amroha", 26 },
                    { 26006, "Auraiya", 26 },
                    { 26007, "Ayodhya", 26 },
                    { 26008, "Azamgarh", 26 },
                    { 26009, "Baghpat", 26 },
                    { 26010, "Bahraich", 26 },
                    { 26011, "Ballia", 26 },
                    { 26012, "Balrampur", 26 },
                    { 26013, "Banda", 26 },
                    { 26014, "Barabanki", 26 },
                    { 26015, "Bareilly", 26 },
                    { 26016, "Basti", 26 },
                    { 26017, "Bhadohi", 26 },
                    { 26018, "Bijnor", 26 },
                    { 26019, "Budaun", 26 },
                    { 26020, "Bulandshahr", 26 },
                    { 26021, "Chandauli", 26 },
                    { 26022, "Chitrakoot", 26 },
                    { 26023, "Deoria", 26 },
                    { 26024, "Etah", 26 },
                    { 26025, "Etawah", 26 },
                    { 26026, "Farrukhabad", 26 },
                    { 26027, "Fatehpur", 26 },
                    { 26028, "Firozabad", 26 },
                    { 26029, "Gautam Buddh Nagar", 26 },
                    { 26030, "Ghaziabad", 26 },
                    { 26031, "Ghazipur", 26 },
                    { 26032, "Gonda", 26 },
                    { 26033, "Gorakhpur", 26 },
                    { 26034, "Hamirpur", 26 },
                    { 26035, "Hapur", 26 },
                    { 26036, "Hardoi", 26 },
                    { 26037, "Hathras", 26 },
                    { 26038, "Jalaun", 26 },
                    { 26039, "Jaunpur", 26 },
                    { 26040, "Jhansi", 26 },
                    { 26041, "Kannauj", 26 },
                    { 26042, "Kanpur Dehat", 26 },
                    { 26043, "Kanpur Nagar", 26 },
                    { 26044, "Kasganj", 26 },
                    { 26045, "Kaushambi", 26 },
                    { 26046, "Kheri", 26 },
                    { 26047, "Kushinagar", 26 },
                    { 26048, "Lalitpur", 26 },
                    { 26049, "Lucknow", 26 },
                    { 26050, "Maharajganj", 26 },
                    { 26051, "Mahoba", 26 },
                    { 26052, "Mainpuri", 26 },
                    { 26053, "Mathura", 26 },
                    { 26054, "Mau", 26 },
                    { 26055, "Meerut", 26 },
                    { 26056, "Mirzapur", 26 },
                    { 26057, "Moradabad", 26 },
                    { 26058, "Muzaffarnagar", 26 },
                    { 26059, "Pilibhit", 26 },
                    { 26060, "Pratapgarh", 26 },
                    { 26061, "Prayagraj", 26 },
                    { 26062, "Rae Bareli", 26 },
                    { 26063, "Rampur", 26 },
                    { 26064, "Saharanpur", 26 },
                    { 26065, "Sambhal", 26 },
                    { 26066, "Sant Kabir Nagar", 26 },
                    { 26067, "Shahjahanpur", 26 },
                    { 26068, "Shamli", 26 },
                    { 26069, "Shravasti", 26 },
                    { 26070, "Siddharthnagar", 26 },
                    { 26071, "Sitapur", 26 },
                    { 26072, "Sonbhadra", 26 },
                    { 26073, "Sultanpur", 26 },
                    { 26074, "Unnao", 26 },
                    { 26075, "Varanasi", 26 },
                    { 27001, "Almora", 27 },
                    { 27002, "Bageshwar", 27 },
                    { 27003, "Chamoli", 27 },
                    { 27004, "Champawat", 27 },
                    { 27005, "Dehradun", 27 },
                    { 27006, "Haridwar", 27 },
                    { 27007, "Nainital", 27 },
                    { 27008, "Pauri Garhwal", 27 },
                    { 27009, "Pithoragarh", 27 },
                    { 27010, "Rudraprayag", 27 },
                    { 27011, "Tehri Garhwal", 27 },
                    { 27012, "Udham Singh Nagar", 27 },
                    { 27013, "Uttarkashi", 27 },
                    { 28001, "Alipurduar", 28 },
                    { 28002, "Bankura", 28 },
                    { 28003, "Paschim Bardhaman", 28 },
                    { 28004, "Purba Bardhaman", 28 },
                    { 28005, "Birbhum", 28 },
                    { 28006, "Cooch Behar", 28 },
                    { 28007, "Dakshin Dinajpur", 28 },
                    { 28008, "Darjeeling", 28 },
                    { 28009, "Hooghly", 28 },
                    { 28010, "Howrah", 28 },
                    { 28011, "Jalpaiguri", 28 },
                    { 28012, "Jhargram", 28 },
                    { 28013, "Kalimpong", 28 },
                    { 28014, "Kolkata", 28 },
                    { 28015, "Malda", 28 },
                    { 28016, "Murshidabad", 28 },
                    { 28017, "Nadia", 28 },
                    { 28018, "North 24 Parganas", 28 },
                    { 28019, "Paschim Medinipur", 28 },
                    { 28020, "Purba Medinipur", 28 },
                    { 28021, "Purulia", 28 },
                    { 28022, "South 24 Parganas", 28 },
                    { 28023, "Uttar Dinajpur", 28 },
                    { 29001, "Nicobar", 29 },
                    { 29002, "North and Middle Andaman", 29 },
                    { 29003, "South Andaman", 29 },
                    { 30001, "Chandigarh", 30 },
                    { 31001, "Dadra and Nagar Haveli", 31 },
                    { 31002, "Daman", 31 },
                    { 31003, "Diu", 31 },
                    { 32001, "Lakshadweep", 32 },
                    { 33001, "Central Delhi", 33 },
                    { 33002, "East Delhi", 33 },
                    { 33003, "New Delhi", 33 },
                    { 33004, "North Delhi", 33 },
                    { 33005, "North East Delhi", 33 },
                    { 33006, "North West Delhi", 33 },
                    { 33007, "Shahdara", 33 },
                    { 33008, "South Delhi", 33 },
                    { 33009, "South East Delhi", 33 },
                    { 33010, "South West Delhi", 33 },
                    { 33011, "West Delhi", 33 },
                    { 34001, "Karaikal", 34 },
                    { 34002, "Mahe", 34 },
                    { 34003, "Puducherry", 34 },
                    { 34004, "Yanam", 34 }
                });

            migrationBuilder.InsertData(
                table: "OrgUnits",
                columns: new[] { "OrgUnitId", "DistrictId", "InstituteId", "Name", "ParentId", "TemplateSourceId" },
                values: new object[,]
                {
                    { 1000, null, 1, "KVK Template", null, null },
                    { 1100, null, 1, "Krishi Vigyan Kendras", null, null },
                    { 2000, null, 1, "Farmers Training Institute (FTI)", null, null },
                    { 3000, null, 1, "Staff Training Unit (STU)", null, null },
                    { 4000, null, 1, "Farm Information Unit (FIU)", null, null },
                    { 5000, null, 1, "Institute of Baking Technology & Value Addition (IBT&VA)", null, null },
                    { 6000, null, 1, "Agricultural Technology Information Centre (ATIC)", null, null },
                    { 7000, null, 1, "Distance Education Unit (DEU)", null, null },
                    { 8000, null, 1, "Agricultural Sciences Museum (ASM)", null, null },
                    { 9000, null, 1, "National Agriculture Extension Project (NAEP)", null, null },
                    { 9500, null, 1, "Extension Education Unit (EEU)", null, null },
                    { 1201, 11016, 1, "KVK, Hassan", 1100, 1000 },
                    { 3001, null, 1, "SAMETI – DESI Programme", 3000, null }
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
                name: "IX_Districts_FK_StateId",
                table: "Districts",
                column: "FK_StateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgUnits_DistrictId",
                table: "OrgUnits",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgUnits_InstituteId",
                table: "OrgUnits",
                column: "InstituteId");

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
                name: "IX_TableDataCells_TableDataRowId_TableColumnId",
                table: "TableDataCells",
                columns: new[] { "TableDataRowId", "TableColumnId" },
                unique: true);

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

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Institutes");

            migrationBuilder.DropTable(
                name: "States");
        }
    }
}
