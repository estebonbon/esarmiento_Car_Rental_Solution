using System.ComponentModel.DataAnnotations;

namespace esarmiento_Car_Rental.Models
{
    public class Employee
    {
        [Display(Name = "Employee")]

        public string Summary
        {
          get
            {
                return char.ToUpper(FirstName[0]) + FirstName.Substring(1)
                + " "
                + char.ToUpper(LastName[0]) + LastName.Substring(1);
            }
            
        }

        [Display(Name = "Phone")]
        public string PhoneFormatted
        {
            get //Prevents it from crashing when a new phone is being entered
            {
                if (string.IsNullOrEmpty(Phone) || Phone.Length != 10)
                {
                    return Phone ?? "";
                }
                return "(" + Phone?.Substring(0, 3) + ") " + Phone?.Substring(3, 3) + "-" + Phone?[6..];
            }
        } 
        public int ID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; } = "";

        [Display(Name = "Middle Name")]
        [StringLength(50, ErrorMessage = "Middle name cannot be more than 50 characters long.")]
        public string? MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(75, ErrorMessage = "Last name cannot be more than 75 characters long.")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[2-9]\d{2}[2-9]\d{6}$", ErrorMessage = "Enter a valid 10-digit phone number.")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(10)]
        public string Phone { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")] //Unique Constraint not applied yet
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = String.Empty; 

        [Required(ErrorMessage = "Hire date must be entered")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? HireDate { get; set; } //The question mark allows it to be nullable which will trigger my required message and not the systems

        public bool IsActive { get; set; } = true;
        public ICollection<Reservation> Reservations { get; set; } = new HashSet<Reservation>(); // Creates a new one right away
    }
}
