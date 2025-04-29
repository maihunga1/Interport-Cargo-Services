using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Interport_Cargo_Services.Views.User
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public string SuccessMessage { get; set; }

        public void OnGet()
        {
            // Initialize any data if needed
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Handle registration logic here (e.g., save user to database)
            // For now, just simulate success
            SuccessMessage = "Registration successful!";
            ModelState.Clear();
            return Page();
        }

        public class RegisterInputModel
        {
            [Required]
            [StringLength(100, MinimumLength = 3)]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }
        }
    }
}