
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using esarmiento_Car_Rental.Models;
using esarmiento_Car_Rental.Data;

public class MembershipTypeController : Controller
{
    private readonly CarRentalContext _context;

    public MembershipTypeController(CarRentalContext context)
    {
        _context = context;
    }

    // GET: MEMBERSHIPTYPES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.MembershipTypes.ToListAsync());
    }

    // GET: MEMBERSHIPTYPES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var membershiptype = await _context.MembershipTypes
            .FirstOrDefaultAsync(m => m.ID == id);
        if (membershiptype == null)
        {
            return NotFound();
        }

        return View(membershiptype);
    }

    // GET: MEMBERSHIPTYPES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: MEMBERSHIPTYPES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,Name,Discount,Description")] MembershipType membershiptype)
    {
        //Step 1 remove the customers
        //Step 2 put everything in a try catch

        try
        {
            if (ModelState.IsValid)
            {
                _context.Add(membershiptype);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        catch (DbUpdateException) // There are no constraints for this table
        {
            ModelState.AddModelError("", "Unable to save changes. Try again, or contact your system administrator.");
        }
        
        return View(membershiptype);
    }

    // GET: MEMBERSHIPTYPES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var membershiptype = await _context.MembershipTypes.FindAsync(id);
        if (membershiptype == null)
        {
            return NotFound();
        }
        return View(membershiptype);
    }

    // POST: MEMBERSHIPTYPES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id) 
    {
        //find the reservation to update
        var membershipToUpdate = await _context.MembershipTypes.FirstOrDefaultAsync(m => m.ID == id);

        if (membershipToUpdate == null)
        {
            return NotFound();
        }

        if (await TryUpdateModelAsync<MembershipType>(membershipToUpdate, "",
            m => m.Name,
            m => m.Discount,
            m => m.Description))
        {
            try
            {
                //_context.Update(membershiptype); This update line has been relaced
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MembershipTypeExists(membershipToUpdate.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException) // There are no constraints for this table
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, or contact your system administrator.");
            }
            return View(membershipToUpdate); //This is for the try method succeeding and Database failing
        }
        return View(membershipToUpdate); // This is for the try method failing
    }

    // GET: MEMBERSHIPTYPES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var membershiptype = await _context.MembershipTypes
            .FirstOrDefaultAsync(m => m.ID == id);
        if (membershiptype == null)
        {
            return NotFound();
        }

        return View(membershiptype);
    }

    // POST: MEMBERSHIPTYPES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var membershiptype = await _context.MembershipTypes.FindAsync(id);

        try
        {
            if (membershiptype != null)
            {
                _context.MembershipTypes.Remove(membershiptype);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException patatoe) // This coded was added so it wouldnt let users delete en employee with reservations
        {
            if (patatoe.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
            {
                ModelState.AddModelError("",
                    "Unable to delete Membership type because they have existing customers.");
            }
            else
            {
                ModelState.AddModelError("",
                    "Unable to delete record. Try again, or contact your system administrator.");
            }
        }
        return View(membershiptype);
    }
        
    

    private bool MembershipTypeExists(int? id)
    {
        return _context.MembershipTypes.Any(e => e.ID == id);
    }
}
