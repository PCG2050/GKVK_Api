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
        public DbSet<TableDataRow> TableDataRows { get; set; } = null!;
        public DbSet<TableDataCell> TableDataCells { get; set; } = null!;
        public DbSet<State> States { get; set; } = null!;
        public DbSet<District> Districts { get; set; } = null!;
        public DbSet<Institute> Institutes { get; set; } = null!;
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
                .HasForeignKey(d => d.StateId)
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
            static OrgUnit OU(int id, string name,int instituteId, int? parentId = null, int? templateSourceId = null,int? districtId = null) =>
                new() { OrgUnitId = id, Name = name, ParentId = parentId ,TemplateSourceId = templateSourceId ,InstituteId = instituteId,DistrictId= districtId };

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
            OU(1201, "KVK, Hassan",       1, parentId: 1100, templateSourceId: 1000, districtId: 2902),

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
        }
    }
}

         

