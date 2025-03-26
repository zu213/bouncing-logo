using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Reflection;

class BouncingLogoScreensaver : Form
{
    private Timer updateTimer;
    private int[] directionStatic = { 1, 2 };
    private int[] directionDynamic;
    private Image pngImage;
    private Panel drawingPanel;

    // Constants for window styles
    private const int WS_EX_LAYERED = 0x00080000;
    private const int WS_EX_TOPMOST = 0x00000008;
    private const int WS_EX_TRANSPARENT = 0x00000020;
    private const int LWA_ALPHA = 0x00000003;

    // Import Windows API functions
    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, byte bAlpha, int dwFlags);

    [DllImport("user32.dll", SetLastError = true)]

    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

    public BouncingLogoScreensaver()
    {
        this.directionDynamic = (int[])this.directionStatic.Clone();
        this.FormBorderStyle = FormBorderStyle.None;
        this.StartPosition = FormStartPosition.Manual;
        this.DoubleBuffered = true;
        this.Location = new Point(0,0);
        this.Text = "Bouncing";
        this.WindowState = FormWindowState.Maximized;
        this.FormBorderStyle = FormBorderStyle.None;
        this.TopMost = true;
        this.BackColor = Color.Black;
        Cursor.Hide();

        drawingPanel = new Panel
        {
            Size = new Size(100, 100),
            Location = new Point(600, 600),
            BackColor = Color.Black
        };
        drawingPanel.Paint += new PaintEventHandler(DrawPanel); // Add paint callback
        this.Controls.Add(drawingPanel);

        this.SendToBack();

        pngImage = LoadImageFromResources("BouncingLogo.sudowoodo.png");

        this.KeyDown += (sender, e) => Application.Exit();
        //this.MouseMove += (sender, e) => Application.Exit(); // removed as too sensitive sometimes
        this.MouseClick += (sender, e) => Application.Exit();

        // Adda  timer for async updates
        updateTimer = new Timer();
        updateTimer.Interval = 10;
        updateTimer.Tick += UpdateTimer_Tick;
        updateTimer.Start();

        this.Load += new EventHandler(LoadWindow);
    }

    private Image LoadImageFromResources(string resourceName) // Loaded this way so we can bundle image together
    {
        var assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            return Image.FromStream(stream);
        }
    }

    private void DrawPanel(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        e.Graphics.DrawImage(pngImage, new Rectangle(0, 0, 100, 100));
    }
    private async void UpdateTimer_Tick(object sender, EventArgs e)
    {
        if (this.directionDynamic[0] != 0)
        {
            if (this.directionDynamic[0] > 0)
            {
                Point point = new Point(drawingPanel.Location.X + 1, drawingPanel.Location.Y);
                drawingPanel.Location = point;
                this.directionDynamic[0]--;
            }
            else
            {
                Point point = new Point(drawingPanel.Location.X - 1, drawingPanel.Location.Y);
                drawingPanel.Location = point;
                this.directionDynamic[0]++;
            }
        }
        if (this.directionDynamic[1] != 0)
        {
            if (this.directionDynamic[1] > 0)
            {
                Point point = new Point(drawingPanel.Location.X, drawingPanel.Location.Y + 1);
                drawingPanel.Location = point;
                this.directionDynamic[1]--;
            }
            else
            {
                Point point = new Point(drawingPanel.Location.X, drawingPanel.Location.Y - 1);
                drawingPanel.Location = point;
                this.directionDynamic[1]++;
            }
        }

        if (this.directionDynamic[0] == 0 && this.directionDynamic[0] == 0)
        {
            this.directionDynamic = (int[])this.directionStatic.Clone();
        }

        // check boundary
        if (drawingPanel.Location.X + drawingPanel.Size.Width >= Screen.PrimaryScreen.Bounds.Width || drawingPanel.Location.X <= 0)
        {
            this.directionStatic[0] = -this.directionStatic[0];
            this.directionDynamic[0] = this.directionStatic[0];
        }

        if (drawingPanel.Location.Y + drawingPanel.Size.Height >= Screen.PrimaryScreen.Bounds.Height || drawingPanel.Location.Y <= 0)
        {
            this.directionStatic[1] = -this.directionStatic[1];
            this.directionDynamic[1] = this.directionStatic[1];
        }
    }


    // Processing creating the windows
    private void LoadWindow(object sender, EventArgs e)
    {
        IntPtr hWnd = this.Handle;
        int currentStyle = GetWindowLong(hWnd, -20);
        SetWindowLong(hWnd, -20, currentStyle | WS_EX_LAYERED | WS_EX_TOPMOST);
        SetLayeredWindowAttributes(hWnd, 128, 255, LWA_ALPHA);
    }

    [STAThread]
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            if (args[0] == "/s")
            {
                Application.Run(new BouncingLogoScreensaver());
            }
            else if (args[0] == "/c")
            {
                MessageBox.Show("Configuration not implemented.");
            }
            else if (args[0] == "/p")
            {
                Application.Run(new BouncingLogoScreensaver());
            }
        }
        else
        {
            Application.Run(new BouncingLogoScreensaver());
        }
    }

}
