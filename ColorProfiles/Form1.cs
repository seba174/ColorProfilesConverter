using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ColorProfiles
{
    public partial class Form1 : Form
    {
        private ProfileManager SourceProfilesManager;
        private ProfileManager TargetProfilesManager;

        public Form1()
        {
            InitializeComponent();

            SourceProfilesManager = new ProfileManager();
            TargetProfilesManager = new ProfileManager();

            menuStrip.Renderer = new CustomMenuStripRenderer(Color.FromArgb(100, 100, 100));

            SourceProfiles.Items.AddRange(SourceProfilesManager.ColorProfiles);
            TargetProfiles.Items.AddRange(TargetProfilesManager.ColorProfiles);

            OriginalPicture.SizeMode = PictureBoxSizeMode.StretchImage;
            ConvertedPicture.SizeMode = PictureBoxSizeMode.StretchImage;

            loadPictureButton.Click += LoadPicture;
            savePictureButton.Click += SavePicture;
            convertToGrayScaleButton.Click += ConvertToGrayScale;
            generatePictureButton.Click += GenerateTargetPicture;
            SourceProfiles.SelectedIndexChanged += ChangeSourceProfileColor;
            TargetProfiles.SelectedIndexChanged += ChangeTargetProfileColor;

            var textboxes = this.FindAllChildrenByType<TextBox>();
            var sourceTextBoxes = tableLayoutPanel2.FindAllChildrenByType<TextBox>();
            var targetTextBoxes = tableLayoutPanel3.FindAllChildrenByType<TextBox>();

            int s = sourceTextBoxes.Count();
            int t = targetTextBoxes.Count();

            foreach (var textbox in textboxes)
            {
                textbox.ShortcutsEnabled = false;
                textbox.KeyPress += TextBoxKeyPress;
                textbox.Validating += TextBoxValidating;
            }
            foreach (var textbox in sourceTextBoxes)
            {
                textbox.TextChanged += ChangeSourceProfileToCustom;
            }
            foreach (var textbox in targetTextBoxes)
            {
                textbox.TextChanged += ChangeTargetProfileToCustom;
            }

            SourceProfiles.SelectedIndex = 0;
            TargetProfiles.SelectedIndex = 0;

            OriginalPicture.Image = Properties.Resources.basePicture;
        }

        private void GenerateTargetPicture(object sender, EventArgs e)
        {
            ColorProfile sourceProfile = SourceProfiles.SelectedItem as ColorProfile;
            ColorProfile targetProfile = TargetProfiles.SelectedItem as ColorProfile;

            Matrix matrixSourceSrSgSb = sourceProfile.XYZConverter();
            Matrix matrixTargetSrSgSb = targetProfile.XYZConverter();

            Bitmap bitmap = OriginalPicture.Image as Bitmap;
            Bitmap newBitmap = new Bitmap(OriginalPicture.Image.Size.Width, OriginalPicture.Image.Size.Height);

            var p = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            using (FastBitmap fastBitmap = bitmap.GetLockedFastBitmap())
            using (FastBitmap newFastBitmap = newBitmap.GetLockedFastBitmap())
            {
                int step = (int)Math.Ceiling((double)fastBitmap.Width / Environment.ProcessorCount);
                Parallel.For(0, Environment.ProcessorCount, p, i =>
                {
                    int end = (i + 1) * step;
                    for (int k = i * step; k < end && k < fastBitmap.Width; k++)
                    {
                        for (int j = 0; j < fastBitmap.Height; j++)
                        {
                            Color color = fastBitmap.GetPixel(k, j);
                            var rgb = color.ToVector().Divide(255).PointwisePower(sourceProfile.Gamma);
                            Vector<double> XYZ = matrixSourceSrSgSb * rgb;

                            Vector<double> RtGtBt = matrixTargetSrSgSb.Solve(XYZ);

                            var res = RtGtBt.PointwisePower(1 / targetProfile.Gamma).Multiply(255);

                            Color newColor = Utilities.ColorFromRgbSafely(res);

                            newFastBitmap.SetPixel(k, j, newColor);
                        }
                    }
                });
            }
            Image old = ConvertedPicture.Image;
            ConvertedPicture.Image = newBitmap;
            old?.Dispose();
        }

        private void ChangeSourceProfileToCustom(object sender, EventArgs e)
        {
            var currentlySelectedProfile = SourceProfiles.SelectedItem as ColorProfile;
            var customProfile = SourceProfilesManager.GetCustomProfile();
            if (currentlySelectedProfile.Name != ProfileManager.CustomProfileName)
            {
                SourceProfiles.SelectedIndexChanged -= ChangeSourceProfileColor;
                MapSourceTextBoxesToProfile(customProfile);
                SourceProfiles.SelectedItem = customProfile;
                SourceProfiles.SelectedIndexChanged += ChangeSourceProfileColor;
            }
            else
            {
                MapSourceTextBoxesToProfile(customProfile);
            }
        }

        private void ChangeTargetProfileToCustom(object sender, EventArgs e)
        {
            var currentlySelectedProfile = TargetProfiles.SelectedItem as ColorProfile;
            var customProfile = TargetProfilesManager.GetCustomProfile();
            if (currentlySelectedProfile.Name != ProfileManager.CustomProfileName)
            {
                TargetProfiles.SelectedIndexChanged -= ChangeTargetProfileColor;
                MapTargetTextBoxesToProfile(customProfile);
                TargetProfiles.SelectedItem = customProfile;
                TargetProfiles.SelectedIndexChanged += ChangeTargetProfileColor;
            }
            else
            {
                MapTargetTextBoxesToProfile(customProfile);
            }
        }

        private void ChangeTargetProfileColor(object sender, EventArgs e)
        {
            var targetTextBoxes = tableLayoutPanel3.FindAllChildrenByType<TextBox>();
            foreach (var textbox in targetTextBoxes)
            {
                textbox.TextChanged -= ChangeTargetProfileToCustom;
            }

            var selectedItem = (ColorProfile)TargetProfiles.SelectedItem;
            TargetRed_X.Text = selectedItem.Red_X.ToString(CultureInfo.InvariantCulture);
            TargetRed_Y.Text = selectedItem.Red_Y.ToString(CultureInfo.InvariantCulture);
            TargetGreen_X.Text = selectedItem.Green_X.ToString(CultureInfo.InvariantCulture);
            TargetGreen_Y.Text = selectedItem.Green_Y.ToString(CultureInfo.InvariantCulture);
            TargetBlue_X.Text = selectedItem.Blue_X.ToString(CultureInfo.InvariantCulture);
            TargetBlue_Y.Text = selectedItem.Blue_Y.ToString(CultureInfo.InvariantCulture);
            TargetWhite_X.Text = selectedItem.White_X.ToString(CultureInfo.InvariantCulture);
            TargetWhite_Y.Text = selectedItem.White_Y.ToString(CultureInfo.InvariantCulture);
            TargetGamma.Text = selectedItem.Gamma.ToString(CultureInfo.InvariantCulture);

            foreach (var textbox in targetTextBoxes)
            {
                textbox.TextChanged += ChangeTargetProfileToCustom;
            }
        }

        private void ChangeSourceProfileColor(object sender, EventArgs e)
        {
            var sourceTextBoxes = tableLayoutPanel2.FindAllChildrenByType<TextBox>();
            foreach (var textbox in sourceTextBoxes)
            {
                textbox.TextChanged -= ChangeSourceProfileToCustom;
            }

            var selectedItem = (ColorProfile)SourceProfiles.SelectedItem;
            SourceRed_X.Text = selectedItem.Red_X.ToString(CultureInfo.InvariantCulture);
            SourceRed_Y.Text = selectedItem.Red_Y.ToString(CultureInfo.InvariantCulture);
            SourceGreen_X.Text = selectedItem.Green_X.ToString(CultureInfo.InvariantCulture);
            SourceGreen_Y.Text = selectedItem.Green_Y.ToString(CultureInfo.InvariantCulture);
            SourceBlue_X.Text = selectedItem.Blue_X.ToString(CultureInfo.InvariantCulture);
            SourceBlue_Y.Text = selectedItem.Blue_Y.ToString(CultureInfo.InvariantCulture);
            SourceWhite_X.Text = selectedItem.White_X.ToString(CultureInfo.InvariantCulture);
            SourceWhite_Y.Text = selectedItem.White_Y.ToString(CultureInfo.InvariantCulture);
            SourceGamma.Text = selectedItem.Gamma.ToString(CultureInfo.InvariantCulture);

            foreach (var textbox in sourceTextBoxes)
            {
                textbox.TextChanged += ChangeSourceProfileToCustom;
            }
        }

        private void LoadPicture(object sender, EventArgs e)
        {
            if (PictureFileDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap bitmap = new Bitmap(PictureFileDialog.FileName);
                Image old = OriginalPicture.Image;
                OriginalPicture.Image = Image.FromFile(PictureFileDialog.FileName);
                old?.Dispose();
                OriginalPicture.Refresh();
            }
        }

        private void SavePicture(object sender, EventArgs e)
        {
            if (ConvertedPicture.Image == null)
            {
                return;
            }

            if (PictureSaveDialog.ShowDialog() == DialogResult.OK)
            {
                ConvertedPicture.Image.Save(PictureSaveDialog.FileName);
            }
        }

        private void ConvertToGrayScale(object sender, EventArgs e)
        {
            Bitmap currentImage = OriginalPicture.Image as Bitmap;
            Bitmap newImage = new Bitmap(currentImage.Width, currentImage.Height);
            using (FastBitmap fastCurrentImage = currentImage.GetLockedFastBitmap())
            using (FastBitmap fastNewImage = newImage.GetLockedFastBitmap())
            {
                for (int i = 0; i < fastCurrentImage.Width; i++)
                {
                    for (int j = 0; j < fastCurrentImage.Height; j++)
                    {
                        Color c = fastCurrentImage.GetPixel(i, j);
                        int value = (int)(0.299 * c.R + 0.587 * c.G + 0.114 * c.B);
                        value = value > 255 ? 255 : value;
                        fastNewImage.SetPixel(i, j, Color.FromArgb(value, value, value));
                    }
                }
            }

            OriginalPicture.Image = newImage;
            currentImage.Dispose();
        }

        private void MapSourceTextBoxesToProfile(ColorProfile profile)
        {
            try
            {
                int def = 0;
                profile.Red_X = def;
                profile.Red_X = double.Parse(SourceRed_X.Text, CultureInfo.InvariantCulture);
                profile.Red_Y = def;
                profile.Red_Y = double.Parse(SourceRed_Y.Text, CultureInfo.InvariantCulture);
                profile.Green_X = def;
                profile.Green_X = double.Parse(SourceGreen_X.Text, CultureInfo.InvariantCulture);
                profile.Green_Y = def;
                profile.Green_Y = double.Parse(SourceGreen_Y.Text, CultureInfo.InvariantCulture);
                profile.Blue_X = def;
                profile.Blue_X = double.Parse(SourceBlue_X.Text, CultureInfo.InvariantCulture);
                profile.Blue_Y = def;
                profile.Blue_Y = double.Parse(SourceBlue_Y.Text, CultureInfo.InvariantCulture);
                profile.White_X = def;
                profile.White_X = double.Parse(SourceWhite_X.Text, CultureInfo.InvariantCulture);
                profile.White_Y = def;
                profile.White_Y = double.Parse(SourceWhite_Y.Text, CultureInfo.InvariantCulture);
                profile.Gamma = def;
                profile.Gamma = double.Parse(SourceGamma.Text, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
        }

        private void MapTargetTextBoxesToProfile(ColorProfile profile)
        {
            try
            {
                int def = 0;
                profile.Red_X = def;
                profile.Red_X = double.Parse(TargetRed_X.Text, CultureInfo.InvariantCulture);
                profile.Red_Y = def;
                profile.Red_Y = double.Parse(TargetRed_Y.Text, CultureInfo.InvariantCulture);
                profile.Green_X = def;
                profile.Green_X = double.Parse(TargetGreen_X.Text, CultureInfo.InvariantCulture);
                profile.Green_Y = def;
                profile.Green_Y = double.Parse(TargetGreen_Y.Text, CultureInfo.InvariantCulture);
                profile.Blue_X = def;
                profile.Blue_X = double.Parse(TargetBlue_X.Text, CultureInfo.InvariantCulture);
                profile.Blue_Y = def;
                profile.Blue_Y = double.Parse(TargetBlue_Y.Text, CultureInfo.InvariantCulture);
                profile.White_X = def;
                profile.White_X = double.Parse(TargetWhite_X.Text, CultureInfo.InvariantCulture);
                profile.White_Y = def;
                profile.White_Y = double.Parse(TargetWhite_Y.Text, CultureInfo.InvariantCulture);
                profile.Gamma = def;
                profile.Gamma = double.Parse(TargetGamma.Text, CultureInfo.InvariantCulture);
            }
            catch (Exception) { }
        }

        private void TextBoxValidating(object sender, CancelEventArgs e)
        {
            if (sender is TextBox textbox)
            {
                if (textbox.Text.Trim() != string.Empty && !double.TryParse(textbox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                {
                    MessageBox.Show($"{textbox.Name} do not have proper double value");
                }
            }
        }

        private void TextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}
