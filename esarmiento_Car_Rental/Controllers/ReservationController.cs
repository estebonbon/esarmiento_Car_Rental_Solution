
using esarmiento_Car_Rental.Data;
using esarmiento_Car_Rental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class ReservationController : Controller
{
    private readonly CarRentalContext _context;

    public ReservationController(CarRentalContext context)
    {
        _context = context;
    }

    // GET: RESERVATIONS
    public async Task<IActionResult> Index()    
    {
        var reservation = _context.Reservations
            .Include(r => r.Employee);
        return View(await reservation.ToListAsync());
    }

    // GET: RESERVATIONS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations
            .Include(r => r.Employee)
            .FirstOrDefaultAsync(m => m.ID == id);
        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }

    // GET: RESERVATIONS/Create
    public IActionResult Create()
    {
        PopulateDropDownLists();
        return View();
    }

    // POST: RESERVATIONS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,ReservationNumber,PickUpDate,ReturnDate,Status,EmployeeID")] Reservation reservation)
    {
        //Step 1 remove theh employee object
        //Step 2 put everything into a try catch

        try
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDropDownLists(reservation); //check this line
            return View(reservation);
        }
        catch (DbUpdateException patatoe) //Write a constraint for the email
        {
            if (patatoe.GetBaseException().Message.Contains("UNIQUE constraint failed: Reservations.ReservationNumber"))
            {
                ModelState.AddModelError("ReservationNumber", "Unable to save changes, you cannot have duplicate reservation numbers.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, or contact your system administrator.");
            }
        }
        PopulateDropDownLists(reservation);
        return View(reservation);
    }

    // GET: RESERVATIONS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations.FindAsync(id);
        if (reservation == null)
        {
            return NotFound();
        }
        PopulateDropDownLists(reservation); // check this line too
        return View(reservation);
    }

    // POST: RESERVATIONS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id)
    {
        //Find the correct reservation to edit
        var reservationToUpdate = await _context.Reservations.FirstOrDefaultAsync(r => r.ID == id);

        if (reservationToUpdate == null)
        {
            return NotFound();
        }

        if (await TryUpdateModelAsync<Reservation>(reservationToUpdate, "",
            r => r.ReservationNumber,
            r => r.PickUpDate,
            r => r.ReturnDate,
            r => r.Status,
            r => r.EmployeeID)) 
        {
            try
            {
                //_context.Update(reservationToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(reservationToUpdate.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException patatoe) //Write a constraint for the email
            {
                if (patatoe.GetBaseException().Message.Contains("UNIQUE constraint failed: Reservations.ReservationNumber"))
                {
                    ModelState.AddModelError("ReservationNumber", "Unable to save changes, you cannot have duplicate reservation numbers.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, or contact your system administrator.");
                }
            }
            PopulateDropDownLists(reservationToUpdate);
            return View(reservationToUpdate);

        }
        PopulateDropDownLists(reservationToUpdate);
        return View(reservationToUpdate);
    }

    // GET: RESERVATIONS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _context.Reservations
            .Include(r => r.Employee)
            .FirstOrDefaultAsync(m => m.ID == id);
        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }

    // POST: RESERVATIONS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.Employee)
            .FirstOrDefaultAsync(m => m.ID == id); 

        try
        {
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        catch (Exception)
        {
            //This is incase anything funny happens and you cant delete the record
            ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
            // After this step update the deletecshtml
        }
        return View(reservation);
    }

    private bool ReservationExists(int? id)
    {
        return _context.Reservations.Any(e => e.ID == id);
    }

    private void PopulateDropDownLists(Reservation? reservation = null)
    {
        var dQueryEmp = from e in _context.Employees
                     orderby e.LastName, e.FirstName
                     select e;

        ViewData["EmployeeID"] = new SelectList(
            dQueryEmp,
            "ID",
            "Summary",
            reservation?.EmployeeID); //If there is already an exsisting employee for this reservation use it

        // This section is for the Status Enum
        ViewData["Status"] = new SelectList(
            Enum.GetValues<Status>(), // Give me every possible value found in the status enum
            reservation?.Status);

    }


}

