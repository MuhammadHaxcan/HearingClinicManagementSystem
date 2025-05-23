using System.Data.Entity;

namespace HearingClinicManagementSystem.Data
{
    public class HearingClinicDbInitializer : CreateDatabaseIfNotExists<HearingClinicDbContext>
    {
        protected override void Seed(HearingClinicDbContext context)
        {
            // The actual seeding will be handled by HearingClinicRepository.SeedInitialData
            base.Seed(context);
        }
    }
}