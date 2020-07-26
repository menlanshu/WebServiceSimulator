using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS_Simulator.Interface
{
    interface ISearchFormRequester
    {
        void SearchFormRequest(string searchComponentName, string inSrouceStr, bool isDownSearch = true, bool isCaseSensitive = true);
        void ReplaceFormRequest(string searchComponentName, string sourceStr, string destStr);
    }
}
