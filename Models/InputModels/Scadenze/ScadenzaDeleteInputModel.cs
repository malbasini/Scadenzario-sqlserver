using System;
using System.ComponentModel.DataAnnotations;
namespace Scadenze.Models.InputModels
{
    public class ScadenzaDeleteInputModel
    {
        [Required]
        public int IDScadenza { get; set; }
    }
}