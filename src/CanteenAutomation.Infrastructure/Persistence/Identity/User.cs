// CanteenAutomation.Infrastructure/Persistence/Identity/User.cs
using Microsoft.AspNetCore.Identity; // This using is now appropriate here

namespace CanteenAutomation.Infrastructure.Persistence.Identity
{
    // Our custom User entity, inheriting from IdentityUser
    // This now lives in the Infrastructure layer, which is allowed to depend on ASP.NET Core Identity.
    public class User : IdentityUser
    {
        // Add your custom properties here
        public string Role { get; set; } // Custom property for primary role

        // You can add other custom properties specific to your canteen system, e.g.:
        // public string FullName { get; set; }
        // public DateTime RegistrationDate { get; set; }
        // public decimal AccountBalance { get; set; }
        // public string ProfilePictureUrl { get; set; }
    }
}