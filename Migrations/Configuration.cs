namespace HearingClinicManagementSystem.Migrations {
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using MySql.Data.EntityFramework;
    using HearingClinicManagementSystem.Data;
    using HearingClinicManagementSystem.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<HearingClinicManagementSystem.Data.HearingClinicDbContext> {
        public Configuration() {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;

            // Set MySQL as the provider
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.EntityFramework.MySqlMigrationSqlGenerator());
        }

        protected override void Seed(HearingClinicManagementSystem.Data.HearingClinicDbContext context) {
            // Create seed data for different user roles
            // ==========================================

            // 1. Admin user
            var adminUser = new User {
                UserID = 1,
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@hearingclinic.com",
                Phone = "555-123-4567",
                Role = "Admin",
                Username = "admin",
                PasswordHash = "admin123", // In a real app, this would be hashed
                IsActive = true
            };

            // 2. Clinic Manager user
            var clinicManagerUser = new User {
                UserID = 2,
                FirstName = "John",
                LastName = "Director",
                Email = "john.director@hearingclinic.com",
                Phone = "555-222-3333",
                Role = "ClinicManager",
                Username = "john.director",
                PasswordHash = "password123",
                IsActive = true
            };

            // 3. Audiologist 1 user
            var audiologist1User = new User {
                UserID = 3,
                FirstName = "Sarah",
                LastName = "Hearing",
                Email = "sarah.hearing@hearingclinic.com",
                Phone = "555-333-4444",
                Role = "Audiologist",
                Username = "sarah.hearing",
                PasswordHash = "password123",
                IsActive = true
            };

            // 4. Audiologist 2 user
            var audiologist2User = new User {
                UserID = 7,
                FirstName = "David",
                LastName = "Cochlear",
                Email = "david.cochlear@hearingclinic.com",
                Phone = "555-333-5555",
                Role = "Audiologist",
                Username = "david.cochlear",
                PasswordHash = "password123",
                IsActive = true
            };

            // 5. Audiologist 3 user
            var audiologist3User = new User {
                UserID = 8,
                FirstName = "Jessica",
                LastName = "Acoustic",
                Email = "jessica.acoustic@hearingclinic.com",
                Phone = "555-333-6666",
                Role = "Audiologist",
                Username = "jessica.acoustic",
                PasswordHash = "password123",
                IsActive = true
            };

            // 6. Receptionist user
            var receptionistUser = new User {
                UserID = 4,
                FirstName = "Emily",
                LastName = "Reception",
                Email = "emily.reception@hearingclinic.com",
                Phone = "555-444-5555",
                Role = "Receptionist",
                Username = "emily.reception",
                PasswordHash = "password123",
                IsActive = true
            };

            // 7. Inventory Manager user
            var inventoryManagerUser = new User {
                UserID = 5,
                FirstName = "Mark",
                LastName = "Inventory",
                Email = "mark.inventory@hearingclinic.com",
                Phone = "555-555-6666",
                Role = "InventoryManager",
                Username = "mark.inventory",
                PasswordHash = "password123",
                IsActive = true
            };

            // 8. Patient user
            var patientUser = new User {
                UserID = 6,
                FirstName = "Robert",
                LastName = "Patient",
                Email = "robert@example.com",
                Phone = "555-777-8888",
                Role = "Patient",
                Username = "robert.patient",
                PasswordHash = "password123",
                IsActive = true
            };

            // Add users to database
            context.Users.AddOrUpdate(u => u.UserID,
                adminUser,
                clinicManagerUser,
                audiologist1User,
                audiologist2User,
                audiologist3User,
                receptionistUser,
                inventoryManagerUser,
                patientUser
            );

            // Force save to get IDs
            context.SaveChanges();

            // Create specific role entities
            // ============================

            // 1. Clinic Manager
            context.ClinicManagers.AddOrUpdate(
                cm => cm.ClinicManagerID,
                new ClinicManager {
                    ClinicManagerID = 1,
                    UserID = clinicManagerUser.UserID
                }
            );

            // 2. Audiologists with different specializations
            context.Audiologists.AddOrUpdate(
                a => a.AudiologistID,
                new Audiologist {
                    AudiologistID = 1,
                    UserID = audiologist1User.UserID,
                    Specialization = "Hearing Aids & Cochlear Implants"
                },
                new Audiologist {
                    AudiologistID = 2,
                    UserID = audiologist2User.UserID,
                    Specialization = "Pediatric Audiology"
                },
                new Audiologist {
                    AudiologistID = 3,
                    UserID = audiologist3User.UserID,
                    Specialization = "Tinnitus Management & Balance Disorders"
                }
            );

            // 3. Receptionist
            context.Receptionists.AddOrUpdate(
                r => r.ReceptionistID,
                new Receptionist {
                    ReceptionistID = 1,
                    UserID = receptionistUser.UserID
                }
            );

            // 4. Inventory Manager
            context.InventoryManagers.AddOrUpdate(
                im => im.InventoryManagerID,
                new InventoryManager {
                    InventoryManagerID = 1,
                    UserID = inventoryManagerUser.UserID
                }
            );

            // 5. Patient
            context.Patients.AddOrUpdate(
                p => p.PatientID,
                new Patient {
                    PatientID = 1,
                    UserID = patientUser.UserID,
                    DateOfBirth = new DateTime(1965, 5, 15),
                    Address = "123 Maple St, Springfield, IL 62701"
                }
            );

            // Save changes to database
            context.SaveChanges();

            // Create schedules for all audiologists
            // ====================================

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

            // Audiologist 3 (Jessica Acoustic) - Friday and Saturday
            var schedule3Friday = new Schedule {
                ScheduleID = 5,
                AudiologistID = 3,
                DayOfWeek = "Friday"
            };

            var schedule3Saturday = new Schedule {
                ScheduleID = 6,
                AudiologistID = 3,
                DayOfWeek = "Saturday"
            };

            context.Schedules.AddOrUpdate(s => s.ScheduleID, schedule1Monday);
            context.Schedules.AddOrUpdate(s => s.ScheduleID, schedule1Wednesday);
            context.Schedules.AddOrUpdate(s => s.ScheduleID, schedule2Tuesday);
            context.Schedules.AddOrUpdate(s => s.ScheduleID, schedule2Thursday);
            context.Schedules.AddOrUpdate(s => s.ScheduleID, schedule3Friday);
            context.Schedules.AddOrUpdate(s => s.ScheduleID, schedule3Saturday);
            context.SaveChanges();

            // Create time slots for each schedule
            // ==================================

            // Helper function to create time slots
            Action<int, TimeSpan, TimeSpan> createTimeSlots = (scheduleId, startTime, endTime) => {
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

            // Define clinic hours
            // Morning hours: 9:00 AM - 12:00 PM
            // Afternoon hours: 1:00 PM - 4:00 PM
            var morningStart = new TimeSpan(9, 0, 0);
            var morningEnd = new TimeSpan(12, 0, 0);
            var afternoonStart = new TimeSpan(13, 0, 0);
            var afternoonEnd = new TimeSpan(16, 0, 0);

            // Create time slots for each schedule
            // Audiologist 1 - Monday and Wednesday
            createTimeSlots(1, morningStart, morningEnd);
            createTimeSlots(1, afternoonStart, afternoonEnd);
            createTimeSlots(2, morningStart, morningEnd);
            createTimeSlots(2, afternoonStart, afternoonEnd);

            // Audiologist 2 - Tuesday and Thursday
            createTimeSlots(3, morningStart, morningEnd);
            createTimeSlots(3, afternoonStart, afternoonEnd);
            createTimeSlots(4, morningStart, morningEnd);
            createTimeSlots(4, afternoonStart, afternoonEnd);

            // Audiologist 3 - Friday and Saturday (Saturday only morning)
            createTimeSlots(5, morningStart, morningEnd);
            createTimeSlots(5, afternoonStart, afternoonEnd);
            createTimeSlots(6, morningStart, morningEnd); // Saturday morning only

            // Create Products
            // ==============
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
                }
            };

            // Add products to database
            foreach (var product in products) {
                context.Products.AddOrUpdate(p => p.ProductID, product);
            }

            // Save final changes
            context.SaveChanges();
        }
    }
}