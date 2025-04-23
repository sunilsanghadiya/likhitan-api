namespace likhitan.Models.ClientDto
{
    public class RegisterDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool IsTeamsAndConditionAccepted { get; set; }
    }
}
