using System;
using System.Collections.Generic;
using System.Text;

namespace CommunAxiom.Commons.Client.Contracts.Encryption
{
    public class EncryptionParams
    {
        public byte[] EncryptedKey { get; set; }
        public byte[] EncryptedIV { get; set; }
    }
}
