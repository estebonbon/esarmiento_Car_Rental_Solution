using System.ComponentModel.DataAnnotations;

namespace esarmiento_Car_Rental.Models
{
    public class MembershipType
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "You cannot leave Membership type blank.")]
        [StringLength(50, ErrorMessage = "Membership type cannot be more than 50 characters long.")]
        public string Name { get; set; } = ""; // A new database table must be created for this


        [Required(ErrorMessage = "Discount is required.")]
        [Range(0.0, 0.5, ErrorMessage = "Discount must be between 0 and 50%")]
        public double Discount { get; set; }

        [StringLength(200, ErrorMessage = "Description cannot be greater than 200 characters")]

        public string? Description { get; set; }
        public ICollection<Customer> Customers { get; set; } = new HashSet<Customer>(); // Creates a new one right away
    }
}
