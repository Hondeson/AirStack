using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStack.Core.Model
{
    public class ItemModel
    {
        public long ID { get; set; }
        public string Code { get; set; }
        public string ParentCode { get; set; }
    }
}
