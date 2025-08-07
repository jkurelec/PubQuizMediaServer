namespace PubQuizMediaServer.Models
{
    public class QuestionMediaPermissions
    {
        public Dictionary<int, List<int>> Permissions { get; set; } = new();
    }
}
