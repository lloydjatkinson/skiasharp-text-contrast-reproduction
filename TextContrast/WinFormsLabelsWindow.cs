using System.Drawing;
using System.Windows.Forms;
using Windows.Win32.Foundation;

namespace TextContrast
{
    // The sample lines in Windows Forms labels, which draw their text with GDI and ClearType, in Segoe UI at the size of each line in
    // points (three quarters of its size in pixels).
    internal sealed class WinFormsLabelsWindow : Form
    {
        private WinFormsLabelsWindow()
        {
            this.Text = "TextContrast: Windows Forms";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.Manual;
            this.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = Color.FromArgb(Samples.Background, Samples.Background, Samples.Background);
            foreach (var line in Samples.Lines)
            {
                this.Controls.Add(new Label
                {
                    Text = line.Text,
                    AutoSize = true,
                    UseMnemonic = false,
                    Font = new Font("Segoe UI", line.Size * 0.75F),
                    ForeColor = Color.Black,
                    Location = new Point(this.Scale(Samples.Left), this.Scale(line.Top)),
                });
            }

            this.ClientSize = new Size(this.Scale(Samples.Width), this.Scale(Samples.Height));
        }

        public static HWND Open(int x, int y)
        {
            var window = new WinFormsLabelsWindow { Location = new Point(x, y) };
            window.Show();
            return new HWND(window.Handle);
        }

        private int Scale(int value)
        {
            return value * this.DeviceDpi / 96;
        }
    }
}