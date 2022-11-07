using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;

namespace CommunAxiom.Commons.Orleans.Security
{
    public class ClaimsContainer
    {
        public byte[] ClaimsData { get; set; }

        public ClaimsPrincipal GetPrincipal()
        {
            BinaryReader binaryReader = new BinaryReader(new MemoryStream(ClaimsData));
            ClaimsPrincipal principal = new ClaimsPrincipal(binaryReader);
            return principal;
        }

        public void SetPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            var ms = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(ms);
            claimsPrincipal.WriteTo(binaryWriter);
            binaryWriter.Flush();
            this.ClaimsData = ms.ToArray();
        }

    }
}
