using System;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MicStatus
{
    public partial class MicPopup : Form
    {

        private bool muted;


        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags); // https://stackoverflow.com/questions/683330/how-to-make-a-window-always-stay-on-top-in-net


        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        ); // https://stackoverflow.com/questions/18822067/rounded-corners-in-c-sharp-windows-forms


        public MicPopup(bool muted)
        {
            this.muted = muted;
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private void MicPopup_Load(object sender, EventArgs e)
        {
            SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);

            int lowerResolutionBound = Math.Min(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            lowerResolutionBound /= 16;

            this.Location = new System.Drawing.Point(lowerResolutionBound, lowerResolutionBound);
            setIcon(muted);

        }

        public void setIcon(bool muted)
        {
            if (muted)
            {
                this.BackgroundImage = Properties.Resources.SndVolSSO_141_0;
            }
            else
            {
                this.BackgroundImage = Properties.Resources.SndVolSSO_140_0;
            }

        }

        public void runAnimation()
        {
            this.Show();

            Thread.Sleep(1000);

            for (int i=0; i<20; i++)
            {
                Thread.Sleep(20); 
                this.Opacity -= 0.05f;
            }
            this.Close();

        }


    }
}
