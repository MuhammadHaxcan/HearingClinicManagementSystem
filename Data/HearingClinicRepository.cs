using HearingClinicManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Globalization;

namespace HearingClinicManagementSystem.Data
{
    public class HearingClinicRepository : IDisposable
    {
        private readonly HearingClinicDbContext _context;
        private static HearingClinicRepository _instance;
        private static readonly object _lock = new object();

        private HearingClinicRepository()
        {
            _context = new HearingClinicDbContext();
        }

        public static HearingClinicRepository Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new HearingClinicRepository();
                        }
                    }
                }
                return _instance;
            }
        }

        // User methods
        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User GetUserById(int id)
        {
            return _context.Users.Find(id);
        }

        public User GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
        }

        // Patient methods
        public List<Patient> GetAllPatients()
        {
            return _context.Patients.Include(p => p.User).ToList();
        }

        public Patient GetPatientById(int id)
        {
            return _context.Patients.Include(p => p.User).FirstOrDefault(p => p.PatientID == id);
        }

        public Patient GetPatientByUserId(int userId)
        {
            return _context.Patients.Include(p => p.User).FirstOrDefault(p => p.UserID == userId);
        }

        public void AddPatient(Patient patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();
        }

        public void UpdatePatient(Patient patient)
        {
            _context.Entry(patient).State = EntityState.Modified;
            _context.SaveChanges();
        }

        // Audiologist methods
        public List<Audiologist> GetAllAudiologists()
        {
            return _context.Audiologists
                .Include(a => a.User)
                .ToList();
        }

        public Audiologist GetAudiologistById(int id)
        {
            return _context.Audiologists.Include(a => a.User).FirstOrDefault(a => a.AudiologistID == id);
        }

        /// <summary>
        /// Gets an audiologist by user ID
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Audiologist entity or null if not found</returns>
        public Audiologist GetAudiologistByUserId(int userId)
        {
            return _context.Audiologists
                .Include(a => a.User)
                .FirstOrDefault(a => a.UserID == userId);
        }

        // Receptionist methods
        /// <summary>
        /// Gets a receptionist by user ID
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>Receptionist entity or null if not found</returns>
        public Receptionist GetReceptionistByUserId(int userId)
        {
            return _context.Receptionists
                .Include(r => r.User)
                .FirstOrDefault(r => r.UserID == userId);
        }

        // Inventory Manager methods
        /// <summary>
        /// Gets an inventory manager by user ID
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>InventoryManager entity or null if not found</returns>
        public InventoryManager GetInventoryManagerByUserId(int userId)
        {
            return _context.InventoryManagers
                .Include(im => im.User)
                .FirstOrDefault(im => im.UserID == userId);
        }

        // Clinic Manager methods
        /// <summary>
        /// Gets a clinic manager by user ID
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <returns>ClinicManager entity or null if not found</returns>
        public ClinicManager GetClinicManagerByUserId(int userId)
        {
            return _context.ClinicManagers
                .Include(cm => cm.User)
                .FirstOrDefault(cm => cm.UserID == userId);
        }

        // Product methods
        public List<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public Product GetProductById(int id)
        {
            return _context.Products.Find(id);
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        // Order methods
        public List<Order> GetAllOrders()
        {
            return _context.Orders.ToList();
        }

        public List<Order> GetOrdersByPatientId(int patientId)
        {
            return _context.Orders.Where(o => o.PatientID == patientId).ToList();
        }

        public Order GetOrderById(int id)
        {
            return _context.Orders.Find(id);
        }

        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteOrder(int id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Removes an order item from the database
        /// </summary>
        /// <param name="item">The order item to remove</param>
        public void RemoveOrderItem(OrderItem item)
        {
            if (item == null)
                return;
            
            var existingItem = _context.OrderItems.Find(item.OrderItemID);
            if (existingItem != null)
            {
                _context.OrderItems.Remove(existingItem);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Removes an order item from the database by ID
        /// </summary>
        /// <param name="orderItemId">The ID of the order item to remove</param>
        public void RemoveOrderItem(int orderItemId)
        {
            var existingItem = _context.OrderItems.Find(orderItemId);
            if (existingItem != null)
            {
                _context.OrderItems.Remove(existingItem);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Removes an order from the database
        /// </summary>
        /// <param name="order">The order to remove</param>
        public void RemoveOrder(Order order)
        {
            if (order == null)
                return;
            
            var existingOrder = _context.Orders.Find(order.OrderID);
            if (existingOrder != null)
            {
                // First remove any associated order items to avoid foreign key constraint violations
                var orderItems = _context.OrderItems.Where(oi => oi.OrderID == existingOrder.OrderID).ToList();
                foreach (var item in orderItems)
                {
                    _context.OrderItems.Remove(item);
                }
                
                _context.Orders.Remove(existingOrder);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Removes an order from the database by ID
        /// </summary>
        /// <param name="orderId">The ID of the order to remove</param>
        public void RemoveOrder(int orderId)
        {
            var existingOrder = _context.Orders.Find(orderId);
            if (existingOrder != null)
            {
                // First remove any associated order items to avoid foreign key constraint violations
                var orderItems = _context.OrderItems.Where(oi => oi.OrderID == orderId).ToList();
                foreach (var item in orderItems)
                {
                    _context.OrderItems.Remove(item);
                }
                
                _context.Orders.Remove(existingOrder);
                _context.SaveChanges();
            }
        }

        // Order management specific methods
        public List<Order> GetAllOrdersWithDetails()
        {
            return _context.Orders
                .Include(o => o.Patient.User)
                .OrderByDescending(o => o.Status == "Pending")
                .ThenByDescending(o => o.OrderDate)
                .ToList();
        }

        public Order GetOrderWithDetails(int orderId)
        {
            return _context.Orders
                .Include(o => o.Patient.User)
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.OrderID == orderId);
        }

        public List<OrderItem> GetOrderItemsWithProductDetails(int orderId)
        {
            return _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.OrderID == orderId)
                .ToList();
        }

        public bool ConfirmOrder(int orderId, int processedByUserId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var order = _context.Orders.Find(orderId);
                    if (order == null || order.Status != "Pending")
                        return false;

                    // Update order status
                    order.Status = "Confirmed";
                    order.ProcessedBy = processedByUserId;
                    
                    // Get order items
                    var orderItems = _context.OrderItems
                        .Include(oi => oi.Product)
                        .Where(oi => oi.OrderID == orderId)
                        .ToList();
                    
                    // Update inventory
                    foreach (var item in orderItems)
                    {
                        var product = item.Product;
                        if (product != null)
                        {
                            // Check if enough stock
                            if (product.QuantityInStock < item.Quantity)
                                throw new InvalidOperationException($"Not enough stock for product {product.Model}.");
                            
                            // Reduce inventory
                            product.QuantityInStock -= item.Quantity;
                            
                            // Record transaction
                            var inventoryTransaction = new InventoryTransaction
                            {
                                ProductID = product.ProductID,
                                TransactionType = "Sale",
                                Quantity = -item.Quantity,  // Negative for deduction
                                TransactionDate = DateTime.Now,
                                ProcessedBy = processedByUserId
                            };
                            
                            _context.InventoryTransactions.Add(inventoryTransaction);
                        }
                    }
                    
                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public bool RejectOrder(int orderId, int processedByUserId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null || order.Status != "Pending")
                return false;
            
            order.Status = "Cancelled";
            order.ProcessedBy = processedByUserId;
            _context.SaveChanges();
            return true;
        }

        // OrderItem methods
        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            return _context.OrderItems.Include(oi => oi.Product).Where(oi => oi.OrderID == orderId).ToList();
        }

        public void AddOrderItem(OrderItem orderItem)
        {
            _context.OrderItems.Add(orderItem);
            _context.SaveChanges();
        }

        public void UpdateOrderItem(OrderItem orderItem)
        {
            _context.Entry(orderItem).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteOrderItem(int id)
        {
            var orderItem = _context.OrderItems.Find(id);
            if (orderItem != null)
            {
                _context.OrderItems.Remove(orderItem);
                _context.SaveChanges();
            }
        }

        // Inventory Transaction methods
        public void AddInventoryTransaction(InventoryTransaction transaction)
        {
            _context.InventoryTransactions.Add(transaction);
            _context.SaveChanges();
        }

        public List<InventoryTransaction> GetInventoryTransactionsByProductId(int productId)
        {
            return _context.InventoryTransactions.Where(it => it.ProductID == productId).ToList();
        }

        // Invoice methods
        public void AddInvoice(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            _context.SaveChanges();
        }

        public void UpdateInvoice(Invoice invoice)
        {
            _context.Entry(invoice).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public Invoice GetInvoiceByOrderId(int orderId)
        {
            return _context.Invoices.FirstOrDefault(i => i.OrderID == orderId);
        }

        public List<Invoice> GetInvoicesByPatientId(int patientId)
        {
            return _context.Invoices
                .Where(i => i.Appointment.PatientID == patientId || i.Order.PatientID == patientId)
                .ToList();
        }

        public List<Invoice> GetInvoicesByAppointmentIds(List<int> appointmentIds)
        {
            return _context.Invoices
                .Where(i => appointmentIds.Contains(i.AppointmentID ?? 0))
                .ToList();
        }

        /// <summary>
        /// Gets all invoices for a specific appointment with related payment information
        /// </summary>
        /// <param name="appointmentId">The appointment ID</param>
        /// <returns>List of invoices with related payment information</returns>
        public List<dynamic> GetInvoicesForAppointment(int appointmentId)
        {
            // Get invoice IDs for this appointment
            var invoices = _context.Invoices
                .Where(i => i.AppointmentID == appointmentId)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();

            // Build result including payment information
            var result = new List<dynamic>();
            foreach (var invoice in invoices)
            {
                // Get payment for this invoice
                var payment = _context.Payments.FirstOrDefault(p => p.InvoiceID == invoice.InvoiceID);
                
                // Get user who created the payment
                User createdByUser = null;
                if (payment != null)
                {
                    createdByUser = _context.Users.Find(payment.ReceivedBy);
                }
                
                // Name the properties explicitly to match what client code expects
                result.Add(new
                {
                    Invoice = invoice,
                    Payment = payment,
                    CreatedByUser = createdByUser
                });
            }
            
            return result;
        }
        
        /// <summary>
        /// Gets all invoices for a specific order with related payment information
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <returns>List of invoices with related payment information</returns>
        public List<dynamic> GetInvoicesForOrder(int orderId)
        {
            // Get invoice IDs for this order
            var invoices = _context.Invoices
                .Where(i => i.OrderID == orderId)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();

            // Build result including payment information
            var result = new List<dynamic>();
            foreach (var invoice in invoices)
            {
                // Get payment for this invoice
                var payment = _context.Payments.FirstOrDefault(p => p.InvoiceID == invoice.InvoiceID);
                
                // Get user who created the payment
                User createdByUser = null;
                if (payment != null)
                {
                    createdByUser = _context.Users.Find(payment.ReceivedBy);
                }
                
                // Name the properties explicitly to match what client code expects
                result.Add(new
                {
                    Invoice = invoice,
                    Payment = payment,
                    CreatedByUser = createdByUser
                });
            }
            
            return result;
        }

        // Appointment methods
        public List<Appointment> GetAppointmentsByPatientId(int patientId)
        {
            return _context.Appointments
                .Include(a => a.Audiologist.User)
                .Include(a => a.TimeSlot)
                .Where(a => a.PatientID == patientId)
                .OrderBy(a => a.Date)
                .ToList();
        }

        public Appointment GetAppointmentById(int appointmentId)
        {
            return _context.Appointments
                .Include(a => a.Audiologist.User)
                .Include(a => a.TimeSlot)
                .FirstOrDefault(a => a.AppointmentID == appointmentId);
        }

        public void AddAppointment(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            _context.SaveChanges();
        }

        public void UpdateAppointment(Appointment appointment)
        {
            _context.Entry(appointment).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public List<Schedule> GetSchedulesByAudiologistId(int audiologistId)
        {
            return _context.Schedules
                .Where(s => s.AudiologistID == audiologistId)
                .ToList();
        }

        public Schedule GetScheduleByAudiologistAndDay(int audiologistId, string dayOfWeek)
        {
            return _context.Schedules
                .FirstOrDefault(s => s.AudiologistID == audiologistId && s.DayOfWeek == dayOfWeek);
        }

        public List<TimeSlot> GetAvailableTimeSlotsByScheduleId(int scheduleId)
        {
            return _context.TimeSlots
                .Where(ts => ts.ScheduleID == scheduleId && ts.IsAvailable)
                .OrderBy(ts => ts.StartTime)
                .ToList();
        }

        public TimeSlot GetTimeSlotById(int timeSlotId)
        {
            return _context.TimeSlots.Find(timeSlotId);
        }

        public void UpdateTimeSlot(TimeSlot timeSlot)
        {
            _context.Entry(timeSlot).State = EntityState.Modified;
            _context.SaveChanges();
        }

        /// <summary>
        /// Gets available time slots for a specific audiologist on a specific date
        /// </summary>
        /// <param name="audiologistId">ID of the audiologist</param>
        /// <param name="date">Date for the appointment</param>
        /// <returns>List of available time slots with display information</returns>
        public List<dynamic> GetAvailableTimeSlots(int audiologistId, DateTime date)
        {
            var dayOfWeek = date.DayOfWeek.ToString();
            
            // Get schedule for selected audiologist on selected day
            var schedule = _context.Schedules
                .FirstOrDefault(s => s.AudiologistID == audiologistId && s.DayOfWeek == dayOfWeek);
                
            if (schedule == null)
                return new List<dynamic>();
                
            // Get all time slots for that schedule
            var allSlots = _context.TimeSlots
                .Where(ts => ts.ScheduleID == schedule.ScheduleID)
                .OrderBy(ts => ts.StartTime)
                .ToList();
                
            // Find which slots are already booked
            // Fix: Compare year, month and day separately instead of using .Date property
            DateTime startOfDay = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            DateTime endOfDay = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            
            var bookedTimeSlots = _context.Appointments
                .Where(a => a.AudiologistID == audiologistId &&
                        a.Date >= startOfDay && a.Date <= endOfDay &&
                        (a.Status == "Confirmed" || a.Status == "Pending"))
                .Select(a => a.TimeSlotID)
                .ToList();
                
            // Filter to only available slots
            var availableSlots = new List<dynamic>();
            
            foreach (var ts in allSlots.Where(ts => !bookedTimeSlots.Contains(ts.TimeSlotID) && ts.IsAvailable))
            {
                // Format times for 12-hour format display
                string startTime = FormatTimeFor12HourDisplay(ts.StartTime);
                string endTime = FormatTimeFor12HourDisplay(ts.EndTime);
                
                availableSlots.Add(new
                {
                    ts.TimeSlotID,
                    DisplayTime = startTime + " - " + endTime
                });
            }
            
            return availableSlots;
        }

        // Medical Record methods
        public List<MedicalRecord> GetMedicalRecordsByPatientId(int patientId)
        {
            return _context.MedicalRecords
                .Include(mr => mr.Appointment)
                .Include(mr => mr.Appointment.Audiologist)
                .Include(mr => mr.Appointment.Audiologist.User)
                .Where(mr => mr.PatientID == patientId)
                .OrderByDescending(mr => mr.RecordDate)
                .ToList();
        }

        public MedicalRecord GetMedicalRecordById(int recordId)
        {
            return _context.MedicalRecords
                .Include(mr => mr.Appointment)
                .Include(mr => mr.Appointment.Audiologist)
                .Include(mr => mr.Appointment.Audiologist.User)
                .FirstOrDefault(mr => mr.RecordID == recordId);
        }

        // Hearing Test methods
        public List<HearingTest> GetHearingTestsByRecordId(int recordId)
        {
            return _context.HearingTests
                .Where(ht => ht.RecordID == recordId)
                .ToList();
        }

        public List<AudiogramData> GetAudiogramDataByTestId(int testId)
        {
            return _context.AudiogramData
                .Where(ad => ad.TestID == testId)
                .ToList();
        }

        // Prescription methods
        public List<Prescription> GetPrescriptionsByAppointmentId(int appointmentId)
        {
            return _context.Prescriptions
                .Include(p => p.Product)
                .Where(p => p.AppointmentID == appointmentId)
                .ToList();
        }

        // Product management specific methods
        public List<Product> GetProductsOrderedByManufacturerAndModel() {
            return _context.Products
                .OrderBy(p => p.Manufacturer)
                .ThenBy(p => p.Model)
                .ToList();
        }

        public Product GetProductByIdForManager(int productId) {
            return _context.Products.Find(productId);
        }

        public int AddProductForManager(Product product) {
            _context.Products.Add(product);
            _context.SaveChanges();
            return product.ProductID;
        }

        public void UpdateProductForManager(Product product) {
            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteProductForManager(int productId) {
            var product = _context.Products.Find(productId);
            if (product != null) {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public void AddStock(int productId, int quantity, string reason, int processedBy) {
            using (var transaction = _context.Database.BeginTransaction()) {
                try {
                    var product = _context.Products.Find(productId);
                    if (product != null) {
                        // Update product quantity
                        product.QuantityInStock += quantity;

                        // Create transaction record
                        var inventoryTransaction = new InventoryTransaction {
                            ProductID = productId,
                            TransactionType = "Restock",
                            Quantity = quantity,
                            TransactionDate = DateTime.Now,
                            ProcessedBy = processedBy
                        };

                        _context.InventoryTransactions.Add(inventoryTransaction);
                        _context.SaveChanges();
                        transaction.Commit();
                    }
                } catch {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public void RemoveStock(int productId, int quantity, string reason, int processedBy) {
            using (var transaction = _context.Database.BeginTransaction()) {
                try {
                    var product = _context.Products.Find(productId);
                    if (product != null) {
                        // Check if enough stock
                        if (product.QuantityInStock < quantity)
                            throw new InvalidOperationException($"Not enough stock available. Only {product.QuantityInStock} units in inventory.");

                        // Update product quantity
                        product.QuantityInStock -= quantity;

                        // Create transaction record
                        string transactionType = reason.ToLower().Contains("sale") ||
                                               reason.ToLower().Contains("sold") ||
                                               reason.ToLower().Contains("purchase") ?
                                               "Sale" : "Adjustment";

                        var inventoryTransaction = new InventoryTransaction {
                            ProductID = productId,
                            TransactionType = transactionType,
                            Quantity = -quantity, // Negative quantity for removal
                            TransactionDate = DateTime.Now,
                            ProcessedBy = processedBy
                        };

                        _context.InventoryTransactions.Add(inventoryTransaction);
                        _context.SaveChanges();
                        transaction.Commit();
                    }
                } catch {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public int GetNextProductId() {
            if (!_context.Products.Any())
                return 1;

            return _context.Products.Max(p => p.ProductID) + 1;
        }

        // Receptionist appointment management methods
        
        /// <summary>
        /// Gets all active patients for appointment booking
        /// </summary>
        /// <returns>List of patients with active user accounts</returns>
        public List<dynamic> GetActivePatients()
        {
            return _context.Patients
                .Include(p => p.User)
                .Where(p => p.User.IsActive)
                .Select(p => new
                {
                    p.PatientID,
                    DisplayName = p.User.FirstName + " " + p.User.LastName
                })
                .OrderBy(p => p.DisplayName)
                .ToList<dynamic>();
        }
        
        /// <summary>
        /// Gets all active audiologists for appointment booking
        /// </summary>
        /// <returns>List of audiologists with active user accounts</returns>
        public List<dynamic> GetActiveAudiologists()
        {
            return _context.Audiologists
                .Include(a => a.User)
                .Where(a => a.User.IsActive)
                .Select(a => new
                {
                    a.AudiologistID,
                    DisplayName = "Dr. " + a.User.FirstName + " " + a.User.LastName + 
                                 (string.IsNullOrEmpty(a.Specialization) ? "" : " (" + a.Specialization + ")")
                })
                .OrderBy(a => a.DisplayName)
                .ToList<dynamic>();
        }
        
        /// <summary>
        /// Helper method to format TimeSpan for 12-hour display
        /// </summary>
        private string FormatTimeFor12HourDisplay(TimeSpan time)
        {
            int hour = time.Hours;
            string minutes = time.Minutes == 0 ? "00" : time.Minutes.ToString();
            string amPm = "AM";
            
            if (hour >= 12)
            {
                amPm = "PM";
                if (hour > 12)
                    hour -= 12;
            }
            else if (hour == 0)
            {
                hour = 12;
            }
            
            return $"{hour}:{minutes} {amPm}";
        }
        
        /// <summary>
        /// Books a new appointment
        /// </summary>
        /// <param name="appointment">The appointment to book</param>
        /// <returns>The ID of the newly created appointment</returns>
        public int CreateAppointment(Appointment appointment)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Add the appointment
                    _context.Appointments.Add(appointment);
                    
                    // Mark the time slot as unavailable
                    var timeSlot = _context.TimeSlots.Find(appointment.TimeSlotID);
                    if (timeSlot != null)
                    {
                        timeSlot.IsAvailable = false;
                    }
                    
                    _context.SaveChanges();
                    transaction.Commit();
                    return appointment.AppointmentID;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        
        /// <summary>
        /// Gets the next available appointment ID
        /// </summary>
        /// <returns>Next available appointment ID</returns>
        public int GetNextAppointmentId()
        {
            if (!_context.Appointments.Any())
                return 1;
                
            return _context.Appointments.Max(a => a.AppointmentID) + 1;
        }

        /// <summary>
        /// Gets all appointments ordered by pending status and date
        /// </summary>
        /// <returns>List of all appointments with details</returns>
        public List<Appointment> GetAllAppointmentsWithDetails()
        {
            return _context.Appointments
                .Include(a => a.Patient.User)
                .Include(a => a.Audiologist.User)
                .Include(a => a.TimeSlot)
                .OrderByDescending(a => a.Status == "Pending")
                .ThenByDescending(a => a.Date)
                .ToList();
        }
        
        /// <summary>
        /// Gets appointment by ID with all related details
        /// </summary>
        /// <param name="appointmentId">The appointment ID</param>
        /// <returns>Appointment with patient, audiologist and time slot details</returns>
        public Appointment GetAppointmentWithDetails(int appointmentId)
        {
            return _context.Appointments
                .Include(a => a.Patient.User)
                .Include(a => a.Audiologist.User)
                .Include(a => a.TimeSlot)
                .FirstOrDefault(a => a.AppointmentID == appointmentId);
        }
        
        /// <summary>
        /// Confirms a pending appointment and sets the fee
        /// </summary>
        /// <param name="appointmentId">The appointment ID</param>
        /// <param name="fee">The appointment fee</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool ConfirmAppointment(int appointmentId, decimal fee)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var appointment = _context.Appointments.Find(appointmentId);
                    if (appointment == null || appointment.Status != "Pending")
                        return false;
                    
                    // Update appointment status and fee
                    appointment.Status = "Confirmed";
                    appointment.Fee = fee;
                    
                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        
        /// <summary>
        /// Cancels a pending appointment and releases the time slot
        /// </summary>
        /// <param name="appointmentId">The appointment ID</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool CancelAppointment(int appointmentId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var appointment = _context.Appointments.Find(appointmentId);
                    if (appointment == null)
                        return false;
                    
                    // Update appointment status
                    appointment.Status = "Cancelled";
                    
                    // Release the time slot
                    var timeSlot = _context.TimeSlots.Find(appointment.TimeSlotID);
                    if (timeSlot != null)
                    {
                        timeSlot.IsAvailable = true;
                    }
                    
                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        
        /// <summary>
        /// Updates an appointment's fee
        /// </summary>
        /// <param name="appointmentId">The appointment ID</param>
        /// <param name="fee">The new fee amount</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool UpdateAppointmentFee(int appointmentId, decimal fee)
        {
            var appointment = _context.Appointments.Find(appointmentId);
            if (appointment == null)
                return false;
                
            appointment.Fee = fee;
            _context.SaveChanges();
            return true;
        }

        // Payment and Invoice management methods
        
        /// <summary>
        /// Gets all completed appointments with payment details
        /// </summary>
        /// <returns>List of completed appointments with payment information</returns>
        public List<Appointment> GetCompletedAppointmentsWithPaymentDetails()
        {
            return _context.Appointments
                .Include(a => a.Patient.User)
                .Include(a => a.Audiologist.User)
                .Where(a => a.Status == "Completed")
                .OrderByDescending(a => a.Date)
                .ToList();
        }
        
        /// <summary>
        /// Gets all confirmed orders with payment details
        /// </summary>
        /// <returns>List of confirmed orders with payment information</returns>
        public List<Order> GetConfirmedOrdersWithPaymentDetails()
        {
            return _context.Orders
                .Include(o => o.Patient.User)
                .Where(o => o.Status == "Confirmed" || 
                           o.Status == "Partially Paid" || 
                           o.Status == "Completed")
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }
        
        /// <summary>
        /// Calculates the total paid amount for an appointment
        /// </summary>
        /// <param name="appointmentId">The appointment ID</param>
        /// <returns>Total paid amount</returns>
        public decimal CalculateTotalPaidAmountForAppointment(int appointmentId)
        {
            // Handle case where no invoices exist for the appointment
            var invoices = _context.Invoices
                .Where(i => i.AppointmentID == appointmentId && i.Status == "Paid")
                .ToList();
                
            // Sum the total amounts, returning 0 if no invoices exist
            return invoices.Any() ? invoices.Sum(i => i.TotalAmount) : 0m;
        }
        
        /// <summary>
        /// Calculates the total paid amount for an order
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <returns>Total paid amount</returns>
        public decimal CalculateTotalPaidAmountForOrder(int orderId)
        {
            // Handle case where no invoices exist for the order
            var invoices = _context.Invoices
                .Where(i => i.OrderID == orderId && i.Status == "Paid")
                .ToList();
                
            // Sum the total amounts, returning 0 if no invoices exist
            return invoices.Any() ? invoices.Sum(i => i.TotalAmount) : 0m;
        }
       
        
        /// <summary>
        /// Creates a new invoice for an appointment
        /// </summary>
        /// <param name="appointmentId">The appointment ID</param>
        /// <param name="amount">Payment amount</param>
        /// <param name="paymentMethod">Payment method (Cash, Credit Card, etc.)</param>
        /// <param name="userId">ID of user creating the invoice</param>
        /// <returns>The ID of the newly created invoice</returns>
        public int CreateAppointmentInvoice(int appointmentId, decimal amount, string paymentMethod, int userId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var appointment = _context.Appointments.Find(appointmentId);
                    if (appointment == null)
                        throw new InvalidOperationException("Appointment not found");

                    // Create new invoice
                    var invoice = new Invoice
                    {
                        AppointmentID = appointmentId,
                        OrderID = null,
                        InvoiceDate = DateTime.Now,
                        TotalAmount = amount,
                        Status = "Paid", // Mark as paid immediately
                        PaymentMethod = paymentMethod
                    };
                    
                    _context.Invoices.Add(invoice);
                    _context.SaveChanges();
                    
                    // Create payment record
                    var payment = new Payment
                    {
                        InvoiceID = invoice.InvoiceID,
                        Amount = amount,
                        PaymentDate = DateTime.Now,
                        ReceivedBy = userId,
                        PaymentMethod = paymentMethod
                    };
                    
                    _context.Payments.Add(payment);
                    
                    // Update appointment payment status as needed
                    decimal totalPaid = CalculateTotalPaidAmountForAppointment(appointmentId) + amount;
                    if (totalPaid >= appointment.Fee)
                    {
                        // Appointment is fully paid
                        // Status remains "Completed" as it was already completed
                    }
                    
                    _context.SaveChanges();
                    transaction.Commit();
                    
                    return invoice.InvoiceID;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        
        /// <summary>
        /// Creates a new invoice for an order
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <param name="amount">Payment amount</param>
        /// <param name="paymentMethod">Payment method (Cash, Credit Card, etc.)</param>
        /// <param name="userId">ID of user creating the invoice</param>
        /// <returns>The ID of the newly created invoice</returns>
        public int CreateOrderInvoice(int orderId, decimal amount, string paymentMethod, int userId)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var order = _context.Orders.Find(orderId);
                    if (order == null)
                        throw new InvalidOperationException("Order not found");

                    // Create new invoice
                    var invoice = new Invoice
                    {
                        AppointmentID = null,
                        OrderID = orderId,
                        InvoiceDate = DateTime.Now,
                        TotalAmount = amount,
                        Status = "Paid", // Mark as paid immediately
                        PaymentMethod = paymentMethod
                    };
                    
                    _context.Invoices.Add(invoice);
                    _context.SaveChanges();
                    
                    // Create payment record
                    var payment = new Payment
                    {
                        InvoiceID = invoice.InvoiceID,
                        Amount = amount,
                        PaymentDate = DateTime.Now,
                        ReceivedBy = userId,
                        PaymentMethod = paymentMethod
                    };
                    
                    _context.Payments.Add(payment);
                    
                    // Update order payment status
                    decimal totalPaid = CalculateTotalPaidAmountForOrder(orderId) + amount;
                    if (totalPaid >= order.TotalAmount)
                    {
                        order.Status = "Completed"; // Mark as completed when fully paid
                    }
                    else if (totalPaid > 0)
                    {
                        order.Status = "Partially Paid";
                    }
                    
                    _context.SaveChanges();
                    transaction.Commit();
                    
                    return invoice.InvoiceID;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        
        /// <summary>
        /// Gets the user who received a payment
        /// </summary>
        /// <param name="paymentId">The payment ID</param>
        /// <returns>The user who received the payment</returns>
        public User GetPaymentReceivedByUser(int paymentId)
        {
            var payment = _context.Payments
                .FirstOrDefault(p => p.PaymentID == paymentId);
                
            if (payment == null)
                return null;
                
            return _context.Users.Find(payment.ReceivedBy);
        }
        
        /// <summary>
        /// Gets the count of order items for an order
        /// </summary>
        /// <param name="orderId">The order ID</param>
        /// <returns>Number of items in the order</returns>
        public int GetOrderItemCount(int orderId)
        {
            return _context.OrderItems.Count(oi => oi.OrderID == orderId);
        }

        // Seed data method
        public void SeedInitialData()
        {
            // Only seed if database is empty
            if (_context.Users.Any())
                return;

            // Get data from StaticDataProvider
            var users = StaticDataProvider.Users;
            var patients = StaticDataProvider.Patients;
            var audiologists = StaticDataProvider.Audiologists;
            var receptionists = StaticDataProvider.Receptionists;
            var inventoryManagers = StaticDataProvider.InventoryManagers;
            var clinicManagers = StaticDataProvider.ClinicManagers;
            var products = StaticDataProvider.Products;
            // ... add other entities as needed

            // Add to context
            _context.Users.AddRange(users);
            _context.SaveChanges();

            // Need to clear navigation properties first since we're seeding data
            foreach (var patient in patients)
            {
                patient.User = null;
            }
            _context.Patients.AddRange(patients);

            foreach (var audiologist in audiologists)
            {
                audiologist.User = null;
            }
            _context.Audiologists.AddRange(audiologists);

            foreach (var receptionist in receptionists)
            {
                receptionist.User = null;
            }
            _context.Receptionists.AddRange(receptionists);

            foreach (var manager in inventoryManagers)
            {
                manager.User = null;
            }
            _context.InventoryManagers.AddRange(inventoryManagers);

            foreach (var manager in clinicManagers)
            {
                manager.User = null;
            }
            _context.ClinicManagers.AddRange(clinicManagers);

            _context.Products.AddRange(products);
            _context.SaveChanges();

            // Continue with other entities...
        }

        #region Updated Clinic Statistics Repository Methods

        /// <summary>
        /// Gets appointments within a date range with optional audiologist filtering
        /// </summary>
        /// <param name="startDate">Start date for filtering</param>
        /// <param name="endDate">End date for filtering</param>
        /// <param name="audiologistId">Optional audiologist ID to filter by</param>
        /// <returns>List of appointments matching the criteria</returns>
        public List<Appointment> GetAppointmentsInDateRange(DateTime startDate, DateTime endDate, int? audiologistId = null)
        {
            try
            {
                var query = _context.Appointments
                    .Include(a => a.Patient.User)
                    .Include(a => a.Audiologist.User)
                    .Where(a => a.Date >= startDate && a.Date <= endDate);
                
                if (audiologistId.HasValue)
                {
                    query = query.Where(a => a.AudiologistID == audiologistId.Value);
                }
                
                return query.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAppointmentsInDateRange: {ex.Message}");
                return new List<Appointment>();
            }
        }

        /// <summary>
        /// Gets a summary of appointment statistics for the dashboard
        /// </summary>
        /// <param name="startDate">Start date for the statistics</param>
        /// <param name="endDate">End date for the statistics</param>
        /// <returns>Summary statistics for appointments</returns>
        public dynamic GetAppointmentSummaryStats(DateTime startDate, DateTime endDate)
        {
            try
            {
                var appointments = GetAppointmentsInDateRange(startDate, endDate);
                
                if (appointments == null || appointments.Count == 0)
                {
                    return new
                    {
                        TotalAppointments = 0,
                        CompletedAppointments = 0,
                        CancelledAppointments = 0,
                        TotalRevenue = 0m,
                        AverageAppointmentValue = 0m,
                        MostActiveDoctor = "No data available",
                        MostActiveDoctorAppointmentCount = 0,
                        CompletionRate = 0d,
                        CancellationRate = 0d
                    };
                }
                
                // Get completed appointments
                var completedAppointments = appointments.Where(a => a.Status == "Completed").ToList();
                
                // Get total revenue - handle null fees
                decimal totalRevenue = completedAppointments.Sum(a => a.Fee);
                
                // Get average appointment value
                decimal avgAppointmentValue = completedAppointments.Count > 0 ? 
                    totalRevenue / completedAppointments.Count : 0;
                
                // Get most active doctor
                var mostActiveDoctor = "No data available";
                int mostActiveDoctorAppointments = 0;
                
                var audiologistAppointments = appointments
                    .GroupBy(a => a.AudiologistID)
                    .Select(g => new { AudiologistID = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count)
                    .FirstOrDefault();
                    
                if (audiologistAppointments != null)
                {
                    var audiologist = _context.Audiologists
                        .Include(a => a.User)
                        .FirstOrDefault(a => a.AudiologistID == audiologistAppointments.AudiologistID);
                        
                    if (audiologist?.User != null)
                    {
                        mostActiveDoctor = $"Dr. {audiologist.User.FirstName} {audiologist.User.LastName}";
                        mostActiveDoctorAppointments = audiologistAppointments.Count;
                    }
                }
                
                // Calculate completion and cancellation rates
                int totalAppointments = appointments.Count;
                int cancelledAppointments = appointments.Count(a => a.Status == "Cancelled");
                
                double completionRate = totalAppointments > 0 ?
                    (double)completedAppointments.Count / totalAppointments * 100 : 0;
                    
                double cancellationRate = totalAppointments > 0 ?
                    (double)cancelledAppointments / totalAppointments * 100 : 0;
                    
                return new
                {
                    TotalAppointments = totalAppointments,
                    CompletedAppointments = completedAppointments.Count,
                    CancelledAppointments = cancelledAppointments,
                    TotalRevenue = totalRevenue,
                    AverageAppointmentValue = avgAppointmentValue,
                    MostActiveDoctor = mostActiveDoctor,
                    MostActiveDoctorAppointmentCount = mostActiveDoctorAppointments,
                    CompletionRate = completionRate,
                    CancellationRate = cancellationRate
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAppointmentSummaryStats: {ex.Message}");
                
                return new
                {
                    TotalAppointments = 0,
                    CompletedAppointments = 0,
                    CancelledAppointments = 0,
                    TotalRevenue = 0m,
                    AverageAppointmentValue = 0m,
                    MostActiveDoctor = "Error retrieving data",
                    MostActiveDoctorAppointmentCount = 0,
                    CompletionRate = 0d,
                    CancellationRate = 0d
                };
            }
        }

        /// <summary>
        /// Gets statistics for top performing audiologists
        /// </summary>
        /// <param name="startDate">Start date for the statistics</param>
        /// <param name="endDate">End date for the statistics</param>
        /// <returns>List of audiologists with their performance statistics</returns>
        public List<dynamic> GetTopAudiologistStats(DateTime startDate, DateTime endDate)
        {
            try
            {
                var result = new List<dynamic>();
                
                // Get all audiologists first to ensure we include those with no appointments
                var allAudiologists = _context.Audiologists
                    .Include(a => a.User)
                    .Where(a => a.User.IsActive)
                    .ToList();
                    
                if (allAudiologists.Count == 0)
                    return result;
                
                // Get all appointments within the date range
                var appointments = GetAppointmentsInDateRange(startDate, endDate);
                
                // Group appointments by audiologist
                var appointmentsByAudiologist = appointments
                    .GroupBy(a => a.AudiologistID)
                    .ToDictionary(g => g.Key, g => g.ToList());
                
                // Create stats for each audiologist
                foreach (var audiologist in allAudiologists)
                {
                    var audiologistAppointments = new List<Appointment>();
                    
                    // Find appointments for this audiologist
                    if (appointmentsByAudiologist.TryGetValue(audiologist.AudiologistID, out var appointments2))
                    {
                        audiologistAppointments = appointments2;
                    }
                    
                    int totalAppointments = audiologistAppointments.Count;
                    int completedAppointments = audiologistAppointments.Count(a => a.Status == "Completed");
                    int cancelledAppointments = audiologistAppointments.Count(a => a.Status == "Cancelled");
                    decimal totalRevenue = audiologistAppointments
                        .Where(a => a.Status == "Completed")
                        .Sum(a => a.Fee);
                        
                    decimal avgRevenue = completedAppointments > 0 ?
                        totalRevenue / completedAppointments : 0;
                        
                    string name = $"Dr. {audiologist.User.FirstName} {audiologist.User.LastName}";
                        
                    result.Add(new
                    {
                        AudiologistID = audiologist.AudiologistID,
                        Name = name,
                        Specialization = audiologist.Specialization ?? "General",
                        TotalAppointments = totalAppointments,
                        CompletedAppointments = completedAppointments,
                        CancelledAppointments = cancelledAppointments,
                        TotalRevenue = totalRevenue,
                        AverageRevenue = avgRevenue
                    });
                }
                
                // Sort by total revenue (descending)
                return result.OrderByDescending(a => ((dynamic)a).TotalRevenue).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTopAudiologistStats: {ex.Message}");
                return new List<dynamic>();
            }
        }

        /// <summary>
        /// Gets the most recent appointments for the clinic
        /// </summary>
        /// <param name="count">Number of appointments to return</param>
        /// <returns>List of recent appointments with all related details</returns>
        public List<dynamic> GetRecentAppointments(int count = 15)
        {
            try
            {
                var result = new List<dynamic>();
                
                // Get the most recent appointments regardless of status
                var recentAppointments = _context.Appointments
                    .Include(a => a.Patient.User)
                    .Include(a => a.Audiologist.User)
                    .OrderByDescending(a => a.Date)
                    .Take(count)
                    .ToList();
                    
                foreach (var appointment in recentAppointments)
                {
                    // Handle potential null references safely
                    string patientName = "Unknown";
                    if (appointment.Patient?.User != null)
                    {
                        patientName = $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}";
                    }
                    
                    string audiologistName = "Unknown";
                    if (appointment.Audiologist?.User != null)
                    {
                        audiologistName = $"Dr. {appointment.Audiologist.User.FirstName} {appointment.Audiologist.User.LastName}";
                    }
                    
                    // Add appointment details to result
                    result.Add(new
                    {
                        AppointmentID = appointment.AppointmentID,
                        Date = appointment.Date,
                        PatientName = patientName,
                        AudiologistName = audiologistName,
                        Purpose = appointment.PurposeOfVisit ?? "Not specified",
                        Fee = appointment.Fee,
                        Status = appointment.Status ?? "Unknown"
                    });
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetRecentAppointments: {ex.Message}");
                return new List<dynamic>();
            }
        }

        /// <summary>
        /// Gets financial summary statistics grouped by visit purpose
        /// </summary>
        /// <param name="startDate">Start date for the statistics</param>
        /// <param name="endDate">End date for the statistics</param>
        /// <param name="audiologistId">Optional audiologist ID to filter by</param>
        /// <returns>Financial statistics grouped by visit purpose</returns>
        public List<dynamic> GetFinancialSummaryByPurpose(DateTime startDate, DateTime endDate, int? audiologistId = null)
        {
            try
            {
                var result = new List<dynamic>();
                
                // Get completed appointments in the date range
                var appointments = GetAppointmentsInDateRange(startDate, endDate, audiologistId)
                    .Where(a => a.Status == "Completed")
                    .ToList();
                    
                if (appointments.Count == 0)
                    return result;
                    
                // Handle null purposes
                foreach (var appointment in appointments)
                {
                    if (string.IsNullOrEmpty(appointment.PurposeOfVisit))
                    {
                        appointment.PurposeOfVisit = "Not specified";
                    }
                }
                
                // Calculate total revenue
                decimal totalRevenue = appointments.Sum(a => a.Fee);
                
                // Group by purpose
                var groupedByPurpose = appointments
                    .GroupBy(a => a.PurposeOfVisit)
                    .ToList();
                    
                // Create summary for each purpose
                foreach (var group in groupedByPurpose)
                {
                    int count = group.Count();
                    decimal groupRevenue = group.Sum(a => a.Fee);
                    decimal averageRevenue = count > 0 ? groupRevenue / count : 0;
                    decimal percentOfTotal = totalRevenue > 0 ? (groupRevenue / totalRevenue) * 100 : 0;
                    
                    result.Add(new
                    {
                        Purpose = group.Key,
                        Count = count,
                        TotalRevenue = groupRevenue,
                        AverageRevenue = averageRevenue,
                        PercentOfTotal = percentOfTotal
                    });
                }
                
                // Add total row
                if (groupedByPurpose.Any())
                {
                    int totalCount = appointments.Count;
                    decimal avgTotalRevenue = totalCount > 0 ? totalRevenue / totalCount : 0;
                    
                    result.Add(new
                    {
                        Purpose = "TOTAL",
                        Count = totalCount,
                        TotalRevenue = totalRevenue,
                        AverageRevenue = avgTotalRevenue,
                        PercentOfTotal = 100m
                    });
                }
                
                return result.OrderByDescending(r => ((dynamic)r).TotalRevenue).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetFinancialSummaryByPurpose: {ex.Message}");
                return new List<dynamic>();
            }
        }

        /// <summary>
        /// Gets payment method statistics for completed appointments
        /// </summary>
        /// <param name="startDate">Start date for the statistics</param>
        /// <param name="endDate">End date for the statistics</param>
        /// <param name="audiologistId">Optional audiologist ID to filter by</param>
        /// <returns>Statistics about payment methods used</returns>
        public List<dynamic> GetPaymentMethodStats(DateTime startDate, DateTime endDate, int? audiologistId = null)
        {
            try
            {
                var result = new List<dynamic>();
                
                // Get completed appointments in the date range
                var appointments = GetAppointmentsInDateRange(startDate, endDate, audiologistId)
                    .Where(a => a.Status == "Completed")
                    .ToList();
                    
                if (appointments.Count == 0)
                    return result;
                    
                // Get appointment IDs
                var appointmentIds = appointments.Select(a => a.AppointmentID).ToList();
                
                // Get invoices for these appointments
                var invoices = _context.Invoices
                    .Where(i => i.AppointmentID.HasValue && appointmentIds.Contains(i.AppointmentID.Value))
                    .ToList();
                    
                if (invoices.Count == 0)
                    return result;
                    
                // Set "Unknown" for any nulls
                foreach (var invoice in invoices)
                {
                    if (string.IsNullOrEmpty(invoice.PaymentMethod))
                    {
                        invoice.PaymentMethod = "Unknown";
                    }
                }
                
                // Get total paid amount
                decimal totalAmount = invoices.Sum(i => i.TotalAmount);
                
                // Group by payment method
                var groupedByMethod = invoices
                    .GroupBy(i => i.PaymentMethod)
                    .ToList();
                    
                // Create summary for each payment method
                foreach (var group in groupedByMethod)
                {
                    int count = group.Count();
                    decimal amount = group.Sum(i => i.TotalAmount);
                    decimal percentOfTotal = totalAmount > 0 ? (amount / totalAmount) * 100 : 0;
                    
                    result.Add(new
                    {
                        PaymentMethod = group.Key,
                        Count = count,
                        TotalAmount = amount,
                        PercentOfTotal = percentOfTotal
                    });
                }
                
                // Add total row
                if (groupedByMethod.Any())
                {
                    result.Add(new
                    {
                        PaymentMethod = "TOTAL",
                        Count = invoices.Count,
                        TotalAmount = totalAmount,
                        PercentOfTotal = 100m
                    });
                }
                
                return result.OrderByDescending(r => ((dynamic)r).TotalAmount).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPaymentMethodStats: {ex.Message}");
                return new List<dynamic>();
            }
        }

        /// <summary>
        /// Gets monthly revenue statistics
        /// </summary>
        /// <param name="monthsToInclude">Number of months to include</param>
        /// <param name="audiologistId">Optional audiologist ID to filter by</param>
        /// <returns>Monthly revenue statistics</returns>
        public List<dynamic> GetMonthlyRevenueStats(int monthsToInclude = 12, int? audiologistId = null)
        {
            try
            {
                var result = new List<dynamic>();
                
                // Calculate date range to include specified number of months
                DateTime startMonth = new DateTime(DateTime.Now.AddMonths(-monthsToInclude + 1).Year, 
                                                DateTime.Now.AddMonths(-monthsToInclude + 1).Month, 1);
                DateTime endMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 
                                            DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month), 23, 59, 59);
                
                // Get all appointments in the date range
                var appointments = GetAppointmentsInDateRange(startMonth, endMonth, audiologistId);
                
                if (appointments.Count == 0)
                    return result;
                    
                // Create a list of month/year combinations to ensure all months are included
                var monthYearList = new List<Tuple<int, int, string>>();
                for (int i = 0; i < monthsToInclude; i++)
                {
                    var currentDate = DateTime.Now.AddMonths(-i);
                    var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(currentDate.Month);
                    monthYearList.Add(new Tuple<int, int, string>(currentDate.Year, currentDate.Month, monthName));
                }
                monthYearList.Reverse(); // Put in chronological order
                
                // Group appointments by month and year
                var appointmentsByMonth = appointments
                    .GroupBy(a => new { Year = a.Date.Year, Month = a.Date.Month })
                    .ToDictionary(g => (g.Key.Year, g.Key.Month), g => g.ToList());
                    
                // Calculate stats for each month
                decimal totalRevenue = 0;
                int totalAppointments = 0;
                int totalCompleted = 0;
                
                foreach (var monthYear in monthYearList)
                {
                    int year = monthYear.Item1;
                    int month = monthYear.Item2;
                    string monthName = monthYear.Item3;
                    
                    // Find appointments for this month
                    List<Appointment> monthAppointments;
                    if (!appointmentsByMonth.TryGetValue((year, month), out monthAppointments))
                    {
                        monthAppointments = new List<Appointment>();
                    }
                    
                    int appointmentCount = monthAppointments.Count;
                    int completedCount = monthAppointments.Count(a => a.Status == "Completed");
                    decimal revenue = monthAppointments.Where(a => a.Status == "Completed").Sum(a => a.Fee);
                    decimal avgRevenue = completedCount > 0 ? revenue / completedCount : 0;
                    double completionRate = appointmentCount > 0 ? (double)completedCount / appointmentCount * 100 : 0;
                    
                    // Add stats to result
                    result.Add(new
                    {
                        Month = monthName,
                        Year = year,
                        AppointmentCount = appointmentCount,
                        CompletedCount = completedCount,
                        Revenue = revenue,
                        AverageRevenue = avgRevenue,
                        CompletionRate = completionRate
                    });
                    
                    // Update totals
                    totalRevenue += revenue;
                    totalAppointments += appointmentCount;
                    totalCompleted += completedCount;
                }
                
                // Add total row
                if (result.Count > 0)
                {
                    decimal avgTotalRevenue = totalCompleted > 0 ? totalRevenue / totalCompleted : 0;
                    double overallCompletionRate = totalAppointments > 0 ? (double)totalCompleted / totalAppointments * 100 : 0;
                    
                    result.Add(new
                    {
                        Month = "TOTAL",
                        Year = (int?)null,
                        AppointmentCount = totalAppointments,
                        CompletedCount = totalCompleted,
                        Revenue = totalRevenue,
                        AverageRevenue = avgTotalRevenue,
                        CompletionRate = overallCompletionRate
                    });
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetMonthlyRevenueStats: {ex.Message}");
                return new List<dynamic>();
            }
        }

        /// <summary>
        /// Gets visit purpose distribution statistics
        /// </summary>
        /// <param name="startDate">Start date for the statistics</param>
        /// <param name="endDate">End date for the statistics</param>
        /// <param name="audiologistId">Optional audiologist ID to filter by</param>
        /// <returns>Statistics about visit purpose distribution</returns>
        public List<dynamic> GetVisitPurposeStats(DateTime startDate, DateTime endDate, int? audiologistId = null)
        {
            try
            {
                var result = new List<dynamic>();
                
                // Get all appointments in the date range
                var appointments = GetAppointmentsInDateRange(startDate, endDate, audiologistId);
                
                if (appointments.Count == 0)
                    return result;
                    
                // Handle null purposes
                foreach (var appointment in appointments)
                {
                    if (string.IsNullOrEmpty(appointment.PurposeOfVisit))
                    {
                        appointment.PurposeOfVisit = "Not specified";
                    }
                }
                
                // Group by purpose
                var groupedByPurpose = appointments
                    .GroupBy(a => a.PurposeOfVisit)
                    .ToList();
                    
                // Calculate total count for percentages
                int totalAppointments = appointments.Count;
                
                // Create summary for each purpose
                foreach (var group in groupedByPurpose)
                {
                    int count = group.Count();
                    int completedCount = group.Count(a => a.Status == "Completed");
                    double percentage = totalAppointments > 0 ? (double)count / totalAppointments * 100 : 0;
                    decimal totalRevenue = group.Where(a => a.Status == "Completed").Sum(a => a.Fee);
                    decimal avgFee = completedCount > 0 ? totalRevenue / completedCount : 0;
                    
                    result.Add(new
                    {
                        Purpose = group.Key,
                        Count = count,
                        CompletedCount = completedCount,
                        Percentage = percentage,
                        AverageFee = avgFee,
                        TotalRevenue = totalRevenue
                    });
                }
                
                // Add total row
                if (groupedByPurpose.Any())
                {
                    int totalCompleted = appointments.Count(a => a.Status == "Completed");
                    decimal totalRevenue = appointments.Where(a => a.Status == "Completed").Sum(a => a.Fee);
                    decimal avgFee = totalCompleted > 0 ? totalRevenue / totalCompleted : 0;
                    
                    result.Add(new
                    {
                        Purpose = "TOTAL",
                        Count = totalAppointments,
                        CompletedCount = totalCompleted,
                        Percentage = 100.0,
                        AverageFee = avgFee,
                        TotalRevenue = totalRevenue
                    });
                }
                
                return result.OrderByDescending(r => ((dynamic)r).Count).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetVisitPurposeStats: {ex.Message}");
                return new List<dynamic>();
            }
        }

        /// <summary>
        /// Gets all audiologists for filtering in the UI
        /// </summary>
        /// <returns>List of audiologists with their ID and name</returns>
        public List<dynamic> GetAudiologistsForFilter()
        {
            try
            {
                var result = new List<dynamic>();
                
                // Add "All Audiologists" option
                result.Add(new
                {
                    AudiologistID = (int?)null,
                    DisplayName = "All Audiologists"
                });
                
                // Get all active audiologists
                var audiologists = _context.Audiologists
                    .Include(a => a.User)
                    .Where(a => a.User.IsActive)
                    .OrderBy(a => a.User.LastName)
                    .ThenBy(a => a.User.FirstName)
                    .ToList();
                    
                foreach (var audiologist in audiologists)
                {
                    if (audiologist.User != null)
                    {
                        result.Add(new
                        {
                            AudiologistID = audiologist.AudiologistID,
                            DisplayName = $"Dr. {audiologist.User.FirstName} {audiologist.User.LastName}"
                        });
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAudiologistsForFilter: {ex.Message}");
                
                // Return at least the "All" option
                return new List<dynamic>
                {
                    new
                    {
                        AudiologistID = (int?)null,
                        DisplayName = "All Audiologists"
                    }
                };
            }
        }

        #endregion

        #region Inventory Reporting Repository Methods
        
        /// <summary>
        /// Gets inventory summary statistics
        /// </summary>
        /// <returns>Summary statistics for inventory dashboard</returns>
        public dynamic GetInventorySummaryStats()
        {
            try
            {
                // Get all confirmed orders
                var confirmedOrders = _context.Orders.Where(o => o.Status == "Confirmed" || o.Status == "Completed").ToList();
                
                // Calculate total sales
                decimal totalSales = confirmedOrders.Sum(o => o.TotalAmount);
                
                // Calculate product sales - use a safer way to calculate
                var orderItems = _context.OrderItems
                    .Include(oi => oi.Order)
                    .Where(oi => (oi.Order.Status == "Confirmed" || oi.Order.Status == "Completed"))
                    .ToList();

                var productSales = new Dictionary<int, int>();
                foreach (var item in orderItems)
                {
                    if (productSales.ContainsKey(item.ProductID))
                        productSales[item.ProductID] += item.Quantity;
                    else
                        productSales[item.ProductID] = item.Quantity;
                }
                
                // Find most popular product
                string mostPopularProduct = "No data available";
                int mostPopularProductCount = 0;
                
                if (productSales.Any())
                {
                    var topProductKvp = productSales.OrderByDescending(kv => kv.Value).FirstOrDefault();
                    var product = _context.Products.Find(topProductKvp.Key);
                    if (product != null)
                    {
                        mostPopularProduct = $"{product.Manufacturer} {product.Model}";
                        mostPopularProductCount = topProductKvp.Value;
                    }
                }
                
                // Count low stock products
                int lowStockCount = _context.Products.Count(p => p.QuantityInStock <= 10);
                
                // Get total products count
                int totalProducts = _context.Products.Count();
                
                // Calculate average order value safely
                decimal averageOrderValue = confirmedOrders.Count > 0 ? 
                    totalSales / confirmedOrders.Count : 0;
                
                return new 
                {
                    TotalOrders = confirmedOrders.Count,
                    TotalSales = totalSales,
                    AverageOrderValue = averageOrderValue,
                    MostPopularProduct = mostPopularProduct,
                    MostPopularProductCount = mostPopularProductCount,
                    LowStockCount = lowStockCount,
                    TotalProducts = totalProducts
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetInventorySummaryStats: {ex.Message}");
                
                // Return empty object with defaults if error occurs
                return new 
                {
                    TotalOrders = 0,
                    TotalSales = 0m,
                    AverageOrderValue = 0m,
                    MostPopularProduct = "Error retrieving data",
                    MostPopularProductCount = 0,
                    LowStockCount = 0,
                    TotalProducts = 0
                };
            }
        }
        
        /// <summary>
        /// Gets statistics for top selling products
        /// </summary>
        /// <param name="count">Number of top products to return</param>
        /// <returns>List of top selling products with sales statistics</returns>
        public List<dynamic> GetTopSellingProducts(int count = 15)
        {
            try
            {
                var result = new List<dynamic>();
                
                // Get completed and confirmed orders
                var orderItems = _context.OrderItems
                    .Include(oi => oi.Order)
                    .Include(oi => oi.Product)
                    .Where(oi => 
                        (oi.Order.Status == "Confirmed" || oi.Order.Status == "Completed") && 
                        oi.Product != null) // Ensure product is not null
                    .ToList();
                
                if (!orderItems.Any())
                    return result; // Return empty list if no order items
                
                // Group by product
                var productGroups = orderItems
                    .GroupBy(oi => oi.ProductID)
                    .Select(g => new {
                        ProductID = g.Key,
                        Product = g.First().Product, // Get the product instance
                        UnitsSold = g.Sum(oi => oi.Quantity),
                        TotalSales = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                    })
                    .Where(g => g.Product != null) // Ensure product is not null
                    .OrderByDescending(g => g.TotalSales)
                    .Take(count)
                    .ToList();
                
                if (!productGroups.Any())
                    return result; // Return empty list if no valid groupings
                
                // Calculate total sales
                decimal totalSales = productGroups.Sum(p => p.TotalSales);
                
                // Generate result list
                for (int i = 0; i < productGroups.Count; i++)
                {
                    var productData = productGroups[i];
                    if (productData.Product == null)
                        continue;
                    
                    decimal percentOfSales = totalSales > 0 ? 
                        (productData.TotalSales / totalSales) * 100 : 0;
                    
                    result.Add(new {
                        ProductID = productData.ProductID,
                        Rank = i + 1,
                        ProductName = productData.Product.Model,
                        Manufacturer = productData.Product.Manufacturer,
                        UnitsSold = productData.UnitsSold,
                        TotalSales = productData.TotalSales,
                        PercentOfSales = percentOfSales
                    });
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTopSellingProducts: {ex.Message}");
                return new List<dynamic>(); // Return empty list on error
            }
        }
        
        /// <summary>
        /// Gets list of products with low stock
        /// </summary>
        /// <param name="threshold">Stock threshold to consider as low</param>
        /// <returns>List of products with stock at or below threshold</returns>
        public List<dynamic> GetLowStockProducts(int threshold = 10)
        {
            try
            {
                var result = new List<dynamic>();
                
                // Get products with low stock
                var lowStockProducts = _context.Products
                    .Where(p => p.QuantityInStock <= threshold)
                    .OrderBy(p => p.QuantityInStock)
                    .ToList();
                
                if (!lowStockProducts.Any())
                    return result; // Return empty list if no low stock products
                
                foreach (var product in lowStockProducts)
                {
                    try
                    {
                        // Get last restock transaction
                        var lastRestock = _context.InventoryTransactions
                            .Where(t => t.ProductID == product.ProductID && t.TransactionType == "Restock")
                            .OrderByDescending(t => t.TransactionDate)
                            .FirstOrDefault();
                        
                        // Format status based on stock level
                        string status;
                        if (product.QuantityInStock <= 0)
                            status = "Out of Stock";
                        else if (product.QuantityInStock <= 5)
                            status = "Critical Low";
                        else
                            status = "Low Stock";
                        
                        result.Add(new {
                            ProductID = product.ProductID,
                            ProductName = product.Model,
                            Manufacturer = product.Manufacturer,
                            CurrentStock = product.QuantityInStock,
                            Status = status,
                            Price = product.Price,
                            LastRestocked = lastRestock?.TransactionDate
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing product {product.ProductID}: {ex.Message}");
                        // Continue with the next product if there's an error with this one
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetLowStockProducts: {ex.Message}");
                return new List<dynamic>(); // Return empty list on error
            }
        }

        /// <summary>
        /// Gets summary of inventory transactions by type
        /// </summary>
        /// <param name="startDate">Start date for filtering</param>
        /// <param name="endDate">End date for filtering</param>
        /// <param name="transactionType">Optional transaction type filter</param>
        /// <returns>Statistics grouped by transaction type</returns>
        public List<dynamic> GetTransactionSummary(DateTime startDate, DateTime endDate, string transactionType = null)
        {
            try
            {
                var result = new List<dynamic>();
                
                // Get transactions within date range
                var query = _context.InventoryTransactions
                    .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate);
                
                // Apply transaction type filter if specified and not "All"
                if (!string.IsNullOrEmpty(transactionType) && transactionType != "All")
                {
                    query = query.Where(t => t.TransactionType == transactionType);
                }
                
                // Execute the query and materialize the results
                var transactions = query.ToList();
                
                if (!transactions.Any())
                    return result; // Return empty list if no transactions
                
                // Group by transaction type
                var transactionGroups = transactions
                    .GroupBy(t => t.TransactionType)
                    .Select(g => new {
                        Type = g.Key,
                        Count = g.Count(),
                        TotalQuantity = g.Sum(t => t.Quantity),
                        ProductsAffected = g.Select(t => t.ProductID).Distinct().Count(),
                        AvgQuantityPerTransaction = g.Average(t => t.Quantity)
                    })
                    .OrderByDescending(g => g.Count)
                    .ToList();
                
                // Calculate total for percentages
                int totalTransactions = transactions.Count;
                
                // Add each transaction type group
                foreach (var group in transactionGroups)
                {
                    double percentOfTotal = totalTransactions > 0 ?
                        (double)group.Count / totalTransactions * 100 : 0;
                    
                    result.Add(new {
                        TransactionType = group.Type,
                        Count = group.Count,
                        TotalQuantity = group.TotalQuantity,
                        PercentOfTotal = percentOfTotal,
                        ProductsAffected = group.ProductsAffected,
                        AvgQuantityPerTransaction = group.AvgQuantityPerTransaction
                    });
                }
                
                // Add total summary row
                if (transactionGroups.Any())
                {
                    int totalCount = transactionGroups.Sum(g => g.Count);
                    int totalQuantity = transactions.Sum(t => t.Quantity);
                    int distinctProducts = transactions.Select(t => t.ProductID).Distinct().Count();
                    double avgQuantity = transactions.Count > 0 ? 
                        transactions.Average(t => t.Quantity) : 0;
                    
                    result.Add(new {
                        TransactionType = "TOTAL",
                        Count = totalCount,
                        TotalQuantity = totalQuantity,
                        PercentOfTotal = 100.0,
                        ProductsAffected = distinctProducts,
                        AvgQuantityPerTransaction = avgQuantity
                    });
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTransactionSummary: {ex.Message}");
                return new List<dynamic>(); // Return empty list on error
            }
        }
        
        /// <summary>
        /// Gets detailed transaction history
        /// </summary>
        /// <param name="startDate">Start date for filtering</param>
        /// <param name="endDate">End date for filtering</param>
        /// <param name="transactionType">Optional transaction type filter</param>
        /// <param name="limit">Maximum number of transactions to return</param>
        /// <returns>List of transactions with product and user details</returns>
        public List<dynamic> GetTransactionHistory(DateTime startDate, DateTime endDate, string transactionType = null, int limit = 100)
        {
            try
            {
                var result = new List<dynamic>();
                
                // Get transactions within date range
                var query = _context.InventoryTransactions
                    .Include(t => t.Product)
                    .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate);
                
                // Apply transaction type filter if specified and not "All"
                if (!string.IsNullOrEmpty(transactionType) && transactionType != "All")
                {
                    query = query.Where(t => t.TransactionType == transactionType);
                }
                
                // Get limited number of transactions ordered by date
                var transactions = query
                    .OrderByDescending(t => t.TransactionDate)
                    .Take(limit)
                    .ToList();
                
                if (!transactions.Any())
                    return result; // Return empty list if no transactions
                
                // Process each transaction
                foreach (var transaction in transactions)
                {
                    // Get product name safely
                    string productName = "Unknown Product";
                    if (transaction.Product != null)
                    {
                        productName = $"{transaction.Product.Manufacturer} {transaction.Product.Model}";
                    }
                    
                    // Get user who processed the transaction
                    string processedBy = "Unknown User";
                    if (transaction.ProcessedBy > 0)
                    {
                        var user = _context.Users.Find(transaction.ProcessedBy);
                        if (user != null)
                        {
                            processedBy = $"{user.FirstName} {user.LastName}";
                        }
                    }
                    
                    result.Add(new {
                        TransactionID = transaction.TransactionID,
                        TransactionDate = transaction.TransactionDate,
                        TransactionType = transaction.TransactionType,
                        ProductID = transaction.ProductID,
                        ProductName = productName,
                        Quantity = transaction.Quantity,
                        ProcessedBy = processedBy
                    });
                }
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTransactionHistory: {ex.Message}");
                return new List<dynamic>(); // Return empty list on error
            }
        }
        #endregion

        #region Audiogram and Hearing Tests Methods

        /// <summary>
        /// Gets audiologist's appointments with medical records for a specific date
        /// </summary>
        /// <param name="audiologistId">The audiologist ID</param>
        /// <param name="date">The appointment date</param>
        /// <returns>List of appointments with medical records</returns>
        public List<dynamic> GetAudiologistAppointmentsWithRecordsForDate(int audiologistId, DateTime date)
        {
            // Create start and end date for the exact day
            DateTime startOfDay = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            DateTime endOfDay = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            
            // First get appointments for that day
            var appointments = _context.Appointments
                .Include(a => a.Patient.User)
                .Where(a => a.AudiologistID == audiologistId &&
                          a.Date >= startOfDay && a.Date <= endOfDay &&
                          a.Status == "Confirmed")
                .ToList();
                
            var appointmentsWithRecords = appointments
                .Select(a => new 
                {
                    Appointment = a,
                    MedicalRecord = _context.MedicalRecords
                        .FirstOrDefault(mr => mr.AppointmentID == a.AppointmentID)
                })
                .Where(x => x.MedicalRecord != null) // Only appointments with medical records
                .Select(x => new
                {
                    AppointmentId = x.Appointment.AppointmentID,
                    PatientId = x.Appointment.PatientID,
                    PatientName = x.Appointment.Patient.User.FirstName + " " + x.Appointment.Patient.User.LastName,
                    Time = x.Appointment.Date,
                    MedicalRecordId = x.MedicalRecord.RecordID
                })
                .OrderBy(x => x.Time)
                .ToList<dynamic>();
                
            return appointmentsWithRecords;
        }

        /// <summary>
        /// Gets patient information by ID
        /// </summary>
        /// <param name="patientId">The patient ID</param>
        /// <returns>Basic patient information</returns>
        public dynamic GetPatientInfoForAudiogram(int patientId)
        {
            var patient = _context.Patients
                .Include(p => p.User)
                .FirstOrDefault(p => p.PatientID == patientId);
                
            if (patient == null)
                return null;
                
            int age = DateTime.Now.Year - patient.DateOfBirth.Year;
            if (DateTime.Now.DayOfYear < patient.DateOfBirth.DayOfYear)
                age--;
                
            return new
            {
                PatientName = $"{patient.User.FirstName} {patient.User.LastName}",
                Age = age,
                DateOfBirth = patient.DateOfBirth
            };
        }

        /// <summary>
        /// Gets all hearing tests for a medical record
        /// </summary>
        /// <param name="medicalRecordId">The medical record ID</param>
        /// <returns>List of hearing tests</returns>
        public List<HearingTest> GetHearingTestsForMedicalRecord(int medicalRecordId)
        {
            return _context.HearingTests
                .Where(t => t.RecordID == medicalRecordId)
                .OrderByDescending(t => t.TestDate)
                .ToList();
        }

        /// <summary>
        /// Gets all audiogram data for a hearing test
        /// </summary>
        /// <param name="testId">The hearing test ID</param>
        /// <returns>List of audiogram data points</returns>
        public List<AudiogramData> GetAudiogramDataForTest(int testId)
        {
            return _context.AudiogramData
                .Where(a => a.TestID == testId)
                .ToList();
        }

        /// <summary>
        /// Gets diagnosis information from a medical record
        /// </summary>
        /// <param name="medicalRecordId">The medical record ID</param>
        /// <returns>Diagnosis information or null if not available</returns>
        public string GetDiagnosisForMedicalRecord(int medicalRecordId)
        {
            var record = _context.MedicalRecords
                .FirstOrDefault(r => r.RecordID == medicalRecordId);
                
            return record?.Diagnosis;
        }

        /// <summary>
        /// Updates hearing test notes
        /// </summary>
        /// <param name="testId">The hearing test ID</param>
        /// <param name="notes">The new notes</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool UpdateHearingTestNotes(int testId, string notes)
        {
            var test = _context.HearingTests.Find(testId);
            if (test == null)
                return false;
                
            test.TestNotes = notes;
            _context.SaveChanges();
            return true;
        }

        #endregion

        #region Hearing Test Repository Methods

        /// <summary>
        /// Gets all confirmed appointments for an audiologist on a specific date
        /// </summary>
        /// <param name="audiologistId">The audiologist ID</param>
        /// <param name="date">The appointment date</param>
        /// <returns>List of confirmed appointments</returns>
        public List<dynamic> GetAudiologistAppointmentsForDate(int audiologistId, DateTime date)
        {
            DateTime startOfDay = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            DateTime endOfDay = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            
            var appointments = _context.Appointments
                .Include(a => a.Patient.User)
                .Where(a => a.AudiologistID == audiologistId &&
                          a.Date >= startOfDay && a.Date <= endOfDay &&
                          a.Status == "Confirmed")
                .OrderBy(a => a.Date)
                .ToList();
            
            return appointments.Select(a => new
            {
                AppointmentId = a.AppointmentID,
                PatientId = a.PatientID,
                PatientName = $"{a.Patient.User.FirstName} {a.Patient.User.LastName}",
                AppointmentTime = a.Date,
                Purpose = a.PurposeOfVisit
            }).ToList<dynamic>();
        }

        /// <summary>
        /// Gets all hearing tests for a specific patient
        /// </summary>
        /// <param name="patientId">The patient ID</param>
        /// <returns>List of hearing tests with their diagnoses</returns>
        public List<dynamic> GetPatientHearingTestHistory(int patientId)
        {
            var result = new List<dynamic>();
            
            // Get medical records for this patient
            var records = _context.MedicalRecords
                .Where(r => r.PatientID == patientId)
                .OrderByDescending(r => r.RecordDate)
                .ToList();
                
            foreach (var record in records)
            {
                // Get hearing tests for each medical record
                var tests = _context.HearingTests
                    .Where(t => t.RecordID == record.RecordID)
                    .OrderByDescending(t => t.TestDate)
                    .ToList();
                    
                foreach (var test in tests)
                {
                    result.Add(new
                    {
                        TestId = test.TestID,
                        TestDate = test.TestDate,
                        TestType = test.TestType,
                        Diagnosis = record.Diagnosis
                    });
                }
            }
            
            return result;
        }

        /// <summary>
        /// Creates a new hearing test with audiogram data
        /// </summary>
        /// <param name="patientId">Patient ID</param>
        /// <param name="appointmentId">Optional appointment ID (0 if not associated with appointment)</param>
        /// <param name="audiologistId">Audiologist ID who created the test</param>
        /// <param name="testType">Type of hearing test</param>
        /// <param name="chiefComplaint">Patient's chief complaints</param>
        /// <param name="audiogramData">Collection of audiogram data points</param>
        /// <returns>The ID of the newly created hearing test</returns>
        public int CreateHearingTest(
            int patientId, 
            int appointmentId, 
            int audiologistId, 
            string testType, 
            string chiefComplaint,
            List<AudiogramData> audiogramData)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Find or create medical record
                    MedicalRecord medicalRecord = null;
                    
                    if (appointmentId > 0)
                    {
                        medicalRecord = _context.MedicalRecords
                            .FirstOrDefault(mr => mr.AppointmentID == appointmentId);
                    }
                    
                    if (medicalRecord == null)
                    {
                        // Create a new medical record
                        medicalRecord = new MedicalRecord
                        {
                            PatientID = patientId,
                            AppointmentID = appointmentId > 0 ? appointmentId : 0,
                            CreatedBy = audiologistId,
                            ChiefComplaint = chiefComplaint,
                            Diagnosis = GetDiagnosisFromAudiogramData(audiogramData),
                            RecordDate = DateTime.Now
                        };
                        
                        _context.MedicalRecords.Add(medicalRecord);
                        _context.SaveChanges(); // Save to get record ID
                    }
                    else
                    {
                        // Update existing record
                        medicalRecord.ChiefComplaint = chiefComplaint;
                        medicalRecord.Diagnosis = GetDiagnosisFromAudiogramData(audiogramData);
                    }
                    
                    // Create hearing test
                    var hearingTest = new HearingTest
                    {
                        RecordID = medicalRecord.RecordID,
                        TestType = testType,
                        TestDate = DateTime.Now
                    };
                    
                    _context.HearingTests.Add(hearingTest);
                    _context.SaveChanges(); // Save to get test ID
                    
                    // Add audiogram data points
                    foreach (var dataPoint in audiogramData)
                    {
                        dataPoint.TestID = hearingTest.TestID;
                        _context.AudiogramData.Add(dataPoint);
                    }
                    
                    _context.SaveChanges();
                    transaction.Commit();
                    
                    return hearingTest.TestID;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Helper method to determine hearing loss severity
        /// </summary>
        private string GetDiagnosisFromAudiogramData(List<AudiogramData> audiogramData)
        {
            // Calculate average thresholds for each ear
            int rightEarCount = 0;
            int leftEarCount = 0;
            double rightEarAvg = 0;
            double leftEarAvg = 0;
            
            foreach (var dataPoint in audiogramData)
            {
                if (dataPoint.Ear.ToLower() == "right")
                {
                    rightEarAvg += dataPoint.Threshold;
                    rightEarCount++;
                }
                else if (dataPoint.Ear.ToLower() == "left")
                {
                    leftEarAvg += dataPoint.Threshold;
                    leftEarCount++;
                }
            }
            
            // Calculate average thresholds
            if (rightEarCount > 0) rightEarAvg /= rightEarCount;
            if (leftEarCount > 0) leftEarAvg /= leftEarCount;
            
            // Determine hearing loss severity
            string rightEarDiagnosis = GetHearingLossSeverity(rightEarAvg);
            string leftEarDiagnosis = GetHearingLossSeverity(leftEarAvg);
            
            return $"Right ear: {rightEarDiagnosis}. Left ear: {leftEarDiagnosis}.";
        }

        /// <summary>
        /// Helper method to determine hearing loss severity based on threshold
        /// </summary>
        private string GetHearingLossSeverity(double avgThreshold)
        {
            if (avgThreshold < 20) return "Normal hearing";
            if (avgThreshold < 40) return "Mild hearing loss";
            if (avgThreshold < 60) return "Moderate hearing loss";
            if (avgThreshold < 80) return "Severe hearing loss";
            return "Profound hearing loss";
        }

        #endregion

        #region Audiologist Appointment Completion Methods

        /// <summary>
        /// Gets audiologist's confirmed appointments with medical records for a specific date
        /// </summary>
        /// <param name="audiologistId">The audiologist ID</param>
        /// <param name="date">The appointment date</param>
        /// <returns>List of appointments eligible for completion</returns>
        public List<dynamic> GetConfirmableAppointmentsForDate(int audiologistId, DateTime date)
        {
            DateTime startOfDay = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            DateTime endOfDay = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            
            // Get appointments for the specified date with medical records
            var result = new List<dynamic>();
            
            var appointments = _context.Appointments
                .Include(a => a.Patient.User)
                .Where(a => a.AudiologistID == audiologistId &&
                           a.Date >= startOfDay && a.Date <= endOfDay &&
                           a.Status == "Confirmed")
                .ToList();
            
            foreach (var appointment in appointments)
            {
                // Check if this appointment has medical records with at least one hearing test
                var medicalRecord = _context.MedicalRecords
                    .FirstOrDefault(mr => mr.AppointmentID == appointment.AppointmentID);
                
                if (medicalRecord != null)
                {
                    var hasHearingTest = _context.HearingTests
                        .Any(ht => ht.RecordID == medicalRecord.RecordID);
                        
                    if (hasHearingTest)
                    {
                        result.Add(new
                        {
                            AppointmentId = appointment.AppointmentID,
                            PatientId = appointment.PatientID,
                            MedicalRecordId = medicalRecord.RecordID,
                            PatientName = $"{appointment.Patient.User.FirstName} {appointment.Patient.User.LastName}",
                            AppointmentTime = appointment.Date,
                            Purpose = appointment.PurposeOfVisit
                        });
                    }
                }
            }
            
            return result;
        }

        /// <summary>
        /// Gets appointment details by ID
        /// </summary>
        /// <param name="appointmentId">The appointment ID</param>
        /// <returns>Appointment details or null if not found</returns>
        public Appointment GetCompletableAppointmentById(int appointmentId)
        {
            return _context.Appointments
                .Include(a => a.Patient.User)
                .Include(a => a.Audiologist.User)
                .FirstOrDefault(a => a.AppointmentID == appointmentId);
        }

        /// <summary>
        /// Gets all hearing aid products for prescriptions
        /// </summary>
        /// <returns>List of hearing aid products</returns>
        public List<Product> GetHearingAidProductsForPrescription()
        {
            return _context.Products
                .OrderBy(p => p.Manufacturer)
                .ThenBy(p => p.Model)
                .ToList();
        }

        /// <summary>
        /// Marks an appointment as completed and creates a prescription if needed
        /// </summary>
        /// <param name="appointmentId">The appointment ID</param>
        /// <param name="audiologistId">The audiologist ID</param>
        /// <param name="productId">Optional product ID for prescription (null if no prescription)</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool CompleteAppointmentWithPrescription(int appointmentId, int audiologistId, int? productId = null)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    // Get the appointment
                    var appointment = _context.Appointments.Find(appointmentId);
                    if (appointment == null || appointment.Status != "Confirmed")
                        return false;
                        
                    // Update appointment status
                    appointment.Status = "Completed";
                    
                    // If a product is specified, create a prescription
                    if (productId.HasValue)
                    {
                        var prescription = new Prescription
                        {
                            AppointmentID = appointmentId,
                            PrescribedBy = audiologistId,
                            PrescribedDate = DateTime.Now,
                            ProductID = productId.Value
                        };
                        
                        _context.Prescriptions.Add(prescription);
                    }
                    
                    _context.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        #endregion

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}