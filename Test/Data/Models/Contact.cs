namespace Test.Data.Models
{
    public class Contact
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public bool Married { get; set; }
        public string Phone { get; set; } = string.Empty;
        public decimal Salary { get; set; }

        public override string? ToString()
        {
            return $"ID: {Id} Name: {Name} Date of Birth {DateOfBirth} Married {Married} Phone {Phone} Salary {Salary}";
        }
    }
}
