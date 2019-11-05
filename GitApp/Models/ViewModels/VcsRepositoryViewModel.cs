using System;
using System.Collections.Generic;

namespace GitApp.Models.ViewModels
{
    public class VcsRepositoryViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public DateTime LastUpdate { get; set; }

        public IReadOnlyList<FileViewModel> Files { get; set; }
    }
}
