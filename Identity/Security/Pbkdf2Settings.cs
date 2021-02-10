namespace SharedKernel.Identity.Security
{
    public class Pbkdf2Settings
    {
        public int SaltLength { get; set; } = 16;

        public int IterationCount { get; set; } = 16000;

        public int KeyLength { get; set; } = 32;
    }
}