using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Scadenze.Models.Entities;

namespace Scadenze.Models.ViewModels
{
    public class ScadenzaEditInputModel
    {
        [Required]
		public int IDScadenza { get; set; }
        public String IdUser { get; set; }
        [Display(Name = "Beneficiario")]
        public int IDBeneficiario { get; set; }
        
		public String Beneficiario { get; set; }
        [Required(ErrorMessage="Data di Scadenza obbligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Data Scadenza")]
        public DateTime DataScadenza { get; set; }
        [Required (ErrorMessage="L'importo è obbligatorio.")]
        [Range(1,100000,ErrorMessage="L'importo deve essere compreso tra 1 e 100.000")]
        [DataType(DataType.Currency)]
        [Display(Name = "Importo in Euro")]
		public Decimal Importo { get; set; }
		public bool Sollecito { get; set; }
        [Display(Name = "Giorni Mancanti/Ritardo")]
		public int? GiorniRitardo { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Data Pagamento")]
        public DateTime? DataPagamento { get; set; }
        public List<SelectListItem> Beneficiari{get;set;}
        public ICollection<RicevutaViewModel> Ricevute { get; set; }
        public static ScadenzaEditInputModel FromEntity(Scadenza scadenza)
        {
            return new ScadenzaEditInputModel {
                IDScadenza = scadenza.IDScadenza,
                Beneficiario = scadenza.Beneficiario,
                IdUser = scadenza.IDUser,
                IDBeneficiario = scadenza.IDBeneficiario,
                DataScadenza = scadenza.DataScadenza,
                Importo = scadenza.Importo,
                Sollecito=scadenza.Sollecito,
                GiorniRitardo=scadenza.GiorniRitardo,
                DataPagamento=scadenza.DataPagamento,
                Ricevute=scadenza.Ricevute.Select(ricevuta=> new RicevutaViewModel{
                    Id=ricevuta.Id,
                    FileName=ricevuta.FileName,
                    FileType=ricevuta.FileType,
                    FileContent=ricevuta.FileContent,
                    Path=ricevuta.Path,
                    Beneficiario=ricevuta.Beneficiario,
                    IDScadenza=ricevuta.IDScadenza
                }).ToList()  
            };
        }
    }
}