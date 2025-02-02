using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class ReceiveMessageModel
    {
        public string ConversationId { get; set;} = string.Empty;
        public string MessageId { get; set;} = string.Empty;
        public string Content { get; set;} = string.Empty;
        public string Type { get; set;} = string.Empty;
    }
}