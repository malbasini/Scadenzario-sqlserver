using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Scadenze.Models.Entities;
using Scadenze.Models.InputModels;
using Scadenze.Models.InputModels.Scadenze;
using Scadenze.Models.Services.Infrastructure;
using Scadenze.Models.ViewModels;

namespace Scadenze.Models.Services.Application.Scadenze
{
    public class MemoryCacheScadenzaService : ICachedScadenzaService
    {
        private readonly IScadenzeService scadenzaService;
        private readonly IMemoryCache memoryCache;
        private readonly ApplicationDbContext dbContext;
        private readonly IHttpContextAccessor user;
        public MemoryCacheScadenzaService(ApplicationDbContext dbContext, IHttpContextAccessor user, IScadenzeService scadenzaService, IMemoryCache memoryCache)
        {
            this.dbContext = dbContext;
            this.scadenzaService = scadenzaService;
            this.memoryCache = memoryCache;
            this.user = user;
        }
        public Task<ListViewModel<ScadenzaViewModel>> GetScadenzeAsync(ScadenzaListInputModel model)
        {
            return scadenzaService.GetScadenzeAsync(model);
        }
        public Task<ScadenzaViewModel> GetScadenzaAsync(int id)
        {
            /*--Andiamo a cercare in memoria un oggetto identificato dalla chiave Scadenza + id
            e se non dovesse esistere lo recuperiamo dal database impostando 60 secondi*/
            return memoryCache.GetOrCreateAsync($"Scadenze{id}", cacheEntry =>
            {
                cacheEntry.SetSize(1);
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                return scadenzaService.GetScadenzaAsync(id);
            });
        }

        public Task<ScadenzaViewModel> CreateScadenzaAsync(ScadenzaCreateInputModel inputModel)
        {
            return scadenzaService.CreateScadenzaAsync(inputModel);
        }

        public Task<ScadenzaEditInputModel> GetScadenzaForEditingAsync(int id)
        {
            return scadenzaService.GetScadenzaForEditingAsync(id);
        }
        public async Task<ScadenzaViewModel> EditScadenzaAsync(ScadenzaEditInputModel inputModel)
        {
            ScadenzaViewModel viewModel = await scadenzaService.EditScadenzaAsync(inputModel);
            memoryCache.Remove($"Scadenze{inputModel.IDScadenza}");
            return viewModel;
        }

        public async Task DeleteScadenzaAsync(ScadenzaDeleteInputModel inputModel)
        {
            await scadenzaService.DeleteScadenzaAsync(inputModel);
            memoryCache.Remove($"Scadenze{inputModel.IDScadenza}");
        }
        public List<SelectListItem> GetBeneficiari()
        {
                List<Beneficiario> beneficiari = new List<Beneficiario>();
                beneficiari = (from b in dbContext.Beneficiari.Where(z=>z.IdUser==user.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
                    select b).ToList();

                var beneficiario = beneficiari.Select(b => new SelectListItem
                {
                    Text = b.Sbeneficiario,
                    Value = b.IDBeneficiario.ToString()
                }).ToList();
                return beneficiario;
        }
        //Recupero Beneficiario
        public string GetBeneficiarioById(int id)
        {
            string Beneficiario = dbContext.Beneficiari
            .Where(t => t.IDBeneficiario == id)
            .Select(t => t.Sbeneficiario).Single();
            return Beneficiario;
        }
        //Calcolo giorni ritardo o giorni mancanti al pagamento
        public int DateDiff(DateTime inizio, DateTime fine)
        {
            int giorni = 0;
            giorni = (inizio.Date - fine.Date).Days;
            return giorni;
        }

        public bool IsDate(string date)
        {
            throw new NotImplementedException();
        }
    }
}