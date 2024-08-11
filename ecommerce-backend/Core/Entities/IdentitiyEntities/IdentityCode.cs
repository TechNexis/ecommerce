
namespace Core.Entities.IdentitiyEntities
{
    public  class IdentityCode
    {
        public int Id { get; set; }
        public string AppUserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = new AppUser();/// <summary>
                                                          /// of the foreign key
                                                          /// </summary>
        public string Code { get; set; }= string.Empty;
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; }
        public DateTime? ActivationTime { get; set; }
    }
}
