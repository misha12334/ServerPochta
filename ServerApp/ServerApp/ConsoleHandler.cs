using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp
{
    public class ConsoleHandler : IDisposable
    {
        public enum ConsoleEvent
        {
            CtrlC = 0, CtrlBreak = 1, CtrlClose = 2, CtrlLogoff = 5, CtrlShutdown = 6
        }

        public delegate void ControlEventHandler(ConsoleEvent consoleEvent);

        public event ControlEventHandler ControlEvent;

        ControlEventHandler eventHandler;

        public ConsoleHandler()
        {
            eventHandler = new ControlEventHandler(Handler);
            SetConsoleCtrlHandler(eventHandler, true);
        }

        ~ConsoleHandler()
        { Dispose(false); }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (eventHandler != null)
            {
                SetConsoleCtrlHandler(eventHandler, false);
                eventHandler = null;
            }
        }

        private void Handler(ConsoleEvent consoleEvent)
        {
            if (ControlEvent != null)
                ControlEvent(consoleEvent);
        }

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleCtrlHandler(ControlEventHandler e, bool add);
    }
}
