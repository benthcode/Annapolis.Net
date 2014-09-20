using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Shared.Model;

namespace Annapolis.Abstract.Work
{
    public interface ISecurityWorker
    {

        bool VerifyToken();
        bool VerifyToken(string userName, string token);
        bool VerifyToken(string userName, string token, out TokenUser user);

        void AddOrUpdateTokenUser(TokenUser user);
        void AddOrUpdateCurrentTokenUser(TokenUser user);

        void RemoveTokenUser();
        void RemoveTokenUser(TokenUser user);
        void RemoveTokenUser(string userName);

        void Clear();


        TokenUser CurrentUser { get; }
        bool IsCurrentUserValid();

        TokenUser ExistingTokenUser(string userName);
    }
}
