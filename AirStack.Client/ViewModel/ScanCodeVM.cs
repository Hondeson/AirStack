using AirStack.Client.ViewModel.Base;
using AirStack.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStack.Client.ViewModel
{
    public class ScanCodeVM : BaseVM
    {
        public ScanCodeVM()
        {
        }

        string _LastScannedCode;
        public string LastScannedCode
        {
            get => _LastScannedCode;
            set => Set(ref _LastScannedCode, value);
        }


    }
}
