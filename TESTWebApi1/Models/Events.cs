using System.ComponentModel.DataAnnotations.Schema;

namespace TESTWebApi1.Models
{
    public class Events
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Speaker { get; set; } = null!;
        public string Location { get; set; } = null!;
        public string DateTime { get; set; } = null!;
        public int Id { get; set; }
      
    }

}
