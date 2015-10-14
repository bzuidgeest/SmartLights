using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLights.Lyte
{
    public class CommandReadyEventArgs : EventArgs
    {
        private Command commandToSend;
        public Command CommandToSend { get { return commandToSend; } }

        public CommandReadyEventArgs(Command command)
        {
            commandToSend = command;
        }
        
    }
}
