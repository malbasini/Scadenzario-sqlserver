using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;
using Scadenze.Models.Entities;
using Scadenze.Models.Services.Infrastructure;

namespace Scadenze.Models.InputModels
{
    public class ScadenzaCreateInputModel
    {
        [Required(ErrorMessage="Il Beneficiario è obbligatorio."),
        Display(Name = "Beneficiario")]
        public int IDBeneficiario { get; set; }
        public string Beneficiario { get; set; }
        [Required (ErrorMessage="La Data Scadenza è obbligatoria."),
        DataType(DataType.Date, ErrorMessage="Formato data non valido."),
        Display(Name = "Data Scadenza")]
        public DateTime DataScadenza { get; set; }
        [Required (ErrorMessage="L'importo è obbligatorio.")]
        [Range(1,100000,ErrorMessage="L'importo deve essere compreso tra 1 e 100.000")]
        [DataType(DataType.Currency)]
        public Decimal Importo { get; set; }
        public List<SelectListItem> Beneficiari{get;set;}
    }
}