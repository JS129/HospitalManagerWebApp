using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagerWebApp.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorID { get; set; }

        [Required]
        [StringLength(100)]
        public string DoctorName { get; set; }

        [Required]
        [StringLength(50)]
        public string ExtName { get; set; }

        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        public int DepartmentID { get; set; }

        [ForeignKey("DepartmentID")]
        [InverseProperty("Doctors")]
        public Department Department { get; set; }

        [NotMapped]
        public PhotoUpload FileUpload { get; set; }

        public ICollection<Schedule> DoctorSchedules { get; set; }

        public ICollection<Booking> DoctorBookings { get; set; }
    }

    public class PhotoUpload
    {
        [Required]
        [Display(Name = "File")]
        public IFormFile FormFile { get; set; }
    }
}
