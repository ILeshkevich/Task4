namespace GitApp.Models.Db
{
    public class File
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ChangesCount { get; set; }

        public Repository Repository { get; set; }

        public int RepositoryId { get; set; }
    }
}
