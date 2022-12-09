using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStack.Core.Model
{
    public class ItemHistoryModel
    {
        public long ID { get; set; }
        public long ItemID { get; set; }
        public long StatusID { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
