using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Security.Cryptography;
using Annapolis.Abstract.Work;

namespace Annapolis.Work
{
    public class CryptoWork :AnnapolisBaseWork, ICryptoWork
    {
        public string CreateHashOnMD5Cng(string plainValue)
        {
            return Cryptographer.CreateHash("MD5CngCrypto", plainValue);
        }

        public bool CompareHashOnMD5Cng(string plainValue, string hash)
        {
            return Cryptographer.CompareHash("MD5CngCrypto", plainValue, hash);
        }

    }
}
