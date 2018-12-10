
using System.Linq;

namespace ColorProfiles
{
    public class ProfileManager
    {
        public static string CustomProfileName = "Custom";
        public ColorProfile[] ColorProfiles { get; private set; }

        public ProfileManager()
        {
            ColorProfiles = new ColorProfile[]{ new ProfileSRGB(), new ProfileAdobeRGB(), new ProfileAppleRGB(),
                new ProfileCieRGB(), new ProfileWideGamut(), new ProfilePalSecam(), new ProfileCustom() };
        }

        public ColorProfile GetCustomProfile()
        {
            return ColorProfiles.Where(c => c.GetType() == typeof(ProfileCustom)).First();
        }
    }
}
