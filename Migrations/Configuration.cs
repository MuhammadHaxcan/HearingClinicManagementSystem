using HearingClinicManagementSystem.Data;
using HearingClinicManagementSystem.Models;
using System;
using System.Data.Entity.Migrations;

namespace HearingClinicManagementSystem.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<HearingClinicDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "HearingClinicManagementSystem.Data.HearingClinicDbContext";
        }

        protected override void Seed(HearingClinicDbContext context)
        {
            // Create users for different roles
            var users = new[]
            {
                // Admin user
                new User
                {
                    UserID = 1,
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = "admin@hearingclinic.com",
                    Phone = "555-123-4567",
                    Role = "Admin",
                    Username = "admin",
                    PasswordHash = "admin123", // In a real app, this would be hashed
                    IsActive = true
                },
                // Clinic Manager
                new User
                {
                    UserID = 2,
                    FirstName = "John",
                    LastName = "Director",
                    Email = "john.director@hearingclinic.com",
                    Phone = "555-222-3333",
                    Role = "ClinicManager",
                    Username = "john.director",
                    PasswordHash = "password123",
                    IsActive = true
                },
                // Audiologist
                new User
                {
                    UserID = 3,
                    FirstName = "Sarah",
                    LastName = "Hearing",
                    Email = "sarah.hearing@hearingclinic.com",
                    Phone = "555-333-4444",
                    Role = "Audiologist",
                    Username = "sarah.hearing",
                    PasswordHash = "password123",
                    IsActive = true
                },
                // Receptionist
                new User
                {
                    UserID = 4,
                    FirstName = "Emily",
                    LastName = "Reception",
                    Email = "emily.reception@hearingclinic.com",
                    Phone = "555-444-5555",
                    Role = "Receptionist",
                    Username = "emily.reception",
                    PasswordHash = "password123",
                    IsActive = true
                },
                // Inventory Manager
                new User
                {
                    UserID = 5,
                    FirstName = "Mark",
                    LastName = "Inventory",
                    Email = "mark.inventory@hearingclinic.com",
                    Phone = "555-555-6666",
                    Role = "InventoryManager",
                    Username = "mark.inventory",
                    PasswordHash = "password123",
                    IsActive = true
                },
                // Patient 1
                new User
                {
                    UserID = 6,
                    FirstName = "Robert",
                    LastName = "Patient",
                    Email = "robert@example.com",
                    Phone = "555-777-8888",
                    Role = "Patient",
                    Username = "robert.patient",
                    PasswordHash = "password123",
                    IsActive = true
                },
                // Patient 2
                new User
                {
                    UserID = 7,
                    FirstName = "Alice",
                    LastName = "Smith",
                    Email = "alice.smith@example.com",
                    Phone = "555-888-9999",
                    Role = "Patient",
                    Username = "alice.smith",
                    PasswordHash = "password123",
                    IsActive = true
                },
                // Patient 3
                new User
                {
                    UserID = 8,
                    FirstName = "Michael",
                    LastName = "Johnson",
                    Email = "michael.johnson@example.com",
                    Phone = "555-999-0000",
                    Role = "Patient",
                    Username = "michael.johnson",
                    PasswordHash = "password123",
                    IsActive = true
                }
            };

            foreach (var user in users)
            {
                context.Users.AddOrUpdate(u => u.UserID, user);
            }
            
            // Force save to get IDs
            context.SaveChanges();

            // Create clinic manager
            context.ClinicManagers.AddOrUpdate(
                cm => cm.ClinicManagerID,
                new ClinicManager { ClinicManagerID = 1, UserID = 2 }
            );

            // Create audiologist
            context.Audiologists.AddOrUpdate(
                a => a.AudiologistID,
                new Audiologist
                {
                    AudiologistID = 1,
                    UserID = 3,
                    Specialization = "Hearing Aids & Cochlear Implants"
                }
            );

            // Create receptionist
            context.Receptionists.AddOrUpdate(
                r => r.ReceptionistID,
                new Receptionist { ReceptionistID = 1, UserID = 4 }
            );

            // Create inventory manager
            context.InventoryManagers.AddOrUpdate(
                im => im.InventoryManagerID,
                new InventoryManager { InventoryManagerID = 1, UserID = 5 }
            );

            // Create patients with different demographics
            context.Patients.AddOrUpdate(
                p => p.PatientID,
                new Patient
                {
                    PatientID = 1,
                    UserID = 6,
                    DateOfBirth = new DateTime(1965, 5, 15),
                    Address = "123 Maple St, Springfield, IL 62701"
                },
                new Patient
                {
                    PatientID = 2,
                    UserID = 7,
                    DateOfBirth = new DateTime(1982, 8, 22),
                    Address = "456 Oak Ave, Springfield, IL 62702"
                },
                new Patient
                {
                    PatientID = 3,
                    UserID = 8,
                    DateOfBirth = new DateTime(1990, 3, 10),
                    Address = "789 Pine Rd, Springfield, IL 62703"
                }
            );

            // Force save to get IDs
            context.SaveChanges();

            // Create schedules and time slots for the audiologist
            var schedule = new Schedule
            {
                ScheduleID = 1,
                AudiologistID = 1,
                DayOfWeek = "Monday"
            };

            context.Schedules.AddOrUpdate(s => s.ScheduleID, schedule);
            context.SaveChanges();

            // Create time slots for Monday
            var timeSlots = new[]
            {
                new TimeSlot { TimeSlotID = 1, ScheduleID = 1, StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(9, 30, 0), IsAvailable = true },
                new TimeSlot { TimeSlotID = 2, ScheduleID = 1, StartTime = new TimeSpan(9, 30, 0), EndTime = new TimeSpan(10, 0, 0), IsAvailable = true },
                new TimeSlot { TimeSlotID = 3, ScheduleID = 1, StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(10, 30, 0), IsAvailable = true },
                new TimeSlot { TimeSlotID = 4, ScheduleID = 1, StartTime = new TimeSpan(10, 30, 0), EndTime = new TimeSpan(11, 0, 0), IsAvailable = true },
                new TimeSlot { TimeSlotID = 5, ScheduleID = 1, StartTime = new TimeSpan(11, 0, 0), EndTime = new TimeSpan(11, 30, 0), IsAvailable = true },
                new TimeSlot { TimeSlotID = 6, ScheduleID = 1, StartTime = new TimeSpan(11, 30, 0), EndTime = new TimeSpan(12, 0, 0), IsAvailable = true },
                new TimeSlot { TimeSlotID = 7, ScheduleID = 1, StartTime = new TimeSpan(13, 0, 0), EndTime = new TimeSpan(13, 30, 0), IsAvailable = true },
                new TimeSlot { TimeSlotID = 8, ScheduleID = 1, StartTime = new TimeSpan(13, 30, 0), EndTime = new TimeSpan(14, 0, 0), IsAvailable = true },
                new TimeSlot { TimeSlotID = 9, ScheduleID = 1, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(14, 30, 0), IsAvailable = true },
                new TimeSlot { TimeSlotID = 10, ScheduleID = 1, StartTime = new TimeSpan(14, 30, 0), EndTime = new TimeSpan(15, 0, 0), IsAvailable = true },
                new TimeSlot { TimeSlotID = 11, ScheduleID = 1, StartTime = new TimeSpan(15, 0, 0), EndTime = new TimeSpan(15, 30, 0), IsAvailable = true },
                new TimeSlot { TimeSlotID = 12, ScheduleID = 1, StartTime = new TimeSpan(15, 30, 0), EndTime = new TimeSpan(16, 0, 0), IsAvailable = true }
            };

            foreach (var timeSlot in timeSlots)
            {
                context.TimeSlots.AddOrUpdate(ts => ts.TimeSlotID, timeSlot);
            }

            // Add some products to inventory
            var products = new[]
            {
                new Product
                {
                    ProductID = 1,
                    Manufacturer = "Phonak",
                    Model = "Audéo Paradise P90",
                    Features = "Premium hearing aid with speech enhancement, noise cancellation, Bluetooth connectivity",
                    Price = 2499.99m,
                    QuantityInStock = 15
                },
                new Product
                {
                    ProductID = 2,
                    Manufacturer = "Oticon",
                    Model = "More 1",
                    Features = "Advanced hearing aid with BrainHearing technology",
                    Price = 2299.99m,
                    QuantityInStock = 12
                },
                new Product
                {
                    ProductID = 3,
                    Manufacturer = "Widex",
                    Model = "Moment 440",
                    Features = "Natural sound with PureSound technology",
                    Price = 2199.99m,
                    QuantityInStock = 10
                },
                new Product
                {
                    ProductID = 4,
                    Manufacturer = "Resound",
                    Model = "ONE 9",
                    Features = "Organic hearing with M&RIE receiver",
                    Price = 2399.99m,
                    QuantityInStock = 8
                },
                new Product
                {
                    ProductID = 5,
                    Manufacturer = "Signia",
                    Model = "Pure Charge&Go AX",
                    Features = "Augmented focus technology for speech clarity",
                    Price = 2349.99m,
                    QuantityInStock = 9
                },
                new Product
                {
                    ProductID = 6,
                    Manufacturer = "Hearing Aid Batteries",
                    Model = "Size 312",
                    Features = "Pack of 60 batteries",
                    Price = 39.99m,
                    QuantityInStock = 100
                },
                new Product
                {
                    ProductID = 7,
                    Manufacturer = "Hearing Aid Cleaning Kit",
                    Model = "Premium",
                    Features = "Complete cleaning kit with wax removal tools",
                    Price = 29.99m,
                    QuantityInStock = 30
                }
            };

            foreach (var product in products)
            {
                context.Products.AddOrUpdate(p => p.ProductID, product);
            }

            // Create a sample appointment and medical record
            var appointment = new Appointment
            {
                AppointmentID = 1,
                PatientID = 1,
                AudiologistID = 1,
                Date = DateTime.Now.Date.AddDays(-7),
                TimeSlotID = 1,
                PurposeOfVisit = "Initial Consultation",
                Status = "Completed",
                Fee = 150.00m,
                FollowUpRequired = true,
                FollowUpDate = DateTime.Now.Date.AddDays(30)
            };

            context.Appointments.AddOrUpdate(a => a.AppointmentID, appointment);
            context.SaveChanges();

            // Create medical record for the appointment
            var medicalRecord = new MedicalRecord
            {
                RecordID = 1,
                PatientID = 1,
                AppointmentID = 1,
                CreatedBy = 3, // Audiologist's UserID
                ChiefComplaint = "Patient reports difficulty hearing in crowded environments",
                Diagnosis = "Moderate sensorineural hearing loss in both ears",
                TreatmentPlan = "Recommended hearing aids: Phonak Audéo Paradise P90",
                RecordDate = DateTime.Now.Date.AddDays(-7)
            };

            context.MedicalRecords.AddOrUpdate(mr => mr.RecordID, medicalRecord);

            // Create a hearing test for the medical record
            var hearingTest = new HearingTest
            {
                TestID = 1,
                RecordID = 1,
                TestType = "Pure Tone Audiometry",
                TestDate = DateTime.Now.Date.AddDays(-7),
                TestNotes = "Patient responded well to testing. Shows moderate hearing loss in high frequencies."
            };

            context.HearingTests.AddOrUpdate(ht => ht.TestID, hearingTest);

            // Add audiogram data for the hearing test
            var audiogramData = new[]
            {
                // Right ear data
                new AudiogramData { AudiogramDataID = 1, TestID = 1, Ear = "Right", Frequency = 250, Threshold = 20 },
                new AudiogramData { AudiogramDataID = 2, TestID = 1, Ear = "Right", Frequency = 500, Threshold = 25 },
                new AudiogramData { AudiogramDataID = 3, TestID = 1, Ear = "Right", Frequency = 1000, Threshold = 30 },
                new AudiogramData { AudiogramDataID = 4, TestID = 1, Ear = "Right", Frequency = 2000, Threshold = 40 },
                new AudiogramData { AudiogramDataID = 5, TestID = 1, Ear = "Right", Frequency = 4000, Threshold = 50 },
                new AudiogramData { AudiogramDataID = 6, TestID = 1, Ear = "Right", Frequency = 8000, Threshold = 55 },
                
                // Left ear data
                new AudiogramData { AudiogramDataID = 7, TestID = 1, Ear = "Left", Frequency = 250, Threshold = 25 },
                new AudiogramData { AudiogramDataID = 8, TestID = 1, Ear = "Left", Frequency = 500, Threshold = 30 },
                new AudiogramData { AudiogramDataID = 9, TestID = 1, Ear = "Left", Frequency = 1000, Threshold = 35 },
                new AudiogramData { AudiogramDataID = 10, TestID = 1, Ear = "Left", Frequency = 2000, Threshold = 45 },
                new AudiogramData { AudiogramDataID = 11, TestID = 1, Ear = "Left", Frequency = 4000, Threshold = 55 },
                new AudiogramData { AudiogramDataID = 12, TestID = 1, Ear = "Left", Frequency = 8000, Threshold = 60 }
            };

            foreach (var data in audiogramData)
            {
                context.AudiogramData.AddOrUpdate(ad => ad.AudiogramDataID, data);
            }

            // Create a prescription
            var prescription = new Prescription
            {
                PrescriptionID = 1,
                AppointmentID = 1,
                ProductID = 1,
                PrescribedBy = 1, // AudiologistID
                PrescribedDate = DateTime.Now.Date.AddDays(-7)
            };

            context.Prescriptions.AddOrUpdate(p => p.PrescriptionID, prescription);

            // Create an order for Patient 1
            var order = new Order
            {
                OrderID = 1,
                PatientID = 1,
                OrderDate = DateTime.Now.Date.AddDays(-5),
                TotalAmount = 2499.99m,
                DeliveryAddress = "123 Maple St, Springfield, IL 62701",
                Status = "Confirmed"
            };

            context.Orders.AddOrUpdate(o => o.OrderID, order);
            context.SaveChanges();

            // Add order item
            var orderItem = new OrderItem
            {
                OrderItemID = 1,
                OrderID = 1,
                ProductID = 1,
                Quantity = 1,
                UnitPrice = 2499.99m
            };

            context.OrderItems.AddOrUpdate(oi => oi.OrderItemID, orderItem);

            // Create an invoice for the order
            var invoice = new Invoice
            {
                InvoiceID = 1,
                OrderID = 1,
                AppointmentID = null,
                InvoiceDate = DateTime.Now.Date.AddDays(-5),
                TotalAmount = 2499.99m,
                Status = "Paid",
                PaymentMethod = "Credit Card"
            };

            context.Invoices.AddOrUpdate(i => i.InvoiceID, invoice);
            context.SaveChanges();

            // Create a payment for the invoice
            var payment = new Payment
            {
                PaymentID = 1,
                InvoiceID = 1,
                Amount = 2499.99m,
                PaymentDate = DateTime.Now.Date.AddDays(-5),
                ReceivedBy = 4, // ReceptionistID
                PaymentMethod = "Credit Card"
            };

            context.Payments.AddOrUpdate(p => p.PaymentID, payment);

            // Final save
            context.SaveChanges();
        }
    }
}