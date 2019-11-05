using System;
using System.Collections.Generic;

namespace GitApp.Models.Db
{
    public class Repository
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public DateTime DateTime { get; set; }

        public List<File> Files { get; set; }
    }
}
