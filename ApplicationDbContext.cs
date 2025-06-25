using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace GKVK_Api
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
       
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<OrgUnit> OrgUnits { get; set; }
        public DbSet<TableColumn> TableColumns { get; set; }
        public DbSet<TableDefinition> TableDefinitions { get; set; }
        public DbSet<TrainerUnitAssignment> TrainerUnitAssignments { get; set; }
        public DbSet<TableDataRow> TableDataRows { get; set; } 
        public DbSet<TableDataCell> TableDataCells { get; set; } 
        public DbSet<State> States { get; set; } 
        public DbSet<District> Districts { get; set; } 
        public DbSet<Institute> Institutes { get; set; }
        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            // Institute ⇄ OrgUnit
            mb.Entity<Institute>()
                .HasMany(i => i.OrgUnits)
                .WithOne(o => o.Institute)
                .HasForeignKey(o => o.InstituteId)
                .OnDelete(DeleteBehavior.Restrict); //

            // OrgUnit ⇄ Children
            mb.Entity<OrgUnit>()
              .HasMany(o => o.Children)
              .WithOne(o => o.Parent)
              .HasForeignKey(o => o.ParentId)
              .OnDelete(DeleteBehavior.Restrict);

            // OrgUnit ⇄ TemplateSource
            mb.Entity<OrgUnit>()
              .HasOne(o => o.TemplateSource)
              .WithMany()
              .HasForeignKey(o => o.TemplateSourceId)
              .OnDelete(DeleteBehavior.Restrict);

            // TableDefinition ⇄ Columns
            mb.Entity<TableDefinition>()
              .HasMany(t => t.Columns)
              .WithOne(c => c.TableDefinition)
              .HasForeignKey(c => c.TableDefinitionId)
              .OnDelete(DeleteBehavior.Cascade);

            //// TableDefinition ⇄ TrainingSessions
            //mb.Entity<TableDefinition>()
            //  .HasMany(t => t.TrainingSessions)
            //  .WithOne(s => s.TableDefinition)
            //  .HasForeignKey(s => s.TableDefinitionId)
            //  .OnDelete(DeleteBehavior.Cascade);

            //// User ⇄ TrainingSessions
            //mb.Entity<User>()
            //  .HasMany(u => u.CreatedSessions)
            //  .WithOne(s => s.User)
            //  .HasForeignKey(s => s.UserId)
            //  .OnDelete(DeleteBehavior.Cascade);
            // TableDefinition ⇄ TableDataRow

            // TableDefinition → TableDataRow
            mb.Entity<TableDefinition>()
              .HasMany(t => t.DataRows)
              .WithOne(r => r.TableDefinition)
              .HasForeignKey(r => r.TableDefinitionId)
              .OnDelete(DeleteBehavior.Cascade); // this is fine

            // User ⇄ TableDataRow
            mb.Entity<User>()
              .HasMany(u => u.CreatedRows)
              .WithOne(r => r.Trainer)
              .HasForeignKey(r => r.TrainerId)
              .OnDelete(DeleteBehavior.Cascade);

            // OrgUnit ⇄ TableDataRow  (optional)
            mb.Entity<OrgUnit>()
              .HasMany(o => o.DataRows)
              .WithOne(r => r.OrgUnit)
              .HasForeignKey(r => r.OrgUnitId)
              .OnDelete(DeleteBehavior.Restrict);// changed to restrict

            // TableDataRow ⇄ TableDataCell
            mb.Entity<TableDataRow>()
              .HasMany(r => r.Cells)
              .WithOne(c => c.TableDataRow)
              .HasForeignKey(c => c.TableDataRowId)
              .OnDelete(DeleteBehavior.Cascade);

            // User ⇄ TrainerUnitAssignments
            mb.Entity<User>()
              .HasMany(u => u.TrainerUnitAssignments)
              .WithOne(a => a.User)
              .HasForeignKey(a => a.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            // OrgUnit ⇄ TrainerUnitAssignments
            mb.Entity<OrgUnit>()
              .HasMany(o => o.TrainerUnitAssignments)
              .WithOne(a => a.OrgUnit)
              .HasForeignKey(a => a.OrgUnitId)
              .OnDelete(DeleteBehavior.Cascade);

            // State ⇄ District
            mb.Entity<State>()
                .HasMany(s => s.Districts)
                .WithOne(d => d.State)
                .HasForeignKey(d => d.FK_StateId)
                .OnDelete(DeleteBehavior.Cascade);

            // District ⇄ OrgUnit
            mb.Entity<District>()
                .HasMany(d => d.OrgUnits)
                .WithOne(o => o.District)
                .HasForeignKey(o => o.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique rule: 1 active assignment per trainer per unit
            mb.Entity<TrainerUnitAssignment>()
              .HasIndex(a => new { a.UserId, a.OrgUnitId, a.UnassignedOn })
              .HasDatabaseName("IX_Assignment_Active");

            mb.Entity<TableDataCell>()
            .HasOne(c => c.TableDataRow)
            .WithMany(r => r.Cells)
            .HasForeignKey(c => c.TableDataRowId)
            .OnDelete(DeleteBehavior.Restrict);

            // Unique = one cell per (row, column)  ← most cases
            mb.Entity<TableDataCell>()
               .HasIndex(c => new { c.TableDataRowId, c.TableColumnId })
               .IsUnique();
            // If you really do need multiple values for the same row+column, 
            // keep the index but drop .IsUnique() to speed up queries.

            // ─────────────────────────────────────────────────────────────
            // 0.  L O C A L   H E L P E R S
            // ─────────────────────────────────────────────────────────────
            static OrgUnit OU(int id, string name, int instituteId, int? parentId = null, int? templateSourceId = null, int? districtId = null) =>
                new() { OrgUnitId = id, Name = name, ParentId = parentId, TemplateSourceId = templateSourceId, InstituteId = instituteId, DistrictId = districtId };

            static TableDefinition TD(int id, int orgId, string title, int order) =>
                new() { TableDefinitionId = id, OrgUnitId = orgId, Title = title, DisplayOrder = order };

            static TableColumn Col(int id, int tableId, int pos, string name, ColumnDataType dt) =>
                new() { TableColumnId = id, TableDefinitionId = tableId, Position = pos, Name = name, DataType = dt };

            // ─────────────────────────────────────────────────────────────
            // 1.  O R G U N I T S
            // ─────────────────────────────────────────────────────────────
            var units = new[]
            {
            OU(1000, "KVK Template", 1),
           // optional “folder” node for all KVKs
            OU(1100, "Krishi Vigyan Kendras", 1),
            // cloned unit: KVK, Hassan
            OU(1201, "KVK, Hassan",       1, parentId: 1100, templateSourceId: 1000, districtId: 11016),

        OU(2000, "Farmers Training Institute (FTI)",1),
        OU(3000, "Staff Training Unit (STU)",1),
        OU(3001, "SAMETI – DESI Programme",1,parentId:3000),
        OU(4000, "Farm Information Unit (FIU)", 1),
        OU(5000, "Institute of Baking Technology & Value Addition (IBT&VA)", 1),
        OU(6000, "Agricultural Technology Information Centre (ATIC)", 1),
        OU(7000, "Distance Education Unit (DEU)", 1),
        OU(8000, "Agricultural Sciences Museum (ASM)", 1),
        OU(9000, "National Agriculture Extension Project (NAEP)", 1),
        OU(9500, "Extension Education Unit (EEU)", 1)
             };
            mb.Entity<OrgUnit>().HasData(units);

            // ─────────────────────────────────────────────────────────────
            // 2.  T A B L E   D E F I N I T I O N S
            // ─────────────────────────────────────────────────────────────
            var td = new[]
            {
        // KVK template
        TD(1101, 1000, "1. OFT", 1),
        TD(1102, 1000, "2. FLD", 2),
        TD(1103, 1000, "3. On-Campus Training Programs Conducted",3),
        TD(1104, 1000, "4. Off-Campus Training Programs Conducted",4),
        //TD(1105, 1000, "5. Lectures delivered as person at KVK and other line departments including NGOs",5),
        //TD(1106, 1000, "6. Scientist visits to farmers field",6),
        //TD(1107, 1000, "7. Consultancy services /Farm Advisory Services provided",7),
        //TD(1108, 1000, "8. Group Discussion Meeeitngs/PRAs/Village meetings",8),
        //TD(1109, 1000, "9. Diagnostic field visits",9),
        //TD(1110, 1000, "10. News Coverage",10),
        //TD(1111, 1000, "11. Visitors to KVK", 11),
        //TD(1112, 1000, "12.Film show organised", 12),
        //TD(1113, 1000, "13.TV/Radio programs", 13),
        //TD(1114, 1000, "14.Exhibitons & Krishimelas organised/ participated", 14),
        //TD(1115, 1000, "15. Exposure visits/ Educational Tours Organised", 15),
        //TD(1116, 1000, "16. Field Days Organized", 16),
        //TD(1117, 1000, "17. Celebration of Imo Events /Days/Programs",17),    
        
        TD(1321, 1000, "32.1 Short Messages",          321),
        TD(1322, 1000, "32.2 Expert Center",           322),
        TD(1323, 1000, "32.3 Other E‑KVK Activities",  323),
        TD(1400, 1000, "40. Any Other Story",          400),

        // FTI
        TD(2101, 2000, "Training Programmes Organized", 1),
        TD(2102, 2000, "Any Other Activities",          2),

        // STU
        TD(3101, 3000, "Training Programmes Organized",            1),
        TD(3102, 3000, "Sponsored Training Programmes Organized",  2),
        TD(3103, 3000, "Any Other Activities",                     3),

        // SAMETI
        TD(3201, 3001, "DESI Programme",          1),
        TD(3202, 3001, "Any Other Activities",    2),

        // FIU
        TD(4101, 4000, "Activities / Programmes", 1),

        // IBT&VA
        TD(5101, 5000, "Programmes Organized", 1),
        TD(5102, 5000, "Any Other Activities", 2),

        // ATIC
        TD(6101, 6000, "Advisory Services",                       1),
        TD(6102, 6000, "Sale of Inputs / Plant Materials",        2),
        TD(6103, 6000, "Any Other Activities",                    3),
        
        // DEU
        TD(7101, 7000, "Diploma Courses",       1),
        TD(7102, 7000, "Certificate Courses",   2),
        TD(7103, 7000, "Any Other Activities",  3),

        // ASM
        TD(8101, 8000, "Museum Visits", 1),

        // NAEP
        TD(9101, 9000, "Extension Programmes", 1),

        // EEU
        TD(9511, 9500, "OFT (On‑Farm Trials)",           1),
        TD(9512, 9500, "FLD (Frontline Demonstrations)", 2),
        TD(9513, 9500, "Training Programmes Organized",  3)
            };
            mb.Entity<TableDefinition>().HasData(td);

            // ─────────────────────────────────────────────────────────────
            // 3.  T A B L E   C O L U M N S
            // ─────────────────────────────────────────────────────────────
            var c = new List<TableColumn>
             {
        // ===== K V K  =====
        //OFT
        Col(110101, 1101, 1, "Title",        ColumnDataType.String),
        Col(110102, 1101, 2, "Crop",         ColumnDataType.String),
        Col(110103, 1101, 3, "Area(ha)",     ColumnDataType.Double),
        Col(110104, 1101, 4, "NoOfTrials_Male_SC/ST",   ColumnDataType.Int),
        Col(110105, 1101, 5, "NoOfTrials_Male_Gen",   ColumnDataType.Int),
        Col(110106, 1101, 6, "NoOfTrials_Female_SC/ST",   ColumnDataType.Int),
        Col(110107, 1101, 7, "NoOfTrials_Female_Gen",   ColumnDataType.Int),
        Col(110108, 1101, 8, "Yield(q/ha)_Y1",   ColumnDataType.Double),
        Col(110109, 1101, 9, "Yield(q/ha)_Y2",   ColumnDataType.Double),
        Col(110110, 1101, 10, "%IncreaseOverT2",   ColumnDataType.Double),

          //FLD
        Col(110201, 1102, 1, "Title",        ColumnDataType.String),
        Col(110202, 1102, 2, "Crop",         ColumnDataType.String),
        Col(110203, 1102, 3, "Area(ha)",     ColumnDataType.Double),
        Col(110204, 1102, 4, "NoOfTrials_Male_SC/ST",   ColumnDataType.Int),
        Col(110205, 1102, 5, "NoOfTrials_Male_Gen",   ColumnDataType.Int),
        Col(110206, 1102, 6, "NoOfTrials_Female_SC/ST",   ColumnDataType.Int),
        Col(110207, 1102, 7, "NoOfTrials_Female_Gen",   ColumnDataType.Int),
        Col(110208, 1102, 8, "Yield(q/ha)_Y1",   ColumnDataType.Double),
        Col(110209, 1102, 9, "Yield(q/ha)_Y2",   ColumnDataType.Double),
        Col(110210, 1102, 10, "%IncreaseOverT2",   ColumnDataType.Double),

         //On-Campus Training Programs
         Col(110301, 1103, 1, "Date",       ColumnDataType.Date),
         Col(110302, 1103, 2, "Title",      ColumnDataType.String),
         Col(110303, 1103, 3, "Thematic area*", ColumnDataType.String),
         Col(110304, 1103, 4, "Type**", ColumnDataType.String),
         Col(110305, 1103, 5, "NoOfTrials_Male_SC/ST",   ColumnDataType.Int),
         Col(110306, 1103, 6, "NoOfTrials_Male_Gen",   ColumnDataType.Int),
         Col(110307, 1103, 7, "NoOfTrials_Female_SC/ST",   ColumnDataType.Int),
         Col(110308, 1103, 8, "NoOfTrials_Female_Gen",   ColumnDataType.Int),

         //Off-Campus Training Programs Conducted         
         Col(110401, 1104, 1, "Date",       ColumnDataType.Date),
         Col(110402, 1104, 2, "Title",      ColumnDataType.String),
         Col(110403, 1104, 3, "Thematic area*", ColumnDataType.String),
         Col(110404, 1104, 4, "Type**", ColumnDataType.String),
         Col(110405, 1104, 5, "Village/Taluk", ColumnDataType.String),
         Col(110406, 1104, 6, "NoOfTrials_Male_SC/ST",   ColumnDataType.Int),
         Col(110407, 1104, 7, "NoOfTrials_Male_Gen",   ColumnDataType.Int),
         Col(110408, 1104, 8, "NoOfTrials_Female_SC/ST",   ColumnDataType.Int),
         Col(110409, 1104, 9, "NoOfTrials_Female_Gen",   ColumnDataType.Int),



        //
        Col(132101, 1321, 1, "SlNo",     ColumnDataType.Int),
        Col(132102, 1321, 2, "Message",  ColumnDataType.String),
        Col(132103, 1321, 3, "Category", ColumnDataType.String),

        Col(132201, 1322, 1, "Date",               ColumnDataType.Date),
        Col(132202, 1322, 2, "ParticipatingCenter",ColumnDataType.String),
        Col(132203, 1322, 3, "Purpose",            ColumnDataType.String),

        Col(132301, 1323, 1, "Details", ColumnDataType.String),
        Col(140001, 1400, 1, "Story",   ColumnDataType.String),

        // ===== F T I =====
        Col(210101, 2101, 1, "SlNo",                   ColumnDataType.Int),
        Col(210102, 2101, 2, "SponsoredOrganization",  ColumnDataType.String),
        Col(210103, 2101, 3, "TrainingTitle",          ColumnDataType.String),
        Col(210104, 2101, 4, "Date",                   ColumnDataType.Date),
        Col(210105, 2101, 5, "Duration",               ColumnDataType.String),
        Col(210106, 2101, 6, "NoOfTrainings",          ColumnDataType.Int),
        Col(210107, 2101, 7, "NoOfParticipants",       ColumnDataType.Int),
        Col(210201, 2102, 1, "Details",                ColumnDataType.String),

        // ===== S T U =====
        Col(310101, 3101, 1, "SlNo",             ColumnDataType.Int),
        Col(310102, 3101, 2, "TrainingTitle",    ColumnDataType.String),
        Col(310103, 3101, 3, "DateAndPlace",     ColumnDataType.String),
        Col(310104, 3101, 4, "Duration",         ColumnDataType.String),
        Col(310105, 3101, 5, "NoOfTrainings",    ColumnDataType.Int),
        Col(310106, 3101, 6, "NoOfParticipants", ColumnDataType.Int),

        Col(310201, 3102, 1, "SlNo",                  ColumnDataType.Int),
        Col(310202, 3102, 2, "SponsoredOrganization", ColumnDataType.String),
        Col(310203, 3102, 3, "TrainingTitle",         ColumnDataType.String),
        Col(310204, 3102, 4, "DateAndPlace",          ColumnDataType.String),
        Col(310205, 3102, 5, "Duration",              ColumnDataType.String),
        Col(310206, 3102, 6, "NoOfTrainings",         ColumnDataType.Int),
        Col(310207, 3102, 7, "NoOfParticipants",      ColumnDataType.Int),
        Col(310301, 3103, 1, "Details",               ColumnDataType.String),

        // ===== S A M E T I =====
        Col(320101, 3201, 1, "SlNo",                 ColumnDataType.Int),
        Col(320102, 3201, 2, "Place",                ColumnDataType.String),
        Col(320103, 3201, 3, "NodalTrainingInstitute", ColumnDataType.String),
        Col(320104, 3201, 4, "NoOfBatch",            ColumnDataType.Int),
        Col(320105, 3201, 5, "NoOfInputDealers",     ColumnDataType.Int),
        Col(320201, 3202, 1, "Details",              ColumnDataType.String),

        // ===== F I U =====
        Col(410101, 4101, 1, "SlNo",         ColumnDataType.Int),
        Col(410102, 4101, 2, "ActivityType", ColumnDataType.String),
        Col(410103, 4101, 3, "Number",       ColumnDataType.Int),

        // ===== I B T & V A =====
        Col(510101, 5101, 1, "SlNo",                 ColumnDataType.Int),
        Col(510102, 5101, 2, "TrainingDetails",      ColumnDataType.String),
        Col(510103, 5101, 3, "Number",               ColumnDataType.Int),
        Col(510104, 5101, 4, "NoOfProgramme",        ColumnDataType.Int),
        Col(510105, 5101, 5, "DateAndPlace",         ColumnDataType.String),
        Col(510106, 5101, 6, "Duration",             ColumnDataType.String),
        Col(510107, 5101, 7, "NoOfParticipants",     ColumnDataType.Int),
        Col(510201, 5102, 1, "Details",              ColumnDataType.String),

        // ===== A T I C =====
        Col(610101, 6101, 1, "SlNo",             ColumnDataType.Int),
        Col(610102, 6101, 2, "ServiceType",      ColumnDataType.String),
        Col(610103, 6101, 3, "Nos",              ColumnDataType.Int),
        Col(610104, 6101, 4, "NoOfBeneficiaries",ColumnDataType.Int),

        Col(610201, 6102, 1, "SlNo",              ColumnDataType.Int),
        Col(610202, 6102, 2, "InputDetails",       ColumnDataType.String),
        Col(610203, 6102, 3, "Quantity",          ColumnDataType.String),
        Col(610204, 6102, 4, "Amount",            ColumnDataType.Decimal),
        Col(610301, 6103, 1, "Details",           ColumnDataType.String),

        // ===== D E U =====
        Col(710101, 7101, 1, "CourseName",        ColumnDataType.String),
        Col(710102, 7101, 2, "CandidatesAdmitted",ColumnDataType.Int),
        Col(710103, 7101, 3, "AttendedExam",      ColumnDataType.Int),
        Col(710104, 7101, 4, "Passed",            ColumnDataType.Int),

        Col(710201, 7102, 1, "CourseName",        ColumnDataType.String),
        Col(710202, 7102, 2, "CandidatesAdmitted",ColumnDataType.Int),
        Col(710203, 7102, 3, "AttendedExam",      ColumnDataType.Int),
        Col(710204, 7102, 4, "Passed",            ColumnDataType.Int),
        Col(710301, 7103, 1, "Details",           ColumnDataType.String),

        // ===== A S M =====
        Col(810101, 8101, 1, "InstitutionName", ColumnDataType.String),
        Col(810102, 8101, 2, "DateOfVisit",     ColumnDataType.Date),
        Col(810103, 8101, 3, "NoOfVisitors",    ColumnDataType.Int),

        // ===== N A E P =====
        Col(910101, 9101, 1, "Particulars",      ColumnDataType.String),
        Col(910102, 9101, 2, "Date",             ColumnDataType.Date),
        Col(910103, 9101, 3, "Place",            ColumnDataType.String),
        Col(910104, 9101, 4, "NoOfProgrammes",   ColumnDataType.Int),
        Col(910105, 9101, 5, "NoOfParticipants", ColumnDataType.Int),

        // ===== E E U =====
        // OFT
        Col(951101, 9511, 1,  "Title",                    ColumnDataType.String),
        Col(951102, 9511, 2,  "Crop",                     ColumnDataType.String),
        Col(951103, 9511, 3,  "Area",                     ColumnDataType.String),
        Col(951104, 9511, 4,  "Trials_SC_Male",           ColumnDataType.Int),
        Col(951105, 9511, 5,  "Trials_SC_Female",         ColumnDataType.Int),
        Col(951106, 9511, 6,  "Trials_ST_Male",           ColumnDataType.Int),
        Col(951107, 9511, 7,  "Trials_ST_Female",         ColumnDataType.Int),
        Col(951108, 9511, 8,  "Trials_General_Male",      ColumnDataType.Int),
        Col(951109, 9511, 9,  "Trials_General_Female",    ColumnDataType.Int),
        Col(951110, 9511, 10, "Yield_T1",                 ColumnDataType.Decimal),
        Col(951111, 9511, 11, "Yield_T2",                 ColumnDataType.Decimal),
        Col(951112, 9511, 12, "%IncreaseOverT2",          ColumnDataType.Decimal),

        // FLD
        Col(951201, 9512, 1,  "Title",                    ColumnDataType.String),
        Col(951202, 9512, 2,  "Crop",                     ColumnDataType.String),
        Col(951203, 9512, 3,  "Area",                     ColumnDataType.String),
        Col(951204, 9512, 4,  "Trials_SC_Male",           ColumnDataType.Int),
        Col(951205, 9512, 5,  "Trials_SC_Female",         ColumnDataType.Int),
        Col(951206, 9512, 6,  "Trials_ST_Male",           ColumnDataType.Int),
        Col(951207, 9512, 7,  "Trials_ST_Female",         ColumnDataType.Int),
        Col(951208, 9512, 8,  "Trials_General_Male",      ColumnDataType.Int),
        Col(951209, 9512, 9,  "Trials_General_Female",    ColumnDataType.Int),
        Col(951210, 9512, 10, "Yield_Demo",               ColumnDataType.Decimal),
        Col(951211, 9512, 11, "Yield_Check",              ColumnDataType.Decimal),
        Col(951212, 9512, 12, "%IncreaseOverCheck",       ColumnDataType.Decimal),

        // Training Programmes
        Col(951301, 9513, 1, "SlNo",              ColumnDataType.Int),
        Col(951302, 9513, 2, "Title",             ColumnDataType.String),
        Col(951303, 9513, 3, "Date",              ColumnDataType.Date),
        Col(951304, 9513, 4, "Duration",          ColumnDataType.String),
        Col(951305, 9513, 5, "NoOfProgrammes",    ColumnDataType.Int),
        Col(951306, 9513, 6, "NoOfParticipants",  ColumnDataType.Int)
            };

            mb.Entity<TableColumn>().HasData(c);

            // ─────────────────────────────────────────────────────────────
            // Seed State Data (as per previous request, included for context)
            // ─────────────────────────────────────────────────────────────
            var states = new List<State>
        {
            new State { StateId = 1, StateName = "Andhra Pradesh" },
            new State { StateId = 2, StateName = "Arunachal Pradesh" },
            new State { StateId = 3, StateName = "Assam" },
            new State { StateId = 4, StateName = "Bihar" },
            new State { StateId = 5, StateName = "Chhattisgarh" },
            new State { StateId = 6, StateName = "Goa" },
            new State { StateId = 7, StateName = "Gujarat" },
            new State { StateId = 8, StateName = "Haryana" },
            new State { StateId = 9, StateName = "Himachal Pradesh" },
            new State { StateId = 10, StateName = "Jharkhand" },
            new State { StateId = 11, StateName = "Karnataka" },
            new State { StateId = 12, StateName = "Kerala" },
            new State { StateId = 13, StateName = "Madhya Pradesh" },
            new State { StateId = 14, StateName = "Maharashtra" },
            new State { StateId = 15, StateName = "Manipur" },
            new State { StateId = 16, StateName = "Meghalaya" },
            new State { StateId = 17, StateName = "Mizoram" },
            new State { StateId = 18, StateName = "Nagaland" },
            new State { StateId = 19, StateName = "Odisha" },
            new State { StateId = 20, StateName = "Punjab" },
            new State { StateId = 21, StateName = "Rajasthan" },
            new State { StateId = 22, StateName = "Sikkim" },
            new State { StateId = 23, StateName = "Tamil Nadu" },
            new State { StateId = 24, StateName = "Telangana" },
            new State { StateId = 25, StateName = "Tripura" },
            new State { StateId = 26, StateName = "Uttar Pradesh" },
            new State { StateId = 27, StateName = "Uttarakhand" },
            new State { StateId = 28, StateName = "West Bengal" },
            new State { StateId = 29, StateName = "Andaman and Nicobar Islands" },
            new State { StateId = 30, StateName = "Chandigarh" },
            new State { StateId = 31, StateName = "Dadra and Nagar Haveli and Daman & Diu" },
            new State { StateId = 32, StateName = "Lakshadweep" },
            new State { StateId = 33, StateName = "Delhi" },
            new State { StateId = 34, StateName = "Puducherry" }
        };
            mb.Entity<State>().HasData(states);


            // ─────────────────────────────────────────────────────────────
            // Seed District Data
            // ─────────────────────────────────────────────────────────────
            var districts = new List<District>
        {
            // Andhra Pradesh (StateId: 1)
            new District { DistrictId = 1001, DistrictName = "Anantapur", FK_StateId = 1 },
            new District { DistrictId = 1002, DistrictName = "Chittoor", FK_StateId = 1 },
            new District { DistrictId = 1003, DistrictName = "East Godavari", FK_StateId = 1 },
            new District { DistrictId = 1004, DistrictName = "Guntur", FK_StateId = 1 },
            new District { DistrictId = 1005, DistrictName = "Krishna", FK_StateId = 1 },
            new District { DistrictId = 1006, DistrictName = "Kurnool", FK_StateId = 1 },
            new District { DistrictId = 1007, DistrictName = "Nellore", FK_StateId = 1 },
            new District { DistrictId = 1008, DistrictName = "Prakasam", FK_StateId = 1 },
            new District { DistrictId = 1009, DistrictName = "Srikakulam", FK_StateId = 1 },
            new District { DistrictId = 1010, DistrictName = "Visakhapatnam", FK_StateId = 1 },
            new District { DistrictId = 1011, DistrictName = "Vizianagaram", FK_StateId = 1 },
            new District { DistrictId = 1012, DistrictName = "West Godavari", FK_StateId = 1 },
            new District { DistrictId = 1013, DistrictName = "YSR Kadapa", FK_StateId = 1 },

            // Arunachal Pradesh (StateId: 2)
            new District { DistrictId = 2001, DistrictName = "Anjaw", FK_StateId = 2 },
            new District { DistrictId = 2002, DistrictName = "Changlang", FK_StateId = 2 },
            new District { DistrictId = 2003, DistrictName = "Dibang Valley", FK_StateId = 2 },
            new District { DistrictId = 2004, DistrictName = "East Kameng", FK_StateId = 2 },
            new District { DistrictId = 2005, DistrictName = "East Siang", FK_StateId = 2 },
            new District { DistrictId = 2006, DistrictName = "Kra Daadi", FK_StateId = 2 },
            new District { DistrictId = 2007, DistrictName = "Kurung Kumey", FK_StateId = 2 },
            new District { DistrictId = 2008, DistrictName = "Lohit", FK_StateId = 2 },
            new District { DistrictId = 2009, DistrictName = "Longding", FK_StateId = 2 },
            new District { DistrictId = 2010, DistrictName = "Lower Dibang Valley", FK_StateId = 2 },
            new District { DistrictId = 2011, DistrictName = "Lower Siang", FK_StateId = 2 },
            new District { DistrictId = 2012, DistrictName = "Lower Subansiri", FK_StateId = 2 },
            new District { DistrictId = 2013, DistrictName = "Namsai", FK_StateId = 2 },
            new District { DistrictId = 2014, DistrictName = "Papum Pare", FK_StateId = 2 },
            new District { DistrictId = 2015, DistrictName = "Shi Yomi", FK_StateId = 2 },
            new District { DistrictId = 2016, DistrictName = "Siang", FK_StateId = 2 },
            new District { DistrictId = 2017, DistrictName = "Tawang", FK_StateId = 2 },
            new District { DistrictId = 2018, DistrictName = "Tirap", FK_StateId = 2 },
            new District { DistrictId = 2019, DistrictName = "Upper Siang", FK_StateId = 2 },
            new District { DistrictId = 2020, DistrictName = "Upper Subansiri", FK_StateId = 2 },
            new District { DistrictId = 2021, DistrictName = "West Kameng", FK_StateId = 2 },
            new District { DistrictId = 2022, DistrictName = "West Siang", FK_StateId = 2 },

            // Assam (StateId: 3)
            new District { DistrictId = 3001, DistrictName = "Baksa", FK_StateId = 3 },
            new District { DistrictId = 3002, DistrictName = "Barpeta", FK_StateId = 3 },
            new District { DistrictId = 3003, DistrictName = "Biswanath", FK_StateId = 3 },
            new District { DistrictId = 3004, DistrictName = "Bongaigaon", FK_StateId = 3 },
            new District { DistrictId = 3005, DistrictName = "Cachar", FK_StateId = 3 },
            new District { DistrictId = 3006, DistrictName = "Charaideo", FK_StateId = 3 },
            new District { DistrictId = 3007, DistrictName = "Chirang", FK_StateId = 3 },
            new District { DistrictId = 3008, DistrictName = "Darrang", FK_StateId = 3 },
            new District { DistrictId = 3009, DistrictName = "Dhemaji", FK_StateId = 3 },
            new District { DistrictId = 3010, DistrictName = "Dhubri", FK_StateId = 3 },
            new District { DistrictId = 3011, DistrictName = "Dibrugarh", FK_StateId = 3 },
            new District { DistrictId = 3012, DistrictName = "Dima Hasao", FK_StateId = 3 },
            new District { DistrictId = 3013, DistrictName = "Goalpara", FK_StateId = 3 },
            new District { DistrictId = 3014, DistrictName = "Golaghat", FK_StateId = 3 },
            new District { DistrictId = 3015, DistrictName = "Hailakandi", FK_StateId = 3 },
            new District { DistrictId = 3016, DistrictName = "Hojai", FK_StateId = 3 },
            new District { DistrictId = 3017, DistrictName = "Jorhat", FK_StateId = 3 },
            new District { DistrictId = 3018, DistrictName = "Kamrup Metropolitan", FK_StateId = 3 },
            new District { DistrictId = 3019, DistrictName = "Kamrup", FK_StateId = 3 },
            new District { DistrictId = 3020, DistrictName = "Karbi Anglong", FK_StateId = 3 },
            new District { DistrictId = 3021, DistrictName = "Karimganj", FK_StateId = 3 },
            new District { DistrictId = 3022, DistrictName = "Kokrajhar", FK_StateId = 3 },
            new District { DistrictId = 3023, DistrictName = "Lakhimpur", FK_StateId = 3 },
            new District { DistrictId = 3024, DistrictName = "Majuli", FK_StateId = 3 },
            new District { DistrictId = 3025, DistrictName = "Morigaon", FK_StateId = 3 },
            new District { DistrictId = 3026, DistrictName = "Nagaon", FK_StateId = 3 },
            new District { DistrictId = 3027, DistrictName = "Nalbari", FK_StateId = 3 },
            new District { DistrictId = 3028, DistrictName = "Sivasagar", FK_StateId = 3 },
            new District { DistrictId = 3029, DistrictName = "Sonitpur", FK_StateId = 3 },
            new District { DistrictId = 3030, DistrictName = "South Salmara-Mankachar", FK_StateId = 3 },
            new District { DistrictId = 3031, DistrictName = "Tinsukia", FK_StateId = 3 },
            new District { DistrictId = 3032, DistrictName = "Udalguri", FK_StateId = 3 },
            new District { DistrictId = 3033, DistrictName = "West Karbi Anglong", FK_StateId = 3 },

            // Bihar (StateId: 4)
            new District { DistrictId = 4001, DistrictName = "Araria", FK_StateId = 4 },
            new District { DistrictId = 4002, DistrictName = "Arwal", FK_StateId = 4 },
            new District { DistrictId = 4003, DistrictName = "Aurangabad", FK_StateId = 4 },
            new District { DistrictId = 4004, DistrictName = "Banka", FK_StateId = 4 },
            new District { DistrictId = 4005, DistrictName = "Begusarai", FK_StateId = 4 },
            new District { DistrictId = 4006, DistrictName = "Bhagalpur", FK_StateId = 4 },
            new District { DistrictId = 4007, DistrictName = "Bhojpur", FK_StateId = 4 },
            new District { DistrictId = 4008, DistrictName = "Buxar", FK_StateId = 4 },
            new District { DistrictId = 4009, DistrictName = "Darbhanga", FK_StateId = 4 },
            new District { DistrictId = 4010, DistrictName = "East Champaran", FK_StateId = 4 },
            new District { DistrictId = 4011, DistrictName = "Gaya", FK_StateId = 4 },
            new District { DistrictId = 4012, DistrictName = "Gopalganj", FK_StateId = 4 },
            new District { DistrictId = 4013, DistrictName = "Jamui", FK_StateId = 4 },
            new District { DistrictId = 4014, DistrictName = "Jehanabad", FK_StateId = 4 },
            new District { DistrictId = 4015, DistrictName = "Kaimur", FK_StateId = 4 },
            new District { DistrictId = 4016, DistrictName = "Katihar", FK_StateId = 4 },
            new District { DistrictId = 4017, DistrictName = "Khagaria", FK_StateId = 4 },
            new District { DistrictId = 4018, DistrictName = "Kishanganj", FK_StateId = 4 },
            new District { DistrictId = 4019, DistrictName = "Lakhisarai", FK_StateId = 4 },
            new District { DistrictId = 4020, DistrictName = "Madhepura", FK_StateId = 4 },
            new District { DistrictId = 4021, DistrictName = "Madhubani", FK_StateId = 4 },
            new District { DistrictId = 4022, DistrictName = "Munger", FK_StateId = 4 },
            new District { DistrictId = 4023, DistrictName = "Muzaffarpur", FK_StateId = 4 },
            new District { DistrictId = 4024, DistrictName = "Nalanda", FK_StateId = 4 },
            new District { DistrictId = 4025, DistrictName = "Nawada", FK_StateId = 4 },
            new District { DistrictId = 4026, DistrictName = "Patna", FK_StateId = 4 },
            new District { DistrictId = 4027, DistrictName = "Purnia", FK_StateId = 4 },
            new District { DistrictId = 4028, DistrictName = "Rohtas", FK_StateId = 4 },
            new District { DistrictId = 4029, DistrictName = "Saharsa", FK_StateId = 4 },
            new District { DistrictId = 4030, DistrictName = "Samastipur", FK_StateId = 4 },
            new District { DistrictId = 4031, DistrictName = "Saran", FK_StateId = 4 },
            new District { DistrictId = 4032, DistrictName = "Sheikhpura", FK_StateId = 4 },
            new District { DistrictId = 4033, DistrictName = "Sheohar", FK_StateId = 4 },
            new District { DistrictId = 4034, DistrictName = "Sitamarhi", FK_StateId = 4 },
            new District { DistrictId = 4035, DistrictName = "Siwan", FK_StateId = 4 },
            new District { DistrictId = 4036, DistrictName = "Supaul", FK_StateId = 4 },
            new District { DistrictId = 4037, DistrictName = "Vaishali", FK_StateId = 4 },
            new District { DistrictId = 4038, DistrictName = "West Champaran", FK_StateId = 4 },

            // Chhattisgarh (StateId: 5)
            new District { DistrictId = 5001, DistrictName = "Balod", FK_StateId = 5 },
            new District { DistrictId = 5002, DistrictName = "Baloda Bazar", FK_StateId = 5 },
            new District { DistrictId = 5003, DistrictName = "Balrampur", FK_StateId = 5 },
            new District { DistrictId = 5004, DistrictName = "Bastar", FK_StateId = 5 },
            new District { DistrictId = 5005, DistrictName = "Bemetara", FK_StateId = 5 },
            new District { DistrictId = 5006, DistrictName = "Bijapur", FK_StateId = 5 },
            new District { DistrictId = 5007, DistrictName = "Bilaspur", FK_StateId = 5 },
            new District { DistrictId = 5008, DistrictName = "Dantewada", FK_StateId = 5 },
            new District { DistrictId = 5009, DistrictName = "Dhamtari", FK_StateId = 5 },
            new District { DistrictId = 5010, DistrictName = "Durg", FK_StateId = 5 },
            new District { DistrictId = 5011, DistrictName = "Gariaband", FK_StateId = 5 },
            new District { DistrictId = 5012, DistrictName = "Gaurela-Pendra-Marwahi", FK_StateId = 5 },
            new District { DistrictId = 5013, DistrictName = "Janjgir-Champa", FK_StateId = 5 },
            new District { DistrictId = 5014, DistrictName = "Jashpur", FK_StateId = 5 },
            new District { DistrictId = 5015, DistrictName = "Kabirdham", FK_StateId = 5 },
            new District { DistrictId = 5016, DistrictName = "Kanker", FK_StateId = 5 },
            new District { DistrictId = 5017, DistrictName = "Kondagaon", FK_StateId = 5 },
            new District { DistrictId = 5018, DistrictName = "Korba", FK_StateId = 5 },
            new District { DistrictId = 5019, DistrictName = "Koriya", FK_StateId = 5 },
            new District { DistrictId = 5020, DistrictName = "Mahasamund", FK_StateId = 5 },
            new District { DistrictId = 5021, DistrictName = "Mungeli", FK_StateId = 5 },
            new District { DistrictId = 5022, DistrictName = "Narayanpur", FK_StateId = 5 },
            new District { DistrictId = 5023, DistrictName = "Raigarh", FK_StateId = 5 },
            new District { DistrictId = 5024, DistrictName = "Raipur", FK_StateId = 5 },
            new District { DistrictId = 5025, DistrictName = "Rajnandgaon", FK_StateId = 5 },
            new District { DistrictId = 5026, DistrictName = "Sukma", FK_StateId = 5 },
            new District { DistrictId = 5027, DistrictName = "Surajpur", FK_StateId = 5 },
            new District { DistrictId = 5028, DistrictName = "Surguja", FK_StateId = 5 },

            // Goa (StateId: 6)
            new District { DistrictId = 6001, DistrictName = "North Goa", FK_StateId = 6 },
            new District { DistrictId = 6002, DistrictName = "South Goa", FK_StateId = 6 },

            // Gujarat (StateId: 7)
            new District { DistrictId = 7001, DistrictName = "Ahmedabad", FK_StateId = 7 },
            new District { DistrictId = 7002, DistrictName = "Amreli", FK_StateId = 7 },
            new District { DistrictId = 7003, DistrictName = "Anand", FK_StateId = 7 },
            new District { DistrictId = 7004, DistrictName = "Aravalli", FK_StateId = 7 },
            new District { DistrictId = 7005, DistrictName = "Banaskantha", FK_StateId = 7 },
            new District { DistrictId = 7006, DistrictName = "Bharuch", FK_StateId = 7 },
            new District { DistrictId = 7007, DistrictName = "Bhavnagar", FK_StateId = 7 },
            new District { DistrictId = 7008, DistrictName = "Botad", FK_StateId = 7 },
            new District { DistrictId = 7009, DistrictName = "Chhota Udaipur", FK_StateId = 7 },
            new District { DistrictId = 7010, DistrictName = "Dahod", FK_StateId = 7 },
            new District { DistrictId = 7011, DistrictName = "Dang", FK_StateId = 7 },
            new District { DistrictId = 7012, DistrictName = "Devbhoomi Dwarka", FK_StateId = 7 },
            new District { DistrictId = 7013, DistrictName = "Gandhinagar", FK_StateId = 7 },
            new District { DistrictId = 7014, DistrictName = "Gir Somnath", FK_StateId = 7 },
            new District { DistrictId = 7015, DistrictName = "Jamnagar", FK_StateId = 7 },
            new District { DistrictId = 7016, DistrictName = "Junagadh", FK_StateId = 7 },
            new District { DistrictId = 7017, DistrictName = "Kheda", FK_StateId = 7 },
            new District { DistrictId = 7018, DistrictName = "Kutch", FK_StateId = 7 },
            new District { DistrictId = 7019, DistrictName = "Mahisagar", FK_StateId = 7 },
            new District { DistrictId = 7020, DistrictName = "Mehsana", FK_StateId = 7 },
            new District { DistrictId = 7021, DistrictName = "Morbi", FK_StateId = 7 },
            new District { DistrictId = 7022, DistrictName = "Narmada", FK_StateId = 7 },
            new District { DistrictId = 7023, DistrictName = "Navsari", FK_StateId = 7 },
            new District { DistrictId = 7024, DistrictName = "Panchmahal", FK_StateId = 7 },
            new District { DistrictId = 7025, DistrictName = "Patan", FK_StateId = 7 },
            new District { DistrictId = 7026, DistrictName = "Porbandar", FK_StateId = 7 },
            new District { DistrictId = 7027, DistrictName = "Rajkot", FK_StateId = 7 },
            new District { DistrictId = 7028, DistrictName = "Sabarkantha", FK_StateId = 7 },
            new District { DistrictId = 7029, DistrictName = "Surat", FK_StateId = 7 },
            new District { DistrictId = 7030, DistrictName = "Surendranagar", FK_StateId = 7 },
            new District { DistrictId = 7031, DistrictName = "Tapi", FK_StateId = 7 },
            new District { DistrictId = 7032, DistrictName = "Vadodara", FK_StateId = 7 },
            new District { DistrictId = 7033, DistrictName = "Valsad", FK_StateId = 7 },

            // Haryana (StateId: 8)
            new District { DistrictId = 8001, DistrictName = "Ambala", FK_StateId = 8 },
            new District { DistrictId = 8002, DistrictName = "Bhiwani", FK_StateId = 8 },
            new District { DistrictId = 8003, DistrictName = "Charkhi Dadri", FK_StateId = 8 },
            new District { DistrictId = 8004, DistrictName = "Faridabad", FK_StateId = 8 },
            new District { DistrictId = 8005, DistrictName = "Fatehabad", FK_StateId = 8 },
            new District { DistrictId = 8006, DistrictName = "Gurugram", FK_StateId = 8 },
            new District { DistrictId = 8007, DistrictName = "Hisar", FK_StateId = 8 },
            new District { DistrictId = 8008, DistrictName = "Jhajjar", FK_StateId = 8 },
            new District { DistrictId = 8009, DistrictName = "Jind", FK_StateId = 8 },
            new District { DistrictId = 8010, DistrictName = "Kaithal", FK_StateId = 8 },
            new District { DistrictId = 8011, DistrictName = "Karnal", FK_StateId = 8 },
            new District { DistrictId = 8012, DistrictName = "Kurukshetra", FK_StateId = 8 },
            new District { DistrictId = 8013, DistrictName = "Mahendragarh", FK_StateId = 8 },
            new District { DistrictId = 8014, DistrictName = "Nuh", FK_StateId = 8 },
            new District { DistrictId = 8015, DistrictName = "Palwal", FK_StateId = 8 },
            new District { DistrictId = 8016, DistrictName = "Panchkula", FK_StateId = 8 },
            new District { DistrictId = 8017, DistrictName = "Panipat", FK_StateId = 8 },
            new District { DistrictId = 8018, DistrictName = "Rewari", FK_StateId = 8 },
            new District { DistrictId = 8019, DistrictName = "Rohtak", FK_StateId = 8 },
            new District { DistrictId = 8020, DistrictName = "Sirsa", FK_StateId = 8 },
            new District { DistrictId = 8021, DistrictName = "Sonipat", FK_StateId = 8 },
            new District { DistrictId = 8022, DistrictName = "Yamunanagar", FK_StateId = 8 },

            // Himachal Pradesh (StateId: 9)
            new District { DistrictId = 9001, DistrictName = "Bilaspur", FK_StateId = 9 },
            new District { DistrictId = 9002, DistrictName = "Chamba", FK_StateId = 9 },
            new District { DistrictId = 9003, DistrictName = "Hamirpur", FK_StateId = 9 },
            new District { DistrictId = 9004, DistrictName = "Kangra", FK_StateId = 9 },
            new District { DistrictId = 9005, DistrictName = "Kinnaur", FK_StateId = 9 },
            new District { DistrictId = 9006, DistrictName = "Kullu", FK_StateId = 9 },
            new District { DistrictId = 9007, DistrictName = "Lahaul and Spiti", FK_StateId = 9 },
            new District { DistrictId = 9008, DistrictName = "Mandi", FK_StateId = 9 },
            new District { DistrictId = 9009, DistrictName = "Shimla", FK_StateId = 9 },
            new District { DistrictId = 9010, DistrictName = "Sirmaur", FK_StateId = 9 },
            new District { DistrictId = 9011, DistrictName = "Solan", FK_StateId = 9 },
            new District { DistrictId = 9012, DistrictName = "Una", FK_StateId = 9 },

            // Jharkhand (StateId: 10)
            new District { DistrictId = 10001, DistrictName = "Bokaro", FK_StateId = 10 },
            new District { DistrictId = 10002, DistrictName = "Chatra", FK_StateId = 10 },
            new District { DistrictId = 10003, DistrictName = "Deoghar", FK_StateId = 10 },
            new District { DistrictId = 10004, DistrictName = "Dhanbad", FK_StateId = 10 },
            new District { DistrictId = 10005, DistrictName = "Dumka", FK_StateId = 10 },
            new District { DistrictId = 10006, DistrictName = "East Singhbhum", FK_StateId = 10 },
            new District { DistrictId = 10007, DistrictName = "Garhwa", FK_StateId = 10 },
            new District { DistrictId = 10008, DistrictName = "Giridih", FK_StateId = 10 },
            new District { DistrictId = 10009, DistrictName = "Godda", FK_StateId = 10 },
            new District { DistrictId = 10010, DistrictName = "Gumla", FK_StateId = 10 },
            new District { DistrictId = 10011, DistrictName = "Hazaribagh", FK_StateId = 10 },
            new District { DistrictId = 10012, DistrictName = "Jamtara", FK_StateId = 10 },
            new District { DistrictId = 10013, DistrictName = "Khunti", FK_StateId = 10 },
            new District { DistrictId = 10014, DistrictName = "Koderma", FK_StateId = 10 },
            new District { DistrictId = 10015, DistrictName = "Latehar", FK_StateId = 10 },
            new District { DistrictId = 10016, DistrictName = "Lohardaga", FK_StateId = 10 },
            new District { DistrictId = 10017, DistrictName = "Pakur", FK_StateId = 10 },
            new District { DistrictId = 10018, DistrictName = "Palamu", FK_StateId = 10 },
            new District { DistrictId = 10019, DistrictName = "Ramgarh", FK_StateId = 10 },
            new District { DistrictId = 10020, DistrictName = "Ranchi", FK_StateId = 10 },
            new District { DistrictId = 10021, DistrictName = "Sahebganj", FK_StateId = 10 },
            new District { DistrictId = 10022, DistrictName = "Seraikela Kharsawan", FK_StateId = 10 },
            new District { DistrictId = 10023, DistrictName = "Simdega", FK_StateId = 10 },
            new District { DistrictId = 10024, DistrictName = "West Singhbhum", FK_StateId = 10 },

            // Karnataka (StateId: 11)
            new District { DistrictId = 11001, DistrictName = "Bagalkot", FK_StateId = 11 },
            new District { DistrictId = 11002, DistrictName = "Ballari (Bellary)", FK_StateId = 11 },
            new District { DistrictId = 11003, DistrictName = "Belagavi (Belgaum)", FK_StateId = 11 },
            new District { DistrictId = 11004, DistrictName = "Bengaluru Rural", FK_StateId = 11 },
            new District { DistrictId = 11005, DistrictName = "Bengaluru Urban", FK_StateId = 11 },
            new District { DistrictId = 11006, DistrictName = "Bidar", FK_StateId = 11 },
            new District { DistrictId = 11007, DistrictName = "Chamarajanagar", FK_StateId = 11 },
            new District { DistrictId = 11008, DistrictName = "Chikkaballapur", FK_StateId = 11 },
            new District { DistrictId = 11009, DistrictName = "Chikkamagaluru", FK_StateId = 11 },
            new District { DistrictId = 11010, DistrictName = "Chitradurga", FK_StateId = 11 },
            new District { DistrictId = 11011, DistrictName = "Dakshina Kannada", FK_StateId = 11 },
            new District { DistrictId = 11012, DistrictName = "Davangere", FK_StateId = 11 },
            new District { DistrictId = 11013, DistrictName = "Dharwad", FK_StateId = 11 },
            new District { DistrictId = 11014, DistrictName = "Gadag", FK_StateId = 11 },
            new District { DistrictId = 11015, DistrictName = "Kalaburagi (Gulbarga)", FK_StateId = 11 },
            new District { DistrictId = 11016, DistrictName = "Hassan", FK_StateId = 11 },
            new District { DistrictId = 11017, DistrictName = "Haveri", FK_StateId = 11 },
            new District { DistrictId = 11018, DistrictName = "Kodagu", FK_StateId = 11 },
            new District { DistrictId = 11019, DistrictName = "Kolar", FK_StateId = 11 },
            new District { DistrictId = 11020, DistrictName = "Koppal", FK_StateId = 11 },
            new District { DistrictId = 11021, DistrictName = "Mandya", FK_StateId = 11 },
            new District { DistrictId = 11022, DistrictName = "Mysuru (Mysore)", FK_StateId = 11 },
            new District { DistrictId = 11023, DistrictName = "Raichur", FK_StateId = 11 },
            new District { DistrictId = 11024, DistrictName = "Ramanagara", FK_StateId = 11 },
            new District { DistrictId = 11025, DistrictName = "Shivamogga (Shimoga)", FK_StateId = 11 },
            new District { DistrictId = 11026, DistrictName = "Tumakuru (Tumkur)", FK_StateId = 11 },
            new District { DistrictId = 11027, DistrictName = "Udupi", FK_StateId = 11 },
            new District { DistrictId = 11028, DistrictName = "Uttara Kannada", FK_StateId = 11 },
            new District { DistrictId = 11029, DistrictName = "Vijayapura (Bijapur)", FK_StateId = 11 },
            new District { DistrictId = 11030, DistrictName = "Yadgir", FK_StateId = 11 },

            // Kerala (StateId: 12)
            new District { DistrictId = 12001, DistrictName = "Alappuzha", FK_StateId = 12 },
            new District { DistrictId = 12002, DistrictName = "Ernakulam", FK_StateId = 12 },
            new District { DistrictId = 12003, DistrictName = "Idukki", FK_StateId = 12 },
            new District { DistrictId = 12004, DistrictName = "Kannur", FK_StateId = 12 },
            new District { DistrictId = 12005, DistrictName = "Kasaragod", FK_StateId = 12 },
            new District { DistrictId = 12006, DistrictName = "Kollam", FK_StateId = 12 },
            new District { DistrictId = 12007, DistrictName = "Kottayam", FK_StateId = 12 },
            new District { DistrictId = 12008, DistrictName = "Kozhikode", FK_StateId = 12 },
            new District { DistrictId = 12009, DistrictName = "Malappuram", FK_StateId = 12 },
            new District { DistrictId = 12010, DistrictName = "Palakkad", FK_StateId = 12 },
            new District { DistrictId = 12011, DistrictName = "Pathanamthitta", FK_StateId = 12 },
            new District { DistrictId = 12012, DistrictName = "Thiruvananthapuram", FK_StateId = 12 },
            new District { DistrictId = 12013, DistrictName = "Thrissur", FK_StateId = 12 },
            new District { DistrictId = 12014, DistrictName = "Wayanad", FK_StateId = 12 },

            // Madhya Pradesh (StateId: 13)
            new District { DistrictId = 13001, DistrictName = "Agar Malwa", FK_StateId = 13 },
            new District { DistrictId = 13002, DistrictName = "Alirajpur", FK_StateId = 13 },
            new District { DistrictId = 13003, DistrictName = "Anuppur", FK_StateId = 13 },
            new District { DistrictId = 13004, DistrictName = "Ashoknagar", FK_StateId = 13 },
            new District { DistrictId = 13005, DistrictName = "Balaghat", FK_StateId = 13 },
            new District { DistrictId = 13006, DistrictName = "Barwani", FK_StateId = 13 },
            new District { DistrictId = 13007, DistrictName = "Betul", FK_StateId = 13 },
            new District { DistrictId = 13008, DistrictName = "Bhind", FK_StateId = 13 },
            new District { DistrictId = 13009, DistrictName = "Bhopal", FK_StateId = 13 },
            new District { DistrictId = 13010, DistrictName = "Burhanpur", FK_StateId = 13 },
            new District { DistrictId = 13011, DistrictName = "Chhatarpur", FK_StateId = 13 },
            new District { DistrictId = 13012, DistrictName = "Chhindwara", FK_StateId = 13 },
            new District { DistrictId = 13013, DistrictName = "Damoh", FK_StateId = 13 },
            new District { DistrictId = 13014, DistrictName = "Datia", FK_StateId = 13 },
            new District { DistrictId = 13015, DistrictName = "Dewas", FK_StateId = 13 },
            new District { DistrictId = 13016, DistrictName = "Dhar", FK_StateId = 13 },
            new District { DistrictId = 13017, DistrictName = "Dindori", FK_StateId = 13 },
            new District { DistrictId = 13018, DistrictName = "Guna", FK_StateId = 13 },
            new District { DistrictId = 13019, DistrictName = "Gwalior", FK_StateId = 13 },
            new District { DistrictId = 13020, DistrictName = "Harda", FK_StateId = 13 },
            new District { DistrictId = 13021, DistrictName = "Hoshangabad", FK_StateId = 13 },
            new District { DistrictId = 13022, DistrictName = "Indore", FK_StateId = 13 },
            new District { DistrictId = 13023, DistrictName = "Jabalpur", FK_StateId = 13 },
            new District { DistrictId = 13024, DistrictName = "Jhabua", FK_StateId = 13 },
            new District { DistrictId = 13025, DistrictName = "Katni", FK_StateId = 13 },
            new District { DistrictId = 13026, DistrictName = "Khandwa", FK_StateId = 13 },
            new District { DistrictId = 13027, DistrictName = "Khargone", FK_StateId = 13 },
            new District { DistrictId = 13028, DistrictName = "Mandla", FK_StateId = 13 },
            new District { DistrictId = 13029, DistrictName = "Mandsaur", FK_StateId = 13 },
            new District { DistrictId = 13030, DistrictName = "Morena", FK_StateId = 13 },
            new District { DistrictId = 13031, DistrictName = "Narsinghpur", FK_StateId = 13 },
            new District { DistrictId = 13032, DistrictName = "Neemuch", FK_StateId = 13 },
            new District { DistrictId = 13033, DistrictName = "Niwari", FK_StateId = 13 },
            new District { DistrictId = 13034, DistrictName = "Panna", FK_StateId = 13 },
            new District { DistrictId = 13035, DistrictName = "Raisen", FK_StateId = 13 },
            new District { DistrictId = 13036, DistrictName = "Rajgarh", FK_StateId = 13 },
            new District { DistrictId = 13037, DistrictName = "Ratlam", FK_StateId = 13 },
            new District { DistrictId = 13038, DistrictName = "Rewa", FK_StateId = 13 },
            new District { DistrictId = 13039, DistrictName = "Sagar", FK_StateId = 13 },
            new District { DistrictId = 13040, DistrictName = "Satna", FK_StateId = 13 },
            new District { DistrictId = 13041, DistrictName = "Sehore", FK_StateId = 13 },
            new District { DistrictId = 13042, DistrictName = "Seoni", FK_StateId = 13 },
            new District { DistrictId = 13043, DistrictName = "Shahdol", FK_StateId = 13 },
            new District { DistrictId = 13044, DistrictName = "Shajapur", FK_StateId = 13 },
            new District { DistrictId = 13045, DistrictName = "Shivpuri", FK_StateId = 13 },
            new District { DistrictId = 13046, DistrictName = "Sidhi", FK_StateId = 13 },
            new District { DistrictId = 13047, DistrictName = "Singrauli", FK_StateId = 13 },
            new District { DistrictId = 13048, DistrictName = "Tikamgarh", FK_StateId = 13 },
            new District { DistrictId = 13049, DistrictName = "Ujjain", FK_StateId = 13 },
            new District { DistrictId = 13050, DistrictName = "Umaria", FK_StateId = 13 },
            new District { DistrictId = 13051, DistrictName = "Vidisha", FK_StateId = 13 },

            // Maharashtra (StateId: 14)
            new District { DistrictId = 14001, DistrictName = "Ahmednagar", FK_StateId = 14 },
            new District { DistrictId = 14002, DistrictName = "Akola", FK_StateId = 14 },
            new District { DistrictId = 14003, DistrictName = "Amravati", FK_StateId = 14 },
            new District { DistrictId = 14004, DistrictName = "Aurangabad", FK_StateId = 14 },
            new District { DistrictId = 14005, DistrictName = "Beed", FK_StateId = 14 },
            new District { DistrictId = 14006, DistrictName = "Bhandara", FK_StateId = 14 },
            new District { DistrictId = 14007, DistrictName = "Buldhana", FK_StateId = 14 },
            new District { DistrictId = 14008, DistrictName = "Chandrapur", FK_StateId = 14 },
            new District { DistrictId = 14009, DistrictName = "Dhule", FK_StateId = 14 },
            new District { DistrictId = 14010, DistrictName = "Gadchiroli", FK_StateId = 14 },
            new District { DistrictId = 14011, DistrictName = "Gondia", FK_StateId = 14 },
            new District { DistrictId = 14012, DistrictName = "Hingoli", FK_StateId = 14 },
            new District { DistrictId = 14013, DistrictName = "Jalgaon", FK_StateId = 14 },
            new District { DistrictId = 14014, DistrictName = "Jalna", FK_StateId = 14 },
            new District { DistrictId = 14015, DistrictName = "Kolhapur", FK_StateId = 14 },
            new District { DistrictId = 14016, DistrictName = "Latur", FK_StateId = 14 },
            new District { DistrictId = 14017, DistrictName = "Mumbai City", FK_StateId = 14 },
            new District { DistrictId = 14018, DistrictName = "Mumbai Suburban", FK_StateId = 14 },
            new District { DistrictId = 14019, DistrictName = "Nagpur", FK_StateId = 14 },
            new District { DistrictId = 14020, DistrictName = "Nanded", FK_StateId = 14 },
            new District { DistrictId = 14021, DistrictName = "Nandurbar", FK_StateId = 14 },
            new District { DistrictId = 14022, DistrictName = "Nashik", FK_StateId = 14 },
            new District { DistrictId = 14023, DistrictName = "Osmanabad", FK_StateId = 14 },
            new District { DistrictId = 14024, DistrictName = "Palghar", FK_StateId = 14 },
            new District { DistrictId = 14025, DistrictName = "Parbhani", FK_StateId = 14 },
            new District { DistrictId = 14026, DistrictName = "Pune", FK_StateId = 14 },
            new District { DistrictId = 14027, DistrictName = "Raigad", FK_StateId = 14 },
            new District { DistrictId = 14028, DistrictName = "Ratnagiri", FK_StateId = 14 },
            new District { DistrictId = 14029, DistrictName = "Sangli", FK_StateId = 14 },
            new District { DistrictId = 14030, DistrictName = "Satara", FK_StateId = 14 },
            new District { DistrictId = 14031, DistrictName = "Sindhudurg", FK_StateId = 14 },
            new District { DistrictId = 14032, DistrictName = "Solapur", FK_StateId = 14 },
            new District { DistrictId = 14033, DistrictName = "Thane", FK_StateId = 14 },
            new District { DistrictId = 14034, DistrictName = "Wardha", FK_StateId = 14 },
            new District { DistrictId = 14035, DistrictName = "Washim", FK_StateId = 14 },
            new District { DistrictId = 14036, DistrictName = "Yavatmal", FK_StateId = 14 },

            // Manipur (StateId: 15)
            new District { DistrictId = 15001, DistrictName = "Bishnupur", FK_StateId = 15 },
            new District { DistrictId = 15002, DistrictName = "Chandel", FK_StateId = 15 },
            new District { DistrictId = 15003, DistrictName = "Churachandpur", FK_StateId = 15 },
            new District { DistrictId = 15004, DistrictName = "Imphal East", FK_StateId = 15 },
            new District { DistrictId = 15005, DistrictName = "Imphal West", FK_StateId = 15 },
            new District { DistrictId = 15006, DistrictName = "Jiribam", FK_StateId = 15 },
            new District { DistrictId = 15007, DistrictName = "Kakching", FK_StateId = 15 },
            new District { DistrictId = 15008, DistrictName = "Kamjong", FK_StateId = 15 },
            new District { DistrictId = 15009, DistrictName = "Kangpokpi", FK_StateId = 15 },
            new District { DistrictId = 15010, DistrictName = "Noney", FK_StateId = 15 },
            new District { DistrictId = 15011, DistrictName = "Pherzawl", FK_StateId = 15 },
            new District { DistrictId = 15012, DistrictName = "Senapati", FK_StateId = 15 },
            new District { DistrictId = 15013, DistrictName = "Tamenglong", FK_StateId = 15 },
            new District { DistrictId = 15014, DistrictName = "Tengnoupal", FK_StateId = 15 },
            new District { DistrictId = 15015, DistrictName = "Thoubal", FK_StateId = 15 },
            new District { DistrictId = 15016, DistrictName = "Ukhrul", FK_StateId = 15 },

            // Meghalaya (StateId: 16)
            new District { DistrictId = 16001, DistrictName = "East Garo Hills", FK_StateId = 16 },
            new District { DistrictId = 16002, DistrictName = "East Jaintia Hills", FK_StateId = 16 },
            new District { DistrictId = 16003, DistrictName = "East Khasi Hills", FK_StateId = 16 },
            new District { DistrictId = 16004, DistrictName = "North Garo Hills", FK_StateId = 16 },
            new District { DistrictId = 16005, DistrictName = "Ri Bhoi", FK_StateId = 16 },
            new District { DistrictId = 16006, DistrictName = "South Garo Hills", FK_StateId = 16 },
            new District { DistrictId = 16007, DistrictName = "South West Garo Hills", FK_StateId = 16 },
            new District { DistrictId = 16008, DistrictName = "South West Khasi Hills", FK_StateId = 16 },
            new District { DistrictId = 16009, DistrictName = "West Garo Hills", FK_StateId = 16 },
            new District { DistrictId = 16010, DistrictName = "West Jaintia Hills", FK_StateId = 16 },
            new District { DistrictId = 16011, DistrictName = "West Khasi Hills", FK_StateId = 16 },

            // Mizoram (StateId: 17)
            new District { DistrictId = 17001, DistrictName = "Aizawl", FK_StateId = 17 },
            new District { DistrictId = 17002, DistrictName = "Champhai", FK_StateId = 17 },
            new District { DistrictId = 17003, DistrictName = "Hnahthial", FK_StateId = 17 },
            new District { DistrictId = 17004, DistrictName = "Khawzawl", FK_StateId = 17 },
            new District { DistrictId = 17005, DistrictName = "Kolasib", FK_StateId = 17 },
            new District { DistrictId = 17006, DistrictName = "Lawngtlai", FK_StateId = 17 },
            new District { DistrictId = 17007, DistrictName = "Lunglei", FK_StateId = 17 },
            new District { DistrictId = 17008, DistrictName = "Mamit", FK_StateId = 17 },
            new District { DistrictId = 17009, DistrictName = "Saiha", FK_StateId = 17 },
            new District { DistrictId = 17010, DistrictName = "Serchhip", FK_StateId = 17 },

            // Nagaland (StateId: 18)
            new District { DistrictId = 18001, DistrictName = "Chumukedima", FK_StateId = 18 },
            new District { DistrictId = 18002, DistrictName = "Dimapur", FK_StateId = 18 },
            new District { DistrictId = 18003, DistrictName = "Kiphire", FK_StateId = 18 },
            new District { DistrictId = 18004, DistrictName = "Kohima", FK_StateId = 18 },
            new District { DistrictId = 18005, DistrictName = "Longleng", FK_StateId = 18 },
            new District { DistrictId = 18006, DistrictName = "Mokokchung", FK_StateId = 18 },
            new District { DistrictId = 18007, DistrictName = "Mon", FK_StateId = 18 },
            new District { DistrictId = 18008, DistrictName = "Niuland", FK_StateId = 18 },
            new District { DistrictId = 18009, DistrictName = "Noklak", FK_StateId = 18 },
            new District { DistrictId = 18010, DistrictName = "Peren", FK_StateId = 18 },
            new District { DistrictId = 18011, DistrictName = "Phek", FK_StateId = 18 },
            new District { DistrictId = 18012, DistrictName = "Shamator", FK_StateId = 18 },
            new District { DistrictId = 18013, DistrictName = "Tseminyu", FK_StateId = 18 },
            new District { DistrictId = 18014, DistrictName = "Tuensang", FK_StateId = 18 },
            new District { DistrictId = 18015, DistrictName = "Wokha", FK_StateId = 18 },
            new District { DistrictId = 18016, DistrictName = "Zunheboto", FK_StateId = 18 },

            // Odisha (StateId: 19)
            new District { DistrictId = 19001, DistrictName = "Angul", FK_StateId = 19 },
            new District { DistrictId = 19002, DistrictName = "Balangir", FK_StateId = 19 },
            new District { DistrictId = 19003, DistrictName = "Balasore", FK_StateId = 19 },
            new District { DistrictId = 19004, DistrictName = "Bargarh", FK_StateId = 19 },
            new District { DistrictId = 19005, DistrictName = "Bhadrak", FK_StateId = 19 },
            new District { DistrictId = 19006, DistrictName = "Boudh", FK_StateId = 19 },
            new District { DistrictId = 19007, DistrictName = "Cuttack", FK_StateId = 19 },
            new District { DistrictId = 19008, DistrictName = "Debagarh (Deogarh)", FK_StateId = 19 },
            new District { DistrictId = 19009, DistrictName = "Dhenkanal", FK_StateId = 19 },
            new District { DistrictId = 19010, DistrictName = "Gajapati", FK_StateId = 19 },
            new District { DistrictId = 19011, DistrictName = "Ganjam", FK_StateId = 19 },
            new District { DistrictId = 19012, DistrictName = "Jagatsinghpur", FK_StateId = 19 },
            new District { DistrictId = 19013, DistrictName = "Jajpur", FK_StateId = 19 },
            new District { DistrictId = 19014, DistrictName = "Jharsuguda", FK_StateId = 19 },
            new District { DistrictId = 19015, DistrictName = "Kalahandi", FK_StateId = 19 },
            new District { DistrictId = 19016, DistrictName = "Kandhamal", FK_StateId = 19 },
            new District { DistrictId = 19017, DistrictName = "Kendrapara", FK_StateId = 19 },
            new District { DistrictId = 19018, DistrictName = "Kendujhar (Keonjhar)", FK_StateId = 19 },
            new District { DistrictId = 19019, DistrictName = "Khordha", FK_StateId = 19 },
            new District { DistrictId = 19020, DistrictName = "Koraput", FK_StateId = 19 },
            new District { DistrictId = 19021, DistrictName = "Malkangiri", FK_StateId = 19 },
            new District { DistrictId = 19022, DistrictName = "Mayurbhanj", FK_StateId = 19 },
            new District { DistrictId = 19023, DistrictName = "Nabarangpur", FK_StateId = 19 },
            new District { DistrictId = 19024, DistrictName = "Nayagarh", FK_StateId = 19 },
            new District { DistrictId = 19025, DistrictName = "Nuapada", FK_StateId = 19 },
            new District { DistrictId = 19026, DistrictName = "Puri", FK_StateId = 19 },
            new District { DistrictId = 19027, DistrictName = "Rayagada", FK_StateId = 19 },
            new District { DistrictId = 19028, DistrictName = "Sambalpur", FK_StateId = 19 },
            new District { DistrictId = 19029, DistrictName = "Subarnapur (Sonepur)", FK_StateId = 19 },
            new District { DistrictId = 19030, DistrictName = "Sundargarh", FK_StateId = 19 },

            // Punjab (StateId: 20)
            new District { DistrictId = 20001, DistrictName = "Amritsar", FK_StateId = 20 },
            new District { DistrictId = 20002, DistrictName = "Barnala", FK_StateId = 20 },
            new District { DistrictId = 20003, DistrictName = "Bathinda", FK_StateId = 20 },
            new District { DistrictId = 20004, DistrictName = "Faridkot", FK_StateId = 20 },
            new District { DistrictId = 20005, DistrictName = "Fatehgarh Sahib", FK_StateId = 20 },
            new District { DistrictId = 20006, DistrictName = "Fazilka", FK_StateId = 20 },
            new District { DistrictId = 20007, DistrictName = "Ferozepur", FK_StateId = 20 },
            new District { DistrictId = 20008, DistrictName = "Gurdaspur", FK_StateId = 20 },
            new District { DistrictId = 20009, DistrictName = "Hoshiarpur", FK_StateId = 20 },
            new District { DistrictId = 20010, DistrictName = "Jalandhar", FK_StateId = 20 },
            new District { DistrictId = 20011, DistrictName = "Kapurthala", FK_StateId = 20 },
            new District { DistrictId = 20012, DistrictName = "Ludhiana", FK_StateId = 20 },
            new District { DistrictId = 20013, DistrictName = "Mansa", FK_StateId = 20 },
            new District { DistrictId = 20014, DistrictName = "Moga", FK_StateId = 20 },
            new District { DistrictId = 20015, DistrictName = "Muktsar", FK_StateId = 20 },
            new District { DistrictId = 20016, DistrictName = "Pathankot", FK_StateId = 20 },
            new District { DistrictId = 20017, DistrictName = "Patiala", FK_StateId = 20 },
            new District { DistrictId = 20018, DistrictName = "Rupnagar", FK_StateId = 20 },
            new District { DistrictId = 20019, DistrictName = "Sangrur", FK_StateId = 20 },
            new District { DistrictId = 20020, DistrictName = "SAS Nagar (Mohali)", FK_StateId = 20 },
            new District { DistrictId = 20021, DistrictName = "Shaheed Bhagat Singh Nagar", FK_StateId = 20 },
            new District { DistrictId = 20022, DistrictName = "Sri Muktsar Sahib", FK_StateId = 20 },
            new District { DistrictId = 20023, DistrictName = "Tarn Taran", FK_StateId = 20 },

            // Rajasthan (StateId: 21)
            new District { DistrictId = 21001, DistrictName = "Ajmer", FK_StateId = 21 },
            new District { DistrictId = 21002, DistrictName = "Alwar", FK_StateId = 21 },
            new District { DistrictId = 21003, DistrictName = "Banswara", FK_StateId = 21 },
            new District { DistrictId = 21004, DistrictName = "Baran", FK_StateId = 21 },
            new District { DistrictId = 21005, DistrictName = "Barmer", FK_StateId = 21 },
            new District { DistrictId = 21006, DistrictName = "Bharatpur", FK_StateId = 21 },
            new District { DistrictId = 21007, DistrictName = "Bhilwara", FK_StateId = 21 },
            new District { DistrictId = 21008, DistrictName = "Bikaner", FK_StateId = 21 },
            new District { DistrictId = 21009, DistrictName = "Bundi", FK_StateId = 21 },
            new District { DistrictId = 21010, DistrictName = "Chittorgarh", FK_StateId = 21 },
            new District { DistrictId = 21011, DistrictName = "Churu", FK_StateId = 21 },
            new District { DistrictId = 21012, DistrictName = "Dausa", FK_StateId = 21 },
            new District { DistrictId = 21013, DistrictName = "Dholpur", FK_StateId = 21 },
            new District { DistrictId = 21014, DistrictName = "Dungarpur", FK_StateId = 21 },
            new District { DistrictId = 21015, DistrictName = "Hanumangarh", FK_StateId = 21 },
            new District { DistrictId = 21016, DistrictName = "Jaipur", FK_StateId = 21 },
            new District { DistrictId = 21017, DistrictName = "Jaisalmer", FK_StateId = 21 },
            new District { DistrictId = 21018, DistrictName = "Jalore", FK_StateId = 21 },
            new District { DistrictId = 21019, DistrictName = "Jhalawar", FK_StateId = 21 },
            new District { DistrictId = 21020, DistrictName = "Jhunjhunu", FK_StateId = 21 },
            new District { DistrictId = 21021, DistrictName = "Jodhpur", FK_StateId = 21 },
            new District { DistrictId = 21022, DistrictName = "Karauli", FK_StateId = 21 },
            new District { DistrictId = 21023, DistrictName = "Kota", FK_StateId = 21 },
            new District { DistrictId = 21024, DistrictName = "Nagaur", FK_StateId = 21 },
            new District { DistrictId = 21025, DistrictName = "Pali", FK_StateId = 21 },
            new District { DistrictId = 21026, DistrictName = "Pratapgarh", FK_StateId = 21 },
            new District { DistrictId = 21027, DistrictName = "Rajsamand", FK_StateId = 21 },
            new District { DistrictId = 21028, DistrictName = "Sawai Madhopur", FK_StateId = 21 },
            new District { DistrictId = 21029, DistrictName = "Sikar", FK_StateId = 21 },
            new District { DistrictId = 21030, DistrictName = "Sirohi", FK_StateId = 21 },
            new District { DistrictId = 21031, DistrictName = "Sri Ganganagar", FK_StateId = 21 },
            new District { DistrictId = 21032, DistrictName = "Tonk", FK_StateId = 21 },
            new District { DistrictId = 21033, DistrictName = "Udaipur", FK_StateId = 21 },

            // Sikkim (StateId: 22)
            new District { DistrictId = 22001, DistrictName = "East Sikkim", FK_StateId = 22 },
            new District { DistrictId = 22002, DistrictName = "North Sikkim", FK_StateId = 22 },
            new District { DistrictId = 22003, DistrictName = "South Sikkim", FK_StateId = 22 },
            new District { DistrictId = 22004, DistrictName = "West Sikkim", FK_StateId = 22 },

            // Tamil Nadu (StateId: 23)
            new District { DistrictId = 23001, DistrictName = "Ariyalur", FK_StateId = 23 },
            new District { DistrictId = 23002, DistrictName = "Chengalpattu", FK_StateId = 23 },
            new District { DistrictId = 23003, DistrictName = "Chennai", FK_StateId = 23 },
            new District { DistrictId = 23004, DistrictName = "Coimbatore", FK_StateId = 23 },
            new District { DistrictId = 23005, DistrictName = "Cuddalore", FK_StateId = 23 },
            new District { DistrictId = 23006, DistrictName = "Dharmapuri", FK_StateId = 23 },
            new District { DistrictId = 23007, DistrictName = "Dindigul", FK_StateId = 23 },
            new District { DistrictId = 23008, DistrictName = "Erode", FK_StateId = 23 },
            new District { DistrictId = 23009, DistrictName = "Kallakurichi", FK_StateId = 23 },
            new District { DistrictId = 23010, DistrictName = "Kanchipuram", FK_StateId = 23 },
            new District { DistrictId = 23011, DistrictName = "Kanyakumari", FK_StateId = 23 },
            new District { DistrictId = 23012, DistrictName = "Karur", FK_StateId = 23 },
            new District { DistrictId = 23013, DistrictName = "Krishnagiri", FK_StateId = 23 },
            new District { DistrictId = 23014, DistrictName = "Madurai", FK_StateId = 23 },
            new District { DistrictId = 23015, DistrictName = "Mayiladuthurai", FK_StateId = 23 },
            new District { DistrictId = 23016, DistrictName = "Nagapattinam", FK_StateId = 23 },
            new District { DistrictId = 23017, DistrictName = "Namakkal", FK_StateId = 23 },
            new District { DistrictId = 23018, DistrictName = "Nilgiris", FK_StateId = 23 },
            new District { DistrictId = 23019, DistrictName = "Perambalur", FK_StateId = 23 },
            new District { DistrictId = 23020, DistrictName = "Pudukkottai", FK_StateId = 23 },
            new District { DistrictId = 23021, DistrictName = "Ramanathapuram", FK_StateId = 23 },
            new District { DistrictId = 23022, DistrictName = "Ranipet", FK_StateId = 23 },
            new District { DistrictId = 23023, DistrictName = "Salem", FK_StateId = 23 },
            new District { DistrictId = 23024, DistrictName = "Sivaganga", FK_StateId = 23 },
            new District { DistrictId = 23025, DistrictName = "Tenkasi", FK_StateId = 23 },
            new District { DistrictId = 23026, DistrictName = "Thanjavur", FK_StateId = 23 },
            new District { DistrictId = 23027, DistrictName = "Theni", FK_StateId = 23 },
            new District { DistrictId = 23028, DistrictName = "Thoothukudi (Tuticorin)", FK_StateId = 23 },
            new District { DistrictId = 23029, DistrictName = "Tiruchirappalli", FK_StateId = 23 },
            new District { DistrictId = 23030, DistrictName = "Tirunelveli", FK_StateId = 23 },
            new District { DistrictId = 23031, DistrictName = "Tirupattur", FK_StateId = 23 },
            new District { DistrictId = 23032, DistrictName = "Tiruppur", FK_StateId = 23 },
            new District { DistrictId = 23033, DistrictName = "Tiruvallur", FK_StateId = 23 },
            new District { DistrictId = 23034, DistrictName = "Tiruvannamalai", FK_StateId = 23 },
            new District { DistrictId = 23035, DistrictName = "Tiruvarur", FK_StateId = 23 },
            new District { DistrictId = 23036, DistrictName = "Vellore", FK_StateId = 23 },
            new District { DistrictId = 23037, DistrictName = "Viluppuram", FK_StateId = 23 },
            new District { DistrictId = 23038, DistrictName = "Virudhunagar", FK_StateId = 23 },

            // Telangana (StateId: 24)
            new District { DistrictId = 24001, DistrictName = "Adilabad", FK_StateId = 24 },
            new District { DistrictId = 24002, DistrictName = "Bhadradri Kothagudem", FK_StateId = 24 },
            new District { DistrictId = 24003, DistrictName = "Hyderabad", FK_StateId = 24 },
            new District { DistrictId = 24004, DistrictName = "Jagtial", FK_StateId = 24 },
            new District { DistrictId = 24005, DistrictName = "Jangaon", FK_StateId = 24 },
            new District { DistrictId = 24006, DistrictName = "Jayashankar Bhupalpally", FK_StateId = 24 },
            new District { DistrictId = 24007, DistrictName = "Jogulamba Gadwal", FK_StateId = 24 },
            new District { DistrictId = 24008, DistrictName = "Kamareddy", FK_StateId = 24 },
            new District { DistrictId = 24009, DistrictName = "Karimnagar", FK_StateId = 24 },
            new District { DistrictId = 24010, DistrictName = "Khammam", FK_StateId = 24 },
            new District { DistrictId = 24011, DistrictName = "Komaram Bheem Asifabad", FK_StateId = 24 },
            new District { DistrictId = 24012, DistrictName = "Mahabubabad", FK_StateId = 24 },
            new District { DistrictId = 24013, DistrictName = "Mahabubnagar", FK_StateId = 24 },
            new District { DistrictId = 24014, DistrictName = "Mancherial", FK_StateId = 24 },
            new District { DistrictId = 24015, DistrictName = "Medak", FK_StateId = 24 },
            new District { DistrictId = 24016, DistrictName = "Medchal-Malkajgiri", FK_StateId = 24 },
            new District { DistrictId = 24017, DistrictName = "Mulugu", FK_StateId = 24 },
            new District { DistrictId = 24018, DistrictName = "Nagarkurnool", FK_StateId = 24 },
            new District { DistrictId = 24019, DistrictName = "Nalgonda", FK_StateId = 24 },
            new District { DistrictId = 24020, DistrictName = "Narayanpet", FK_StateId = 24 },
            new District { DistrictId = 24021, DistrictName = "Nirmal", FK_StateId = 24 },
            new District { DistrictId = 24022, DistrictName = "Nizamabad", FK_StateId = 24 },
            new District { DistrictId = 24023, DistrictName = "Peddapalli", FK_StateId = 24 },
            new District { DistrictId = 24024, DistrictName = "Rajanna Sircilla", FK_StateId = 24 },
            new District { DistrictId = 24025, DistrictName = "Rangareddy", FK_StateId = 24 },
            new District { DistrictId = 24026, DistrictName = "Sangareddy", FK_StateId = 24 },
            new District { DistrictId = 24027, DistrictName = "Siddipet", FK_StateId = 24 },
            new District { DistrictId = 24028, DistrictName = "Suryapet", FK_StateId = 24 },
            new District { DistrictId = 24029, DistrictName = "Vikarabad", FK_StateId = 24 },
            new District { DistrictId = 24030, DistrictName = "Wanaparthy", FK_StateId = 24 },
            new District { DistrictId = 24031, DistrictName = "Warangal Rural", FK_StateId = 24 },
            new District { DistrictId = 24032, DistrictName = "Warangal Urban", FK_StateId = 24 },
            new District { DistrictId = 24033, DistrictName = "Yadadri Bhuvanagiri", FK_StateId = 24 },

            // Tripura (StateId: 25)
            new District { DistrictId = 25001, DistrictName = "Dhalai", FK_StateId = 25 },
            new District { DistrictId = 25002, DistrictName = "Gomati", FK_StateId = 25 },
            new District { DistrictId = 25003, DistrictName = "Khowai", FK_StateId = 25 },
            new District { DistrictId = 25004, DistrictName = "North Tripura", FK_StateId = 25 },
            new District { DistrictId = 25005, DistrictName = "Sepahijala", FK_StateId = 25 },
            new District { DistrictId = 25006, DistrictName = "South Tripura", FK_StateId = 25 },
            new District { DistrictId = 25007, DistrictName = "Unakoti", FK_StateId = 25 },
            new District { DistrictId = 25008, DistrictName = "West Tripura", FK_StateId = 25 },

            // Uttar Pradesh (StateId: 26)
            new District { DistrictId = 26001, DistrictName = "Agra", FK_StateId = 26 },
            new District { DistrictId = 26002, DistrictName = "Aligarh", FK_StateId = 26 },
            new District { DistrictId = 26003, DistrictName = "Ambedkar Nagar", FK_StateId = 26 },
            new District { DistrictId = 26004, DistrictName = "Amethi", FK_StateId = 26 },
            new District { DistrictId = 26005, DistrictName = "Amroha", FK_StateId = 26 },
            new District { DistrictId = 26006, DistrictName = "Auraiya", FK_StateId = 26 },
            new District { DistrictId = 26007, DistrictName = "Ayodhya", FK_StateId = 26 },
            new District { DistrictId = 26008, DistrictName = "Azamgarh", FK_StateId = 26 },
            new District { DistrictId = 26009, DistrictName = "Baghpat", FK_StateId = 26 },
            new District { DistrictId = 26010, DistrictName = "Bahraich", FK_StateId = 26 },
            new District { DistrictId = 26011, DistrictName = "Ballia", FK_StateId = 26 },
            new District { DistrictId = 26012, DistrictName = "Balrampur", FK_StateId = 26 },
            new District { DistrictId = 26013, DistrictName = "Banda", FK_StateId = 26 },
            new District { DistrictId = 26014, DistrictName = "Barabanki", FK_StateId = 26 },
            new District { DistrictId = 26015, DistrictName = "Bareilly", FK_StateId = 26 },
            new District { DistrictId = 26016, DistrictName = "Basti", FK_StateId = 26 },
            new District { DistrictId = 26017, DistrictName = "Bhadohi", FK_StateId = 26 },
            new District { DistrictId = 26018, DistrictName = "Bijnor", FK_StateId = 26 },
            new District { DistrictId = 26019, DistrictName = "Budaun", FK_StateId = 26 },
            new District { DistrictId = 26020, DistrictName = "Bulandshahr", FK_StateId = 26 },
            new District { DistrictId = 26021, DistrictName = "Chandauli", FK_StateId = 26 },
            new District { DistrictId = 26022, DistrictName = "Chitrakoot", FK_StateId = 26 },
            new District { DistrictId = 26023, DistrictName = "Deoria", FK_StateId = 26 },
            new District { DistrictId = 26024, DistrictName = "Etah", FK_StateId = 26 },
            new District { DistrictId = 26025, DistrictName = "Etawah", FK_StateId = 26 },
            new District { DistrictId = 26026, DistrictName = "Farrukhabad", FK_StateId = 26 },
            new District { DistrictId = 26027, DistrictName = "Fatehpur", FK_StateId = 26 },
            new District { DistrictId = 26028, DistrictName = "Firozabad", FK_StateId = 26 },
            new District { DistrictId = 26029, DistrictName = "Gautam Buddh Nagar", FK_StateId = 26 },
            new District { DistrictId = 26030, DistrictName = "Ghaziabad", FK_StateId = 26 },
            new District { DistrictId = 26031, DistrictName = "Ghazipur", FK_StateId = 26 },
            new District { DistrictId = 26032, DistrictName = "Gonda", FK_StateId = 26 },
            new District { DistrictId = 26033, DistrictName = "Gorakhpur", FK_StateId = 26 },
            new District { DistrictId = 26034, DistrictName = "Hamirpur", FK_StateId = 26 },
            new District { DistrictId = 26035, DistrictName = "Hapur", FK_StateId = 26 },
            new District { DistrictId = 26036, DistrictName = "Hardoi", FK_StateId = 26 },
            new District { DistrictId = 26037, DistrictName = "Hathras", FK_StateId = 26 },
            new District { DistrictId = 26038, DistrictName = "Jalaun", FK_StateId = 26 },
            new District { DistrictId = 26039, DistrictName = "Jaunpur", FK_StateId = 26 },
            new District { DistrictId = 26040, DistrictName = "Jhansi", FK_StateId = 26 },
            new District { DistrictId = 26041, DistrictName = "Kannauj", FK_StateId = 26 },
            new District { DistrictId = 26042, DistrictName = "Kanpur Dehat", FK_StateId = 26 },
            new District { DistrictId = 26043, DistrictName = "Kanpur Nagar", FK_StateId = 26 },
            new District { DistrictId = 26044, DistrictName = "Kasganj", FK_StateId = 26 },
            new District { DistrictId = 26045, DistrictName = "Kaushambi", FK_StateId = 26 },
            new District { DistrictId = 26046, DistrictName = "Kheri", FK_StateId = 26 },
            new District { DistrictId = 26047, DistrictName = "Kushinagar", FK_StateId = 26 },
            new District { DistrictId = 26048, DistrictName = "Lalitpur", FK_StateId = 26 },
            new District { DistrictId = 26049, DistrictName = "Lucknow", FK_StateId = 26 },
            new District { DistrictId = 26050, DistrictName = "Maharajganj", FK_StateId = 26 },
            new District { DistrictId = 26051, DistrictName = "Mahoba", FK_StateId = 26 },
            new District { DistrictId = 26052, DistrictName = "Mainpuri", FK_StateId = 26 },
            new District { DistrictId = 26053, DistrictName = "Mathura", FK_StateId = 26 },
            new District { DistrictId = 26054, DistrictName = "Mau", FK_StateId = 26 },
            new District { DistrictId = 26055, DistrictName = "Meerut", FK_StateId = 26 },
            new District { DistrictId = 26056, DistrictName = "Mirzapur", FK_StateId = 26 },
            new District { DistrictId = 26057, DistrictName = "Moradabad", FK_StateId = 26 },
            new District { DistrictId = 26058, DistrictName = "Muzaffarnagar", FK_StateId = 26 },
            new District { DistrictId = 26059, DistrictName = "Pilibhit", FK_StateId = 26 },
            new District { DistrictId = 26060, DistrictName = "Pratapgarh", FK_StateId = 26 },
            new District { DistrictId = 26061, DistrictName = "Prayagraj", FK_StateId = 26 },
            new District { DistrictId = 26062, DistrictName = "Rae Bareli", FK_StateId = 26 },
            new District { DistrictId = 26063, DistrictName = "Rampur", FK_StateId = 26 },
            new District { DistrictId = 26064, DistrictName = "Saharanpur", FK_StateId = 26 },
            new District { DistrictId = 26065, DistrictName = "Sambhal", FK_StateId = 26 },
            new District { DistrictId = 26066, DistrictName = "Sant Kabir Nagar", FK_StateId = 26 },
            new District { DistrictId = 26067, DistrictName = "Shahjahanpur", FK_StateId = 26 },
            new District { DistrictId = 26068, DistrictName = "Shamli", FK_StateId = 26 },
            new District { DistrictId = 26069, DistrictName = "Shravasti", FK_StateId = 26 },
            new District { DistrictId = 26070, DistrictName = "Siddharthnagar", FK_StateId = 26 },
            new District { DistrictId = 26071, DistrictName = "Sitapur", FK_StateId = 26 },
            new District { DistrictId = 26072, DistrictName = "Sonbhadra", FK_StateId = 26 },
            new District { DistrictId = 26073, DistrictName = "Sultanpur", FK_StateId = 26 },
            new District { DistrictId = 26074, DistrictName = "Unnao", FK_StateId = 26 },
            new District { DistrictId = 26075, DistrictName = "Varanasi", FK_StateId = 26 },

            // Uttarakhand (StateId: 27)
            new District { DistrictId = 27001, DistrictName = "Almora", FK_StateId = 27 },
            new District { DistrictId = 27002, DistrictName = "Bageshwar", FK_StateId = 27 },
            new District { DistrictId = 27003, DistrictName = "Chamoli", FK_StateId = 27 },
            new District { DistrictId = 27004, DistrictName = "Champawat", FK_StateId = 27 },
            new District { DistrictId = 27005, DistrictName = "Dehradun", FK_StateId = 27 },
            new District { DistrictId = 27006, DistrictName = "Haridwar", FK_StateId = 27 },
            new District { DistrictId = 27007, DistrictName = "Nainital", FK_StateId = 27 },
            new District { DistrictId = 27008, DistrictName = "Pauri Garhwal", FK_StateId = 27 },
            new District { DistrictId = 27009, DistrictName = "Pithoragarh", FK_StateId = 27 },
            new District { DistrictId = 27010, DistrictName = "Rudraprayag", FK_StateId = 27 },
            new District { DistrictId = 27011, DistrictName = "Tehri Garhwal", FK_StateId = 27 },
            new District { DistrictId = 27012, DistrictName = "Udham Singh Nagar", FK_StateId = 27 },
            new District { DistrictId = 27013, DistrictName = "Uttarkashi", FK_StateId = 27 },

            // West Bengal (StateId: 28)
            new District { DistrictId = 28001, DistrictName = "Alipurduar", FK_StateId = 28 },
            new District { DistrictId = 28002, DistrictName = "Bankura", FK_StateId = 28 },
            new District { DistrictId = 28003, DistrictName = "Paschim Bardhaman", FK_StateId = 28 },
            new District { DistrictId = 28004, DistrictName = "Purba Bardhaman", FK_StateId = 28 },
            new District { DistrictId = 28005, DistrictName = "Birbhum", FK_StateId = 28 },
            new District { DistrictId = 28006, DistrictName = "Cooch Behar", FK_StateId = 28 },
            new District { DistrictId = 28007, DistrictName = "Dakshin Dinajpur", FK_StateId = 28 },
            new District { DistrictId = 28008, DistrictName = "Darjeeling", FK_StateId = 28 },
            new District { DistrictId = 28009, DistrictName = "Hooghly", FK_StateId = 28 },
            new District { DistrictId = 28010, DistrictName = "Howrah", FK_StateId = 28 },
            new District { DistrictId = 28011, DistrictName = "Jalpaiguri", FK_StateId = 28 },
            new District { DistrictId = 28012, DistrictName = "Jhargram", FK_StateId = 28 },
            new District { DistrictId = 28013, DistrictName = "Kalimpong", FK_StateId = 28 },
            new District { DistrictId = 28014, DistrictName = "Kolkata", FK_StateId = 28 },
            new District { DistrictId = 28015, DistrictName = "Malda", FK_StateId = 28 },
            new District { DistrictId = 28016, DistrictName = "Murshidabad", FK_StateId = 28 },
            new District { DistrictId = 28017, DistrictName = "Nadia", FK_StateId = 28 },
            new District { DistrictId = 28018, DistrictName = "North 24 Parganas", FK_StateId = 28 },
            new District { DistrictId = 28019, DistrictName = "Paschim Medinipur", FK_StateId = 28 },
            new District { DistrictId = 28020, DistrictName = "Purba Medinipur", FK_StateId = 28 },
            new District { DistrictId = 28021, DistrictName = "Purulia", FK_StateId = 28 },
            new District { DistrictId = 28022, DistrictName = "South 24 Parganas", FK_StateId = 28 },
            new District { DistrictId = 28023, DistrictName = "Uttar Dinajpur", FK_StateId = 28 },

            // Andaman and Nicobar Islands (StateId: 29)
            new District { DistrictId = 29001, DistrictName = "Nicobar", FK_StateId = 29 },
            new District { DistrictId = 29002, DistrictName = "North and Middle Andaman", FK_StateId = 29 },
            new District { DistrictId = 29003, DistrictName = "South Andaman", FK_StateId = 29 },

            // Chandigarh (StateId: 30)
            new District { DistrictId = 30001, DistrictName = "Chandigarh", FK_StateId = 30 },

            // Dadra and Nagar Haveli and Daman & Diu (StateId: 31)
            new District { DistrictId = 31001, DistrictName = "Dadra and Nagar Haveli", FK_StateId = 31 },
            new District { DistrictId = 31002, DistrictName = "Daman", FK_StateId = 31 },
            new District { DistrictId = 31003, DistrictName = "Diu", FK_StateId = 31 },

            // Lakshadweep (StateId: 32)
            new District { DistrictId = 32001, DistrictName = "Lakshadweep", FK_StateId = 32 },

            // Delhi (StateId: 33)
            new District { DistrictId = 33001, DistrictName = "Central Delhi", FK_StateId = 33 },
            new District { DistrictId = 33002, DistrictName = "East Delhi", FK_StateId = 33 },
            new District { DistrictId = 33003, DistrictName = "New Delhi", FK_StateId = 33 },
            new District { DistrictId = 33004, DistrictName = "North Delhi", FK_StateId = 33 },
            new District { DistrictId = 33005, DistrictName = "North East Delhi", FK_StateId = 33 },
            new District { DistrictId = 33006, DistrictName = "North West Delhi", FK_StateId = 33 },
            new District { DistrictId = 33007, DistrictName = "Shahdara", FK_StateId = 33 },
            new District { DistrictId = 33008, DistrictName = "South Delhi", FK_StateId = 33 },
            new District { DistrictId = 33009, DistrictName = "South East Delhi", FK_StateId = 33 },
            new District { DistrictId = 33010, DistrictName = "South West Delhi", FK_StateId = 33 },
            new District { DistrictId = 33011, DistrictName = "West Delhi", FK_StateId = 33 },

            // Puducherry (StateId: 34)
            new District { DistrictId = 34001, DistrictName = "Karaikal", FK_StateId = 34 },
            new District { DistrictId = 34002, DistrictName = "Mahe", FK_StateId = 34 },
            new District { DistrictId = 34003, DistrictName = "Puducherry", FK_StateId = 34 },
            new District { DistrictId = 34004, DistrictName = "Yanam", FK_StateId = 34 }
        };
            mb.Entity<District>().HasData(districts);

            mb.Entity<Institute>().HasData(
           new Institute { InstituteId = 1, InstituteName = "University of Agricultural Sciences, GKVK", InstituteLogo = "logo1.png", Address = "GKVK, Bengaluru, Karnataka, India", ContactEmail = "gkvk@uasb.edu.in" },
           new Institute { InstituteId = 2, InstituteName = "Indian Council of Agricultural Research (ICAR)", InstituteLogo = "logo2.png", Address = "Krishi Bhawan, New Delhi, India", ContactEmail = "icar@nic.in" },
           new Institute { InstituteId = 3, InstituteName = "National Institute of Horticultural Research", InstituteLogo = "logo3.png", Address = "Hesaraghatta, Bengaluru, India", ContactEmail = "nihr@example.com" }
         );
            
        }
    }

    
}

         

