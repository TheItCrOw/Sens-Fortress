using System;
using System.Collections.Generic;
using System.Text;

namespace SensFortress.Guardian.Events
{
    public static class GuardianCommunicationEvents
    {
        public delegate void GuardianHandledTaskEvent();

        public static event GuardianHandledTaskEvent GuardianHandledTask;

    }
}
