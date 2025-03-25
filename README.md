Compile and Rename to .scr

    Build the project → Get the .exe file from bin/Release/

    Rename MyScreenSaver.exe to MyScreenSaver.scr

    Move it to C:\Windows\System32

4️⃣ Set It as the Screensaver

    Right-click the desktop → Personalize

    Go to Lock Screen → Screen saver settings

    Select your screensaver from the list

    Click Apply & OK

            this.BackColor = Color.Black;  // Background color
        this.WindowState = FormWindowState.Maximized;  // Fullscreen
        this.FormBorderStyle = FormBorderStyle.None;  // No border
        this.TopMost = true;  // Stay on top

        // Close when any key is pressed
        this.KeyDown += (sender, e) => Application.Exit();
        this.MouseMove += (sender, e) => Application.Exit();
        this.MouseClick += (sender, e) => Application.Exit();

               if (args.Length > 0 && args[0].ToLower().StartsWith("/c"))  
        {
            // Settings mode (optional)
            MessageBox.Show("No settings available.");
        }
        else if (args.Length > 1 && args[0].ToLower().StartsWith("/p"))  
        {
            // Preview mode (small thumbnail in settings)
            Application.Exit();
        }
        else
        {
            // Normal mode (run fullscreen)
            Application.Run(new MyScreenSaver());
        }