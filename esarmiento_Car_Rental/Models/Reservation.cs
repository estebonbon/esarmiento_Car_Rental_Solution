using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace esarmiento_Car_Rental.Models
{
    //public enum ReservationStatus 
    //    //Created an enum for reservation status
    //{
    //    Pending,
    //    Confirmed,
    //    Completed,
    //    Cancelled
    //}

    public class Reservation : IValidatableObject
    {
        [Display(Name = "Pick Up Date")]
        public string PickUpDateSummary => PickUpDate?.ToString("dddd MMMM d, yyyy") ?? "";

        [Display(Name = "Return Date")]
        public string ReturnDateSummary => ReturnDate?.ToString("dddd MMMM d, yyyy") ?? ""; //?? "" if there is not date give an empts string

        public int ID { get; set; }

        
        [Required(ErrorMessage = "Reservation number cannot be left blank")]
        [StringLength(20, ErrorMessage = "Reservation number cannot be more than 20 characters long.")]
        public string ReservationNumber { get; set; } = ""; //Unique constraint somehow

        [Required(ErrorMessage = "Pickup date is required.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? PickUpDate { get; set; }

        [Required(ErrorMessage = "Return date is required.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? ReturnDate { get; set; }

        [Required(ErrorMessage = "You must select a Reservation Status")]
        public Status? Status { get; set; }

        [Required(ErrorMessage = "You must select an Employee ID")]
        [Display(Name = "Employee ID")]
        public int? EmployeeID { get; set; }

        public Employee? Employee { get; set; } // The ? mark here does not mean it does not exsit it means that if employee it didnt load right away it doesnt crash the program

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (PickUpDate == null || ReturnDate == null)
            {
                yield break; //Incase the values are null
            }

            //Reservation date must be later than the pickup date
            if (PickUpDate.Value.AddDays(1) > ReturnDate) //.AddDays helps add a day numerically
            {
                yield return new ValidationResult("The return date must be minimum one day after the pickup date");
            }

            if(PickUpDate.Value < DateTime.Today) //The .Value gives that actual datetime stored after it has been validated
            {
                yield return new ValidationResult("The pickup date must be in the future");
            }

            if((ReturnDate.Value - PickUpDate.Value).TotalDays > 30) //The total days converts it into a numerical value
            {
                yield return new ValidationResult("The reservation peroid must be 30 days or less");
            }
            
        }
    }

}

