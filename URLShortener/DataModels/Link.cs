using System.ComponentModel.DataAnnotations;

namespace URLShortener.DataModels
{
    public class Link
    {
        public Guid Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string Url { get; set; }
        public string? ShortName { get; set; }
        public DateTime Created { get; set; }
        public string? Owner { get; set; }
        public Link()
        {
            Id = Guid.NewGuid();
            Created = DateTime.Now;
        }
    }
}
