namespace Go8_SQL_Deployment
{
    public class FilterDate(string name, DateOnly value)
    {
        public string Name { get; set; } = name;
        public DateOnly Value { get; set; } = value;

        public override string ToString()
        {
            return Name;
        }
    }
}
