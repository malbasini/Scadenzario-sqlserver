using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scadenze.Models.Entities;
using Scadenze.Models.Exceptions.Application;
using Scadenze.Models.InputModels;
using Scadenze.Models.Services.Infrastructure;
using Scadenze.Models.ViewModels;

namespace Scadenze.Models.Services.Application
{
    public class EFCoreRicevutaService:IRicevuteService
    {
        private readonly ILogger<EFCoreRicevutaService> logger;
        private readonly ApplicationDbContext dbContext;
        public EFCoreRicevutaService(ILogger<EFCoreRicevutaService> logger, ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public async Task<RicevutaViewModel> CreateRicevutaAsync(List<RicevutaCreateInputModel> input)
        {
            foreach(var item in input)
            {
                Ricevuta ricevuta = new Ricevuta();
                ricevuta.Beneficiario=item.Beneficiario;
                ricevuta.IDScadenza=item.IDScadenza;
                ricevuta.FileContent=item.FileContent;
                ricevuta.FileName=item.FileName;
                ricevuta.FileType=item.FileType;
                ricevuta.Path=item.Path;
                await dbContext.AddAsync(ricevuta);
            }
            if(input.Count > 0)
               await dbContext.SaveChangesAsync();
            return null;   
        }

        public async Task DeleteRicevutaAsync(int Id)
        {
            logger.LogInformation("Ricevuto {id}", Id);
            Ricevuta ricevuta = await dbContext.Ricevute.FindAsync(Id);
            if (ricevuta == null)
            {
                throw new RicevutaNotFoundException(Id);
            }
            dbContext.Remove(ricevuta);
            await dbContext.SaveChangesAsync();
        }

        public List<RicevutaViewModel> GetRicevute(int id)
        {
            logger.LogInformation("Ricevuto {id}", id);
            var queryLinq = dbContext.Ricevute
                .AsNoTracking()
                .Where(ricevuta => ricevuta.IDScadenza == id);
            List<RicevutaViewModel> viewModel = new();
            foreach(var item in queryLinq)
            {
                RicevutaViewModel view = RicevutaViewModel.FromEntity(item);
                viewModel.Add(view);
            }
            if (viewModel == null)
            {
                throw new RicevutaNotFoundException(id);
            }
            return viewModel;
        }
        public async Task<RicevutaViewModel> GetRicevutaAsync(int id)
        {
            logger.LogInformation("Ricevuto {id}", id);
            var queryLinq = dbContext.Ricevute
                .AsNoTracking()
                .Where(ricevuta => ricevuta.Id == id)
                .Select(ricevuta=>RicevutaViewModel.FromEntity(ricevuta));
                RicevutaViewModel viewModel = await queryLinq.FirstOrDefaultAsync();
                if (viewModel == null)
                {
                    logger.LogWarning("Ricevuta {id} not found", id);
                    throw new RicevutaNotFoundException(id);
                }
                return viewModel;
        }
    }
}