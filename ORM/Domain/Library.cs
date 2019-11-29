using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ORM.Domain
{
    public class Library : BaseEntity
    {
        public string Name { get; set; }

        public string Branch { get; set; }
    }
}
