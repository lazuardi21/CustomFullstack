using System.ComponentModel.DataAnnotations.Schema;

namespace SportEventsAPI.Models
{
    public class UserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
    }

    public class UpdateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }      
    }

    public class UpdateDeleteUserRequest
    {
        public string Id { get; set; }       
    }
}
