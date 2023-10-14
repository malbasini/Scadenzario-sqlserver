using System;
using System.ComponentModel.DataAnnotations;
namespace Scadenze.Models.InputModels
{
    public class BeneficiarioDeleteInputModel
    {
        [Required]
        public int IDBeneficiario { get; set; }
    }
}