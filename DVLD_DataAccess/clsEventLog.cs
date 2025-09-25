using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLD_DataAccess
{
    public class clsEventLog
    {
        private static string _sourceName = "DVLD_App";

        private static void _EventLog(string message, EventLogEntryType eventLogEntryType)
        {
            {if (!EventLog.SourceExists(_sourceName))
            {
                EventLog.CreateEventSource(_sourceName, "Application");
            }

            EventLog.WriteEntry(_sourceName, message, eventLogEntryType);}
        }

        public static void setEventLogInformation(string message)
        {
            _EventLog(message, EventLogEntryType.Information);
        }

        public static void setEventLogWarning(string message) 
        {
            _EventLog(message, EventLogEntryType.Warning);
        }

        public static void setEventLogError(string message)
        {
            _EventLog(message, EventLogEntryType.Error);
        }
    }
}
