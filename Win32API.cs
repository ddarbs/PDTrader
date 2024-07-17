using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static PDTrader.Library;

namespace PDTrader
{
    internal class Win32API
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr FindWindow(string _class, string _title);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool SetWindowText(IntPtr _hWnd, String _lpString);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);
        
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        internal static extern Int32 SendMessage(IntPtr _hWnd, uint _msg, IntPtr _wParam, IntPtr _lParam);

        [DllImport("user32.dll")]
        internal static extern Int32 PostMessage(IntPtr _hWnd, uint _msg, IntPtr _wParam, IntPtr _lParam);

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);
        
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);
        
        [DllImport("user32.dll")]
        static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);
        
        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs,  [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);
        
        internal static void SendTextMessage(IntPtr _hWnd, string _text)
        {
            Thread.Sleep(50);
            foreach (char c in _text)
            {
                switch (c)
                {
                    // UPPER CASE
                    case 'A':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_A_UPPER, 0);
                        break;
                    case 'B':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_B_UPPER, 0);
                        break;
                    case 'C':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_C_UPPER, 0);
                        break;
                    case 'D':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_D_UPPER, 0);
                        break;
                    case 'E':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_E_UPPER, 0);
                        break;
                    case 'F':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_F_UPPER, 0);
                        break;
                    case 'G':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_G_UPPER, 0);
                        break;
                    case 'H':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_H_UPPER, 0);
                        break;
                    case 'I':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_I_UPPER, 0);
                        break;
                    case 'J':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_J_UPPER, 0);
                        break;
                    case 'K':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_K_UPPER, 0);
                        break;
                    case 'L':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_L_UPPER, 0);
                        break;
                    case 'M':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_M_UPPER, 0);
                        break;
                    case 'N':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_N_UPPER, 0);
                        break;
                    case 'O':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_O_UPPER, 0);
                        break;
                    case 'P':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_P_UPPER, 0);
                        break;
                    case 'Q':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_Q_UPPER, 0);
                        break;
                    case 'R':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_R_UPPER, 0);
                        break;
                    case 'S':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_S_UPPER, 0);
                        break;
                    case 'T':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_T_UPPER, 0);
                        break;
                    case 'U':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_U_UPPER, 0);
                        break;
                    case 'V':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_V_UPPER, 0);
                        break;
                    case 'W':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_W_UPPER, 0);
                        break;
                    case 'X':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_X_UPPER, 0);
                        break;
                    case 'Y':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_Y_UPPER, 0);
                        break;
                    case 'Z':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_Z_UPPER, 0);
                        break;
                    // LOWER CASE
                    case 'a':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_A_LOWER, 0);
                        break;
                    case 'b':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_B_LOWER, 0);
                        break;
                    case 'c':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_C_LOWER, 0);
                        break;
                    case 'd':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_D_LOWER, 0);
                        break;
                    case 'e':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_E_LOWER, 0);
                        break;
                    case 'f':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_F_LOWER, 0);
                        break;
                    case 'g':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_G_LOWER, 0);
                        break;
                    case 'h':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_H_LOWER, 0);
                        break;
                    case 'i':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_I_LOWER, 0);
                        break;
                    case 'j':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_J_LOWER, 0);
                        break;
                    case 'k':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_K_LOWER, 0);
                        break;
                    case 'l':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_L_LOWER, 0);
                        break;
                    case 'm':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_M_LOWER, 0);
                        break;
                    case 'n':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_N_LOWER, 0);
                        break;
                    case 'o':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_O_LOWER, 0);
                        break;
                    case 'p':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_P_LOWER, 0);
                        break;
                    case 'q':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_Q_LOWER, 0);
                        break;
                    case 'r':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_R_LOWER, 0);
                        break;
                    case 's':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_S_LOWER, 0);
                        break;
                    case 't':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_T_LOWER, 0);
                        break;
                    case 'u':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_U_LOWER, 0);
                        break;
                    case 'v':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_V_LOWER, 0);
                        break;
                    case 'w':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_W_LOWER, 0);
                        break;
                    case 'x':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_X_LOWER, 0);
                        break;
                    case 'y':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_Y_LOWER, 0);
                        break;
                    case 'z':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_Z_LOWER, 0);
                        break;
                    // NUMBERS
                    case '0':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_ZERO, 0);
                        break;
                    case '1':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_ONE, 0);
                        break;
                    case '2':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_TWO, 0);
                        break;
                    case '3':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_THREE, 0);
                        break;
                    case '4':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_FOUR, 0);
                        break;
                    case '5':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_FIVE, 0);
                        break;
                    case '6':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_SIX, 0);
                        break;
                    case '7':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_SEVEN, 0);
                        break;
                    case '8':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_EIGHT, 0);
                        break;
                    case '9':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_NINE, 0);
                        break;
                    // EXTENDED
                    case ' ':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_SPACE, 0);
                        break;
                    case '\'':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_APOSTROPHE, 0);
                        break;
                    case '(':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_ROUNDBRACKETOPEN, 0);
                        break;
                    case ')':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_ROUNDBRACKETCLOSE, 0);
                        break;
                    case ',':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_COMMA, 0);
                        break;
                    case '.':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_PERIOD, 0);
                        break;
                    case '/':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_COMMANDSLASH, 0);
                        break;
                    case ':':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_COLON, 0);
                        break;
                    case '?':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_QUESTIONMARK, 0);
                        break;
                    case '|':
                        SendMessage(_hWnd, Library.WM_CHAR, Library.ASCII_VERTICALBAR, 0);
                        break;
                }
                Thread.Sleep(50);
            }
        }

        internal static void HitEnter(IntPtr _hWnd)
        {
            SendMessage(_hWnd, Library.WM_KEYDOWN, Library.VK_RETURN, 1);
            Thread.Sleep(100);
            SendMessage(_hWnd, Library.WM_KEYUP, Library.VK_RETURN, 0);
            Thread.Sleep(100);
        }
        
        internal static void MouseMove(int[] _position)
        {
            bool _moveSuccess = false;
            while (!_moveSuccess)
            {
                _moveSuccess = SetCursorPos(_position[0], _position[1]);
                Thread.Sleep(10);
            }
            Thread.Sleep(100);
        }

        /*internal static void MouseClick(IntPtr _hWnd, int[] _position)
        {
            PostMessage(_hWnd, WM_LBUTTONDOWN, 1, MakeXYParam(_position[0], _position[1]));
            Thread.Sleep(200);
            PostMessage(_hWnd, WM_LBUTTONUP, 0, MakeXYParam(_position[0], _position[1]));
            Thread.Sleep(200);
        }*/
        
        internal static void MouseClickTest()
        {
            uint _DownSuccess = 0;
            INPUT[] _Down = new INPUT[1];
            _Down[0].type = 0;
            _Down[0].U.mi.time = 0;
            _Down[0].U.mi.dwFlags = MOUSEEVENTF.LEFTDOWN | MOUSEEVENTF.ABSOLUTE;
            _Down[0].U.mi.dwExtraInfo = UIntPtr.Zero;
            _Down[0].U.mi.dx = 1;
            _Down[0].U.mi.dy = 1;
            while (_DownSuccess == 0)
            {
                _DownSuccess = SendInput(1, _Down, INPUT.Size);
                Thread.Sleep(10);
            }
            Thread.Sleep(50);
            
            uint _UpSuccess = 0;
            INPUT[] _Up = new INPUT[1];
            _Up[0].type = 0;
            _Up[0].U.mi.time = 0;
            _Up[0].U.mi.dwFlags = MOUSEEVENTF.LEFTUP | MOUSEEVENTF.ABSOLUTE;
            _Up[0].U.mi.dwExtraInfo = UIntPtr.Zero;
            _Up[0].U.mi.dx = 1;
            _Up[0].U.mi.dy = 1;
            while (_UpSuccess == 0)
            {
                _UpSuccess = SendInput(1, _Up, INPUT.Size);
                Thread.Sleep(10);
            }
            Thread.Sleep(50);
        }
        
        /*internal static void MouseDrag(IntPtr _hWnd, int[] _sourcePosition, int[] _destinationPosition)
        {
            Thread.Sleep(500);
            int _BreakIndex = 0;
            int _Down = 0;
            while (_Down == 0)
            {
                _Down = SendMessage(_hWnd, WM_LBUTTONDOWN, 1, MakeXYParam(_sourcePosition[0], _sourcePosition[1]));
                _BreakIndex++;
                if (_BreakIndex > 100)
                {
                    Debug.WriteLine("[ERROR] shits fucked");
                    break;
                }
                Thread.Sleep(10);
            }
            
            Thread.Sleep(200);
            
            MouseMove(_destinationPosition);

            _BreakIndex = 0;
            int _Up = 0;
            while (_Up == 0)
            {
                _Up = SendMessage(_hWnd, WM_LBUTTONUP, 0, MakeXYParam(_destinationPosition[0], _destinationPosition[1]));
                _BreakIndex++;
                if (_BreakIndex > 100)
                {
                    Debug.WriteLine("[ERROR] shits fucked");
                    break;
                }
                Thread.Sleep(10);
            }
            
            Thread.Sleep(200);
        }*/
        
        internal static void MouseScrollDownTest()
        {
            uint _ScrollDownSuccess = 0;
            INPUT[] _ScrollDown = new INPUT[1];
            _ScrollDown[0].type = 0;
            _ScrollDown[0].U.mi.time = 0;
            _ScrollDown[0].U.mi.dwFlags = MOUSEEVENTF.WHEEL | MOUSEEVENTF.ABSOLUTE;
            _ScrollDown[0].U.mi.mouseData = -120;
            _ScrollDown[0].U.mi.dwExtraInfo = UIntPtr.Zero;
            _ScrollDown[0].U.mi.dx = 1;
            _ScrollDown[0].U.mi.dy = 1;
            while (_ScrollDownSuccess == 0)
            {
                _ScrollDownSuccess = SendInput(1, _ScrollDown, INPUT.Size);
                Thread.Sleep(10);
            }
            
            Thread.Sleep(50);
            
        }
        
        internal static void MouseDragTest(int[] _destinationPosition)
        {
            uint _DownSuccess = 0;
            INPUT[] _Down = new INPUT[1];
            _Down[0].type = 0;
            _Down[0].U.mi.time = 0;
            _Down[0].U.mi.dwFlags = MOUSEEVENTF.LEFTDOWN | MOUSEEVENTF.ABSOLUTE;
            _Down[0].U.mi.dwExtraInfo = UIntPtr.Zero;
            _Down[0].U.mi.dx = 1;
            _Down[0].U.mi.dy = 1;
            while (_DownSuccess == 0)
            {
                _DownSuccess = SendInput(1, _Down, INPUT.Size);
                Thread.Sleep(10);
            }
            Thread.Sleep(50);
            
            MouseMove(_destinationPosition);
            
            uint _UpSuccess = 0;
            INPUT[] _Up = new INPUT[1];
            _Up[0].type = 0;
            _Up[0].U.mi.time = 0;
            _Up[0].U.mi.dwFlags = MOUSEEVENTF.LEFTUP | MOUSEEVENTF.ABSOLUTE;
            _Up[0].U.mi.dwExtraInfo = UIntPtr.Zero;
            _Up[0].U.mi.dx = 1;
            _Up[0].U.mi.dy = 1;
            while (_UpSuccess == 0)
            {
                _UpSuccess = SendInput(1, _Up, INPUT.Size);
                Thread.Sleep(10);
            }
            Thread.Sleep(50);
        }

        internal static void MouseRehoverItem()
        {
            POINT _lpPoint = new POINT();
            GetCursorPos(out _lpPoint);

            int[] _UpPosition = new[] { _lpPoint.X, _lpPoint.Y - 48 };
            int[] _DownPosition = new[] { _lpPoint.X, _lpPoint.Y };
            MouseMove(_UpPosition);
            MouseMove(_DownPosition);
        }
    }
}
