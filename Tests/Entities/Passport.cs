using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests.Entities
{
    public class Passport
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Number { get; set; }
    }
}
