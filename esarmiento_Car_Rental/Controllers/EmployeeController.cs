
using esarmiento_Car_Rental.Data;
using esarmiento_Car_Rental.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

public class EmployeeController : Controller
{
    private readonly CarRentalContext _context;

    public EmployeeController(CarRentalContext context)
    {
        _context = context;
    }

    // GET: EMPLOYEES
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Employees.ToListAsync());
    }

    // GET: EMPLOYEES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees
            .FirstOrDefaultAsync(m => m.ID == id);
        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // GET: EMPLOYEES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: EMPLOYEES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ID,FirstName,MiddleName,LastName,Phone,Email,HireDate,IsActive")] Employee employee)
    {

        //Step 1 remove reservations
        //Step 2 put everything in a try catch

        try
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        catch (DbUpdateException patatoe) //Write a constraint for the email
        {
            if (patatoe.GetBaseException().Message.Contains("UNIQUE constraint failed: Employees.Email"))
            {
                ModelState.AddModelError("Email", "Unable to save changes, you cannot have duplicate employee emials.");
            }
            else
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, or contact your system administrator.");
            }
        }
        return View(employee);
    }

    // GET: EMPLOYEES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound();
        }
        return View(employee);
    }

    // POST: EMPLOYEES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id) 
    {
        //Find the appropriate employee for the program
        var employeeToUpdate = await _context.Employees.FirstOrDefaultAsync(e => e.ID == id); //entity framework core must be here to

        //See if we can find the employee record
        if (employeeToUpdate == null)
        {
            return NotFound();
        }

        //TryToUpdateModelAsync replaces _context.Update
        if (await TryUpdateModelAsync<Employee>(employeeToUpdate, "",
            e => e.FirstName,
            e => e.MiddleName,
            e => e.LastName,
            e => e.Phone,
            e => e.Email,
            e => e.HireDate,
            e => e.IsActive)) 
        {
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!EmployeeExists(employeeToUpdate.ID))
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
                if (patatoe.GetBaseException().Message.Contains("UNIQUE constraint failed: Employees.Email"))
                {
                    ModelState.AddModelError("Email", "Unable to save changes, you cannot have duplicate employee emails.");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, or contact your system administrator.");
                }
            }
            return View(employeeToUpdate);

        }
        return View(employeeToUpdate);
    }

    // GET: EMPLOYEES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var employee = await _context.Employees
            .FirstOrDefaultAsync(m => m.ID == id);
        if (employee == null)
        {
            return NotFound();
        }

        return View(employee);
    }

    // POST: EMPLOYEES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var employee = await _context.Employees
            .FindAsync(id);

        try
        {
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException patatoe) // This coded was added so it wouldnt let users delete en employee with reservations
        {
            if (patatoe.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
            {
                ModelState.AddModelError("",
                    "Unable to delete employee because they have existing reservations.");
            }
            else
            {
                ModelState.AddModelError("",
                    "Unable to delete record. Try again, or contact your system administrator.");
            }
        }
        return View(employee);
    }
    private bool EmployeeExists(int? id)
    {
        return _context.Employees.Any(e => e.ID == id);
    }
}
