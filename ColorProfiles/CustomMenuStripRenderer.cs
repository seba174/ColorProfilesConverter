using System.Drawing;
using System.Windows.Forms;

namespace ColorProfiles
{
    public class CustomMenuStripRenderer : ToolStripProfessionalRenderer
    {
        public CustomMenuStripRenderer(Color color)
            : base(new CustomsColorsForMenuStrip(color)) { }
    }

    public class CustomsColorsForMenuStrip : ProfessionalColorTable
    {
        private readonly Color color;

        public CustomsColorsForMenuStrip(Color c) => color = c;

        public override Color MenuItemSelected => color;
        public override Color MenuItemBorder => color;
        public override Color MenuItemSelectedGradientBegin => color;
        public override Color MenuItemSelectedGradientEnd => color;
    }
}
