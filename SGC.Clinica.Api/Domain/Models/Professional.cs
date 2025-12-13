namespace SGC.Clinica.Api.Domain.Models
{
    public abstract class Professional
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Document { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;
        public bool Active { get; private set; } = true;

        protected Professional(){}

        protected Professional(string name, string document, string phone)
        {
            Name = name;
            Document = document;
            Phone = phone;
            Active = true;
        }
    }
}
