using System.Collections.Generic;
using Scadenze.Models.InputModels.Scadenze;

namespace Scadenze.Models.ViewModels
{
    public class ScadenzaListViewModel:IPaginationInfo
    {
        public ListViewModel<ScadenzaViewModel> Scadenze {get;set;}
        public ScadenzaListInputModel Input {get;set;}

        #region Implementazione IPaginationInfo
        int IPaginationInfo.CurrentPage => Input.Page;

        int IPaginationInfo.TotalResults => Scadenze.TotalCount;

        int IPaginationInfo.ResultsPerPage => Input.Limit;

        string IPaginationInfo.Search => Input.Search;

        string IPaginationInfo.OrderBy => Input.OrderBy;

        bool IPaginationInfo.Ascending => Input.Ascending;
        
        #endregion
    }
    
}