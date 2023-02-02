using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.ClientUI.Shared.ViewModels.Interfaces
{
    public interface ITreeviewItem
    {
        Guid Id { get; set; }
        Guid ParentId { get; set; }
        string Text { get; set; }
        string IconName { get; set; }
        IList<ITreeviewItem> Children { get; set; }
    }
}
