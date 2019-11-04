using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitApp.Models.Db
{
    public class Repository
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public DateTime DateTime { get; set; }

        public IReadOnlyList<File> Files { get; set; }
    }
}
