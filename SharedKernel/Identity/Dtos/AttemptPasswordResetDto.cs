namespace SharedKernel.Identity.Dtos
{
    public class AttemptPasswordResetDto
    {
        public string UserId { get; set; }

        public string PasswordResetToken { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }
    }
}