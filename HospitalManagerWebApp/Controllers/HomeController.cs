using HospitalManagerWebApp.Data;
using HospitalManagerWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HospitalManagerWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult PerformBooking()
        {
            var doctors = from s in _context.Doctors.Include(m => m.Department)
                          select new { s.DoctorID, DoctorName = s.DoctorName + " (" + s.Department.DepartmentName + ")" };
            ViewData["DoctorID"] = new SelectList(doctors, "DoctorID", "DoctorName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PerformBooking([Bind("BookingID,DoctorID,BookingDate,ContactNo")] Booking booking)
        {
            ModelState.Remove("UserName");
            if (ModelState.IsValid)
            {
                var schedule = await _context.Schedules
                .Include(s => s.Doctor).OrderByDescending(s => s.ScheduleID)
                .FirstOrDefaultAsync(m => m.DoctorID == booking.DoctorID);
                if(schedule==null)
                {
                    ModelState.AddModelError("DoctorID", "This Doctor has not any schedule.");
                }
                else
                {
                    string dayname = booking.BookingDate.DayOfWeek.ToString();
                    if(schedule.OPDDays.Contains(dayname))
                    {
                        if(booking.BookingDate > DateTime.Now)
                        {
                            booking.UserName = _userManager.GetUserName(this.User);
                            _context.Add(booking);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(BookingHistory));
                        }
                        else
                        {
                            ModelState.AddModelError("BookingDate", "Booking Date must be greater than current date");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("DoctorID", "This Doctor is not available on selected Date. Available Days are " + schedule.OPDDays);
                    }
                }                
            }
            var doctors = from s in _context.Doctors.Include(m => m.Department)
                          select new { s.DoctorID, DoctorName = s.DoctorName + " (" + s.Department.DepartmentName + ")" };
            ViewData["DoctorID"] = new SelectList(doctors, "DoctorID", "DoctorName", booking.DoctorID);
            return View(booking);
        }

        public IActionResult BookingHistory()
        {
            string userid = _userManager.GetUserName(this.User);
            var bookings = _context.Bookings.Include(s => s.Doctor).Where(s => s.UserName == userid);
            return View(bookings);
        }

        public IActionResult AllSchedules()
        {
            var schedules = _context.Schedules.Include(s => s.Doctor);
            return View(schedules);
        }

        [Authorize(Roles ="doctor")]
        public async Task<IActionResult> MyUpcomingBooking()
        {
            string userid = _userManager.GetUserName(this.User);
            var doctor = await _context.Doctors
                .Include(j => j.Department)
                .FirstOrDefaultAsync(m => m.UserName == userid);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["DoctorName"] = doctor.DoctorName;
            ViewData["DepartmentName"] = doctor.Department.DepartmentName;
            var bookings = _context.Bookings.Include(s => s.Doctor)
                .Where(s => s.DoctorID == doctor.DoctorID);
            return View(bookings);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
