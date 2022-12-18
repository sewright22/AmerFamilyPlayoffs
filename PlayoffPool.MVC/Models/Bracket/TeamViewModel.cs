using System.ComponentModel.DataAnnotations;

namespace PlayoffPool.MVC.Models.Bracket
{
    public class TeamViewModel
    {
        public int Id { get; set; }
        public int Seed { get; set; }
        public string ViewId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }

    }
}
