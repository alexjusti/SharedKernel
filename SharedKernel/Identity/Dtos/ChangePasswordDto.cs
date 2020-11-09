namespace SharedKernel.Identity.Dtos
{
    public abstract class ChangePasswordDto
    {
        public string UserId { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }
    }
}