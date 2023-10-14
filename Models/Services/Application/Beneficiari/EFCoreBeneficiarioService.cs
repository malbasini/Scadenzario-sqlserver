using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scadenze.Models.Entities;
using Scadenze.Models.Exceptions.Application;
using Scadenze.Models.InputModels;
using Scadenze.Models.InputModels.Beneficiari;
using Scadenze.Models.Options;
using Scadenze.Models.Services.Application;
using Scadenze.Models.Services.Infrastructure;
using Scadenze.Models.ViewModels;


namespace Scadenze.Models.Services.Application.Beneficiari
{
    public class EFCoreBeneficiarioService:IBeneficiariService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<EFCoreBeneficiarioService> logger;
        private readonly IOptionsMonitor<BeneficiariOptions> beneficiariOptions;
        private readonly IHttpContextAccessor user;
        public EFCoreBeneficiarioService(ILogger<EFCoreBeneficiarioService> logger, IHttpContextAccessor user, ApplicationDbContext dbContext,IOptionsMonitor<BeneficiariOptions> beneficiariOptions)
        {
            this.beneficiariOptions = beneficiariOptions;
            this.dbContext = dbContext;
            this.logger = logger;
            this.user = user;
        }
        public async Task<BeneficiarioViewModel> CreateBeneficiarioAsync(BeneficiarioCreateInputModel inputModel)
        {
            Beneficiario beneficiario = new Beneficiario();
            beneficiario.Sbeneficiario = inputModel.Beneficiario;
            beneficiario.Descrizione = inputModel.Descrizione;
            beneficiario.IdUser = user.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await dbContext.AddAsync(beneficiario);
            await dbContext.SaveChangesAsync();
            return BeneficiarioViewModel.FromEntity(beneficiario);
        }

        public async Task<ListViewModel<BeneficiarioViewModel>> GetBeneficiariAsync(BeneficiarioListInputModel model)
        {
            IQueryable<Beneficiario> baseQuery = dbContext.Beneficiari;
            switch(model.OrderBy)
            {
                case "Beneficiario":
                    if(model.Ascending)
                    {
                        baseQuery=baseQuery.OrderBy(z=>z.Sbeneficiario);
                    }
                    else
                    {
                        baseQuery=baseQuery.OrderByDescending(z=>z.Sbeneficiario);
                    }
                break; 
                case "Descrizione":
                    if(model.Ascending)
                    {
                        baseQuery=baseQuery.OrderBy(z=>z.Descrizione);
                    }
                    else
                    {
                        baseQuery=baseQuery.OrderByDescending(z=>z.Descrizione);
                    }
                break; 
                
            }
            IQueryable<BeneficiarioViewModel> queryLinq = baseQuery
                .AsNoTracking()
                .Where(beneficiari => beneficiari.Sbeneficiario.Contains(model.Search))
                .Where(beneficiari => beneficiari.IdUser == user.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .Select(beneficiari => BeneficiarioViewModel.FromEntity(beneficiari)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato
                 List<BeneficiarioViewModel> beneficiari = await queryLinq
                .Skip(model.Offset)
                .Take(model.Limit)                       
                .ToListAsync(); //La query al database viene inviata qui, quando manifestiamo l'intenzione di voler leggere i risultati
                int totalCount = await queryLinq.CountAsync();
                ListViewModel<BeneficiarioViewModel> results = new ListViewModel<BeneficiarioViewModel>
                {
                     Results=beneficiari,
                     TotalCount=totalCount
                };
                return results;
        }
        public async Task<BeneficiarioViewModel> GetBeneficiarioAsync(int id)
        {
            
            logger.LogInformation("Ricevuto identificativo beneficiario {id}",id);
            IQueryable<BeneficiarioViewModel> queryLinq = dbContext.Beneficiari
                .AsNoTracking()
                .Where(beneficiario => beneficiario.IDBeneficiario == id)
                .Select(beneficiario => BeneficiarioViewModel.FromEntity(beneficiario)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato
            
            BeneficiarioViewModel viewModel = await queryLinq.FirstOrDefaultAsync();
                                                           //.FirstOrDefaultAsync(); //Restituisce null se l'elenco è vuoto e non solleva mai un'eccezione
            if(viewModel==null)
            {
                 throw new BeneficiarioNotFoundException(id); 
            }                                              //.SingleOrDefaultAsync(); //Tollera il fatto che l'elenco sia vuoto e in quel caso restituisce null, oppure se l'elenco contiene più di 1 elemento, solleva un'eccezione                                            //.FirstAsync(); //Restituisce il primo elemento, ma se l'elenco è vuoto solleva un'eccezione  
            return viewModel;    
        }
        public async Task<BeneficiarioEditInputModel> GetBeneficiarioForEditingAsync(int id)
        {
            logger.LogInformation("Ricevuto identificativo beneficiario {id}",id);
            IQueryable<BeneficiarioEditInputModel> queryLinq = dbContext.Beneficiari
                .AsNoTracking()
                .Where(beneficiario => beneficiario.IDBeneficiario == id)
                .Select(beneficiario => BeneficiarioEditInputModel.FromEntity(beneficiario)); //Usando metodi statici come FromEntity, la query potrebbe essere inefficiente. Mantenere il mapping nella lambda oppure usare un extension method personalizzato

            BeneficiarioEditInputModel viewModel = await queryLinq.FirstOrDefaultAsync();

            if (viewModel == null)
            {
                logger.LogWarning("Beneficiario {id} not found", id);
                throw new BeneficiarioNotFoundException(id);
            }

            return viewModel;
        }

        public async Task DeleteBeneficiarioAsync(BeneficiarioDeleteInputModel inputModel)
        {
            Beneficiario beneficiario = await dbContext.Beneficiari.FindAsync(inputModel.IDBeneficiario);
            if (beneficiario == null)
            {
                throw new BeneficiarioNotFoundException(inputModel.IDBeneficiario);
            }
            dbContext.Remove(beneficiario);
            await dbContext.SaveChangesAsync();
        }
        public async Task<BeneficiarioViewModel> EditBeneficiarioAsync(BeneficiarioEditInputModel inputModel)
        {
            Beneficiario beneficiario = await dbContext.Beneficiari.FindAsync(inputModel.IDBeneficiario);

            if (beneficiario == null)
            {
                throw new BeneficiarioNotFoundException(inputModel.IDBeneficiario);
            }
            try
            {   
                //Mapping
                beneficiario.Sbeneficiario=inputModel.Beneficiario;
                beneficiario.Descrizione=inputModel.Descrizione;
                beneficiario.Email=inputModel.Email;
                beneficiario.Telefono=inputModel.Telefono;
                beneficiario.SitoWeb=inputModel.SitoWeb;
                beneficiario.IdUser = user.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                dbContext.Update(beneficiario);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return BeneficiarioViewModel.FromEntity(beneficiario);
        }
        public async Task<bool> VerificationExistenceAsync(string beneficiario)
        {
            logger.LogInformation("Ricevuto nome beneficiario {beneficiario}",beneficiario);
            Beneficiario verify = await dbContext.Beneficiari.FirstOrDefaultAsync(b=>b.Sbeneficiario==beneficiario);
            if(verify==null)
               return false;
            return true;   
        }
        
    }
}