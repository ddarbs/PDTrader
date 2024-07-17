using System.Diagnostics;
using System.Drawing;
using Tesseract;
using static PDTrader.Library;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

namespace PDTrader
{
    internal class OCRAPI
    {
        internal static string ReturnCurrentItemName()
        {
            Bitmap _Screenshot = ScreenCaptureAPI.CapturePaxDeiWindow(RECT_ITEMINFO);
            _Screenshot.Save("item_name.bmp", ImageFormat.Bmp);
            Bitmap _Desaturated = ImageMatchAPI.DesaturateImage(_Screenshot, IMAGEMATCH_DESATURATIONTHRESHOLD_TEXT);
            _Desaturated.Save("item_name_desaturated.bmp", ImageFormat.Bmp);
            Bitmap _Inverted = ImageMatchAPI.InvertImage(_Desaturated);
            _Inverted.Save("item_name_inverted.bmp", ImageFormat.Bmp);

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
                                var _SplitText = text.Split('\n');
                                Debug.WriteLine($"PASS: {_SplitText[0]}");
                                return _SplitText[0];
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

            return "fail";
        }

        internal static Tuple<bool, int[]> ReturnMatchingItemPosition(int _startRow, int _startColumn, string _itemName)
        {
            for (int i = _startRow; i < GetInventoryRows; i++)
            {
                for (int j = _startColumn; j < GetInventoryColumns; j++)
                {
                    Debug.WriteLine(String.Format("== Inventory[{0},{1}] ==", i, j));
                    
                    int[] _CellPosition = new int[]
                    {
                        RECTS_INVENTORY[i, j].X + WIN32_MOUSECELLOFFSET,
                        RECTS_INVENTORY[i, j].Y + WIN32_MOUSECELLOFFSET
                    };
                    
                    Win32API.MouseMove(_CellPosition);
                    string _CurrentItemName = ReturnCurrentItemName();

                    if (_CurrentItemName != _itemName)
                    {
                        continue;
                    }
                    
                    Debug.WriteLine($"found match for desired item \'{_itemName}\' : \'{_CurrentItemName}\'");
                    return new Tuple<bool, int[]>(true, new[] { i, j });
                }
            }
            
            
            return new Tuple<bool, int[]>(false, new int[2]);
        }
        
        internal static int ReturnCurrentItemQuantity(int _invRow, int _invColumn)
        {
            Bitmap _ItemImage = ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_INVENTORY[_invRow, _invColumn]);
            Bitmap _QuantityCrop = ImageMatchAPI.CropImage(_ItemImage, RECT_CROPOCRQUANTITY);
            Bitmap _DesaturatedQuantityCrop = ImageMatchAPI.DesaturateImage(_QuantityCrop, IMAGEMATCH_DESATURATIONTHRESHOLD_DIGITS);
            _QuantityCrop.Save("item_quantity.bmp", ImageFormat.Bmp);
            _DesaturatedQuantityCrop.Save("item_quantity_desaturated.bmp", ImageFormat.Bmp);
            Bitmap _InvertedQuantityCrop = ImageMatchAPI.InvertImage(_DesaturatedQuantityCrop);
            _InvertedQuantityCrop.Save("item_quantity_inverted.bmp", ImageFormat.Bmp);
            
            // DEBUG
            try
            {
                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    engine.SetVariable("tessedit_char_whitelist", "0123456789"); // only allow 0-9
                    engine.SetVariable("classify_bln_numeric_mode", "1");
                    using (var img = PixConverter.ToPix(_InvertedQuantityCrop))
                    {
                        using (var page = engine.Process(img, PageSegMode.SingleWord))
                        {
                            var text = page.GetText();
                            Debug.WriteLine("Mean confidence: {0}", page.GetMeanConfidence());
                            if(page.GetMeanConfidence() >= TESSERACT_MINCONFIDENCE)
                            {
                                var _SplitText = text.Split('\n');
                                Debug.WriteLine($"QUANTITY: {_SplitText[0]}");
                                if(int.TryParse(_SplitText[0], out int _Quantity))
                                {
                                    return _Quantity;
                                }
                                
                                Debug.WriteLine("[ERROR] could not parse the quantity text to an integer");
                            }
                            Debug.WriteLine($"text: \'{text}\'");
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

            return -1;
        }
    }
}
