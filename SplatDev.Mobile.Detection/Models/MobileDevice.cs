namespace SplatDev.Mobile.Detection.Models
{
    public class MobileDevice
    {
        public string Code { get; set; }

        public override string ToString()
        {
            return $"{Code}";
        }
    }
}
