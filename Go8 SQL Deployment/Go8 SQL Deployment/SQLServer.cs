namespace Go8_SQL_Deployment
{
    public class SQLServer
    {
        public string? Server { get; set; }
        public string? UserId { get; set; }
        public string? Password { get; set; }

        public SQLServer(string? keyString)
        {
            string[]? values = keyString?.Split(";");

            for (int i = 0; i < values?.Length; i++)
            {
                string property = values[i].Split("=").First().Trim();
                string value = values[i].Split("=").Last().Trim();

                switch (property)
                {
                    case nameof(Server):
                        Server = value;
                        break;
                    case nameof(UserId):
                        UserId = value;
                        break;
                    case nameof(Password):
                        Password = value;
                        break;
                }
            }
        }

        public override string? ToString()
        {
            if (string.IsNullOrWhiteSpace(Server) == false)
                return Server;
            else
                return base.ToString();
        }
    }
}
