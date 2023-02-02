using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirStack.Core.Model
{
    public class StatusModel
    {
        public StatusModel()
        {

        }

        public StatusModel(StatusEnum status)
        {
            SetStatus(status);
        }

        public StatusModel(long id)
        {
            this.ID = id;
            Name = ((StatusEnum)id).ToString();
        }

        public long ID { get; set; }
        public string Name { get; set; }

        public StatusEnum GetStatus()
        {
            return (StatusEnum)ID;
        }

        public void SetStatus(StatusEnum status)
        {
            this.ID = (long)status;
            this.Name = status.ToString();
        }
    }

    /// <summary>
    /// Odpovídá ID v DB
    /// </summary>
    public enum StatusEnum : long
    {
        Production = 1,
        Tests = 2,
        Dispatched = 3,
        Complaint = 4,
        ComplaintToSupplier = 5
    }
}
