using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Abstract.Work;

namespace Annapolis.Work
{
    public class LoggingWork :AnnapolisBaseWork, ILoggingWork
    {
        public void Info(string message)
        {
            
        }

        public void Warning(string warningMessage)
        {
           
        }

        public void Warning(Exception exception)
        {
        }

        public void Error(string errorMessage)
        {
           
        }

        public void Error(Exception exception)
        {
           
        }

        public void Debug(string debugMessage)
        {
            
        }

        public void Debug(Exception exception)
        {
        }

        public void Fatal(string fatalMessage)
        {
           
        }

        public void Fatal(Exception exception)
        {
            
        }

    }
}
