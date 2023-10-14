using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Scadenze.Controllers;

namespace Scadenze.Models.InputModels
{
    public class BeneficiarioCreateInputModel
    {
        [Required(ErrorMessage ="il campo Beneficiario è obbligatorio")]
        [Remote(action:nameof(BeneficiariController.IsBeneficiarioAvailable),controller:"Beneficiari",ErrorMessage="Il Beneficiario esiste già")]
        public string Beneficiario { get; set; }
        [Required(ErrorMessage ="il campo Descrizione è obbligatoria")]
        public string Descrizione { get; set; }
    }
}