using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageBoard
{
    class Message
    {
        public int Id { get; set; }
        public int MessageThreadId { get; set; }
        public int UserId { get; set; }
        public string Messages { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public int MessageType { get; set; }
        public string GPName { get; set; }
        public Boolean IsExpanded { get; set; }
        public Boolean IsAdmin { get; set; }
        public Boolean IsHidden { get; set; }
    }
}
