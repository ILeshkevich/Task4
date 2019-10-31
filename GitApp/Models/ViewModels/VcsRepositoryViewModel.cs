using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitApp.Models.ViewModels
{
    public class VcsRepositoryViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public DateTime LastUpdate { get; set; }

        // todo: rename to Files
        public List<FileViewModel> Fiels { get; set; }
    }
}
