namespace HearingClinicManagementSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Appointment",
                c => new
                    {
                        AppointmentID = c.Int(nullable: false, identity: true),
                        PatientID = c.Int(nullable: false),
                        AudiologistID = c.Int(nullable: false),
                        CreatedBy = c.Int(),
                        Date = c.DateTime(nullable: false),
                        TimeSlotID = c.Int(nullable: false),
                        PurposeOfVisit = c.String(nullable: false, maxLength: 500),
                        Status = c.String(nullable: false, maxLength: 20),
                        Fee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FollowUpRequired = c.Boolean(nullable: false),
                        FollowUpDate = c.DateTime(),
                        Creator_UserID = c.Int(),
                    })
                .PrimaryKey(t => t.AppointmentID)
                .ForeignKey("dbo.Audiologist", t => t.AudiologistID)
                .ForeignKey("dbo.User", t => t.Creator_UserID)
                .ForeignKey("dbo.Patient", t => t.PatientID)
                .ForeignKey("dbo.TimeSlot", t => t.TimeSlotID)
                .Index(t => t.PatientID)
                .Index(t => t.AudiologistID)
                .Index(t => t.TimeSlotID)
                .Index(t => t.Creator_UserID);
            
            CreateTable(
                "dbo.Audiologist",
                c => new
                    {
                        AudiologistID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        Specialization = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.AudiologistID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 100),
                        Phone = c.String(nullable: false, maxLength: 20),
                        Role = c.String(nullable: false, maxLength: 20),
                        Username = c.String(nullable: false, maxLength: 50),
                        PasswordHash = c.String(nullable: false, maxLength: 255),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserID);
            
            CreateTable(
                "dbo.Patient",
                c => new
                    {
                        PatientID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false),
                        Address = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.PatientID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.TimeSlot",
                c => new
                    {
                        TimeSlotID = c.Int(nullable: false, identity: true),
                        ScheduleID = c.Int(nullable: false),
                        StartTime = c.Time(nullable: false, precision: 7),
                        EndTime = c.Time(nullable: false, precision: 7),
                        IsAvailable = c.Boolean(nullable: false),
                        Schedule_ScheduleID = c.Int(),
                    })
                .PrimaryKey(t => t.TimeSlotID)
                .ForeignKey("dbo.Schedule", t => t.Schedule_ScheduleID)
                .ForeignKey("dbo.Schedule", t => t.ScheduleID, cascadeDelete: true)
                .Index(t => t.ScheduleID)
                .Index(t => t.Schedule_ScheduleID);
            
            CreateTable(
                "dbo.Schedule",
                c => new
                    {
                        ScheduleID = c.Int(nullable: false, identity: true),
                        AudiologistID = c.Int(nullable: false),
                        DayOfWeek = c.String(nullable: false, maxLength: 10),
                    })
                .PrimaryKey(t => t.ScheduleID)
                .ForeignKey("dbo.Audiologist", t => t.AudiologistID, cascadeDelete: true)
                .Index(t => t.AudiologistID);
            
            CreateTable(
                "dbo.AudiogramData",
                c => new
                    {
                        AudiogramDataID = c.Int(nullable: false, identity: true),
                        TestID = c.Int(nullable: false),
                        Ear = c.String(nullable: false, maxLength: 10),
                        Frequency = c.Int(nullable: false),
                        Threshold = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AudiogramDataID)
                .ForeignKey("dbo.HearingTest", t => t.TestID)
                .Index(t => t.TestID);
            
            CreateTable(
                "dbo.HearingTest",
                c => new
                    {
                        TestID = c.Int(nullable: false, identity: true),
                        RecordID = c.Int(nullable: false),
                        TestType = c.String(nullable: false, maxLength: 50),
                        TestDate = c.DateTime(nullable: false),
                        TestNotes = c.String(maxLength: 2000),
                    })
                .PrimaryKey(t => t.TestID)
                .ForeignKey("dbo.MedicalRecord", t => t.RecordID)
                .Index(t => t.RecordID);
            
            CreateTable(
                "dbo.MedicalRecord",
                c => new
                    {
                        RecordID = c.Int(nullable: false, identity: true),
                        PatientID = c.Int(nullable: false),
                        AppointmentID = c.Int(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        ChiefComplaint = c.String(nullable: false, maxLength: 1000),
                        Diagnosis = c.String(maxLength: 1000),
                        TreatmentPlan = c.String(maxLength: 2000),
                        RecordDate = c.DateTime(nullable: false),
                        Creator_AudiologistID = c.Int(),
                    })
                .PrimaryKey(t => t.RecordID)
                .ForeignKey("dbo.Appointment", t => t.AppointmentID)
                .ForeignKey("dbo.Audiologist", t => t.Creator_AudiologistID)
                .ForeignKey("dbo.Patient", t => t.PatientID)
                .Index(t => t.PatientID)
                .Index(t => t.AppointmentID)
                .Index(t => t.Creator_AudiologistID);
            
            CreateTable(
                "dbo.ClinicManager",
                c => new
                    {
                        ClinicManagerID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ClinicManagerID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.InventoryManager",
                c => new
                    {
                        InventoryManagerID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InventoryManagerID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.InventoryTransaction",
                c => new
                    {
                        TransactionID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        TransactionType = c.String(nullable: false, maxLength: 20),
                        Quantity = c.Int(nullable: false),
                        TransactionDate = c.DateTime(nullable: false),
                        ProcessedBy = c.Int(nullable: false),
                        Processor_InventoryManagerID = c.Int(),
                    })
                .PrimaryKey(t => t.TransactionID)
                .ForeignKey("dbo.InventoryManager", t => t.Processor_InventoryManagerID)
                .ForeignKey("dbo.Product", t => t.ProductID)
                .Index(t => t.ProductID)
                .Index(t => t.Processor_InventoryManagerID);
            
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        ProductID = c.Int(nullable: false, identity: true),
                        Manufacturer = c.String(nullable: false, maxLength: 100),
                        Model = c.String(nullable: false, maxLength: 100),
                        Features = c.String(maxLength: 500),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        QuantityInStock = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductID);
            
            CreateTable(
                "dbo.Invoice",
                c => new
                    {
                        InvoiceID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(),
                        AppointmentID = c.Int(),
                        InvoiceDate = c.DateTime(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.String(nullable: false, maxLength: 20),
                        PaymentMethod = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.InvoiceID)
                .ForeignKey("dbo.Appointment", t => t.AppointmentID)
                .ForeignKey("dbo.Order", t => t.OrderID)
                .Index(t => t.OrderID)
                .Index(t => t.AppointmentID);
            
            CreateTable(
                "dbo.Order",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        PatientID = c.Int(nullable: false),
                        ProcessedBy = c.Int(),
                        OrderDate = c.DateTime(nullable: false),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveryAddress = c.String(nullable: false, maxLength: 200),
                        Status = c.String(nullable: false, maxLength: 20),
                        Processor_InventoryManagerID = c.Int(),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.Patient", t => t.PatientID)
                .ForeignKey("dbo.InventoryManager", t => t.Processor_InventoryManagerID)
                .Index(t => t.PatientID)
                .Index(t => t.Processor_InventoryManagerID);
            
            CreateTable(
                "dbo.OrderItem",
                c => new
                    {
                        OrderItemID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Order_OrderID = c.Int(),
                    })
                .PrimaryKey(t => t.OrderItemID)
                .ForeignKey("dbo.Order", t => t.OrderID, cascadeDelete: true)
                .ForeignKey("dbo.Product", t => t.ProductID)
                .ForeignKey("dbo.Order", t => t.Order_OrderID)
                .Index(t => t.OrderID)
                .Index(t => t.ProductID)
                .Index(t => t.Order_OrderID);
            
            CreateTable(
                "dbo.Payment",
                c => new
                    {
                        PaymentID = c.Int(nullable: false, identity: true),
                        InvoiceID = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentDate = c.DateTime(nullable: false),
                        ReceivedBy = c.Int(nullable: false),
                        PaymentMethod = c.String(maxLength: 20),
                        Receiver_ReceptionistID = c.Int(),
                    })
                .PrimaryKey(t => t.PaymentID)
                .ForeignKey("dbo.Invoice", t => t.InvoiceID)
                .ForeignKey("dbo.Receptionist", t => t.Receiver_ReceptionistID)
                .Index(t => t.InvoiceID)
                .Index(t => t.Receiver_ReceptionistID);
            
            CreateTable(
                "dbo.Receptionist",
                c => new
                    {
                        ReceptionistID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReceptionistID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Prescription",
                c => new
                    {
                        PrescriptionID = c.Int(nullable: false, identity: true),
                        AppointmentID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        PrescribedBy = c.Int(nullable: false),
                        PrescribedDate = c.DateTime(nullable: false),
                        Prescriber_AudiologistID = c.Int(),
                    })
                .PrimaryKey(t => t.PrescriptionID)
                .ForeignKey("dbo.Appointment", t => t.AppointmentID)
                .ForeignKey("dbo.Audiologist", t => t.Prescriber_AudiologistID)
                .ForeignKey("dbo.Product", t => t.ProductID)
                .Index(t => t.AppointmentID)
                .Index(t => t.ProductID)
                .Index(t => t.Prescriber_AudiologistID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Prescription", "ProductID", "dbo.Product");
            DropForeignKey("dbo.Prescription", "Prescriber_AudiologistID", "dbo.Audiologist");
            DropForeignKey("dbo.Prescription", "AppointmentID", "dbo.Appointment");
            DropForeignKey("dbo.Payment", "Receiver_ReceptionistID", "dbo.Receptionist");
            DropForeignKey("dbo.Receptionist", "UserID", "dbo.User");
            DropForeignKey("dbo.Payment", "InvoiceID", "dbo.Invoice");
            DropForeignKey("dbo.Invoice", "OrderID", "dbo.Order");
            DropForeignKey("dbo.Order", "Processor_InventoryManagerID", "dbo.InventoryManager");
            DropForeignKey("dbo.Order", "PatientID", "dbo.Patient");
            DropForeignKey("dbo.OrderItem", "Order_OrderID", "dbo.Order");
            DropForeignKey("dbo.OrderItem", "ProductID", "dbo.Product");
            DropForeignKey("dbo.OrderItem", "OrderID", "dbo.Order");
            DropForeignKey("dbo.Invoice", "AppointmentID", "dbo.Appointment");
            DropForeignKey("dbo.InventoryTransaction", "ProductID", "dbo.Product");
            DropForeignKey("dbo.InventoryTransaction", "Processor_InventoryManagerID", "dbo.InventoryManager");
            DropForeignKey("dbo.InventoryManager", "UserID", "dbo.User");
            DropForeignKey("dbo.ClinicManager", "UserID", "dbo.User");
            DropForeignKey("dbo.AudiogramData", "TestID", "dbo.HearingTest");
            DropForeignKey("dbo.HearingTest", "RecordID", "dbo.MedicalRecord");
            DropForeignKey("dbo.MedicalRecord", "PatientID", "dbo.Patient");
            DropForeignKey("dbo.MedicalRecord", "Creator_AudiologistID", "dbo.Audiologist");
            DropForeignKey("dbo.MedicalRecord", "AppointmentID", "dbo.Appointment");
            DropForeignKey("dbo.Appointment", "TimeSlotID", "dbo.TimeSlot");
            DropForeignKey("dbo.TimeSlot", "ScheduleID", "dbo.Schedule");
            DropForeignKey("dbo.TimeSlot", "Schedule_ScheduleID", "dbo.Schedule");
            DropForeignKey("dbo.Schedule", "AudiologistID", "dbo.Audiologist");
            DropForeignKey("dbo.Appointment", "PatientID", "dbo.Patient");
            DropForeignKey("dbo.Patient", "UserID", "dbo.User");
            DropForeignKey("dbo.Appointment", "Creator_UserID", "dbo.User");
            DropForeignKey("dbo.Appointment", "AudiologistID", "dbo.Audiologist");
            DropForeignKey("dbo.Audiologist", "UserID", "dbo.User");
            DropIndex("dbo.Prescription", new[] { "Prescriber_AudiologistID" });
            DropIndex("dbo.Prescription", new[] { "ProductID" });
            DropIndex("dbo.Prescription", new[] { "AppointmentID" });
            DropIndex("dbo.Receptionist", new[] { "UserID" });
            DropIndex("dbo.Payment", new[] { "Receiver_ReceptionistID" });
            DropIndex("dbo.Payment", new[] { "InvoiceID" });
            DropIndex("dbo.OrderItem", new[] { "Order_OrderID" });
            DropIndex("dbo.OrderItem", new[] { "ProductID" });
            DropIndex("dbo.OrderItem", new[] { "OrderID" });
            DropIndex("dbo.Order", new[] { "Processor_InventoryManagerID" });
            DropIndex("dbo.Order", new[] { "PatientID" });
            DropIndex("dbo.Invoice", new[] { "AppointmentID" });
            DropIndex("dbo.Invoice", new[] { "OrderID" });
            DropIndex("dbo.InventoryTransaction", new[] { "Processor_InventoryManagerID" });
            DropIndex("dbo.InventoryTransaction", new[] { "ProductID" });
            DropIndex("dbo.InventoryManager", new[] { "UserID" });
            DropIndex("dbo.ClinicManager", new[] { "UserID" });
            DropIndex("dbo.MedicalRecord", new[] { "Creator_AudiologistID" });
            DropIndex("dbo.MedicalRecord", new[] { "AppointmentID" });
            DropIndex("dbo.MedicalRecord", new[] { "PatientID" });
            DropIndex("dbo.HearingTest", new[] { "RecordID" });
            DropIndex("dbo.AudiogramData", new[] { "TestID" });
            DropIndex("dbo.Schedule", new[] { "AudiologistID" });
            DropIndex("dbo.TimeSlot", new[] { "Schedule_ScheduleID" });
            DropIndex("dbo.TimeSlot", new[] { "ScheduleID" });
            DropIndex("dbo.Patient", new[] { "UserID" });
            DropIndex("dbo.Audiologist", new[] { "UserID" });
            DropIndex("dbo.Appointment", new[] { "Creator_UserID" });
            DropIndex("dbo.Appointment", new[] { "TimeSlotID" });
            DropIndex("dbo.Appointment", new[] { "AudiologistID" });
            DropIndex("dbo.Appointment", new[] { "PatientID" });
            DropTable("dbo.Prescription");
            DropTable("dbo.Receptionist");
            DropTable("dbo.Payment");
            DropTable("dbo.OrderItem");
            DropTable("dbo.Order");
            DropTable("dbo.Invoice");
            DropTable("dbo.Product");
            DropTable("dbo.InventoryTransaction");
            DropTable("dbo.InventoryManager");
            DropTable("dbo.ClinicManager");
            DropTable("dbo.MedicalRecord");
            DropTable("dbo.HearingTest");
            DropTable("dbo.AudiogramData");
            DropTable("dbo.Schedule");
            DropTable("dbo.TimeSlot");
            DropTable("dbo.Patient");
            DropTable("dbo.User");
            DropTable("dbo.Audiologist");
            DropTable("dbo.Appointment");
        }
    }
}
