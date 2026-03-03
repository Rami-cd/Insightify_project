namespace Auth_service.Models
{
    public class Tenant
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = null!;

        public Guid OwnerUserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User OwnerUser { get; set; } = null!;
    }
}
