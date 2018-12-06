using System.Drawing;
using System.Windows.Forms;

namespace ColorProfiles
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            menuStrip.Renderer = new CustomMenuStripRenderer(Color.FromArgb(100, 100, 100));
        }
    }
}
