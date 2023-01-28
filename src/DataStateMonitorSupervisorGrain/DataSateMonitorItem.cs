using System.Collections.Generic;

namespace CommunAxiom.Commons.Client.Grains.DataStateMonitorSupervisorGrain
{
    public class DataSateMonitorItem
    {
        private List<string> _keys;

        public List<string> Keys
        {
            get => _keys ??= new List<string>();
            set => _keys = value;
        }
    }
}