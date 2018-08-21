using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakePdf.Core
{
    public enum MessageType
    {
        Info,
        Warning,
        Error
    }

    public struct Message
    {
        public MessageType Type { get; set; }
        public string Content { get; set; }
    }
}
