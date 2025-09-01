namespace PubQuizMediaServer.Models
{
    public class QuestionMediaPermissions
    {
        public Dictionary<int, HashSet<int>> Permissions { get; set; } = new();
    }
}
