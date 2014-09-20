using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Abstract.Work
{
    public interface ILoggingWork : IAnnapolisBaseWork
    {
        void Info(string message);

        void Warning(string warningMessage);
        void Warning(Exception exception);

        void Debug(string debugMessage);
        void Debug(Exception exception);

        void Error(string errorMessage);
        void Error(Exception exception);

        void Fatal(string fatalMessage);
        void Fatal(Exception exception);
    }
}
