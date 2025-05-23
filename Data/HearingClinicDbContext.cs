using HearingClinicManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HearingClinicManagementSystem.Data
{
    public class HearingClinicDbContext : DbContext
    {
        public HearingClinicDbContext() : base("name=HearingClinicConnection")
        {
            // Configure database initialization strategy
            Database.SetInitializer(new HearingClinicDbInitializer());
        }

        // Define DbSets for each entity in the system
        public DbSet<User> Users { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Audiologist> Audiologists { get; set; }
        public DbSet<Receptionist> Receptionists { get; set; }
        public DbSet<InventoryManager> InventoryManagers { get; set; }
        public DbSet<ClinicManager> ClinicManagers { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<HearingTest> HearingTests { get; set; }
        public DbSet<AudiogramData> AudiogramData { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Remove pluralizing table names convention
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Define primary keys for entities missing key definitions

            // Set the primary key for HearingTest
            modelBuilder.Entity<HearingTest>()
                .HasKey(ht => ht.TestID);

            // Set the primary key for MedicalRecord
            modelBuilder.Entity<MedicalRecord>()
                .HasKey(mr => mr.RecordID);

            // Set the primary key for InventoryTransaction
            modelBuilder.Entity<InventoryTransaction>()
                .HasKey(it => it.TransactionID);

            // Configure relationships
            // User relationships
            modelBuilder.Entity<Patient>()
                .HasRequired(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Audiologist>()
                .HasRequired(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Receptionist>()
                .HasRequired(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<InventoryManager>()
                .HasRequired(im => im.User)
                .WithMany()
                .HasForeignKey(im => im.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ClinicManager>()
                .HasRequired(cm => cm.User)
                .WithMany()
                .HasForeignKey(cm => cm.UserID)
                .WillCascadeOnDelete(false);

            // Schedule and TimeSlot
            modelBuilder.Entity<TimeSlot>()
                .HasRequired(ts => ts.Schedule)
                .WithMany()
                .HasForeignKey(ts => ts.ScheduleID)
                .WillCascadeOnDelete(true);

            // Appointment relationships
            modelBuilder.Entity<Appointment>()
                .HasRequired(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Appointment>()
                .HasRequired(a => a.Audiologist)
                .WithMany()
                .HasForeignKey(a => a.AudiologistID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Appointment>()
                .HasRequired(a => a.TimeSlot)
                .WithMany()
                .HasForeignKey(a => a.TimeSlotID)
                .WillCascadeOnDelete(false);

            // Medical Record relationships
            modelBuilder.Entity<MedicalRecord>()
                .HasRequired(mr => mr.Patient)
                .WithMany()
                .HasForeignKey(mr => mr.PatientID)
                .WillCascadeOnDelete(false);

            // Change from HasOptional to HasRequired since AppointmentID is required in the model
            modelBuilder.Entity<MedicalRecord>()
                .HasRequired(mr => mr.Appointment)  // Change from HasOptional to HasRequired
                .WithMany()
                .HasForeignKey(mr => mr.AppointmentID)
                .WillCascadeOnDelete(false);

            // Hearing Test relationships
            modelBuilder.Entity<HearingTest>()
                .HasRequired(ht => ht.MedicalRecord)
                .WithMany()
                .HasForeignKey(ht => ht.RecordID)
                .WillCascadeOnDelete(false);

            // AudiogramData relationships
            modelBuilder.Entity<AudiogramData>()
                .HasRequired(ad => ad.HearingTest)
                .WithMany(ht => ht.AudiogramData)
                .HasForeignKey(ad => ad.TestID)
                .WillCascadeOnDelete(false);

            // Order relationships
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.Patient)
                .WithMany()
                .HasForeignKey(o => o.PatientID)
                .WillCascadeOnDelete(false);

            // OrderItem relationships
            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Order)
                .WithMany()
                .HasForeignKey(oi => oi.OrderID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductID)
                .WillCascadeOnDelete(false);

            // Prescription relationships
            modelBuilder.Entity<Prescription>()
                .HasRequired(p => p.Appointment)
                .WithMany()
                .HasForeignKey(p => p.AppointmentID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Prescription>()
                .HasRequired(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductID)
                .WillCascadeOnDelete(false);

            // Invoice relationships
            modelBuilder.Entity<Invoice>()
                .HasOptional(i => i.Appointment)
                .WithMany()
                .HasForeignKey(i => i.AppointmentID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Invoice>()
                .HasOptional(i => i.Order)
                .WithMany()
                .HasForeignKey(i => i.OrderID)
                .WillCascadeOnDelete(false);

            // Payment relationships
            modelBuilder.Entity<Payment>()
                .HasRequired(p => p.Invoice)
                .WithMany()
                .HasForeignKey(p => p.InvoiceID)
                .WillCascadeOnDelete(false);

            // InventoryTransaction relationships
            modelBuilder.Entity<InventoryTransaction>()
                .HasRequired(it => it.Product)
                .WithMany()
                .HasForeignKey(it => it.ProductID)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}