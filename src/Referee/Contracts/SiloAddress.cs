using System.Net;

namespace Referee.Contracts
{
    public class SiloAddress
    {
        public long Address { get; set; }
        public int Port { get; set; }
        public int Gen { get; set; }

        public static SiloAddress Parse(Orleans.Runtime.SiloAddress entry)
        {
            return new SiloAddress
            {
                Address = entry.Endpoint.Address.Address,
                Gen = entry.Generation,
                Port = entry.Endpoint.Port
            };
        }
        public Orleans.Runtime.SiloAddress ToOrleans()
        {
            return Orleans.Runtime.SiloAddress.New(new System.Net.IPEndPoint(this.Address, this.Port), this.Gen);
        }
    }
}
