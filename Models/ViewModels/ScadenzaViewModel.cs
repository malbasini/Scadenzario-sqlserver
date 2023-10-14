using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Scadenze.Models.Entities;
using Scadenze.Models.Services.Infrastructure;

namespace Scadenze.Models.ViewModels
{
    public class ScadenzaViewModel
    {
        public int IDScadenza { get; set; }
        public String IdUser { get; set; }
        public int IDBeneficiario { get; set; }
        public String Beneficiario { get; set; }
        [Display(Name = "Data Scadenza")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataScadenza { get; set; }
        [Display(Name = "Importo Euro")]
        public decimal Importo { get; set; }
        public bool Sollecito { get; set; }
        [Display(Name = "Giorni Mancanti/Ritardo")]
        public int? GiorniRitardo { get; set; }
        [Display(Name = "Data Pagamento")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataPagamento { get; set; }
        public List<RicevutaViewModel> Ricevute { get; set; }
        public static ScadenzaViewModel FromEntity(Scadenza scadenza)
        {
            return new ScadenzaViewModel
            {
                IDScadenza = scadenza.IDScadenza,
                Beneficiario = scadenza.Beneficiario,
                IdUser = scadenza.IDUser,
                IDBeneficiario = scadenza.IDBeneficiario,
                DataScadenza = scadenza.DataScadenza,
                Importo = scadenza.Importo,
                Sollecito = scadenza.Sollecito,
                GiorniRitardo = scadenza.GiorniRitardo,
                DataPagamento = scadenza.DataPagamento,
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