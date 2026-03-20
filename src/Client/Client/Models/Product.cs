using System;
using System.Collections.Generic;
using System.Text;

namespace Client.Models
{
    public class Product: BaseModel
    {
        public int? ID { get; set; }
        public string? Name { get; set; }
        public int? Price  { get; set; }
        public string? ImageURL { get; set; } = null;

        public int? CategoryID { get; set; }
    }
}
