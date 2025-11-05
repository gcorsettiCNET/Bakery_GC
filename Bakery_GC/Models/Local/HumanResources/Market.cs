using Bakery_GC.Models.Local.ObjectToSell;

namespace Bakery_GC.Models.Local.HumanResources
{
    public class Market
    {
        public Guid Id { get; set; } // Unique identifier for the market  
        public required string Name { get; set; } // Name of the market  
        public required string Address { get; set; } // Address of the market  
        public required string City { get; set; } // City where the market is located  
        public required string State { get; set; } // State where the market is located  
        public required string ZipCode { get; set; } // Zip code of the market  
        public required string PhoneNumber { get; set; } // Contact phone number  
        public required string Email { get; set; } // Contact email address  
        public bool IsOpen { get; set; } // Indicates if the market is currently open  
        public TimeSpan OpeningTime { get; set; } // Opening time of the market  
        public TimeSpan ClosingTime { get; set; } // Closing time of the market  
        public required List<Product> Products { get; set; } // List of products available in the market  

    }
}