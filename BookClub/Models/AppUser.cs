using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookClub.Models
{
    public class AppUser : IdentityUser
    {
        public DateTime DateOfBirthday { get; set; }

        public List<Book>? Books { get; set; }
    }
}
