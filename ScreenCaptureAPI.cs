using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PDTrader.Library;

namespace PDTrader
{
    internal static class ScreenCaptureAPI
    {
        internal static Bitmap CapturePaxDeiWindow(Rectangle _bounds)
        {
            // check for 1920x1080
            /*var rect = new Rect();
            Win32API.GetWindowRect(Main.p_Handle, ref rect);
            int _Width = rect.Right - rect.Left;
            int _Height = rect.Bottom - rect.Top;
            if (_Width < 1919.5f || _Width > 1920.5f || _Height < 1079.5f || _Height > 1080.5f)
            {
                Debug.WriteLine("[ERROR] window isn't 1920x1080, the bot will not work correctly");
                Debug.WriteLine("[ERROR] window isn't 1920x1080, the bot will not work correctly");
                Debug.WriteLine("[ERROR] window isn't 1920x1080, the bot will not work correctly");
                Thread.Sleep(600);
            }*/
            
            return CaptureWindow(_bounds);
        }

        // 335x39 size
        // at 83, 918
        // this expects chat to be open
        private static Bitmap CaptureWindow(Rectangle _bounds)
        {
            var result = new Bitmap(_bounds.Width, _bounds.Height);

            using (var graphics = Graphics.FromImage(result))
            {
                graphics.CopyFromScreen(new Point(_bounds.Left, _bounds.Top), Point.Empty, _bounds.Size);
            }

            return result;
        }
    }
}
