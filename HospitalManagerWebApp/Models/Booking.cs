using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagerWebApp.Models
{
    public class Booking
    {
        [Key]
        public int BookingID { get; set; }

        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        public int DoctorID { get; set; }

        [ForeignKey("DoctorID")]
        [InverseProperty("DoctorBookings")]
        public Doctor Doctor { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required]
        public string   ContactNo { get; set; }
    }
}
