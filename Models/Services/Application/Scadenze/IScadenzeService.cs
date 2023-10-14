using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Scadenze.Models.InputModels;
using Scadenze.Models.InputModels.Scadenze;
using Scadenze.Models.ViewModels;

namespace Scadenze.Models.Services.Application.Scadenze
{
    public interface IScadenzeService
    {
        Task<ListViewModel<ScadenzaViewModel>> GetScadenzeAsync(ScadenzaListInputModel model);
        Task<ScadenzaViewModel> CreateScadenzaAsync(ScadenzaCreateInputModel inputModel);
        Task<ScadenzaViewModel> GetScadenzaAsync(int id);
        Task DeleteScadenzaAsync(ScadenzaDeleteInputModel inputModel);
        Task<ScadenzaEditInputModel> GetScadenzaForEditingAsync(int id);
        Task<ScadenzaViewModel> EditScadenzaAsync(ScadenzaEditInputModel inputModel);
        List<SelectListItem> GetBeneficiari();
        string GetBeneficiarioById(int id);
        int DateDiff(DateTime inizio, DateTime fine);
        bool IsDate(string date);
    }
}