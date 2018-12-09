
namespace ColorProfiles
{
    public class ProfileManager
    {
        public ColorProfile[] ColorProfiles { get; private set; }

        public ProfileManager()
        {
            ColorProfiles = new ColorProfile[]{ new ProfileSRGB(), new ProfileAdobeRGB(), new ProfileAppleRGB(),
                new ProfileCieRGB(), new ProfileWideGamut(), new ProfilePalSecam() };
        }
    }
}
