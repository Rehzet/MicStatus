using System;
using System.Windows.Forms;
using System.Threading;

using NAudio.CoreAudioApi;


namespace MicStatus
{
    public partial class Form1 : Form
    {

        private MMDevice mic;

        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;

            var enumerator = new MMDeviceEnumerator();
            mic = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);

        }


        private void toggleMic()
        {
            mic.AudioEndpointVolume.Mute = !mic.AudioEndpointVolume.Mute;
            setMicIcon();
        }

        private void setMicIcon()
        {
            if (mic.AudioEndpointVolume.Mute)
                notifyIcon1.Icon = Properties.Resources.Mic_muted;
            else
                notifyIcon1.Icon = Properties.Resources.Mic;
        }

   

        // --------- EVENTS ---------

        private void Form1_Load(object sender, EventArgs e)
        {

            // Load status
            trackBar1.Value = (int)(mic.AudioEndpointVolume.MasterVolumeLevelScalar * 100);
            setMicIcon();    

            mic.AudioEndpointVolume.OnVolumeNotification += Microphone_OnVolumeNotification; // Microphone event.

            exitItem.Click += taskbarMenu_exitItem_Click;


        }

        private void Microphone_OnVolumeNotification(AudioVolumeNotificationData data)
        {
            setMicIcon();

            Thread thread = new Thread(delegate() {

                MicPopup micPopup = new MicPopup(data.Muted);
                micPopup.runAnimation();
                micPopup.Dispose();
            });

            thread.Start();
            GC.Collect();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            toggleMic();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            mic.AudioEndpointVolume.MasterVolumeLevelScalar = (float)trackBar1.Value/100;                
        }

        private void notifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            String status = "";
            if (mic.AudioEndpointVolume.Mute)
                status = "Silenciado";
            else
                status = (int)(mic.AudioEndpointVolume.MasterVolumeLevelScalar * 100) + "%";
            notifyIcon1.Text = "Micrófono: " + status;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            /*
                this.WindowState = FormWindowState.Normal;
                this.Visible = true;
                this.ShowInTaskbar = true;
            */  

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                this.Visible = false;
                this.ShowInTaskbar = false;
                
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                toggleMic();
            }
            
        }

        private void taskbarMenu_exitItem_Click(Object sender, System.EventArgs e)
        {
            Application.Exit();
        }
    }
}
