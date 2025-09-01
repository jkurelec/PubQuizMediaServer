namespace PubQuizMediaServer.Models
{
    public class EditionPermissionDto
    {
        public int EditionId { get; set; }
        public HashSet<int> UserIds { get; set; } = new();
    }

    public class UserPermissionDto
    {
        public int UserId { get; set; }
        public HashSet<int> EditionIds { get; set; } = new();
    }
}
