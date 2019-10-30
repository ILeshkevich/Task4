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

        // Todo: Investigate if virtual is required here
        public List<File> Files { get; set; }
    }
}
