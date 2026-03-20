using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Models
{
    public class Category : BaseModel
    {
        public int ID { get; set; }
        public string? Name { get; set; }

        public int Count { get; set; }
    }
}
