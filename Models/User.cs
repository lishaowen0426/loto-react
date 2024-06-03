namespace Loto.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string EmailVerificationToken { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        public DateTime? EmailVerificationTokenGeneratedAt { get; set; } 
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
    }
}
