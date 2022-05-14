using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientUI.Shared.Services
{
    public interface IStdMessagesService
    {
        string ErrorMessage(string code, string source, string failedAction);
        void ToastError(string code, string source, string failedAction);
    }
}
