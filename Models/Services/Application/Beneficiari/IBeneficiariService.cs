using System.Collections.Generic;
using System.Threading.Tasks;
using Scadenze.Models.InputModels;
using Scadenze.Models.InputModels.Beneficiari;
using Scadenze.Models.ViewModels;

namespace Scadenze.Models.Services.Application.Beneficiari
{
    public interface IBeneficiariService
    {
        Task<ListViewModel<BeneficiarioViewModel>> GetBeneficiariAsync(BeneficiarioListInputModel model);
        Task<BeneficiarioViewModel> CreateBeneficiarioAsync(BeneficiarioCreateInputModel inputModel);
        Task<BeneficiarioViewModel> GetBeneficiarioAsync(int id);
        Task DeleteBeneficiarioAsync(BeneficiarioDeleteInputModel inputModel);
        Task<BeneficiarioEditInputModel> GetBeneficiarioForEditingAsync(int id);
        Task<BeneficiarioViewModel> EditBeneficiarioAsync(BeneficiarioEditInputModel inputModel);
        Task<bool> VerificationExistenceAsync(string beneficiario);
    }
}