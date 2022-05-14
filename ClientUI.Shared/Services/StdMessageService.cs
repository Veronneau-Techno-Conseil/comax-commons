using Blazored.Toast.Services;
using CommunAxiom.Commons.Client.Contracts;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUI.Shared.Services
{
    public class StdMessageService : IStdMessagesService
    {
        IStringLocalizer _stringLocalizer;
        IToastService _toastService;

        public StdMessageService(IStringLocalizer stringLocalizer, IToastService toastService)
        {
            _stringLocalizer = stringLocalizer;
            _toastService = toastService;
        }
        public string ErrorMessage(string code, string source, string failedAction)
        {
            switch (code)
            {
                case OperationResult.ERR_UNEXP_ERR:
                    return _stringLocalizer[OperationResult.ERR_UNEXP_ERR, source, failedAction];
                default:
                    throw new ArgumentException($"Parameter code=\"{code}\" not supported");
            }
        }

        public void ToastError(string code, string source, string failedAction)
        {
            string msg = ErrorMessage(code, source, failedAction);
            _toastService.ShowError(msg, source);
        }
    }
}
