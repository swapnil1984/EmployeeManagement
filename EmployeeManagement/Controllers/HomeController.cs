using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    // Home Controller has a dependency on IEmployeeRepository 
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IWebHostEnvironment webHostEnvironment;

        // Here we are injecting IEmployeeRepository into the HomeController by using this constructor
        // Then we are using _employeeRepository, an injected dependency for different operations shown below
        public HomeController(IEmployeeRepository employeeRepository, IWebHostEnvironment webHostEnvironment)
        {
            this.employeeRepository = employeeRepository;
            this.webHostEnvironment = webHostEnvironment;
        }

        public ViewResult Index()
        {
            var model = employeeRepository.GetAllEmployees();
            return View(model);
        }

        public ViewResult Details(int? id)
        {
            throw new Exception("Error in Details View");

            Employee employee = employeeRepository.GetEmployee(id.Value);

            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
                PageTitle = "Employee Details"
            };
            
            return View(homeDetailsViewModel);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = employeeRepository.GetEmployee(id);

            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel 
            { 
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = employeeRepository.GetEmployee(model.Id);

                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;

                if (model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }

                    employee.PhotoPath = processUploadedFile(model);
                }

                employeeRepository.Update(employee);
                return RedirectToAction("index");
            }
            return View();
        }

        private string processUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;

            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
                
            }

            return uniqueFileName;
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = processUploadedFile(model);

                Employee newEmployee = new Employee {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };

                employeeRepository.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }
            return View();
        }
    }
}
