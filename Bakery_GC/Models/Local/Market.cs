namespace Bakery_GC.Models.Local
{
    public class Market
    {
        public Guid Id { get; set; } // Unique identifier for the market  
        public string Name { get; set; } // Name of the market  
        public string Address { get; set; } // Address of the market  
        public string City { get; set; } // City where the market is located  
        public string State { get; set; } // State where the market is located  
        public string ZipCode { get; set; } // Zip code of the market  
        public string PhoneNumber { get; set; } // Contact phone number  
        public string Email { get; set; } // Contact email address  
        public bool IsOpen { get; set; } // Indicates if the market is currently open  
        public TimeSpan OpeningTime { get; set; } // Opening time of the market  
        public TimeSpan ClosingTime { get; set; } // Closing time of the market  
        public List<Product> Products { get; set; } // List of products available in the market  

    }
}