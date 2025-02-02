using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class ReceiveMessageModel
    {
        public string ConversationId { get; set;}
        public string MessageId { get; set;}
        public string Content { get; set;}
        public string Type { get; set;}
    }
}