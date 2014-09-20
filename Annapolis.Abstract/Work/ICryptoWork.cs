using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Abstract.Work
{
    public interface ICryptoWork : IAnnapolisBaseWork
    {
        string CreateHashOnMD5Cng(string plainValue);

        bool CompareHashOnMD5Cng(string plainValue, string hash);
    }
}
