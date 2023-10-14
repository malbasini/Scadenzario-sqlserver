using System;
using System.ComponentModel.DataAnnotations;
using Scadenze.Models.Entities;

namespace Scadenze.Models.ViewModels
{
    public class BeneficiarioViewModel
    {
		public int IDBeneficiario { get; set; }
        public string Beneficiario { get; set; }
        public string Descrizione { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        [Display(Name = "Sito Web")]
        public string SitoWeb { get; set; }
        
        public String IdUser { get; set; }
        public static BeneficiarioViewModel FromEntity(Beneficiario beneficiario)
        {
            return new BeneficiarioViewModel {
                IDBeneficiario = beneficiario.IDBeneficiario,
                Beneficiario = beneficiario.Sbeneficiario,
                Descrizione = beneficiario.Descrizione,
                Email = beneficiario.Email,
                Telefono = beneficiario.Telefono,
                SitoWeb = beneficiario.SitoWeb,
                IdUser = beneficiario.IdUser
            };
        }
    }
}