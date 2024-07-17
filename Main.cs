using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Tesseract;
using static System.Net.Mime.MediaTypeNames;
using static PDTrader.Library;
using ImageFormat = System.Drawing.Imaging.ImageFormat;
using Timer = System.Timers.Timer;

namespace PDTrader
{
    internal class Main
    {
        private static bool i_Looping = false;
        private static IntPtr i_Handle = IntPtr.Zero;
        private static string i_LastWhisperedUserName = "";

        private static int i_NextTransactionID = 0;
        private static bool i_Greeted = false;
        private static bool i_DiscussedItem = false;
        private static bool i_DiscussedPrice = false;
        private static bool i_Trading = false;
        private static Transaction? i_ActiveTransaction;
        private static Queue<Transaction> i_CancelledTransactions = new Queue<Transaction>(); // don't see a situation where I
        private static Queue<Transaction> i_CompletedTransactions = new Queue<Transaction>(); // wouldn't want to export everything
        private static Timer i_TransactionTimeout = new Timer(TIMER_TRANSACTIONTIMEOUT);
        internal static DateTime p_TransactionTimeoutTime = new DateTime();
        
        internal static bool Initialize()
        {
            i_Handle = Win32API.FindWindow(null, "Pax Dei  ");
            if(i_Handle == IntPtr.Zero)
            {
                i_Handle = Win32API.FindWindow(null, "Pax Dei - Merchants Client");
            }
            if (i_Handle == IntPtr.Zero)
            {
                Debug.WriteLine("[ERROR] could not find Pax Dei window");
                return false;
            }
            Win32API.SetWindowText(i_Handle, "Pax Dei - Merchants Client");
            i_TransactionTimeout.Elapsed += Timer_Elapsed;
            return true;
        }

        private static void Timer_Elapsed(object source, ElapsedEventArgs e)
        {
            // TODO: cancel transaction  
            ResetTransaction();
            
            Win32API.SendTextMessage(i_Handle, "Have a good one!");
            Win32API.HitEnter(i_Handle); // send message

            StartFreshOCRChatLoop();
        }
        
        internal static async void WatchChestTest()
        {
            // DEBUG
            //i_Trading = true;
            //i_ActiveTransaction = Transaction.New(0, "Lunark");
            //i_ActiveTransaction.Wants = Item.SteelAxe;
            //i_ActiveTransaction.WantsQuantity = 1;
            //i_ActiveTransaction.Has = Item.SteelIngot;
            //i_ActiveTransaction.HasQuantity = PRICES_STEELAXE[Item.SteelIngot] * i_ActiveTransaction.WantsQuantity;
            // DEBUG
            
            i_Looping = true;
            while (i_Looping)
            {
                if (!i_Trading)
                {
                    return; // return back to OCR loop after resetting transaction since I'm dumb and use the same variable for both loops
                }
                
                // clicking into the chat window seems to bug the next left click for dragging 
                Win32API.MouseMove(POSITION_INVENTORYSPACER);
                Win32API.MouseClickTest();
                Win32API.MouseMove(POSITION_INVENTORYSPACERTWO);
                
                // get an open inventory slot to move any item we find in the chest
                Debug.WriteLine("looking for an empty slot in bot inventory");
                Tuple<bool, int[]> _InvEmptySlotCheck = ImageMatchAPI.ReturnFirstUnoccupiedInventoryCell(0, 0);
                if (!_InvEmptySlotCheck.Item1)
                {
                    Debug.WriteLine("[ERROR] no open inventory slots for bot to handle items");
                    ResetTransaction();
                    return;
                }
                Debug.WriteLine("Found an open slot in bot inv at [{0},{1}]", _InvEmptySlotCheck.Item2[0], _InvEmptySlotCheck.Item2[1]);
                
                // look for items in the chest
                Debug.WriteLine("looking for items in the chest");
                Tuple<bool, int[]> _ChestItemCheck = ImageMatchAPI.ReturnFirstOccupiedChestCell(0, 0);
                if (!_ChestItemCheck.Item1)
                {
                    Debug.WriteLine("Did not find any items in the chest");
                    //await Task.Delay(1000);
                    Thread.Sleep(1000);
                    continue;
                }
                Debug.WriteLine("Found an item in the chest at [{0},{1}]", _ChestItemCheck.Item2[0], _ChestItemCheck.Item2[1]);
                
                i_TransactionTimeout.Stop(); // stop the timer since it's on us to work now
                
                // found an item in the chest, gotta move it to bot inventory so we can work on it safely 
                //i_Looping = false;
                
                // move the item from chest to inventory
                Debug.WriteLine("moving item from chest to bot inv");
                int[] _CheckSourcePosition = new int[]
                {
                    RECTS_SMALLWOODENBARREL[_ChestItemCheck.Item2[0], _ChestItemCheck.Item2[1]].X + WIN32_MOUSECELLOFFSET,
                    RECTS_SMALLWOODENBARREL[_ChestItemCheck.Item2[0], _ChestItemCheck.Item2[1]].Y + WIN32_MOUSECELLOFFSET
                };
                int[] _CheckDestinationPosition = new int[]
                {
                    RECTS_INVENTORY[_InvEmptySlotCheck.Item2[0], _InvEmptySlotCheck.Item2[1]].X + WIN32_MOUSECELLOFFSET,
                    RECTS_INVENTORY[_InvEmptySlotCheck.Item2[0], _InvEmptySlotCheck.Item2[1]].Y + WIN32_MOUSECELLOFFSET
                };
                //Debug.WriteLine($"source x{_CheckSourcePosition[0]} y{_CheckSourcePosition[1]} | destination x{_CheckDestinationPosition[0]} y{_CheckDestinationPosition[1]}");
                Win32API.MouseMove(_CheckSourcePosition);
                Win32API.MouseDragTest( _CheckDestinationPosition);
                
                Thread.Sleep(1000); // DEBUG
                
                // check if this is the item we're expecting
                Debug.WriteLine("checking if they paid the correct item");
                Win32API.MouseRehoverItem(); // game doesn't show description after dragging
                Win32API.MouseMove(_CheckDestinationPosition); // hover the item
                string _ExpectedItemName = FormatItemNames(i_ActiveTransaction.Has);
                string _ReceivedItemName = OCRAPI.ReturnCurrentItemName();
                if (_ReceivedItemName != _ExpectedItemName)
                {
                    Debug.WriteLine($"wrong item, expected \'{_ExpectedItemName}\' but got \'{_ReceivedItemName}\'");
                    // move item back
                    Win32API.MouseMove(_CheckDestinationPosition);
                    Win32API.MouseDragTest(_CheckSourcePosition);
                    // respond in chat
                    Win32API.MouseMove(POSITION_CHAT);
                    Win32API.MouseClickTest();
                    Win32API.SendTextMessage(i_Handle, $"Sorry I don't think that's {_ExpectedItemName}");
                    Win32API.HitEnter(i_Handle); // send message
                    Win32API.HitEnter(i_Handle); // open chat up
                    
                    //await Task.Delay(2000); // wait a couple sec for them to grab the invalid item
                    Thread.Sleep(2000);
                    i_TransactionTimeout.Reset(); // reset the timer
                    //i_Looping = true;
                    continue;
                }
                Debug.WriteLine($"right item, expecting \'{_ExpectedItemName}\' and got \'{_ReceivedItemName}\'");
                
                // we received the right item as payment
                Debug.WriteLine($"checking if they paid the correct quantity");
                int _ExpectedQuantity = i_ActiveTransaction.HasQuantity;
                int _ReceivedQuantity = OCRAPI.ReturnCurrentItemQuantity(_InvEmptySlotCheck.Item2[0], _InvEmptySlotCheck.Item2[1]);
                if (_ReceivedQuantity != _ExpectedQuantity)
                {
                    Debug.WriteLine($"wrong quantity, expected x{_ExpectedQuantity} but only got x{_ReceivedQuantity}");
                    // move item back
                    Win32API.MouseMove(_CheckDestinationPosition);
                    Win32API.MouseDragTest(_CheckSourcePosition);
                    // respond in chat
                    Win32API.MouseMove(POSITION_CHAT);
                    Win32API.MouseClickTest();
                    Win32API.SendTextMessage(i_Handle, $"Sorry I don't think that's x{_ExpectedQuantity}");
                    Win32API.HitEnter(i_Handle); // send message
                    Win32API.HitEnter(i_Handle); // open chat up
                    
                    //await Task.Delay(2000); // wait a couple sec for them to grab the invalid item
                    Thread.Sleep(2000);
                    i_TransactionTimeout.Reset(); // reset the timer
                    //i_Looping = true;
                    continue;
                }
                Debug.WriteLine($"right quantity, expecting x{_ExpectedQuantity} and got x{_ReceivedQuantity}");
                
                // we received the right quantity
                
                // find an empty chest slot for their purchase
                Debug.WriteLine($"looking for an empty slot in chest");
                Tuple<bool, int[]> _ChestEmptySlotCheck = ImageMatchAPI.ReturnFirstUnoccupiedChestCell(0, 0);
                if (!_ChestEmptySlotCheck.Item1)
                {
                    Debug.WriteLine("[ERROR] not seeing any open chest slots to move their purchase");
                    // TODO: handling for no open chest slots, brute force it and replace something?
                    i_Looping = false;
                    return;
                }
                Debug.WriteLine($"found an empty chest slot to move their purchase at [{_ChestEmptySlotCheck.Item2[0]},{_ChestEmptySlotCheck.Item2[1]}]");
                
                // find the item they purchased in bot inventory
                string _PurchasedItemName = FormatItemNames(i_ActiveTransaction.Wants);
                Debug.WriteLine($"looking for \'{_PurchasedItemName}\' in bot inventory");
                Tuple<bool, int[]> _InvPurchasedItemCheck = OCRAPI.ReturnMatchingItemPosition(0, 0, _PurchasedItemName);
                if (!_InvPurchasedItemCheck.Item1)
                {
                    Debug.WriteLine($"no \'{_PurchasedItemName}\' in the bot inventory");
                    // TODO: handling for no remaining items
                    i_Looping = false;
                    return;
                }
                Debug.WriteLine($"found \'{_PurchasedItemName}\' in bot inventory at [{_InvPurchasedItemCheck.Item2[0]},{_InvPurchasedItemCheck.Item2[1]}]");
                
                // move their purchase to the chest
                Debug.WriteLine("moving their purchase to the chest");
                // mouse should already be over the correct position, but we can move it again just in case
                int[] _InvPurchasedItemPosition = new int[] 
                {
                    RECTS_INVENTORY[_InvPurchasedItemCheck.Item2[0], _InvPurchasedItemCheck.Item2[1]].X + WIN32_MOUSECELLOFFSET,
                    RECTS_INVENTORY[_InvPurchasedItemCheck.Item2[0], _InvPurchasedItemCheck.Item2[1]].Y + WIN32_MOUSECELLOFFSET
                };
                int[] _ChestEmptySlotPosition = new int[]
                {
                    RECTS_SMALLWOODENBARREL[_ChestEmptySlotCheck.Item2[0], _ChestEmptySlotCheck.Item2[1]].X + WIN32_MOUSECELLOFFSET,
                    RECTS_SMALLWOODENBARREL[_ChestEmptySlotCheck.Item2[0], _ChestEmptySlotCheck.Item2[1]].Y + WIN32_MOUSECELLOFFSET
                };
                Win32API.MouseMove(_InvPurchasedItemPosition);
                Win32API.MouseDragTest(_ChestEmptySlotPosition);
                Debug.WriteLine("moved their purchase to the chest, thanking them");
                
                // update transaction status
                i_ActiveTransaction.Status = TransactionStatus.Thanking;
                MainWindow.OnUpdateStatus("Thanking");
                MainWindow.OnUpdateTransaction(i_ActiveTransaction);
                
                // thank them
                Win32API.MouseMove(POSITION_CHAT);
                Win32API.MouseClickTest();
                Win32API.SendTextMessage(i_Handle, $"Thanks for your business {i_ActiveTransaction.Name}.");
                Win32API.HitEnter(i_Handle); // send message
                Win32API.HitEnter(i_Handle); // open chat up
                
                i_CompletedTransactions.Enqueue(i_ActiveTransaction); // get this one out
                MainWindow.OnEndTransaction(i_ActiveTransaction);
                
                // complete the transaction
                ResetTransaction();
                
                // watch chat
                Debug.WriteLine("back to watching chat");
                OCRChatLoop(true);
                return;
                
                // old shit
                /*if (_InvEmptySlotCheck.Item1)
                {
                    Tuple<bool, int[]> _ChestItemCheck = ImageMatchAPI.ReturnFirstOccupiedChestCell(0, 0);
                    if (_ChestItemCheck.Item1)
                    {
                        
                        
                        float _ItemConfidence = 0f;

                        switch (i_ActiveTransaction.Has)
                        {
                            case Item.SteelIngot:
                                _ItemConfidence = ImageMatchAPI.CompareImages(
                                    ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_INVENTORY[_InvEmptySlotCheck.Item2[0], _InvEmptySlotCheck.Item2[1]]),
                                    ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_HOTBAR[IMAGEMATCH_HOTBAR_STEELINGOT])
                                );
                                break;
                            case Item.OrangeHide:
                                _ItemConfidence = ImageMatchAPI.CompareImages(
                                    ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_INVENTORY[_InvEmptySlotCheck.Item2[0], _InvEmptySlotCheck.Item2[1]]),
                                    ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_HOTBAR[IMAGEMATCH_HOTBAR_ORANGEHIDE])
                                );
                                break;
                            case Item.OrangeLargeHide:
                                _ItemConfidence = ImageMatchAPI.CompareImages(
                                    ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_INVENTORY[_InvEmptySlotCheck.Item2[0], _InvEmptySlotCheck.Item2[1]]),
                                    ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_HOTBAR[IMAGEMATCH_HOTBAR_ORANGELEATHER])
                                );
                                break;
                            default:
                                Debug.WriteLine("[ERROR] shits fucked up");
                                throw new ArgumentOutOfRangeException();
                        }
                        
                        if (_ItemConfidence >= IMAGEMATCH_MINCONFIDENCE) // correct item
                        {
                            if (OCRQuantityTest(_InvEmptySlotCheck.Item2[0], _InvEmptySlotCheck.Item2[1])) // correct quantity
                            {
                                Debug.WriteLine("trying to move their purchase into the chest");
                                
                                Thread.Sleep(1000); // DEBUG
                                
                                Tuple<bool, int[]> _UnoccupiedChestCheck = ImageMatchAPI.ReturnFirstUnoccupiedChestCell(0, 0);
                                if (_UnoccupiedChestCheck.Item1) // we found an unoccupied slot
                                {
                                    int _HotbarIndex = -1;
                                    switch (i_ActiveTransaction.Wants)
                                    {
                                        case Item.SteelAxe:
                                            _HotbarIndex = IMAGEMATCH_HOTBAR_STEELAXE;
                                            break;
                                        case Item.SteelPickaxe:
                                            _HotbarIndex = IMAGEMATCH_HOTBAR_STEELPICKAXE;
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException();
                                    }
                                    
                                    Tuple<bool, int[]> _MatchingInventoryCheck = ImageMatchAPI.ReturnFirstMatchingInventoryCell(0, 0, 
                                        ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_HOTBAR[_HotbarIndex]));
                                    if (_MatchingInventoryCheck.Item1) // we found the item they purchased in our inventory
                                    {
                                        int[] _ReturnSourcePosition = new int[]
                                        {
                                            RECTS_INVENTORY[_MatchingInventoryCheck.Item2[0], _MatchingInventoryCheck.Item2[1]].X + WIN32_MOUSECELLOFFSET,
                                            RECTS_INVENTORY[_MatchingInventoryCheck.Item2[0], _MatchingInventoryCheck.Item2[1]].Y + WIN32_MOUSECELLOFFSET
                                        };
                                        /*int[] _ReturnSourcePosition = new int[]
                                        {
                                            RECTS_INVENTORY[2, 1].X + WIN32_MOUSECELLOFFSET, // DEBUG: 2,1 is only for testing, will have
                                            RECTS_INVENTORY[2, 1].Y + WIN32_MOUSECELLOFFSET  // DEBUG: it take from an assigned inv slot eventually
                                        };#1#
                                        int[] _ReturnDestinationPosition = new int[]
                                        {
                                            RECTS_SMALLWOODENBARREL[_UnoccupiedChestCheck.Item2[0], _UnoccupiedChestCheck.Item2[1]].X + WIN32_MOUSECELLOFFSET,
                                            RECTS_SMALLWOODENBARREL[_UnoccupiedChestCheck.Item2[0], _UnoccupiedChestCheck.Item2[1]].Y + WIN32_MOUSECELLOFFSET
                                        };
                                        
                                        Debug.WriteLine($"moving their purchase to chest slot {_UnoccupiedChestCheck.Item2[0]}, {_UnoccupiedChestCheck.Item2[1]}");
                                        
                                        Win32API.MouseMove(_ReturnSourcePosition);
                                        //Win32API.MouseDrag(i_Handle, _ReturnSourcePosition, _ReturnDestinationPosition);
                                        Win32API.MouseDragTest(_ReturnDestinationPosition);
                                    
                                        i_ActiveTransaction.Status = TransactionStatus.Thanking;
                                        MainWindow.OnUpdateStatus("Thanking");
                                        MainWindow.OnUpdateTransaction(i_ActiveTransaction);
                                        i_CompletedTransactions.Enqueue(i_ActiveTransaction);

                                        Win32API.MouseMove(POSITION_CHAT);
                                        //Win32API.MouseClick(i_Handle, POSITION_CHAT);
                                        Win32API.MouseClickTest();
                                        
                                        Win32API.SendTextMessage(i_Handle, $"Thanks for your business {i_ActiveTransaction.Name}.");
                                        Win32API.HitEnter(i_Handle); // send message
                                        Win32API.HitEnter(i_Handle); // open chat up
                                    
                                        i_Trading = false;
                                        MainWindow.OnEndTransaction(i_ActiveTransaction);
                                        OCRLoop(true);
                                        return;
                                    }
                                    else
                                    {
                                        Debug.WriteLine("[ERROR] we're out of what they purchased");
                                        // TODO: handling for no items left
                                        return;
                                    }
                                    
                                }
                                else
                                {
                                    Debug.WriteLine("no open chest slot found to place their purchase");
                                    
                                    // TODO: handling for no open chest slots, brute force it?
                                    Debug.WriteLine("bot broken - restart it");
                                    return;
                                }
                            }
                            else
                            {
                                Debug.WriteLine("not the quantity we're expecting");
                                
                                // move item back
                                Win32API.MouseMove(_CheckDestinationPosition);
                                Win32API.MouseDragTest(_CheckSourcePosition);
                            
                                // respond in chat
                                Win32API.MouseMove(POSITION_CHAT);
                                Win32API.MouseClickTest();
                                Win32API.SendTextMessage(i_Handle, $"Sorry I don't think that's x{i_ActiveTransaction.HasQuantity}");
                                Win32API.HitEnter(i_Handle); // send message
                                Win32API.HitEnter(i_Handle); // open chat up
                                Win32API.MouseClickTest();
                                
                                await Task.Delay(2000); // wait a sec for them to grab the invalid item
                                i_Looping = true;
                                continue;
                            } 
                        }
                        else
                        {
                            Debug.WriteLine("not the item we're expecting");
                            
                            // move item back
                            Win32API.MouseMove(_CheckDestinationPosition);
                            Win32API.MouseDragTest(_CheckSourcePosition);
                            
                            // respond in chat
                            Win32API.MouseMove(POSITION_CHAT);
                            Win32API.MouseClickTest();
                            Win32API.SendTextMessage(i_Handle, $"Sorry I don't think that's {i_ActiveTransaction.Has}");
                            Win32API.HitEnter(i_Handle); // send message
                            Win32API.HitEnter(i_Handle); // open chat up
                            
                            await Task.Delay(2000); // wait a sec for them to grab the invalid item
                            i_Looping = true;
                            continue;
                        }
                        return;
                    }
                    else
                    {
                        Debug.WriteLine("Did not find any occupied cells in the chest");
                    }
                }
                await Task.Delay(1000);*/
            }
        }

        private static void ResetTransaction()
        {
            i_TransactionTimeout.Stop();
            
            i_Trading = false;
            i_Greeted = false;
            i_DiscussedItem = false;
            i_DiscussedPrice = false;
            i_ActiveTransaction = null;
            
            i_Looping = true;
        }

        internal static void StartFreshOCRChatLoop()
        {
            MainWindow.OnUpdateStatus("Starting");
            OCRChatLoop(false);
        }

        internal static void StopOCRChatLoop()
        {
            MainWindow.OnUpdateStatus("Stopping");
            i_Looping = false;
        }

        private static async void OCRChatLoop(bool _chatOpen)
        {
            Debug.WriteLine("starting to watch chat");
            if (!_chatOpen)
            {
                Win32API.HitEnter(i_Handle); // open chat up
            }
            //Thread.Sleep(500);
            await Task.Delay(500);

            i_Looping = true;
            while (i_Looping)
            {
                if(Win32API.GetForegroundWindow() == i_Handle)
                {
                    if (i_TransactionTimeout.Enabled)
                    {
                        MainWindow.OnUpdateTransactionTimer(GetTransactionTimeRemaining() / 1000d);
                    }
                    if (!i_Trading)
                    {
                        MainWindow.OnUpdateStatus("Waiting");
                    }
                    else
                    {
                        switch (i_ActiveTransaction!.Status) // active transaction should never be null while i_Trading is true
                        {
                            case TransactionStatus.Greeting:
                                MainWindow.OnUpdateStatus("Greeting");
                                break;
                            case TransactionStatus.Discussing:
                                MainWindow.OnUpdateStatus("Discussing");
                                break;
                            case TransactionStatus.Trading:
                                MainWindow.OnUpdateStatus("Trading");
                                break;
                            case TransactionStatus.Thanking:
                                MainWindow.OnUpdateStatus("Thanking");
                                break;
                        }
                    }
                    Win32API.MouseScrollDownTest();
                    Win32API.MouseScrollDownTest();
                    Win32API.MouseScrollDownTest();
                    Win32API.MouseScrollDownTest();
                    //Thread.Sleep(500);
                    await Task.Delay(500);
                    await Task.Run(OCRChatTest);
                    await Task.Delay(500);
                    //Thread.Sleep(500);
                }
                else
                {
                    MainWindow.OnUpdateStatus("-Paused-");
                    Debug.WriteLine("Pax Dei not the active window, keep chat ocr loop paused");
                    await Task.Delay(1000); // don't want to break the GUI
                    //Thread.Sleep(1000);
                }
            }
        }

        private static void OCRChatTest()
        {
            //Debug.WriteLine("screenshotting chat");
            Bitmap _Screenshot = ScreenCaptureAPI.CapturePaxDeiWindow(RECT_CHAT);
            _Screenshot.Save("chat_original.bmp", ImageFormat.Bmp);

            Bitmap _Desaturated = ImageMatchAPI.DesaturateImage(_Screenshot, IMAGEMATCH_DESATURATIONTHRESHOLD_TEXT);
            _Desaturated.Save("chat_desaturated.bmp", ImageFormat.Bmp);
            Bitmap _Inverted = ImageMatchAPI.InvertImage(_Desaturated);
            _Inverted.Save("chat_inverted.bmp", ImageFormat.Bmp);
            
            try
            {
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    using (var img = PixConverter.ToPix(_Inverted))
                    {
                        using (var page = engine.Process(img))
                        {
                            var text = page.GetText();
                            Debug.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());
                            if(page.GetMeanConfidence() >= TESSERACT_MINCONFIDENCE)
                            {
                                Debug.WriteLine($"PASS: {text}");
                                HandleMessage(text);
                            }
                            else
                            {
                                Debug.WriteLine($"FAIL: {text}");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                Debug.WriteLine("Unexpected Error: " + e.Message);
                Debug.WriteLine("Details: ");
                Debug.WriteLine(e.ToString());
            }
        }

        private static void HandleMessage(string _text)
        {
            _text = Regex.Replace(_text, @"/(\r\n)+|\r+|\n+|\t+/", string.Empty); // sanitize
            
            MainWindow.OnNewChat(_text); // update the GUI

            // our own message
            if(_text.Contains(BOT_NAME + ":") || _text.Contains(BOT_NAME + " :") || _text.Contains(BOT_NAME + ";") || _text.Contains(BOT_NAME + " ;"))
            {
                Debug.WriteLine("most recent line is from us, ignoring");
                return;
            }
            
            // whisper stuff
            /*if(_text.Contains($"To {i_LastWhisperedUserName}:"))
            {
                Debug.WriteLine("we just whispered this person, ignoring");
                return;
            }

            if (_text.Contains("From ") && _text.Contains(':'))
            {
                i_Looping = false;
                
                MainWindow.OnNewStatus("Whisper");
                
                string _Name = Regex.Match(_text, @"From (.*):").Groups[1].Value;
                _Name = _Name.Replace(":", "");
                i_LastWhisperedUserName = _Name;
                
                Win32API.SendTextMessage(p_Handle, $"/t {_Name} hey {_Name}, sorry i don't trade through whispers :(");
                Win32API.HitEnter(p_Handle); // send message
                Win32API.HitEnter(p_Handle); // open chat up
                Win32API.SendTextMessage(p_Handle, "/s "); // swap back to normal chat and leave it open
                
                OCRLoop(true);
                return;
            }*/

            if (i_Trading)
            {
                if (!_text.Contains(i_ActiveTransaction.Name + ":") && !_text.Contains(i_ActiveTransaction.Name + " :") && !_text.Contains(i_ActiveTransaction.Name + ";")&& !_text.Contains(i_ActiveTransaction.Name + " ;"))
                {
                    return;
                }
                Debug.WriteLine("last chat name matches person we're trading");
                //i_Looping = false;
                i_TransactionTimeout.Reset(); // reset the timer
            
                if (i_DiscussedPrice)
                {
                    Win32API.SendTextMessage(i_Handle, $"{i_ActiveTransaction.Name} please put {FormatItemNames(i_ActiveTransaction.Has)} x{i_ActiveTransaction.HasQuantity} in the chest.");
                    Win32API.HitEnter(i_Handle); // send message
                    Win32API.HitEnter(i_Handle); // open chat up
                            
                    Thread.Sleep(1000);
                            
                    WatchChestTest();
                    return;
                    /*if (i_ActiveTransaction.Wants == Item.Undecided)
                    {
                        if (i_ActiveTransaction.Status != TransactionStatus.Discussing)
                        {
                            i_ActiveTransaction.Status = TransactionStatus.Discussing;
                            MainWindow.OnUpdateStatus("Discussing");
                            MainWindow.OnUpdateTransaction(i_ActiveTransaction);
                        }
                        
                        if (_text.Contains("buy 1", StringComparison.CurrentCultureIgnoreCase))
                        {
                            i_ActiveTransaction.Wants = Item.SteelPickaxe;
                            i_ActiveTransaction.WantsQuantity = 1; // DEBUG: forced 1 for now
                            
                            i_ActiveTransaction.Has = Item.SteelIngot;
                            i_ActiveTransaction.HasQuantity = PRICES_STEELPICKAXE[i_ActiveTransaction.Has] * i_ActiveTransaction.WantsQuantity;

                            i_ActiveTransaction.Status = TransactionStatus.Trading;
                            MainWindow.OnUpdateStatus("Trading");
                            MainWindow.OnUpdateTransaction(i_ActiveTransaction);
                        }
                        else if (_text.Contains("buy 2", StringComparison.CurrentCultureIgnoreCase))
                        {
                            i_ActiveTransaction.Wants = Item.SteelAxe;
                            i_ActiveTransaction.WantsQuantity = 1; // DEBUG: forced 1 for now

                            i_ActiveTransaction.Has = Item.SteelIngot;
                            i_ActiveTransaction.HasQuantity = PRICES_STEELAXE[i_ActiveTransaction.Has] * i_ActiveTransaction.WantsQuantity;
                            
                            i_ActiveTransaction.Status = TransactionStatus.Trading;
                            MainWindow.OnUpdateStatus("Trading");
                            MainWindow.OnUpdateTransaction(i_ActiveTransaction);
                        }
                        else
                        {
                            Win32API.SendTextMessage(i_Handle, "Sorry I'm not seeing something I can sell you.");
                            Win32API.HitEnter(i_Handle); // send message
                            Win32API.HitEnter(i_Handle); // open chat up
                            
                            OCRChatLoop(true);
                            return;
                        }

                        if (i_ActiveTransaction.Status == TransactionStatus.Trading)
                        {
                            Win32API.SendTextMessage(i_Handle, $"{i_ActiveTransaction.Name} please put {i_ActiveTransaction.Has} x{i_ActiveTransaction.HasQuantity} in the chest.");
                            Win32API.HitEnter(i_Handle); // send message
                            Win32API.HitEnter(i_Handle); // open chat up
                            
                            Thread.Sleep(1000);
                            
                            WatchChestTest();
                            return; // no longer watching chat, since watching chest
                        }
                    }
                    else
                    {
                        // TODO: after chest watch stuff is confirmed
                    }
                    
                    OCRChatLoop(true);
                    
                    
                    return; // don't look for hello after greeting someone*/
                }

                // discussing currency
                if (i_DiscussedItem)
                {
                    ref Dictionary<Item, int> _PriceDictionary = ref PRICES_STEELPICKAXE;
                
                    switch (i_ActiveTransaction.Wants)
                    {
                        case Item.SteelPickaxe:
                            break;
                        case Item.SteelAxe:
                            _PriceDictionary = ref PRICES_STEELAXE;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(i_ActiveTransaction.Wants), i_ActiveTransaction.Wants, null);
                    }
                    
                    if (_text.Contains("pay 1", StringComparison.CurrentCultureIgnoreCase))
                    {
                        i_ActiveTransaction.Has = ITEMS_BUYING[0];
                        i_ActiveTransaction.HasQuantity = _PriceDictionary[ITEMS_BUYING[0]];
                        i_DiscussedPrice = true;

                        MainWindow.OnUpdateStatus("Trading");
                        i_ActiveTransaction.Status = TransactionStatus.Trading;
                        MainWindow.OnUpdateTransaction(i_ActiveTransaction);
                    }
                    else if (_text.Contains("pay 2", StringComparison.CurrentCultureIgnoreCase))
                    {
                        i_ActiveTransaction.Has = ITEMS_BUYING[1];
                        i_ActiveTransaction.HasQuantity = _PriceDictionary[ITEMS_BUYING[1]];
                        i_DiscussedPrice = true;

                        MainWindow.OnUpdateStatus("Trading");
                        i_ActiveTransaction.Status = TransactionStatus.Trading;
                        MainWindow.OnUpdateTransaction(i_ActiveTransaction);
                    }
                    else if (_text.Contains("pay 3", StringComparison.CurrentCultureIgnoreCase))
                    {
                        i_ActiveTransaction.Has = ITEMS_BUYING[2];
                        i_ActiveTransaction.HasQuantity = _PriceDictionary[ITEMS_BUYING[2]];
                        i_DiscussedPrice = true;

                        MainWindow.OnUpdateStatus("Trading");
                        i_ActiveTransaction.Status = TransactionStatus.Trading;
                        MainWindow.OnUpdateTransaction(i_ActiveTransaction);
                    }
                    else if (_text.Contains("pay 0", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // go back to selecting an item
                        i_DiscussedItem = false;
                        SendItemMessages();
                    }
                    else
                    {
                        Win32API.SendTextMessage(i_Handle, "Sorry I'm not seeing something I can sell you.");
                        Win32API.HitEnter(i_Handle); // send message
                        Win32API.HitEnter(i_Handle); // open chat up
                    }
                    
                    //OCRChatLoop(true);
                    return;
                }
                
                // discussing item
                if (i_Greeted)
                {
                    if (_text.Contains("buy 1", StringComparison.CurrentCultureIgnoreCase))
                    {
                        i_ActiveTransaction.Wants = ITEMS_SELLING[0];
                        i_ActiveTransaction.WantsQuantity = 1; // DEBUG: forced 1 for now
                        i_DiscussedItem = true;

                        MainWindow.OnUpdateStatus("Discussing");
                        i_ActiveTransaction.Status = TransactionStatus.Discussing;
                        MainWindow.OnUpdateTransaction(i_ActiveTransaction);
                        
                        SendPriceMessages(i_ActiveTransaction.Wants);
                    }
                    else if (_text.Contains("buy 2", StringComparison.CurrentCultureIgnoreCase))
                    {
                        i_ActiveTransaction.Wants = ITEMS_SELLING[1];
                        i_ActiveTransaction.WantsQuantity = 1; // DEBUG: forced 1 for now
                        i_DiscussedItem = true;
                        
                        MainWindow.OnUpdateStatus("Discussing");
                        i_ActiveTransaction.Status = TransactionStatus.Discussing;
                        MainWindow.OnUpdateTransaction(i_ActiveTransaction);
                        
                        SendPriceMessages(i_ActiveTransaction.Wants);
                    }
                    else if (_text.Contains("buy 0", StringComparison.CurrentCultureIgnoreCase))
                    {
                        // TODO: cancel transaction
                        Debug.WriteLine($"cancelling transaction with {i_ActiveTransaction.Name}");
                        i_TransactionTimeout.Stop(); // stop the timer
                        i_CancelledTransactions.Enqueue(i_ActiveTransaction);
                        ResetTransaction();
                        
                        Win32API.SendTextMessage(i_Handle, "Have a good one!");
                        Win32API.HitEnter(i_Handle); // send message
                        Win32API.HitEnter(i_Handle); // open chat up
                    }
                    else
                    {
                        Win32API.SendTextMessage(i_Handle, "Sorry I'm not seeing something I can sell you.");
                        Win32API.HitEnter(i_Handle); // send message
                        Win32API.HitEnter(i_Handle); // open chat up
                    }
                    //OCRChatLoop(true);
                    return;
                }

                Debug.WriteLine("[ERROR] we're trading but haven't greeted, discussed item, or discussed price?");
                i_TransactionTimeout.Stop(); // stop the timer
                return;
            }
            
            // greeting
            if ((!_text.Contains("hello", StringComparison.CurrentCultureIgnoreCase) &&
                 !_text.Contains("hey", StringComparison.CurrentCultureIgnoreCase) &&
                 !_text.Contains("hi", StringComparison.CurrentCultureIgnoreCase) &&
                 !_text.Contains("yo", StringComparison.CurrentCultureIgnoreCase) &&
                 !_text.Contains("howdy", StringComparison.CurrentCultureIgnoreCase)) ||
                !_text.Contains(BOT_NAME, StringComparison.CurrentCultureIgnoreCase)) 
            {
                return; // idk what to do about this right now lol
            }
            //i_Looping = false;
                
            string _Name = Regex.Match(_text, @"(.*):").Groups[1].Value;
            if (_Name == "")
            {
                _Name = Regex.Match(_text, @"(.*);").Groups[1].Value;
            }
            if (_Name == "")
            {
                _Name = Regex.Match(_text, @"([A-Z]{1}[a-z]*?)\W").Groups[1].Value;
            }
            _Name = _Name.Replace("Party ", ""); // DEBUG: may only have say chat on in the end
            _Name = _Name.Replace("Clan ", "");  // DEBUG: these are just for testing purposes
            _Name = _Name.Replace("To ", "");    // DEBUG: 
            _Name = _Name.Replace(":", "");
            _Name = _Name.Replace(";", "");
            _Name = _Name.Replace(" ", "");
            
            Debug.WriteLine($"found greeting, writing a book in chat to {_Name}");

            i_Trading = true;
            i_ActiveTransaction = Transaction.New(i_NextTransactionID, _Name);
            i_Greeted = true;
            i_NextTransactionID++;
                
            MainWindow.OnUpdateStatus("Greeting");
            MainWindow.OnUpdateTransaction(i_ActiveTransaction);
            
            Win32API.SendTextMessage(i_Handle, $"Hey {_Name}, want to buy something?");
            Win32API.HitEnter(i_Handle); // send message
            Win32API.HitEnter(i_Handle); // open chat up

            SendItemMessages();

            i_TransactionTimeout.Reset(); // start the timer
            
            //OCRChatLoop(true); // back into the loop? go into a different loop?
            return;
        }

        private static void SendItemMessages()
        {
            for (int i = 0; i < ITEMS_SELLING.Length; i++)
            {
                Win32API.SendTextMessage(i_Handle, $"buy {i+1} | {FormatItemNames(ITEMS_SELLING[i])}");
                Win32API.HitEnter(i_Handle); // send message
                Win32API.HitEnter(i_Handle); // open chat up
            }
            
            Win32API.SendTextMessage(i_Handle, $"buy 0 | cancel");
            Win32API.HitEnter(i_Handle); // send message
            Win32API.HitEnter(i_Handle); // open chat up
        }
        
        private static void SendPriceMessages(Item _desiredPurchase)
        {
            ref Dictionary<Item, int> _PriceDictionary = ref PRICES_STEELPICKAXE;
            
            switch (_desiredPurchase)
            {
                case Item.SteelPickaxe:
                    break;
                case Item.SteelAxe:
                    _PriceDictionary = ref PRICES_STEELAXE;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_desiredPurchase), _desiredPurchase, null);
            }
            
            for (int i = 0; i < ITEMS_BUYING.Length; i++)
            {
                Win32API.SendTextMessage(i_Handle, $"pay {i+1} | {FormatItemNames(_desiredPurchase)} x{_PriceDictionary[ITEMS_BUYING[i]]} {FormatItemNames(ITEMS_BUYING[i])}");
                Win32API.HitEnter(i_Handle); // send message
                Win32API.HitEnter(i_Handle); // open chat up
            }
            
            Win32API.SendTextMessage(i_Handle, $"pay 0 | go back");
            Win32API.HitEnter(i_Handle); // send message
            Win32API.HitEnter(i_Handle); // open chat up
        }
    }
}
