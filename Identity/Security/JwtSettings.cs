namespace SharedKernel.Identity.Security
{
    public class JwtSettings
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int DurationMinutes { get; set; }

        public byte[] Secret { get; set; }
    }
}