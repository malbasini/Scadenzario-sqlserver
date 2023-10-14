using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Scadenze.Models.Entities
{
    public partial class Beneficiario
    {
        public Beneficiario()
        {
            Scadenze = new HashSet<Scadenza>();
        }
        public int IDBeneficiario { get; set; }
        public string Sbeneficiario { get; set; }
        public string Descrizione { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string SitoWeb { get; set; }
         
        public String IdUser { get; set; }
        public virtual ICollection<Scadenza> Scadenze { get; set; }

    }
}
