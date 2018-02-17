using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageBoard
{
    class MessageThread
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public String GPName { get; set; }
        public String NumOfMessage { get; set; }
        public Boolean IsExpanded { get; set; }
        public String MsgSummary { get; set; }
        public Boolean IsAdmin { get; set; }
        public Boolean IsHidden { get; set; }
    }
}
