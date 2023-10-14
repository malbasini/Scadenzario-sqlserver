using System.ComponentModel.DataAnnotations;
using Scadenze.Models.Entities;

namespace Scadenze.Models.InputModels
{
    public class RicevutaCreateInputModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int IDScadenza { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public string FileType { get; set; }
        [Required]
        public byte[] FileContent { get; set; }
        [Required]
        public string Beneficiario { get; set; }
        [Required]
        public string Path { get; set; }
    }
}