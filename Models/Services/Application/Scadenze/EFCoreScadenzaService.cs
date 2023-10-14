using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scadenze.Models.Entities;
using Scadenze.Models.Exceptions.Application;
using Scadenze.Models.InputModels;
using Scadenze.Models.InputModels.Scadenze;
using Scadenze.Models.Options;
using Scadenze.Models.Services.Application;
using Scadenze.Models.Services.Application.Scadenze;
using Scadenze.Models.Services.Infrastructure;
using Scadenze.Models.ViewModels;

namespace Scadenze.Models.Services.Application
{
    public class EFCoreScadenzaService : IScadenzeService
    {
        private readonly ILogger<EFCoreScadenzaService> logger;
        private readonly ApplicationDbContext dbContext;
        private readonly IHttpContextAccessor user;
        private readonly IOptionsMonitor<ScadenzeOptions> scadenzeOptions;
        public EFCoreScadenzaService(ILogger<EFCoreScadenzaService> logger, ApplicationDbContext dbContext, IHttpContextAccessor user, IOptionsMonitor<ScadenzeOptions> scadenzeOptions)
        {
            this.scadenzeOptions = scadenzeOptions;
            this.user = user;
            this.dbContext = dbContext;
            this.logger = logger;

        }
        public async Task<ScadenzaViewModel> CreateScadenzaAsync(ScadenzaCreateInputModel inputModel)
        {
            Scadenza scadenza = new();
            scadenza.Beneficiario = inputModel.Beneficiario;
            scadenza.DataScadenza = inputModel.DataScadenza;
            scadenza.Importo = inputModel.Importo;
            scadenza.IDBeneficiario = inputModel.IDBeneficiario;
            scadenza.IDUser = user.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await dbContext.AddAsync(scadenza);
            await dbContext.SaveChangesAsync();
            return ScadenzaViewModel.FromEntity(scadenza);
        }

        public async Task<ListViewModel<ScadenzaViewModel>> GetScadenzeAsync(ScadenzaListInputModel model)
        {
            IQueryable<Scadenza> baseQuery = dbContext.Scadenze;
            switch(model.OrderBy)
            {
                case "Beneficiario":
                    if(model.Ascending)
                    {
                        baseQuery=baseQuery.OrderBy(z=>z.Beneficiario);
                    }
                    else
                    {
                        baseQuery=baseQuery.OrderByDescending(z=>z.Beneficiario);
                    }
                break; 
                case "DataScadenza":
                    if(model.Ascending)
                    {
                        baseQuery=baseQuery.OrderBy(z=>z.DataScadenza);
                    }
                    else
                    {
                        baseQuery=baseQuery.OrderByDescending(z=>z.DataScadenza);
                    }
                break; 
                case "Importo":
                    if(model.Ascending)
                    {
                        baseQuery=baseQuery.OrderBy(z=>z.Importo);
                    }
                    else
                    {
                        baseQuery=baseQuery.OrderByDescending(z=>z.Importo);
                    }
                break;    
            }
            if (IsDate(model.Search))
            {
                DateTime data = Convert.ToDateTime(model.Search);
                IQueryable<ScadenzaViewModel> queryLinq = baseQuery
                    .AsNoTracking()
                    .Include(Scadenza => Scadenza.Ricevute)
                    .Where(Scadenze => Scadenze.IDUser == user.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
                    .Where(scadenze => scadenze.DataScadenza == data)
                    .Select(scadenze => ScadenzaViewModel.FromEntity(scadenze)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato
                List<ScadenzaViewModel> scadenza = await queryLinq
                    .Skip(model.Offset)
                    .Take(model.Limit).ToListAsync();
                int totalCount = await queryLinq.CountAsync();
                ListViewModel<ScadenzaViewModel> results = new ListViewModel<ScadenzaViewModel>
                {
                     Results=scadenza,
                     TotalCount=totalCount
                };
                return results;
            }
            else
            {
               IQueryable<ScadenzaViewModel> queryLinq = baseQuery
                    .AsNoTracking()
                    .Include(Scadenza => Scadenza.Ricevute)
                    .Where(Scadenze => Scadenze.IDUser == user.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
                    .Where(scadenze => scadenze.Beneficiario.Contains(model.Search))
                    .Select(scadenze => ScadenzaViewModel.FromEntity(scadenze)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato
                List<ScadenzaViewModel> scadenza = await queryLinq
                    .Skip(model.Offset)
                    .Take(model.Limit).ToListAsync();
                int totalCount = await queryLinq.CountAsync();
                ListViewModel<ScadenzaViewModel> results = new ListViewModel<ScadenzaViewModel>
                {
                     Results=scadenza,
                     TotalCount=totalCount
                };
                return results;
            }
        }

        public async Task<ScadenzaViewModel> GetScadenzaAsync(int id)
        {
            logger.LogInformation("Ricevuto identificativo scadenza {id}", id);
            IQueryable<ScadenzaViewModel> queryLinq = dbContext.Scadenze
                    .AsNoTracking()
                    .Include(Scadenza => Scadenza.Ricevute)
                    .Where(scadenza => scadenza.IDScadenza == id)
                    .Select(scadenza => ScadenzaViewModel.FromEntity(scadenza)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            ScadenzaViewModel viewModel = await queryLinq.FirstOrDefaultAsync();
            //Restituisce il primo elemento dell'elenco, ma se ne contiene 0 o più di 1 solleva un eccezione.
            //.FirstOrDefaultAsync(); //Restituisce null se l'elenco è vuoto e non solleva mai un'eccezione
            //.SingleOrDefaultAsync(); //Tollera il fatto che l'elenco sia vuoto e in quel caso restituisce null, oppure se l'elenco contiene più di 1 elemento, solleva un'eccezione
            //.FirstAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto solleva un'eccezione
            if (viewModel == null)
            {
                throw new ScadenzaNotFoundException(id);
            }
            return viewModel;
        }
        public async Task<ScadenzaEditInputModel> GetScadenzaForEditingAsync(int id)
        {
            logger.LogInformation("Ricevuto identificativo scadenza {id}", id);
            IQueryable<ScadenzaEditInputModel> queryLinq = dbContext.Scadenze
                .AsNoTracking()
                .Where(scadenza => scadenza.IDScadenza == id)
                .Select(scadenza => ScadenzaEditInputModel.FromEntity(scadenza));
                
            ScadenzaEditInputModel viewModel = await queryLinq.FirstOrDefaultAsync();

            if (viewModel == null)
            {
                logger.LogWarning("Scadenza {id} not found", id);
                throw new ScadenzaNotFoundException(id);
            }

            return viewModel;
        }

        public async Task DeleteScadenzaAsync(ScadenzaDeleteInputModel inputModel)
        {
            Scadenza scadenza = await dbContext.Scadenze.FindAsync(inputModel.IDScadenza);
            if (scadenza == null)
            {
                throw new ScadenzaNotFoundException(inputModel.IDScadenza);
            }
            dbContext.Remove(scadenza);
            await dbContext.SaveChangesAsync();
        }
        public async Task<ScadenzaViewModel> EditScadenzaAsync(ScadenzaEditInputModel inputModel)
        {
            Scadenza scadenza = await dbContext.Scadenze.FindAsync(inputModel.IDScadenza);

            if (scadenza == null)
            {
                throw new ScadenzaNotFoundException(inputModel.IDScadenza);
            }
            try
            {
                //Mapping
                scadenza.Beneficiario = inputModel.Beneficiario;
                scadenza.DataScadenza = inputModel.DataScadenza;
                scadenza.DataPagamento = inputModel.DataPagamento;
                scadenza.GiorniRitardo = inputModel.GiorniRitardo;
                scadenza.Sollecito = inputModel.Sollecito;
                scadenza.Importo = inputModel.Importo;
                scadenza.IDBeneficiario = inputModel.IDBeneficiario;
                scadenza.IDUser = user.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;


                dbContext.Update(scadenza);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return ScadenzaViewModel.FromEntity(scadenza);
        }
        public List<SelectListItem> GetBeneficiari()
        {
                List<Beneficiario> beneficiari = new List<Beneficiario>();
                beneficiari = (from b in dbContext.Beneficiari.Where(z =>
                    z.IdUser == user.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
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
            logger.LogInformation("Ricevuto identificativo beneficiario {id}", id);
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
            try
            {
                string[] formats = { "dd/MM/yyyy" };
                DateTime parsedDateTime;
                return DateTime.TryParseExact(date, formats, new CultureInfo("it-IT"), DateTimeStyles.None, out parsedDateTime);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}