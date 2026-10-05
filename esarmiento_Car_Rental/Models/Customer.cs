using System.ComponentModel.DataAnnotations;

namespace esarmiento_Car_Rental.Models
{
    public class Customer : IValidatableObject
    {
        [Display(Name = "Customer")]

        public string Summary
        {
            get
            {
                return FirstName
                    + (string.IsNullOrEmpty(MiddleName) ? " " :
                    (" " + (char?)MiddleName[0] + ". ").ToUpper())
                    + LastName;
            }
        }

        public string? Age
        {
            get
            {
                if (DOB == null)
                {
                    return null;
                }
                DateTime today = DateTime.Today;
                int? a = today.Year - DOB.Value.Year - ((today.Month < DOB.Value.Month) || (today.Month == DOB.Value.Month && today.Day < DOB.Value.Day) ? 1 : 0); //If true subtract 1, if not subtract 0
                return a?.ToString();
            }
        }

        [Display(Name = "Age (DOB)")]
        //public string AgeSummary => Age + " (" + DOB.Value.ToString("yyyy-MM-dd") + ")"; old code would break
        public string AgeSummary => (DOB == null)
            ? "Unknown"
            : Age + " (" + DOB.Value.ToString("yyyy-MM-dd") + ")";

        [Display(Name = "Phone")]
        public string PhoneFormatted => "(" + Phone?.Substring(0, 3) + ") " + Phone?.Substring(3, 3) + "-" + Phone?[6..];
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

        [Required(ErrorMessage = "Drivers Licsence cannot be left blank")]
        [StringLength(15, ErrorMessage = "Maximun 15 characters")] // Unique
        public string DriverLicense { get; set; } = "";


        [Required(ErrorMessage = "Birth date must be entered")]
        [DataType(DataType.Date)]// This helps display the birthday already stored in the database
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }

        [Required(ErrorMessage = "You must select a Membership Type ID")]
        [Display(Name = "Membership Type ID")]
        public int? MembershipTypeID { get; set; }
        public MembershipType? MembershipType {  get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DOB == null)
            {
                yield break;
            } // This makes sure it exsist first

            // Check for validation customer must be 19 years old, and date of birth not in the future

            if (DOB.Value > DateTime.Today) 
            { 
                yield return new ValidationResult("Date of Birth cannot be in the future.", ["DOB"]); 
            }


            DateTime today = DateTime.Today;

            int age = today.Year - DOB.Value.Year
                - ((today.Month < DOB.Value.Month ||
                   (today.Month == DOB.Value.Month && today.Day < DOB.Value.Day)) ? 1 : 0);

            if (age < 19)
            {
                yield return new ValidationResult(
                    "Customer must be at least 19 years old.",
                    ["DOB"]);
            }
        }
    }
}
