namespace PubQuizMediaServer.Models
{
    public class EditionPermissionDto
    {
        public int EditionId { get; set; }
        public List<int> UserIds { get; set; } = new();
    }

    public class UserPermissionDto
    {
        public int UserId { get; set; }
        public List<int> EditionIds { get; set; } = new();
    }
}
