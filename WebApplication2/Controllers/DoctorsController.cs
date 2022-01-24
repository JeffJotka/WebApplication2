using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Doctors
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            var applicationDbContext = _context.Doctors.Include(d => d.Specialization);
            ViewData["CurrentSort"] = sortOrder;
            ViewBag.FNameSortParm = String.IsNullOrEmpty(sortOrder) ? "FName_Desc" : "";
            ViewBag.LNameSortParm = sortOrder == "LName_Asc" ? "LName_Desc" : "LName_Asc";
            ViewBag.AddrSortParm = sortOrder == "Address_Asc" ? "Address_Desc" : "Address_Asc";
            ViewBag.PhoneNumSortParm = sortOrder == "PhoneNum_Asc" ? "PhoneNum_Desc" : "PhoneNum_Asc";
            ViewBag.NoteSortParm = sortOrder == "Note_Asc" ? "Note_Desc" : "Note_Asc";
            ViewBag.MonSalarySortParm = sortOrder == "MonSalary_Asc" ? "MonSalary_Desc" : "MonSalary_Asc";
            ViewBag.EmailSortParm = sortOrder == "Email_Asc" ? "Email_Desc" : "Email_Asc";
            ViewBag.SpecSortParm = sortOrder == "Spec_Asc" ? "Spec_Desc" : "Spec_Asc";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;
            var doctors = from p in applicationDbContext select p;
            if (!String.IsNullOrEmpty(searchString))
            {
                doctors = doctors.Where(s => (s.FirstName + ' ' + s.LastName).Contains(searchString));
            }
            switch (sortOrder)
            {
                case "FName_Desc":
                    doctors = doctors.OrderByDescending(p => p.FirstName);
                    break;
                case "LName_Asc":
                    doctors = doctors.OrderBy(p => p.LastName);
                    break;
                case "LName_Desc":
                    doctors = doctors.OrderByDescending(p => p.LastName);
                    break;
                case "Address_Asc":
                    doctors = doctors.OrderBy(p => p.Address);
                    break;
                case "Address_Desc":
                    doctors = doctors.OrderByDescending(p => p.Address);
                    break;
                case "PhoneNum_Asc":
                    doctors = doctors.OrderBy(p => p.PhoneNumber);
                    break;
                case "PhoneNum_Desc":
                    doctors = doctors.OrderByDescending(p => p.PhoneNumber);
                    break;
                case "Note_Asc":
                    doctors = doctors.OrderBy(p => p.Notes);
                    break;
                case "Note_Desc":
                    doctors = doctors.OrderByDescending(p => p.Notes);
                    break;
                case "MonSalary_Asc":
                    doctors = doctors.OrderBy(p => p.MonthlySalary);
                    break;
                case "MonSalary_Desc":
                    doctors = doctors.OrderByDescending(p => p.MonthlySalary);
                    break;
                case "Email_Asc":
                    doctors = doctors.OrderBy(p => p.Email);
                    break;
                case "Email_Desc":
                    doctors = doctors.OrderByDescending(p => p.Email);
                    break;
                case "Spec_Asc":
                    doctors = doctors.OrderBy(p => p.Specialization);
                    break;
                case "Spec_Desc":
                    doctors = doctors.OrderByDescending(p => p.Specialization);
                    break;
                default:
                    doctors = doctors.OrderBy(p => p.FirstName);
                    break;
            }
            int pageSize = 3;
            return View(await PaginatedList<Doctor>.CreateAsync(doctors.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            ViewData["SpecializationId"] = new SelectList(_context.Specializations, "Id", "SpecializationName");
            return View();
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Address,Notes,MonthlySalary,PhoneNumber,Email,SpecializationId")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecializationId"] = new SelectList(_context.Specializations, "Id", "SpecializationName", doctor.SpecializationId);
            return View(doctor);
        }

        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["SpecializationId"] = new SelectList(_context.Specializations, "Id", "SpecializationName", doctor.SpecializationId);
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,FirstName,LastName,Address,Notes,MonthlySalary,PhoneNumber,Email,SpecializationId")] Doctor doctor)
        {
            if (id != doctor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecializationId"] = new SelectList(_context.Specializations, "Id", "SpecializationName", doctor.SpecializationId);
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Specialization)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(long id)
        {
            return _context.Doctors.Any(e => e.Id == id);
        }
    }
}
