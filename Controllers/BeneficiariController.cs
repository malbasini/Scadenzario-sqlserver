using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Scadenze.Models.InputModels;
using Scadenze.Models.InputModels.Beneficiari;
using Scadenze.Models.Services.Application.Beneficiari;
using Scadenze.Models.ViewModels;

namespace Scadenze.Controllers
{
    [Authorize] 
    public class BeneficiariController : Controller
    {
        public static string Beneficiario { get; internal set; }
        private readonly IBeneficiariService service;

        public async Task<IActionResult> IsBeneficiarioAvailable(string beneficiario)
        {
            bool result = await service.VerificationExistenceAsync(beneficiario);
            return Json(!result);
        }

        public BeneficiariController(ICachedBeneficiarioService service)
        {
            this.service = service;
        }
        public async Task<IActionResult> Index(BeneficiarioListInputModel model)
        {
            ViewData["Title"] = "Lista Beneficiari".ToUpper();
            ListViewModel<BeneficiarioViewModel> viewModel = new();
            viewModel = await service.GetBeneficiariAsync(model);
            BeneficiarioListViewModel view = new BeneficiarioListViewModel{
                 Beneficiari = viewModel,
                 Input = model
            };
            return View(view);
        }
        public async Task<IActionResult> Detail(int id)
        {
            ViewData["Title"] = "Dettaglio Beneficiario".ToUpper();
            BeneficiarioViewModel viewModel;
            viewModel = await service.GetBeneficiarioAsync(id);
            viewModel.ToString();
            return View(viewModel);
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "Nuovo beneficiario".ToUpper();
            BeneficiarioCreateInputModel inputModel = new();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(BeneficiarioCreateInputModel inputModel)
        {
            if(ModelState.IsValid)
            {
                if(await service.VerificationExistenceAsync(inputModel.Beneficiario))
                {
                    ViewData["Title"] = "Nuovo beneficiario".ToUpper();
                    ModelState.AddModelError("key","Il beneficiario è già esistente. Scegli un nome diverso.");
                    return View(inputModel); 
                }
                await service.CreateBeneficiarioAsync(inputModel);
                TempData["Message"] = "Inserimento effettuato correttamente".ToUpper();
                return RedirectToAction("Index");
            }
            else
            {
                ViewData["Title"] = "Nuovo beneficiario".ToUpper();
                return View(inputModel); 
            }
              
        }
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Aggiorna Beneficiario".ToUpper();
            BeneficiarioEditInputModel inputModel = new();
            inputModel = await service.GetBeneficiarioForEditingAsync(id);
            Beneficiario=inputModel.Beneficiario;
            return View(inputModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(BeneficiarioEditInputModel inputModel)
        {
            if(ModelState.IsValid)
            {
                //SE L'UTENTE HA CAMBIATO BENEFICIARIO VERIFICO CHE NON SIA GIA' ESISTENTE
                if(inputModel.Beneficiario!=Beneficiario)
                {
                   if(await service.VerificationExistenceAsync(inputModel.Beneficiario))
                   {
                        ViewData["Title"] = "Aggiorna beneficiario".ToUpper();
                        ModelState.AddModelError("key","Il beneficiario è già esistente. Scegli un nome diverso.");
                        return View(inputModel); 
                   }
                }
                await service.EditBeneficiarioAsync(inputModel);
                Beneficiario=String.Empty;
                TempData["Message"] = "Aggiornamento effettuato correttamente".ToUpper();
                return RedirectToAction("Index");
            }
            else
            {
                ViewData["Title"] = "Aggiorna beneficiario".ToUpper();
                return View(inputModel); 
            }
              
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            BeneficiarioDeleteInputModel inputModel = new BeneficiarioDeleteInputModel();
            inputModel.IDBeneficiario=id;
            
            if(ModelState.IsValid)
            {
                await service.DeleteBeneficiarioAsync(inputModel);
                TempData["Message"] = "Cancellazione effettuata correttamente".ToUpper();
                return RedirectToAction("Index");
            }
            else
            {
                ViewData["Title"] = "Elimina beneficiario".ToUpper();
                return View(inputModel); 
            }
              
        }
    }
}