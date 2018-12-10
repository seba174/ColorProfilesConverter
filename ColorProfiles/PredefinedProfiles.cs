
namespace ColorProfiles
{
    public class ProfileSRGB : ColorProfile
    {
        public ProfileSRGB()
        {
            Name = "sRGB";

            Red_X = 0.64;
            Red_Y = 0.33;

            Green_X = 0.3;
            Green_Y = 0.6;

            Blue_X = 0.15;
            Blue_Y = 0.06;

            White_X = 0.3127;
            White_Y = 0.329;

            Gamma = 2.2;
        }
    }

    public class ProfileAdobeRGB : ColorProfile
    {
        public ProfileAdobeRGB()
        {
            Name = "Adobe RGB";

            Red_X = 0.64;
            Red_Y = 0.33;

            Green_X = 0.21;
            Green_Y = 0.71;

            Blue_X = 0.15;
            Blue_Y = 0.06;

            White_X = 0.3127;
            White_Y = 0.3290;

            Gamma = 2.2;
        }
    }

    public class ProfileAppleRGB : ColorProfile
    {
        public ProfileAppleRGB()
        {
            Name = "Apple RGB";

            Red_X = 0.625;
            Red_Y = 0.34;

            Green_X = 0.28;
            Green_Y = 0.595;

            Blue_X = 0.155;
            Blue_Y = 0.07;

            White_X = 0.3127;
            White_Y = 0.3290;

            Gamma = 1.8;
        }
    }

    public class ProfileCieRGB : ColorProfile
    {
        public ProfileCieRGB()
        {
            Name = "CIE RGB";

            Red_X = 0.735;
            Red_Y = 0.265;

            Green_X = 0.274;
            Green_Y = 0.717;

            Blue_X = 0.167;
            Blue_Y = 0.009;

            White_X = 0.3333;
            White_Y = 0.3333;

            Gamma = 2.2;
        }
    }

    public class ProfileWideGamut : ColorProfile
    {
        public ProfileWideGamut()
        {
            Name = "Wide Gamut";

            Red_X = 0.7347;
            Red_Y = 0.2653;

            Green_X = 0.1152;
            Green_Y = 0.8264;

            Blue_X = 0.1566;
            Blue_Y = 0.0177;

            White_X = 0.3457;
            White_Y = 0.3585;

            Gamma = 1.2;
        }
    }

    public class ProfilePalSecam : ColorProfile
    {
        public ProfilePalSecam()
        {
            Name = "PAL/SECAM";

            Red_X = 0.64;
            Red_Y = 0.33;

            Green_X = 0.29;
            Green_Y = 0.6;

            Blue_X = 0.15;
            Blue_Y = 0.06;

            White_X = 0.3127;
            White_Y = 0.329;

            Gamma = 1.95;
        }
    }

    public class ProfileCustom : ColorProfile
    {
        public ProfileCustom()
        {
            Name = "Custom";

            Red_X = 0.64;
            Red_Y = 0.33;

            Green_X = 0.3;
            Green_Y = 0.6;

            Blue_X = 0.15;
            Blue_Y = 0.06;

            White_X = 0.3127;
            White_Y = 0.329;

            Gamma = 2.2;
        }
    }
}
