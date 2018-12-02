using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NestaMigrations.Model
{
    public class Translate
    {
        [Key]
        public string index { get; set; }
        public string details { get; set; }
        public string en { get; set; }
        public string de { get; set; }
        public string fr { get; set; }
        public string it { get; set; }
        public string es { get; set; }
        public string ca { get; set; }

    }
}
