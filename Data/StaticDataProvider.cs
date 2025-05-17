using System;
using System.Collections.Generic;
using System.Linq;
using HearingClinicManagementSystem.Models;

namespace HearingClinicManagementSystem.Data
{
    /// <summary>
    /// This class provides static sample data for the Hearing Clinic Management System
    /// to be used during development and testing.
    /// </summary>
    public static class StaticDataProvider
    {
        // Collections to store sample data
        public static List<User> Users { get; private set; }
        public static List<Patient> Patients { get; private set; }
        public static List<Audiologist> Audiologists { get; private set; }
        public static List<Receptionist> Receptionists { get; private set; }
        public static List<InventoryManager> InventoryManagers { get; private set; }
        public static List<ClinicManager> ClinicManagers { get; private set; }
        public static List<Schedule> Schedules { get; private set; }
        public static List<TimeSlot> TimeSlots { get; private set; }
        public static List<Appointment> Appointments { get; private set; }
        public static List<MedicalRecord> MedicalRecords { get; private set; }
        public static List<HearingTest> HearingTests { get; private set; }
        public static List<AudiogramData> AudiogramData { get; private set; }
        public static List<Product> Products { get; private set; }
        public static List<Order> Orders { get; private set; }
        public static List<OrderItem> OrderItems { get; private set; }
        public static List<InventoryTransaction> InventoryTransactions { get; private set; }
        public static List<Prescription> Prescriptions { get; private set; }
        public static List<Invoice> Invoices { get; private set; }
        public static List<Payment> Payments { get; private set; }

        /// <summary>
        /// Initialize all static data
        /// </summary>
        public static void Initialize()
        {
            InitializeUsers();
            InitializePatients();
            InitializeAudiologists();
            InitializeReceptionists();
            InitializeInventoryManagers();
            InitializeClinicManagers();
            InitializeSchedules();
            InitializeTimeSlots();
            InitializeProducts();
            InitializeAppointments();
            InitializeMedicalRecords();
            InitializeHearingTests();
            InitializeAudiogramData();
            InitializeOrders();
            InitializeOrderItems();
            InitializeInventoryTransactions();
            InitializePrescriptions();
            InitializeInvoices();
            InitializePayments();
        }

        private static void InitializeUsers()
        {
            Users = new List<User>
            {
                // Patients
                new User { UserID = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Phone = "555-123-4567", Role = "Patient", Username = "johndoe", PasswordHash = "password", IsActive = true },
                new User { UserID = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Phone = "555-234-5678", Role = "Patient", Username = "janesmith", PasswordHash = "password", IsActive = true },
                new User { UserID = 3, FirstName = "Michael", LastName = "Johnson", Email = "michael.johnson@example.com", Phone = "555-345-6789", Role = "Patient", Username = "michaelj", PasswordHash = "password", IsActive = true },
                new User { UserID = 4, FirstName = "Emily", LastName = "Brown", Email = "emily.brown@example.com", Phone = "555-456-7890", Role = "Patient", Username = "emilyb", PasswordHash = "password", IsActive = true },
                new User { UserID = 5, FirstName = "David", LastName = "Wilson", Email = "david.wilson@example.com", Phone = "555-567-8901", Role = "Patient", Username = "davidw", PasswordHash = "password", IsActive = true },
                
                // Audiologists
                new User { UserID = 6, FirstName = "Sarah", LastName = "Davis", Email = "sarah.davis@clinic.com", Phone = "555-678-9012", Role = "Audiologist", Username = "sarahd", PasswordHash = "password", IsActive = true },
                new User { UserID = 7, FirstName = "Robert", LastName = "Miller", Email = "robert.miller@clinic.com", Phone = "555-789-0123", Role = "Audiologist", Username = "robertm", PasswordHash = "password", IsActive = true },
                
                // Receptionists
                new User { UserID = 8, FirstName = "Jennifer", LastName = "Taylor", Email = "jennifer.taylor@clinic.com", Phone = "555-890-1234", Role = "Receptionist", Username = "jennifert", PasswordHash = "password", IsActive = true },
                
                // Inventory Managers
                new User { UserID = 9, FirstName = "William", LastName = "Anderson", Email = "william.anderson@clinic.com", Phone = "555-901-2345", Role = "InventoryManager", Username = "williama", PasswordHash = "password", IsActive = true },
                
                // Clinic Managers
                new User { UserID = 10, FirstName = "Patricia", LastName = "Thomas", Email = "patricia.thomas@clinic.com", Phone = "555-012-3456", Role = "ClinicManager", Username = "patriciat", PasswordHash = "password", IsActive = true }
            };
        }

        private static void InitializePatients()
        {
            Patients = new List<Patient>
    {
        new Patient {
            PatientID = 1,
            UserID = 1,
            User = Users.First(u => u.UserID == 1),  // Add this line
            DateOfBirth = new DateTime(1980, 5, 15),
            Address = "123 Main St, Anytown, USA"
        },
        new Patient {
            PatientID = 2,
            UserID = 2,
            User = Users.First(u => u.UserID == 2),  // Add this line
            DateOfBirth = new DateTime(1975, 8, 22),
            Address = "456 Oak Ave, Anytown, USA"
        },
        new Patient {
            PatientID = 3,
            UserID = 3,
            User = Users.First(u => u.UserID == 3),  // Add this line
            DateOfBirth = new DateTime(1992, 3, 10),
            Address = "789 Pine Rd, Anytown, USA"
        },
        new Patient {
            PatientID = 4,
            UserID = 4,
            User = Users.First(u => u.UserID == 4),  // Add this line
            DateOfBirth = new DateTime(1988, 11, 30),
            Address = "321 Elm St, Anytown, USA"
        },
        new Patient {
            PatientID = 5,
            UserID = 5,
            User = Users.First(u => u.UserID == 5),  // Add this line
            DateOfBirth = new DateTime(1965, 7, 8),
            Address = "654 Maple Dr, Anytown, USA"
        }
    };
        }

        private static void InitializeAudiologists()
        {
            Audiologists = new List<Audiologist>
    {
        new Audiologist {
            AudiologistID = 1,
            UserID = 6,
            User = Users.First(u => u.UserID == 6),  // Add this line
            Specialization = "Pediatric Audiology"
        },
        new Audiologist {
            AudiologistID = 2,
            UserID = 7,
            User = Users.First(u => u.UserID == 7),  // Add this line
            Specialization = "Geriatric Audiology"
        }
    };
        }

        private static void InitializeReceptionists()
        {
            Receptionists = new List<Receptionist>
            {
                new Receptionist { ReceptionistID = 1, UserID = 8 }
            };
        }

        private static void InitializeInventoryManagers()
        {
            InventoryManagers = new List<InventoryManager>
            {
                new InventoryManager { InventoryManagerID = 1, UserID = 9 }
            };
        }

        private static void InitializeClinicManagers()
        {
            ClinicManagers = new List<ClinicManager>
            {
                new ClinicManager { ClinicManagerID = 1, UserID = 10 }
            };
        }

        private static void InitializeSchedules()
        {
            Schedules = new List<Schedule>
            {
                // Schedules for the first audiologist
                new Schedule { ScheduleID = 1, AudiologistID = 1, DayOfWeek = "Monday" },
                new Schedule { ScheduleID = 2, AudiologistID = 1, DayOfWeek = "Wednesday" },
                new Schedule { ScheduleID = 3, AudiologistID = 1, DayOfWeek = "Friday" },
                
                // Schedules for the second audiologist
                new Schedule { ScheduleID = 4, AudiologistID = 2, DayOfWeek = "Tuesday" },
                new Schedule { ScheduleID = 5, AudiologistID = 2, DayOfWeek = "Thursday" }
            };
        }

        private static void InitializeTimeSlots()
        {
            TimeSlots = new List<TimeSlot>();
            int timeSlotId = 1;

            // For each schedule, create time slots from 9:00 AM to 5:00 PM in 1-hour increments
            foreach (var schedule in Schedules)
            {
                for (int hour = 9; hour < 17; hour++)
                {
                    TimeSlots.Add(new TimeSlot
                    {
                        TimeSlotID = timeSlotId++,
                        ScheduleID = schedule.ScheduleID,
                        StartTime = new TimeSpan(hour, 0, 0),
                        EndTime = new TimeSpan(hour + 1, 0, 0),
                        IsAvailable = true
                    });
                }
            }
        }

        private static void InitializeProducts()
        {
            Products = new List<Product>
            {
                new Product { ProductID = 1, Manufacturer = "Hearing Tech", Model = "ClearSound Pro", Features = "Behind-the-ear, Rechargeable, Bluetooth connectivity", Price = 1299.99m, QuantityInStock = 15 },
                new Product { ProductID = 2, Manufacturer = "Hearing Tech", Model = "ClearSound Lite", Features = "Behind-the-ear, Battery-powered, Basic functionality", Price = 899.99m, QuantityInStock = 20 },
                new Product { ProductID = 3, Manufacturer = "AudioClear", Model = "InvisiFit", Features = "In-the-canal, Rechargeable, Noise cancellation", Price = 1499.99m, QuantityInStock = 10 },
                new Product { ProductID = 4, Manufacturer = "AudioClear", Model = "ComfortFit", Features = "In-the-ear, Battery-powered, Water resistant", Price = 999.99m, QuantityInStock = 18 },
                new Product { ProductID = 5, Manufacturer = "SoundLife", Model = "Elite Plus", Features = "Receiver-in-canal, Rechargeable, AI-powered sound adjustment", Price = 1799.99m, QuantityInStock = 8 },
                new Product { ProductID = 6, Manufacturer = "SoundLife", Model = "Standard", Features = "Receiver-in-canal, Battery-powered, Multiple programs", Price = 1199.99m, QuantityInStock = 12 },
                new Product { ProductID = 7, Manufacturer = "Hearing Tech", Model = "ClearSound Cleaning Kit", Features = "Cleaning tools, Drying capsules, Replacement filters", Price = 49.99m, QuantityInStock = 50 },
                new Product { ProductID = 8, Manufacturer = "AudioClear", Model = "Battery Pack", Features = "Pack of 60 batteries, Size 312", Price = 29.99m, QuantityInStock = 100 },
                new Product { ProductID = 9, Manufacturer = "SoundLife", Model = "Bluetooth Streamer", Features = "Connects hearing aids to phones and TV", Price = 199.99m, QuantityInStock = 25 },
                new Product { ProductID = 10, Manufacturer = "Hearing Tech", Model = "Protective Case", Features = "Waterproof, Impact-resistant", Price = 24.99m, QuantityInStock = 40 }
            };
        }

        private static void InitializeAppointments()
        {
            Appointments = new List<Appointment>
            {
                new Appointment {
                    AppointmentID = 1,
                    PatientID = 1,
                    AudiologistID = 1,
                    CreatedBy = 8, // Receptionist created
                    Date = DateTime.Now.AddDays(-14),
                    TimeSlotID = 1,
                    PurposeOfVisit = "Initial consultation",
                    Status = "Completed",
                    Fee = 200.00m,
                    FollowUpRequired = true,
                    FollowUpDate = DateTime.Now.AddDays(30)
                },
                new Appointment {
                    AppointmentID = 2,
                    PatientID = 2,
                    AudiologistID = 2,
                    CreatedBy = 2, // Patient self-booked
                    Date = DateTime.Now.AddDays(-7),
                    TimeSlotID = 9,
                    PurposeOfVisit = "Hearing test",
                    Status = "Completed",
                    Fee = 200.00m,
                    FollowUpRequired = false
                },
                new Appointment {
                    AppointmentID = 3,
                    PatientID = 3,
                    AudiologistID = 1,
                    CreatedBy = 8,
                    Date = DateTime.Now.AddDays(-3),
                    TimeSlotID = 3,
                    PurposeOfVisit = "Hearing aid fitting",
                    Status = "Completed",
                    Fee = 175.00m,
                    FollowUpRequired = true,
                    FollowUpDate = DateTime.Now.AddDays(14)
                },
                new Appointment {
                    AppointmentID = 4,
                    PatientID = 4,
                    AudiologistID = 2,
                    CreatedBy = 4,
                    Date = DateTime.Now.AddDays(1),
                    TimeSlotID = 10,
                    PurposeOfVisit = "Initial consultation",
                    Status = "Confirmed",
                    Fee = 150.00m
                },
                new Appointment {
                    AppointmentID = 5,
                    PatientID = 5,
                    AudiologistID = 1,
                    CreatedBy = 8,
                    Date = DateTime.Now.AddDays(3),
                    TimeSlotID = 5,
                    PurposeOfVisit = "Follow-up appointment",
                    Status = "Confirmed",
                    Fee = 125.00m
                },
                new Appointment {
                    AppointmentID = 6,
                    PatientID = 1,
                    AudiologistID = 1,
                    CreatedBy = 1,
                    Date = DateTime.Now.AddDays(7),
                    TimeSlotID = 2,
                    PurposeOfVisit = "Hearing aid adjustment",
                    Status = "Pending",
                    Fee = 100.00m
                },
                new Appointment {
                    AppointmentID = 7,
                    PatientID = 2,
                    AudiologistID = 2,
                    CreatedBy = 2,
                    Date = DateTime.Now.AddDays(10),
                    TimeSlotID = 11,
                    PurposeOfVisit = "Annual check-up",
                    Status = "Pending",
                    Fee = 125.00m
                }
            };
        }

        private static void InitializeMedicalRecords()
        {
            MedicalRecords = new List<MedicalRecord>
            {
                new MedicalRecord {
                    RecordID = 1,
                    PatientID = 1,
                    AppointmentID = 1,
                    CreatedBy = 1,
                    ChiefComplaint = "Difficulty hearing in noisy environments",
                    Diagnosis = "Mild to moderate sensorineural hearing loss",
                    TreatmentPlan = "Recommended hearing aids in both ears. Scheduled fitting appointment.",
                    RecordDate = DateTime.Now.AddDays(-14)
                },
                new MedicalRecord {
                    RecordID = 2,
                    PatientID = 2,
                    AppointmentID = 2,
                    CreatedBy = 2,
                    ChiefComplaint = "Ringing in ears",
                    Diagnosis = "Tinnitus with mild hearing loss",
                    TreatmentPlan = "Counseling on tinnitus management strategies. Recommended sound therapy.",
                    RecordDate = DateTime.Now.AddDays(-7)
                },
                new MedicalRecord {
                    RecordID = 3,
                    PatientID = 3,
                    AppointmentID = 3,
                    CreatedBy = 1,
                    ChiefComplaint = "Hearing aid not working properly",
                    Diagnosis = "Hearing aid malfunction",
                    TreatmentPlan = "Repaired hearing aid. Instructed on proper maintenance and care.",
                    RecordDate = DateTime.Now.AddDays(-3)
                }
            };
        }

        private static void InitializeHearingTests()
        {
            HearingTests = new List<HearingTest>
            {
                new HearingTest {
                    TestID = 1,
                    RecordID = 1,
                    TestType = "PureTone",
                    TestDate = DateTime.Now.AddDays(-14),
                    TestNotes = "Patient reported difficulty hearing high-frequency sounds."
                },
                new HearingTest {
                    TestID = 2,
                    RecordID = 1,
                    TestType = "Speech",
                    TestDate = DateTime.Now.AddDays(-14),
                    TestNotes = "Word recognition score of 85% in right ear, 80% in left ear."
                },
                new HearingTest {
                    TestID = 3,
                    RecordID = 2,
                    TestType = "PureTone",
                    TestDate = DateTime.Now.AddDays(-7),
                    TestNotes = "Mild hearing loss at high frequencies."
                },
                new HearingTest {
                    TestID = 4,
                    RecordID = 2,
                    TestType = "Tympanometry",
                    TestDate = DateTime.Now.AddDays(-7),
                    TestNotes = "Normal middle ear function bilaterally."
                }
            };
        }

        private static void InitializeAudiogramData()
        {
            AudiogramData = new List<AudiogramData>
            {
                // Audiogram data for first patient's PureTone test - Right ear
                new AudiogramData { AudiogramDataID = 1, TestID = 1, Ear = "Right", Frequency = 250, Threshold = 15 },
                new AudiogramData { AudiogramDataID = 2, TestID = 1, Ear = "Right", Frequency = 500, Threshold = 20 },
                new AudiogramData { AudiogramDataID = 3, TestID = 1, Ear = "Right", Frequency = 1000, Threshold = 25 },
                new AudiogramData { AudiogramDataID = 4, TestID = 1, Ear = "Right", Frequency = 2000, Threshold = 30 },
                new AudiogramData { AudiogramDataID = 5, TestID = 1, Ear = "Right", Frequency = 4000, Threshold = 40 },
                new AudiogramData { AudiogramDataID = 6, TestID = 1, Ear = "Right", Frequency = 8000, Threshold = 45 },
                
                // Audiogram data for first patient's PureTone test - Left ear
                new AudiogramData { AudiogramDataID = 7, TestID = 1, Ear = "Left", Frequency = 250, Threshold = 20 },
                new AudiogramData { AudiogramDataID = 8, TestID = 1, Ear = "Left", Frequency = 500, Threshold = 25 },
                new AudiogramData { AudiogramDataID = 9, TestID = 1, Ear = "Left", Frequency = 1000, Threshold = 30 },
                new AudiogramData { AudiogramDataID = 10, TestID = 1, Ear = "Left", Frequency = 2000, Threshold = 35 },
                new AudiogramData { AudiogramDataID = 11, TestID = 1, Ear = "Left", Frequency = 4000, Threshold = 45 },
                new AudiogramData { AudiogramDataID = 12, TestID = 1, Ear = "Left", Frequency = 8000, Threshold = 50 },
                
                // Audiogram data for second patient's PureTone test - Right ear
                new AudiogramData { AudiogramDataID = 13, TestID = 3, Ear = "Right", Frequency = 250, Threshold = 10 },
                new AudiogramData { AudiogramDataID = 14, TestID = 3, Ear = "Right", Frequency = 500, Threshold = 15 },
                new AudiogramData { AudiogramDataID = 15, TestID = 3, Ear = "Right", Frequency = 1000, Threshold = 15 },
                new AudiogramData { AudiogramDataID = 16, TestID = 3, Ear = "Right", Frequency = 2000, Threshold = 20 },
                new AudiogramData { AudiogramDataID = 17, TestID = 3, Ear = "Right", Frequency = 4000, Threshold = 30 },
                new AudiogramData { AudiogramDataID = 18, TestID = 3, Ear = "Right", Frequency = 8000, Threshold = 35 },
                
                // Audiogram data for second patient's PureTone test - Left ear
                new AudiogramData { AudiogramDataID = 19, TestID = 3, Ear = "Left", Frequency = 250, Threshold = 15 },
                new AudiogramData { AudiogramDataID = 20, TestID = 3, Ear = "Left", Frequency = 500, Threshold = 15 },
                new AudiogramData { AudiogramDataID = 21, TestID = 3, Ear = "Left", Frequency = 1000, Threshold = 20 },
                new AudiogramData { AudiogramDataID = 22, TestID = 3, Ear = "Left", Frequency = 2000, Threshold = 25 },
                new AudiogramData { AudiogramDataID = 23, TestID = 3, Ear = "Left", Frequency = 4000, Threshold = 35 },
                new AudiogramData { AudiogramDataID = 24, TestID = 3, Ear = "Left", Frequency = 8000, Threshold = 40 }
            };
        }

        private static void InitializeOrders()
        {
            Orders = new List<Order>
            {
                new Order {
                    OrderID = 1,
                    PatientID = 1,
                    ProcessedBy = 1,
                    OrderDate = DateTime.Now.AddDays(-10),
                    TotalAmount = 2599.98m,
                    DeliveryAddress = "123 Main St, Anytown, USA",
                    Status = "Confirmed"
                },
                new Order {
                    OrderID = 2,
                    PatientID = 2,
                    ProcessedBy = 1,
                    OrderDate = DateTime.Now.AddDays(-5),
                    TotalAmount = 949.98m,
                    DeliveryAddress = "456 Oak Ave, Anytown, USA",
                    Status = "Confirmed"
                },
                new Order {
                    OrderID = 3,
                    PatientID = 3,
                    ProcessedBy = null,
                    OrderDate = DateTime.Now.AddDays(-1),
                    TotalAmount = 229.98m,
                    DeliveryAddress = "789 Pine Rd, Anytown, USA",
                    Status = "Pending"
                },
                new Order {
                    OrderID = 4,
                    PatientID = 4,
                    ProcessedBy = null,
                    OrderDate = DateTime.Now,
                    TotalAmount = 1799.99m,
                    DeliveryAddress = "321 Elm St, Anytown, USA",
                    Status = "Cart"
                }
            };
        }

        private static void InitializeOrderItems()
        {
            OrderItems = new List<OrderItem>
            {
                // Order 1 Items
                new OrderItem { OrderItemID = 1, OrderID = 1, ProductID = 1, Quantity = 2, UnitPrice = 1299.99m },
                
                // Order 2 Items
                new OrderItem { OrderItemID = 2, OrderID = 2, ProductID = 2, Quantity = 1, UnitPrice = 899.99m },
                new OrderItem { OrderItemID = 3, OrderID = 2, ProductID = 7, Quantity = 1, UnitPrice = 49.99m },
                
                // Order 3 Items
                new OrderItem { OrderItemID = 4, OrderID = 3, ProductID = 9, Quantity = 1, UnitPrice = 199.99m },
                new OrderItem { OrderItemID = 5, OrderID = 3, ProductID = 10, Quantity = 1, UnitPrice = 29.99m },
                
                // Order 4 Items (Cart)
                new OrderItem { OrderItemID = 6, OrderID = 4, ProductID = 5, Quantity = 1, UnitPrice = 1799.99m }
            };
        }

        private static void InitializeInventoryTransactions()
        {
            InventoryTransactions = new List<InventoryTransaction>
            {
                new InventoryTransaction {
                    TransactionID = 1,
                    ProductID = 1,
                    TransactionType = "Restock",
                    Quantity = 20,
                    TransactionDate = DateTime.Now.AddDays(-30),
                    ProcessedBy = 1
                },
                new InventoryTransaction {
                    TransactionID = 2,
                    ProductID = 2,
                    TransactionType = "Restock",
                    Quantity = 25,
                    TransactionDate = DateTime.Now.AddDays(-30),
                    ProcessedBy = 1
                },
                new InventoryTransaction {
                    TransactionID = 3,
                    ProductID = 1,
                    TransactionType = "Sale",
                    Quantity = -2,
                    TransactionDate = DateTime.Now.AddDays(-10),
                    ProcessedBy = 1
                },
                new InventoryTransaction {
                    TransactionID = 4,
                    ProductID = 2,
                    TransactionType = "Sale",
                    Quantity = -1,
                    TransactionDate = DateTime.Now.AddDays(-5),
                    ProcessedBy = 1
                },
                new InventoryTransaction {
                    TransactionID = 5,
                    ProductID = 3,
                    TransactionType = "Restock",
                    Quantity = 15,
                    TransactionDate = DateTime.Now.AddDays(-15),
                    ProcessedBy = 1
                },
                new InventoryTransaction {
                    TransactionID = 6,
                    ProductID = 3,
                    TransactionType = "Adjustment",
                    Quantity = -5,
                    TransactionDate = DateTime.Now.AddDays(-2),
                    ProcessedBy = 1
                }
            };
        }

        private static void InitializePrescriptions()
        {
            Prescriptions = new List<Prescription>
            {
                new Prescription {
                    PrescriptionID = 1,
                    AppointmentID = 1,
                    ProductID = 1,
                    PrescribedBy = 1,
                    PrescribedDate = DateTime.Now.AddDays(-14)
                },
                new Prescription {
                    PrescriptionID = 2,
                    AppointmentID = 2,
                    ProductID = 2,
                    PrescribedBy = 2,
                    PrescribedDate = DateTime.Now.AddDays(-7)
                }
            };
        }

        private static void InitializeInvoices()
        {
            Invoices = new List<Invoice>
            {
                // Appointment invoices
                new Invoice {
                    InvoiceID = 1,
                    AppointmentID = 1,
                    OrderID = null,
                    InvoiceDate = DateTime.Now.AddDays(-14),
                    TotalAmount = 150.00m,
                    Status = "Paid",
                    PaymentMethod = "Credit Card"
                },
                new Invoice {
                    InvoiceID = 2,
                    AppointmentID = 2,
                    OrderID = null,
                    InvoiceDate = DateTime.Now.AddDays(-7),
                    TotalAmount = 200.00m,
                    Status = "Paid",
                    PaymentMethod = "Credit Card"
                },
                new Invoice {
                    InvoiceID = 3,
                    AppointmentID = 3,
                    OrderID = null,
                    InvoiceDate = DateTime.Now.AddDays(-3),
                    TotalAmount = 175.00m,
                    Status = "Paid",
                    PaymentMethod = "Cash"
                },
                
                // Order invoices
                new Invoice {
                    InvoiceID = 4,
                    AppointmentID = null,
                    OrderID = 1,
                    InvoiceDate = DateTime.Now.AddDays(-10),
                    TotalAmount = 2599.98m,
                    Status = "Paid",
                    PaymentMethod = "Credit Card"
                },
                new Invoice {
                    InvoiceID = 5,
                    AppointmentID = null,
                    OrderID = 2,
                    InvoiceDate = DateTime.Now.AddDays(-5),
                    TotalAmount = 949.98m,
                    Status = "Paid",
                    PaymentMethod = "Credit Card"
                },
                new Invoice {
                    InvoiceID = 6,
                    AppointmentID = null,
                    OrderID = 3,
                    InvoiceDate = DateTime.Now.AddDays(-1),
                    TotalAmount = 229.98m,
                    Status = "Pending",
                    PaymentMethod = null
                }
            };
        }

        private static void InitializePayments()
        {
            Payments = new List<Payment>
            {
                new Payment {
                    PaymentID = 1,
                    InvoiceID = 1,
                    Amount = 150.00m,
                    PaymentDate = DateTime.Now.AddDays(-14),
                    ReceivedBy = 1,
                    PaymentMethod = "Credit Card"
                },
                new Payment {
                    PaymentID = 2,
                    InvoiceID = 2,
                    Amount = 200.00m,
                    PaymentDate = DateTime.Now.AddDays(-7),
                    ReceivedBy = 1,
                    PaymentMethod = "Credit Card"
                },
                new Payment {
                    PaymentID = 3,
                    InvoiceID = 3,
                    Amount = 175.00m,
                    PaymentDate = DateTime.Now.AddDays(-3),
                    ReceivedBy = 1,
                    PaymentMethod = "Cash"
                },
                new Payment {
                    PaymentID = 4,
                    InvoiceID = 4,
                    Amount = 2599.98m,
                    PaymentDate = DateTime.Now.AddDays(-10),
                    ReceivedBy = 1,
                    PaymentMethod = "Credit Card"
                },
                new Payment {
                    PaymentID = 5,
                    InvoiceID = 5,
                    Amount = 949.98m,
                    PaymentDate = DateTime.Now.AddDays(-5),
                    ReceivedBy = 1,
                    PaymentMethod = "Credit Card"
                }
            };
        }
    }
}