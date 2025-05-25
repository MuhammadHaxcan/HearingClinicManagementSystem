using HearingClinicManagementSystem.Data;
using HearingClinicManagementSystem.Models;
using System;
using System.Data.Entity.Migrations;
using System.Linq;

namespace HearingClinicManagementSystem.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<HearingClinicDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "HearingClinicManagementSystem.Data.HearingClinicDbContext";
        }

        protected override void Seed(HearingClinicDbContext context) {
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
        // Audiologist 1
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
        // Audiologist 2
        new User
        {
            UserID = 9,
            FirstName = "David",
            LastName = "Cochlear",
            Email = "david.cochlear@hearingclinic.com",
            Phone = "555-333-5555",
            Role = "Audiologist",
            Username = "david.cochlear",
            PasswordHash = "password123",
            IsActive = true
        },
        // Audiologist 3
        new User
        {
            UserID = 10,
            FirstName = "Jessica",
            LastName = "Acoustic",
            Email = "jessica.acoustic@hearingclinic.com",
            Phone = "555-333-6666",
            Role = "Audiologist",
            Username = "jessica.acoustic",
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
        },
        // Patient 4
        new User
        {
            UserID = 11,
            FirstName = "Susan",
            LastName = "Williams",
            Email = "susan.williams@example.com",
            Phone = "555-111-2222",
            Role = "Patient",
            Username = "susan.williams",
            PasswordHash = "password123",
            IsActive = true
        },
        // Patient 5
        new User
        {
            UserID = 12,
            FirstName = "James",
            LastName = "Brown",
            Email = "james.brown@example.com",
            Phone = "555-222-3333",
            Role = "Patient",
            Username = "james.brown",
            PasswordHash = "password123",
            IsActive = true
        }
    };

            foreach (var user in users) {
                context.Users.AddOrUpdate(u => u.UserID, user);
            }

            // Force save to get IDs
            context.SaveChanges();

            // Create clinic manager
            context.ClinicManagers.AddOrUpdate(
                cm => cm.ClinicManagerID,
                new ClinicManager { ClinicManagerID = 1, UserID = 2 }
            );

            // Create audiologists with different specializations
            context.Audiologists.AddOrUpdate(
                a => a.AudiologistID,
                new Audiologist {
                    AudiologistID = 1,
                    UserID = 3,
                    Specialization = "Hearing Aids & Cochlear Implants"
                },
                new Audiologist {
                    AudiologistID = 2,
                    UserID = 9,
                    Specialization = "Pediatric Audiology"
                },
                new Audiologist {
                    AudiologistID = 3,
                    UserID = 10,
                    Specialization = "Tinnitus Management & Balance Disorders"
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
                new Patient {
                    PatientID = 1,
                    UserID = 6,
                    DateOfBirth = new DateTime(1965, 5, 15),
                    Address = "123 Maple St, Springfield, IL 62701"
                },
                new Patient {
                    PatientID = 2,
                    UserID = 7,
                    DateOfBirth = new DateTime(1982, 8, 22),
                    Address = "456 Oak Ave, Springfield, IL 62702"
                },
                new Patient {
                    PatientID = 3,
                    UserID = 8,
                    DateOfBirth = new DateTime(1990, 3, 10),
                    Address = "789 Pine Rd, Springfield, IL 62703"
                },
                new Patient {
                    PatientID = 4,
                    UserID = 11,
                    DateOfBirth = new DateTime(1975, 11, 8),
                    Address = "321 Elm St, Springfield, IL 62704"
                },
                new Patient {
                    PatientID = 5,
                    UserID = 12,
                    DateOfBirth = new DateTime(1958, 2, 27),
                    Address = "654 Birch Dr, Springfield, IL 62705"
                }
            );

            // Force save to get IDs
            context.SaveChanges();

            // Create schedules for all audiologists
            // Audiologist 1 (Sarah Hearing) - Monday and Wednesday
            var schedule1Monday = new Schedule {
                ScheduleID = 1,
                AudiologistID = 1,
                DayOfWeek = "Monday"
            };

            var schedule1Wednesday = new Schedule {
                ScheduleID = 2,
                AudiologistID = 1,
                DayOfWeek = "Wednesday"
            };

            // Audiologist 2 (David Cochlear) - Tuesday and Thursday
            var schedule2Tuesday = new Schedule {
                ScheduleID = 3,
                AudiologistID = 2,
                DayOfWeek = "Tuesday"
            };

            var schedule2Thursday = new Schedule {
                ScheduleID = 4,
                AudiologistID = 2,
                DayOfWeek = "Thursday"
            };

            // Audiologist 3 (Jessica Acoustic) - Friday
            var schedule3Friday = new Schedule {
                ScheduleID = 5,
                AudiologistID = 3,
                DayOfWeek = "Friday"
            };

            context.Schedules.AddOrUpdate(s => s.ScheduleID, schedule1Monday);
            context.Schedules.AddOrUpdate(s => s.ScheduleID, schedule1Wednesday);
            context.Schedules.AddOrUpdate(s => s.ScheduleID, schedule2Tuesday);
            context.Schedules.AddOrUpdate(s => s.ScheduleID, schedule2Thursday);
            context.Schedules.AddOrUpdate(s => s.ScheduleID, schedule3Friday);
            context.SaveChanges();

            // Create time slots for each schedule
            // Helper function to create time slots
            Action<int, TimeSpan, TimeSpan> createTimeSlots = (scheduleId, startTime, endTime) =>
            {
                int slotId = context.TimeSlots.Any() ? context.TimeSlots.Max(ts => ts.TimeSlotID) + 1 : 1;
                var currentTime = startTime;

                while (currentTime < endTime) {
                    var endSlotTime = currentTime.Add(new TimeSpan(0, 30, 0)); // 30-minute slots
                    context.TimeSlots.AddOrUpdate(
                        ts => new { ts.ScheduleID, ts.StartTime, ts.EndTime },
                        new TimeSlot {
                            TimeSlotID = slotId++,
                            ScheduleID = scheduleId,
                            StartTime = currentTime,
                            EndTime = endSlotTime,
                            IsAvailable = true
                        }
                    );
                    currentTime = endSlotTime;
                }
            };

            // Morning hours: 9:00 AM - 12:00 PM
            // Afternoon hours: 1:00 PM - 4:00 PM
            var morningStart = new TimeSpan(9, 0, 0);
            var morningEnd = new TimeSpan(12, 0, 0);
            var afternoonStart = new TimeSpan(13, 0, 0);
            var afternoonEnd = new TimeSpan(16, 0, 0);

            // Create time slots for each schedule
            createTimeSlots(1, morningStart, morningEnd);
            createTimeSlots(1, afternoonStart, afternoonEnd);
            createTimeSlots(2, morningStart, morningEnd);
            createTimeSlots(2, afternoonStart, afternoonEnd);
            createTimeSlots(3, morningStart, morningEnd);
            createTimeSlots(3, afternoonStart, afternoonEnd);
            createTimeSlots(4, morningStart, morningEnd);
            createTimeSlots(4, afternoonStart, afternoonEnd);
            createTimeSlots(5, morningStart, morningEnd);
            createTimeSlots(5, afternoonStart, afternoonEnd);

            // Add products to inventory
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

            foreach (var product in products) {
                context.Products.AddOrUpdate(p => p.ProductID, product);
            }

            // Final save
            context.SaveChanges();
        }
    }
}