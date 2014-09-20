using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;
using Annapolis.Abstract.Repository;
using Annapolis.Shared.Model;

namespace Annapolis.Abstract.Work
{
    public interface IMemberUserWork : IAnnapolisBaseCrudWork<MemberUser>
    {

        bool ValidateUser(string identifier, string password, out MemberUser user);

        bool ValidateToken(string username, string token, out MemberUser user);

        OperationStatus UpdatePassword(string userName, string oldPassword, string newPassword);

    }
}
