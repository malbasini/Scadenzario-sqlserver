using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

#nullable disable

namespace Scadenze.Models.Entities
{
    public partial class Scadenza
    {
        public Scadenza()
        {
            Ricevute = new HashSet<Ricevuta>();
        }
        public int IDScadenza { get; set; }
        public String IDUser { get; set; }
        public int IDBeneficiario { get; set; }
        public string Beneficiario { get; set; }
        public DateTime DataScadenza { get; set; }
        public Decimal Importo { get; set; }
        public bool Sollecito { get; set; }
        public int? GiorniRitardo { get; set; }
        public DateTime? DataPagamento { get; set; }

        public virtual Beneficiario beneficiario { get; set; }
        public virtual ICollection<Ricevuta> Ricevute { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

    }
    
}
