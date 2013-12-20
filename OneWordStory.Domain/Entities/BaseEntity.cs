using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneWordStory.Domain.Entities
{
    public class BaseEntity
    {

        public string Id { get; set; }
        public int DocumentId
        {
            get
            {
                if (string.IsNullOrEmpty(Id)) return 0;
                return int.Parse(Id.Split('/')[1]);
            }
        }

    }
}
