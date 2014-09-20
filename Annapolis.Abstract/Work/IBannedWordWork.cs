using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Abstract.Work
{
    public interface IBannedWordWork : IAnnapolisBaseCacheCrudWork<ContentBannedWord>
    {
        string ReplaceBannedWord(string text, char replaceHolder);
    }
}
