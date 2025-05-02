namespace likhitan.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public bool IsUserVerified { get; set; }
        public int RoleId { get; set; }
        public UserRoles Role { get; set; }
        public DateTime Created { get; set; }
        public string OTP { get; set; }
        public DateTime OTPExpire { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? Deleted { get; set; }
        public bool IsTeamsAndConditionAccepted { get; set; }
        public int Gender { get; set; }
        public DateTime? DOB { get; set; }
        public int? AuthorId { get; set; }
        //public int? BlogCommentsId { get; set; }
        public int? BlogLikesId { get; set; }
        //public int? BlogViewsId { get; set; }
    }
}
