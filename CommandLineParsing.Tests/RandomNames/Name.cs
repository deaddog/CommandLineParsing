namespace CommandLineParsing.Tests.RandomNames
{
    public class Name
    {
        public string FirstName { get; }
        public string LastName { get; }
        public Genders Gender { get; }

        public Name(string firstname, string lastname, Genders gender)
        {
            FirstName = firstname;
            LastName = lastname;
            Gender = gender;
        }

        public override string ToString()
        {
            return $"{FirstName} {LastName} ({Gender})";
        }
    }
}
