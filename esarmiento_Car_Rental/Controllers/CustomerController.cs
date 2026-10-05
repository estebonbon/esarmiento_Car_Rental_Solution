
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using esarmiento_Car_Rental.Models;
using esarmiento_Car_Rental.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

public class CustomerController : Controller
{
    private readonly CarRentalContext _context;

    public CustomerController(CarRentalContext context)
    {
        _context = context;
    }

    // GET: CUSTOMERS
    public async Task<IActionResult> Index()    
    {
        var customer = _context.Customers
            .Include(c => c.MembershipType) //This adds membershiptype to the customers context
            .AsNoTracking(); 

        return View(await customer.ToListAsync());
    }

    // GET: CUSTOMERS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var customer = await _context.Customers
            .Include(c => c.MembershipType)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == id);
        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    // GET: CUSTOMERS/Create
    public IActionResult Create()
    {
        PopulateDropDownlists();
        return View();
    }

    // POST: CUSTOMERS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,FirstName,MiddleName,LastName,Phone,Email,DriverLicense,DOB,MembershipTypeID")] Customer customer)
    {

        //Step 1 remove the membershipType<Object>
        //Step 2 put everything into a try catch
        try
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        catch (DbUpdateException patatoe)
        {
            if(patatoe.GetBaseException().Message.Contains("UNIQUE constraint failed: Customers.DriverLicense"))
            {
                ModelState.AddModelError("DriverLicense", "Unable to save changes, you cannot have duplicate drivers license.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, or contact your system administrator.");
            }
        }
        
        PopulateDropDownlists(customer); // If everything fails return them to the create page!!
        return View(customer);
    }

    // GET: CUSTOMERS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var customer = await _context.Customers.FindAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        PopulateDropDownlists(customer);
        return View(customer);
    }

    // POST: CUSTOMERS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    //public async Task<IActionResult> Edit(int? id, [Bind("ID,FirstName,MiddleName,LastName,Phone,Email,DriverLicense,DOB,MembershipTypeId,MembershipType")] Customer customer)
    //this is the old bind method that TryUpdateModelAsync will replace

    public async Task<IActionResult> Edit(int id)
    {
        //This tells the program which customer to get and update
        var customerToUpdate = await _context.Customers.FirstOrDefaultAsync(c  => c.ID == id); //Make sure to use the using entity framework core here

        //Make sure we found the customer record or exit with an error
        if(customerToUpdate == null)
        {
            return NotFound();
        }

        //Below is the TryUpdateModelAsync from step 3, and customer is the object, customerToUpdate represents the variable name of the customer we are trying to edit
        if(await TryUpdateModelAsync<Customer>(customerToUpdate, "", 
            c => c.FirstName, 
            c => c.MiddleName, 
            c => c.LastName, 
            c => c.Phone, 
            c => c.Email, 
            c => c.DriverLicense, 
            c => c.DOB, 
            c => c.MembershipTypeID))
        {
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Only when things succed
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(customerToUpdate.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException patatoe) // This catch was added 
            {
                if (patatoe.GetBaseException().Message.Contains("UNIQUE constraint failed: Customers.DriversLicense"))
                {
                    ModelState.AddModelError("DriverLicense", "Unable to save changes, you cannot have duplicate drivers license.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, or contact your system administrator.");
                }
            }

        }
        PopulateDropDownlists(customerToUpdate);
        return View(customerToUpdate);
    }

    // GET: CUSTOMERS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var customer = await _context.Customers
            .Include(c => c.MembershipType)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.ID == id);
        if (customer == null)
        {
            return NotFound();
        }


        return View(customer);
    }

    // POST: CUSTOMERS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        //var customer = await _context.Customers.FindAsync(id); The findAsync method does not include the membership type which is why we have to use .Include

        var customer = await _context.Customers 
            .Include(c => c.MembershipType)
            .FirstOrDefaultAsync(c => c.ID == id); // For this object customer, use the c.ID to find a matching ID in the Customers table

        try
        {
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        catch (DbUpdateException)
        {
            //This is incase anything funny happens and you cant delete the record
            ModelState.AddModelError("", "Unable to delete record. Try again, and if the problem persists see your system administrator.");
        }
        return View(customer); //If it fails return them to this exact page
    }

    private bool CustomerExists(int? id)
    {
        return _context.Customers.Any(e => e.ID == id);
    }

    // I may need to creet a list here for membership types

    private void PopulateDropDownlists(Customer? customer = null)
    {
        var dQuery = from m in _context.MembershipTypes
            orderby m.Name
            select m;

        ViewData["MembershipTypeID"] = new SelectList( 
            dQuery, 
            "ID", 
            "Name", 
            customer?.MembershipTypeID);
    }
}
