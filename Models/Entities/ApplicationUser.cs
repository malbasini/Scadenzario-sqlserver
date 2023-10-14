using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
#nullable disable

namespace Scadenze.Models.Entities
{
   
    public class ApplicationUser : IdentityUser
    {
       public virtual ICollection<Scadenza> Scadenze { get; set; }
    }
}