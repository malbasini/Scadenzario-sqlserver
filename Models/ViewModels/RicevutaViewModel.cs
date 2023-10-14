using System;
using System.ComponentModel.DataAnnotations;
using Scadenze.Models.Entities;

namespace Scadenze.Models.ViewModels
{
    public class RicevutaViewModel
    {
        public int Id { get; set; }
        public int IDScadenza { get; set; }
        [Display(Name = "Nome File")]
        public string FileName { get; set; }
        [Display(Name = "Tipo File")]
        public string FileType { get; set; }
        public byte[] FileContent { get; set; }
        public string Beneficiario { get; set; }
        public string Path { get; set; }


        public static RicevutaViewModel FromEntity(Ricevuta ricevuta)
        {
            return new RicevutaViewModel {
                Id = ricevuta.Id,
                Beneficiario = ricevuta.Beneficiario,
                IDScadenza = ricevuta.IDScadenza,
                FileName= ricevuta.FileName,
                FileContent = ricevuta.FileContent,
                FileType = ricevuta.FileType,
                Path = ricevuta.Path
            };
        }
    }
}