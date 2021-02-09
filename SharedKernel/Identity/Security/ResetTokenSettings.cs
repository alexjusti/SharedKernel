namespace SharedKernel.Identity.Security
{
    public class ResetTokenSettings
    {
        public string Secret { get; set; }

        public int TokenLength { get; set; } = 12;
    }
}