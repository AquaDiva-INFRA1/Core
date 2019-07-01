using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BExIS.Rbm.Entities.Booking;
using BExIS.Rbm.Services.Booking;

namespace BExIS.Modules.RBM.UI.Controllers
{
    public class AdminBookingEventController : Controller
    {
        // GET: AdminBookingEvent
        public ActionResult Index()
        {
            BookingEventManager bookingEventManager = new BookingEventManager();
            List<BookingEvent> listOfBooking = bookingEventManager.GetAllBookingEvents().ToList();

            return View(listOfBooking);
        }

        public ActionResult Validate(Int64 id, string status)
        {
            BookingEventManager bookingEventManager = new BookingEventManager();
            BookingEvent bookingEvent = bookingEventManager.GetBookingEventById(id);
            bookingEvent.Status = Int32.Parse(status);
            bookingEventManager.UpdateBookingEvent(bookingEvent);
            List<BookingEvent> listOfBooking = bookingEventManager.GetAllBookingEvents().ToList();
            return RedirectToAction("Index");
        }
    }
}
