using System;
using System.Collections.Generic;
using Scadenze.Models.ViewModels;

namespace Scadenze.Models.Services.Application
{
    /*
    public class ScadenzeService:IScadenzeService
    {
        public List<ScadenzaViewModel> GetScadenze()
        {
            List<ScadenzaViewModel> lista = new();
            var rnd = new Random();
            for(int i=1; i<=20; i++)
            {
                var importo = Convert.ToDecimal(rnd.NextDouble() * 10 + 10);
                var scadenza = new ScadenzaViewModel();
                scadenza.IDScadenza=i;
                scadenza.Beneficiario="Enel";
                scadenza.DataPagamento = new DateTime(2021, 04, 20);
                scadenza.DataScadenza = new DateTime(2021, 03, 21);
                scadenza.IdUser= i.ToString() + "12wert5678ilkm";
                scadenza.GiorniRitardo=5;
                scadenza.Importo=Math.Round(importo,2);
                scadenza.Sollecito=false;
                lista.Add(scadenza);   
            }
            return lista;
        }
        public ScadenzaViewModel GetScadenza(int IDScadenza)
        {
            List<ScadenzaViewModel> lista = new();
            var rnd = new Random();
            for(int i=1; i<=20; i++)
            {
                var importo = Convert.ToDecimal(rnd.NextDouble() * 10 + 10);
                var scadenza = new ScadenzaViewModel();
                scadenza.IDScadenza=i;
                scadenza.Beneficiario="Enel";
                scadenza.DataPagamento = new DateTime(2021, 04, 20);
                scadenza.DataScadenza = new DateTime(2021, 03, 21);
                scadenza.IdUser= i.ToString() + "12wert5678ilkm";
                scadenza.GiorniRitardo=5;
                scadenza.Importo=Math.Round(importo,2);
                scadenza.Sollecito=false;
                lista.Add(scadenza);   
            }
            return lista.Find(z=>z.IDScadenza==IDScadenza);
        }
    }
    */
}