namespace PubQuizMediaServer.Util
{
    public static class MediaPaths
    {
        private static string BasePath = string.Empty;

        public static string Temp => Path.Combine(BasePath, "temp");

        public static class Private
        {
            public static string Base => Path.Combine(BasePath, "private");
            public static string Question => Path.Combine(Base, "question");
            public static string QuestionImage => Path.Combine(Question, "image");
            public static string QuestionVideo => Path.Combine(Question, "video");
            public static string QuestionAudio => Path.Combine(Question, "audio");
        }

        public static class Public
        {
            public static string Base => Path.Combine(BasePath, "public");

            public static string Question => Path.Combine(Base, "question");
            public static string QuestionImage => Path.Combine(Question, "image");
            public static string QuestionVideo => Path.Combine(Question, "video");
            public static string QuestionAudio => Path.Combine(Question, "audio");

            public static string User => Path.Combine(Base, "user");
            public static string Team => Path.Combine(Base, "team");
            public static string Organization => Path.Combine(Base, "organization");
            public static string Quiz => Path.Combine(Base, "quiz");
            public static string Edition => Path.Combine(Base, "edition");
            public static string Location => Path.Combine(Base, "location");
        }

        public static void EnsureDirectoriesExist()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);

            while (dir != null && !dir.Name.Equals("PubQuizMediaServer", StringComparison.OrdinalIgnoreCase))
            {
                dir = dir.Parent;
            }

            if (dir == null)
            {
                throw new InvalidOperationException("Could not find the PubQuizMediaServer directory.");
            }

            BasePath = Path.GetFullPath(Path.Combine(dir.Parent!.FullName, "PubQuizMediaFiles"));

            Directory.CreateDirectory(Temp);

            Directory.CreateDirectory(Private.QuestionImage);
            Directory.CreateDirectory(Private.QuestionVideo);
            Directory.CreateDirectory(Private.QuestionAudio);

            Directory.CreateDirectory(Public.QuestionImage);
            Directory.CreateDirectory(Public.QuestionVideo);
            Directory.CreateDirectory(Public.QuestionAudio);

            Directory.CreateDirectory(Public.User);
            Directory.CreateDirectory(Public.Team);
            Directory.CreateDirectory(Public.Organization);
            Directory.CreateDirectory(Public.Quiz);
            Directory.CreateDirectory(Public.Edition);
            Directory.CreateDirectory(Public.Location);
        }
    }

}
