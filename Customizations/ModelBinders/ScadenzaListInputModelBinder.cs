using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;
using Scadenze.Models.InputModels;
using Scadenze.Models.InputModels.Scadenze;
using Scadenze.Models.Options;

namespace Scadenze.Customizations.ModelBinders
{
    public class ScadenzaListInputModelBinder : IModelBinder
    {
        private readonly IOptionsMonitor<ScadenzeOptions> coursesOptions;
        public ScadenzaListInputModelBinder(IOptionsMonitor<ScadenzeOptions> coursesOptions)
        {
            this.coursesOptions = coursesOptions;
        }
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //Recuperiamo i valori grazie ai value provider
            string search = bindingContext.ValueProvider.GetValue("Search").FirstValue;
            string orderBy = bindingContext.ValueProvider.GetValue("OrderBy").FirstValue;
            int.TryParse(bindingContext.ValueProvider.GetValue("Page").FirstValue, out int page);
            bool.TryParse(bindingContext.ValueProvider.GetValue("Ascending").FirstValue, out bool ascending);

            //Creiamo l'istanza del CourseListInputModel
            ScadenzeOptions options = coursesOptions.CurrentValue;
            var inputModel = new ScadenzaListInputModel(search, page, orderBy, ascending, options.PerPage, options.Order);

            //Impostiamo il risultato per notificare che la creazione è avvenuta con successo
            bindingContext.Result = ModelBindingResult.Success(inputModel);

            //Restituiamo un task completato
            return Task.CompletedTask;
        }
    }
}