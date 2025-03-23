using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Diagnostics;

class TempGlass : Form
{
    private Panel drawingPanel;
    private Timer updateTimer;
    private int[] directionStatic = { 2, 1 };
    private int[] directionDynamic;
    private Image pngImage;

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

    private static readonly IntPtr HWND_BOTTOM = new IntPtr(1); // Moves window to the bottom
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_SHOWWINDOW = 0x0040;

    // Override method to hide the form from alt-tab [https://www.csharp411.com/hide-form-from-alttab/]
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= 0x80;
            return cp;
        }
    }

    public TempGlass()
    {
        this.directionDynamic = (int[])this.directionStatic.Clone();
        int middleWidth = Screen.PrimaryScreen.Bounds.Width / 2;
        int middleHeight = Screen.PrimaryScreen.Bounds.Height / 2;
        this.FormBorderStyle = FormBorderStyle.None;
        this.MinimumSize = new Size(1, 1);
        this.Size = new Size(100, 100);
        //this.BackColor = Color.Transparent;


        this.ShowInTaskbar = false;
        this.Padding = new Padding(0);
        this.AutoSize = false;

        this.Location = new Point(middleWidth - this.Size.Width / 2, middleHeight - this.Size.Height / 2);
        this.Load += new EventHandler(LoadWindow);
        this.Text = "Bouncing";
        this.StartPosition = FormStartPosition.Manual;

        this.SendToBack();

        // Set the position to the top middle of the screen
        pngImage = Image.FromFile("./sudowoodo.png");

        // Add the panel witht he contents
        drawingPanel = new Panel
        {
            Size = new Size(100, 100),
            Location = new Point(0, 0),
            BackColor = Color.Transparent,
        };
        drawingPanel.Paint += new PaintEventHandler(DrawPanel); // Add paint callback
        this.Controls.Add(drawingPanel);

        // Adda  timer for async updates
        updateTimer = new Timer();
        updateTimer.Interval = 10;
        updateTimer.Tick += UpdateTimer_Tick;
        updateTimer.Start();
    }

    private async void UpdateTimer_Tick(object sender, EventArgs e)
    {
        if (this.directionDynamic[0] != 0)
        {
            if (this.directionDynamic[0] > 0)
            {
                this.Location = new Point(this.Location.X + 1, this.Location.Y);
                this.directionDynamic[0]--;
            }
            else
            {
                this.Location = new Point(this.Location.X - 1, this.Location.Y);
                this.directionDynamic[0]++;
            }
        }
        if (this.directionDynamic[1] != 0)
        {
            if (this.directionDynamic[1] > 0)
            {
                this.Location = new Point(this.Location.X, this.Location.Y + 1);
                this.directionDynamic[1]--;
            }
            else
            {
                this.Location = new Point(this.Location.X, this.Location.Y - 1);
                this.directionDynamic[1]++;
            }
        }

        if (this.directionDynamic[0] == 0 && this.directionDynamic[0] == 0)
        {
            this.directionDynamic = (int[])this.directionStatic.Clone();
        }

        // check boundary
        if (this.Location.X + this.Size.Width >= Screen.PrimaryScreen.Bounds.Width || this.Location.X <= 0)
        {
            this.directionStatic[0] = -this.directionStatic[0];
            this.directionDynamic[0] = this.directionStatic[0];
        }

        if (this.Location.Y + this.Size.Height >= Screen.PrimaryScreen.Bounds.Height || this.Location.Y <= 0)
        {
            this.directionStatic[1] = -this.directionStatic[1];
            this.directionDynamic[1] = this.directionStatic[1];
        }
    }

    // Updating the panel when need be
    private void DrawPanel(object sender, PaintEventArgs e)
    {
        Graphics g = e.Graphics;
        g.DrawImage(pngImage, new Rectangle(0, 0, 100, 100));
    }

    // Processing creating the windows
    private void LoadWindow(object sender, EventArgs e)
    {
        IntPtr hWnd = this.Handle;
        int currentStyle = GetWindowLong(hWnd, -20);
        SetWindowLong(hWnd, -20, currentStyle | WS_EX_LAYERED | WS_EX_TOPMOST | WS_EX_TRANSPARENT);
        SetLayeredWindowAttributes(hWnd, 128, 255, LWA_ALPHA);

        SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE | SWP_SHOWWINDOW);
    }

    [STAThread]
    static void Main()
    {
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.Run(new TempGlass());
    }
}
