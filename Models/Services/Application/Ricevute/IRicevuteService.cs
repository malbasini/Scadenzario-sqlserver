using System.Collections.Generic;
using System.Threading.Tasks;
using Scadenze.Models.Entities;
using Scadenze.Models.InputModels;
using Scadenze.Models.ViewModels;

public interface IRicevuteService
{
       
    Task<RicevutaViewModel> CreateRicevutaAsync(List<RicevutaCreateInputModel> input);
    List<RicevutaViewModel> GetRicevute(int id);
    Task DeleteRicevutaAsync(int Id);
    Task<RicevutaViewModel> GetRicevutaAsync(int id);
   
        
}