using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace PDTrader
{
    internal static class Library
    {
        internal enum TransactionStatus
        {
            Greeting,
            Discussing,
            Trading,
            Thanking
        }

        internal enum Item
        {
            Undecided,
            
            // we buy these
            SteelIngot,
            OrangeHide,
            OrangeLargeHide,
            
            // we sell these
            SteelPickaxe,
            SteelAxe
        }

        internal static readonly Item[] ITEMS_SELLING = new Item[]
        {
            Item.SteelPickaxe,
            Item.SteelAxe
        };
        
        internal static readonly Item[] ITEMS_BUYING = new Item[]
        {
            Item.SteelIngot, 
            Item.OrangeHide, 
            Item.OrangeLargeHide
        };

        [StructLayout(LayoutKind.Sequential)]
        internal struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point) {
                return new Point(point.X, point.Y);
            }
        }
        
        #region SendInput Stuff
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            internal uint type;
            internal InputUnion U;
            internal static int Size
            {
                get { return Marshal.SizeOf(typeof(INPUT)); }
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct InputUnion
        {
            [FieldOffset(0)]
            internal MOUSEINPUT mi;
            [FieldOffset(0)]
            internal KEYBDINPUT ki;
            [FieldOffset(0)]
            internal HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            internal int dx;
            internal int dy;
            internal int mouseData;
            internal MOUSEEVENTF dwFlags;
            internal uint time;
            internal UIntPtr dwExtraInfo;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        [Flags]
        internal enum MOUSEEVENTF : uint
        {
            ABSOLUTE = 0x8000,
            HWHEEL = 0x01000,
            MOVE = 0x0001,
            MOVE_NOCOALESCE = 0x2000,
            LEFTDOWN = 0x0002,
            LEFTUP = 0x0004,
            RIGHTDOWN = 0x0008,
            RIGHTUP = 0x0010,
            MIDDLEDOWN = 0x0020,
            MIDDLEUP = 0x0040,
            VIRTUALDESK = 0x4000,
            WHEEL = 0x0800,
            XDOWN = 0x0080,
            XUP = 0x0100
        }
        #endregion SendInput Stuff

        internal const string BOT_NAME = "darbs";
        internal const float TESSERACT_MINCONFIDENCE = 0.60f;
        
        internal const float IMAGEMATCH_MINCONFIDENCE = 0.89f; // do not go below 0.88, may need to increase when testing
        internal const int IMAGEMATCH_RGBVARIANCE = 6; // RGB values are 0-255, and this var gets +- for colors during image match 
        internal const float IMAGEMATCH_DESATURATIONTHRESHOLD_DIGITS = 0.33f;
        internal const float IMAGEMATCH_DESATURATIONTHRESHOLD_TEXT = 0.30f;

        internal const double TIMER_TRANSACTIONTIMEOUT = 30000; // 30s * 1000ms
        
        internal const int WIN32_MOUSECELLOFFSET = 10;
        
        //internal static readonly int IMAGEMATCH_HOTBAR_STEELINGOT = 0;
        //internal static readonly int IMAGEMATCH_HOTBAR_ORANGEHIDE = 1;
        //internal static readonly int IMAGEMATCH_HOTBAR_ORANGELEATHER = 2;
        //internal static readonly int IMAGEMATCH_HOTBAR_STEELAXE = 5;
        //internal static readonly int IMAGEMATCH_HOTBAR_STEELPICKAXE = 6;
        internal static readonly int IMAGEMATCH_HOTBAR_EMPTYCELL = 7; // TODO: save a screenshot and use that instead of requiring an empty hotbar slot

        internal static readonly int[] POSITION_CHAT = new int[] { 260, 1000 };
        internal static readonly int[] POSITION_INVENTORYSPACER = new int[] { 1500, 750 };
        internal static readonly int[] POSITION_INVENTORYSPACERTWO = new int[] { 1510, 760 };
        
        internal static readonly Rectangle RECT_CHAT = new Rectangle(83, 910, 331, 39);

        internal static readonly Rectangle RECT_CROPIMAGEMATCH = new Rectangle(12, 12, 20, 20);
        internal static readonly Rectangle RECT_CROPOCRQUANTITY = new Rectangle(29, 29, 17, 17);

        internal static readonly Rectangle RECT_ITEMINFO = new Rectangle(1600, 95, 310, 55);
        
        /// <summary>
        /// Accepted values are [0,0] through [1,6]
        /// </summary>
        internal static readonly Rectangle[,] RECTS_SMALLWOODENBARREL = new Rectangle[,]
        {
            {
                new Rectangle(1044, 843, 47, 47),
                new Rectangle(1094, 843, 47, 47),
                new Rectangle(1143, 843, 47, 47),
                new Rectangle(1193, 843, 47, 47),
                new Rectangle(1242, 843, 47, 47),
                new Rectangle(1292, 843, 47, 47),
                new Rectangle(1341, 843, 47, 47),
            },
            
            {   
                new Rectangle(1044, 891, 47, 47),
                new Rectangle(1094, 891, 47, 47), // sometimes flags empty vs empty incorrectly below 90% at 4 variance
                new Rectangle(1143, 891, 47, 47),
                new Rectangle(1193, 891, 47, 47),
                new Rectangle(1242, 891, 47, 47),
                new Rectangle(1292, 891, 47, 47),
                new Rectangle(1341, 891, 47, 47),
            },
        };
        
        /// <summary>
        /// Accepted values are [0] through [7]
        /// </summary>
        internal static readonly Rectangle[] RECTS_HOTBAR = new Rectangle[]
        {
                new Rectangle(1496, 988, 47, 47),
                new Rectangle(1544, 988, 47, 47),
                new Rectangle(1594, 988, 47, 47),
                new Rectangle(1644, 988, 47, 47),
                new Rectangle(1693, 988, 47, 47),
                new Rectangle(1743, 988, 47, 47),
                new Rectangle(1792, 988, 47, 47),
                new Rectangle(1842, 988, 47, 47),
        };
        
        /// <summary>
        /// Accepted values are [0,0] through [3,7]
        /// </summary>
        internal static readonly Rectangle[,] RECTS_INVENTORY = new Rectangle[,] // these depend on the chest screen showing, not normal inventory menu
        {
            // 0,0 through 0,7
            {
                new Rectangle(1495, 792, 47, 47),
                new Rectangle(1544, 792, 47, 47),
                new Rectangle(1594, 792, 47, 47),
                new Rectangle(1644, 792, 47, 47),
                new Rectangle(1693, 792, 47, 47),
                new Rectangle(1743, 792, 47, 47),
                new Rectangle(1792, 792, 47, 47),
                new Rectangle(1842, 792, 47, 47),
            },
            // 1,0 through 1,7
            {
                new Rectangle(1495, 840, 47, 47),
                new Rectangle(1544, 840, 47, 47),
                new Rectangle(1593, 840, 47, 47),
                new Rectangle(1643, 840, 47, 47),
                new Rectangle(1692, 840, 47, 47),
                new Rectangle(1742, 840, 47, 47),
                new Rectangle(1791, 840, 47, 47),
                new Rectangle(1841, 840, 47, 47),
            },
            // 2,0 through 2,7
            {
                new Rectangle(1495, 888, 47, 47),
                new Rectangle(1544, 888, 47, 47),
                new Rectangle(1593, 888, 47, 47),
                new Rectangle(1643, 888, 47, 47),
                new Rectangle(1692, 888, 47, 47),
                new Rectangle(1742, 888, 47, 47),
                new Rectangle(1791, 888, 47, 47),
                new Rectangle(1841, 888, 47, 47),
            },
            // 3,0 through 3,7
            {
                new Rectangle(1495, 936, 47, 47),
                new Rectangle(1544, 936, 47, 47),
                new Rectangle(1593, 936, 47, 47),
                new Rectangle(1643, 936, 47, 47),
                new Rectangle(1692, 936, 47, 47),
                new Rectangle(1742, 936, 47, 47),
                new Rectangle(1791, 936, 47, 47),
                new Rectangle(1841, 936, 47, 47),
            }
        };

        #region Item Prices 
        internal static Dictionary<Item, int> PRICES_STEELPICKAXE = new Dictionary<Item, int>()
        {
            { Item.SteelIngot, 2 },
            { Item.OrangeHide, 4 },
            { Item.OrangeLargeHide, 2 },
            
        };
        internal static Dictionary<Item, int> PRICES_STEELAXE = new Dictionary<Item, int>()
        {
            { Item.SteelIngot, 5 },
            { Item.OrangeHide, 10 },
            { Item.OrangeLargeHide, 5 },
            
        };
        #endregion Item Prices

        #region Window Messages
        internal const uint WM_ACTIVATEAPP = 0x001C;
        internal const uint WM_NCACTIVATE = 0x0086;
        internal const uint WM_IME_SETCONTEXT = 0x0281;
        internal const uint WM_SETFOCUS = 0x0007;
        internal const uint WM_KEYDOWN = 0x0100;
        internal const uint WM_KEYUP = 0x0101;
        internal const uint WM_CHAR = 0x0102;
        internal const uint WM_MOUSEMOVE = 0x0200;
        internal const uint WM_LBUTTONDOWN = 0x0201;
        internal const uint WM_LBUTTONUP = 0x0202;
        #endregion Window Messages

        #region Virtual Keys
        internal const IntPtr VK_KEY_F2 = 0x71;
        internal const IntPtr VK_KEY_F3 = 0x72;
        internal const IntPtr VK_KEY_F4 = 0x73;
        internal const IntPtr VK_KEY_F5 = 0x74;
        internal const IntPtr VK_LSHIFT = 0xA0;
        internal const IntPtr VK_KEY_E = 0x45;
        internal const IntPtr VK_MBUTTON = 0x04;
        internal const IntPtr VK_KEY_XBUTTON2 = 0x06;
        internal const IntPtr VK_RETURN = 0x0D;
        #endregion Virtual Keys

        #region Hotkey IDs
        internal const int HK_PAUSEBINDINGS = 0;
        internal const int HK_SWITCHMACRO = 1;
        internal const int HK_USEMACRO = 2;
        internal const int HK_AUTORUN = 3;
        #endregion Hotkey IDs

        #region ASCII Uppercase Characters
        internal const int ASCII_A_UPPER = 65;
        internal const int ASCII_B_UPPER = 66;
        internal const int ASCII_C_UPPER = 67;
        internal const int ASCII_D_UPPER = 68;
        internal const int ASCII_E_UPPER = 69;
        internal const int ASCII_F_UPPER = 70;
        internal const int ASCII_G_UPPER = 71;
        internal const int ASCII_H_UPPER = 72;
        internal const int ASCII_I_UPPER = 73;
        internal const int ASCII_J_UPPER = 74;
        internal const int ASCII_K_UPPER = 75;
        internal const int ASCII_L_UPPER = 76;
        internal const int ASCII_M_UPPER = 77;
        internal const int ASCII_N_UPPER = 78;
        internal const int ASCII_O_UPPER = 79;
        internal const int ASCII_P_UPPER = 80;
        internal const int ASCII_Q_UPPER = 81;
        internal const int ASCII_R_UPPER = 82;
        internal const int ASCII_S_UPPER = 83;
        internal const int ASCII_T_UPPER = 84;
        internal const int ASCII_U_UPPER = 85;
        internal const int ASCII_V_UPPER = 86;
        internal const int ASCII_W_UPPER = 87;
        internal const int ASCII_X_UPPER = 88;
        internal const int ASCII_Y_UPPER = 89;
        internal const int ASCII_Z_UPPER = 90;
        #endregion ASCII Uppercase Characters
        
        #region ASCII Lowercase Characters
        internal const int ASCII_A_LOWER = 97;
        internal const int ASCII_B_LOWER = 98;
        internal const int ASCII_C_LOWER = 99;
        internal const int ASCII_D_LOWER = 100;
        internal const int ASCII_E_LOWER = 101;
        internal const int ASCII_F_LOWER = 102;
        internal const int ASCII_G_LOWER = 103;
        internal const int ASCII_H_LOWER = 104;
        internal const int ASCII_I_LOWER = 105;
        internal const int ASCII_J_LOWER = 106;
        internal const int ASCII_K_LOWER = 107;
        internal const int ASCII_L_LOWER = 108;
        internal const int ASCII_M_LOWER = 109;
        internal const int ASCII_N_LOWER = 110;
        internal const int ASCII_O_LOWER = 111;
        internal const int ASCII_P_LOWER = 112;
        internal const int ASCII_Q_LOWER = 113;
        internal const int ASCII_R_LOWER = 114;
        internal const int ASCII_S_LOWER = 115;
        internal const int ASCII_T_LOWER = 116;
        internal const int ASCII_U_LOWER = 117;
        internal const int ASCII_V_LOWER = 118;
        internal const int ASCII_W_LOWER = 119;
        internal const int ASCII_X_LOWER = 120;
        internal const int ASCII_Y_LOWER = 121;
        internal const int ASCII_Z_LOWER = 122;
        #endregion ASCII Lowercase Characters
        
        #region ASCII Numbers
        public const int ASCII_ZERO = 48;
        public const int ASCII_ONE = 49;
        public const int ASCII_TWO = 50;
        public const int ASCII_THREE = 51;
        public const int ASCII_FOUR = 52;
        public const int ASCII_FIVE = 53;
        public const int ASCII_SIX = 54;
        public const int ASCII_SEVEN = 55;
        public const int ASCII_EIGHT = 56;
        public const int ASCII_NINE = 57;
        #endregion ASCII Numbers
        
        #region ASCII Characters Extended
        internal const int ASCII_SPACE = 32;
        internal const int ASCII_APOSTROPHE = 39;
        internal const int ASCII_ROUNDBRACKETOPEN = 40;
        internal const int ASCII_ROUNDBRACKETCLOSE = 41;
        internal const int ASCII_COMMA = 44;
        internal const int ASCII_PERIOD = 46;
        internal const int ASCII_COMMANDSLASH = 47;
        internal const int ASCII_COLON = 58;
        internal const int ASCII_QUESTIONMARK = 63;
        internal const int ASCII_VERTICALBAR = 124;
        #endregion ASCII Characters Extended

        
        internal static int GetChestRows => RECTS_SMALLWOODENBARREL.GetLength(0);
        internal static int GetChestColumns => RECTS_SMALLWOODENBARREL.GetLength(1);
        
        internal static int GetInventoryRows => RECTS_INVENTORY.GetLength(0);
        internal static int GetInventoryColumns => RECTS_INVENTORY.GetLength(1);
        
        internal static int MakeXYParam(int _lo, int _hi)
        {
            return (int)((_hi << 16) | (_lo & 0xFFFF));
        }

        internal static string FormatItemNames(Item _item)
        {
            switch (_item)
            {
                case Item.Undecided:
                    return "Undecided";
                case Item.SteelIngot:
                    return "Steel Ingot";
                case Item.OrangeHide:
                    return "Animal Hide";
                case Item.OrangeLargeHide:
                    return "Large Animal Hide";
                case Item.SteelPickaxe:
                    return "Steel Pickaxe";
                case Item.SteelAxe:
                    return "Steel Chopping Axe";
                default:
                    throw new ArgumentOutOfRangeException(nameof(_item), _item, null);
            }
        }
        
        
        internal static void Reset(this Timer timer)
        {
            timer.Stop();
            timer.Start();
            Main.p_TransactionTimeoutTime = DateTime.Now.AddMilliseconds(TIMER_TRANSACTIONTIMEOUT);
        }

        internal static double GetTransactionTimeRemaining()
        {
            return (Main.p_TransactionTimeoutTime - DateTime.Now).TotalMilliseconds;
        }
    }
}
