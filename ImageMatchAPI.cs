using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PDTrader.Library;

namespace PDTrader
{
    internal static class ImageMatchAPI
    {
        
        internal static float CompareImages(Bitmap _chestImage, Bitmap _invImage)
        {
            if (_chestImage.Width != _invImage.Width || _chestImage.Height != _invImage.Height)
            {
                Debug.WriteLine("[ERROR] image heights aren't matching");
                return 0f;
            }
            _chestImage.Save("chest_original.bmp", ImageFormat.Bmp);
            _invImage.Save("inv_original.bmp", ImageFormat.Bmp);
            
            //Bitmap _imageOneResized = new Bitmap(_imageOne,new Size(_imageOne.Width/3,_imageOne.Height/3));
            //Bitmap _imageTwoResized = new Bitmap(_imageTwo,new Size(_imageTwo.Width/3,_imageTwo.Height/3));
            //_imageOneResized.Save("1r.bmp", ImageFormat.Bmp);
            //_imageTwoResized.Save("2r.bmp", ImageFormat.Bmp);
            
            Bitmap _imageOneCropped = CropImage(_chestImage, RECT_CROPIMAGEMATCH);
            Bitmap _imageTwoCropped = CropImage(_invImage, RECT_CROPIMAGEMATCH);
            _imageOneCropped.Save("chest_cropped.bmp", ImageFormat.Bmp);
            _imageTwoCropped.Save("inv_cropped.bmp", ImageFormat.Bmp);

            Color _imageOneColor = new Color();
            Color _imageTwoColor = new Color();
            int _invalidMatches = 0;
            int _validMatches = 0;
                
            // original test
            /*for (int i = 0; i < _imageOne.Width; i++)
            {
                for (int j = 0; j < _imageOne.Height; j++)
                {
                    _imageOneColor = _imageOne.GetPixel(i, j).ToString();
                    _imageTwoColor = _imageTwo.GetPixel(i, j).ToString();
                    if (_imageOneColor != _imageTwoColor)
                    {
                        _invalidMatches++;
                    }
                    else
                    {
                        _validMatches++;
                    }
                }
            }*/

            // resize test
            /*for (int i = 0; i < _imageOneResized.Width; i++)
            {
                for (int j = 0; j < _imageOneResized.Height; j++)
                {
                    _imageOneColor = _imageOneResized.GetPixel(i, j).ToString();
                    _imageTwoColor = _imageTwoResized.GetPixel(i, j).ToString();
                    if (_imageOneColor != _imageTwoColor)
                    {
                        _invalidMatches++;
                    }
                    else
                    {
                        _validMatches++;
                    }
                }
            }*/

            // crop test
            /*for (int i = 0; i < _imageOneCropped.Width; i++)
            {
                for (int j = 0; j < _imageOneCropped.Height; j++)
                {
                    _imageOneColor = _imageOneCropped.GetPixel(i, j);
                    _imageTwoColor = _imageTwoCropped.GetPixel(i, j);
                    if (_imageOneColor != _imageTwoColor)
                    {
                        _invalidMatches++;
                    }
                    else
                    {
                        _validMatches++;
                    }
                }
            }*/
            
            // crop with rgb variation test
            for (int i = 0; i < _imageOneCropped.Width; i++)
            {
                for (int j = 0; j < _imageOneCropped.Height; j++)
                {
                    _imageOneColor = _imageOneCropped.GetPixel(i, j);
                    _imageTwoColor = _imageTwoCropped.GetPixel(i, j);

                    if (Math.Abs(_imageOneColor.R - _imageTwoColor.R) > IMAGEMATCH_RGBVARIANCE)
                    {
                        _invalidMatches++;
                        continue;
                    }
                    if (Math.Abs(_imageOneColor.G - _imageTwoColor.G) > IMAGEMATCH_RGBVARIANCE)
                    {
                        _invalidMatches++;
                        continue;
                    }
                    if (Math.Abs(_imageOneColor.B - _imageTwoColor.B) > IMAGEMATCH_RGBVARIANCE)
                    {
                        _invalidMatches++;
                        continue;
                    }

                    _validMatches++;
                }
            }

            int _totalMatches = _invalidMatches + _validMatches;
            float _validConfidence = (float)_validMatches / (float)_totalMatches;
            
            Debug.WriteLine($"valid: {_validMatches} invalid: {_invalidMatches} rgb variance: {IMAGEMATCH_RGBVARIANCE}");
            
            return _validConfidence;
        }

        internal static Bitmap CropImage(Bitmap img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            return bmpImage.Clone(cropArea, bmpImage.PixelFormat);
        }

        internal static Tuple<bool, int[]> ReturnFirstOccupiedChestCell(int _startRow, int _startColumn)
        {
            for (int i = _startRow; i < GetChestRows; i++)
            {
                for (int j = _startColumn; j < GetChestColumns; j++)
                {
                    Debug.WriteLine(String.Format("== Chest[{0},{1}] ==", i, j));
                    float _MatchConfidence = CompareImages(
                        ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_SMALLWOODENBARREL[i, j]), 
                        ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_HOTBAR[IMAGEMATCH_HOTBAR_EMPTYCELL])
                    );

                    if (_MatchConfidence >= IMAGEMATCH_MINCONFIDENCE)
                    {
                        Debug.WriteLine($"Images match with {_MatchConfidence * 100f:N2}% confidence");
                    }
                    else
                    {
                        Debug.WriteLine($"Images are not a match with only {_MatchConfidence * 100f:N2}% confidence");
                        return new Tuple<bool, int[]>(true, new[] { i, j });
                    }
                }
            }

            return new Tuple<bool, int[]>(false, new int[2]);
        }
        
        internal static Tuple<bool, int[]> ReturnFirstUnoccupiedChestCell(int _startRow, int _startColumn)
        {
            for (int i = _startRow; i < GetChestRows; i++)
            {
                for (int j = _startColumn; j < GetChestColumns; j++)
                {
                    Debug.WriteLine(String.Format("== Chest[{0},{1}] ==", i, j));
                    float _MatchConfidence = CompareImages(
                        ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_SMALLWOODENBARREL[i, j]), 
                        ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_HOTBAR[IMAGEMATCH_HOTBAR_EMPTYCELL])
                    );

                    if (_MatchConfidence >= IMAGEMATCH_MINCONFIDENCE)
                    {
                        Debug.WriteLine($"Images match with {_MatchConfidence * 100f:N2}% confidence");
                        return new Tuple<bool, int[]>(true, new[] { i, j });
                    }
                    else
                    {
                        Debug.WriteLine($"Images are not a match with only {_MatchConfidence * 100f:N2}% confidence");
                    }
                }
            }

            return new Tuple<bool, int[]>(false, new int[2]);
        }
        
        internal static Tuple<bool, int[]> ReturnFirstUnoccupiedInventoryCell(int _startRow, int _startColumn)
        {
            for (int i = _startRow; i < GetInventoryRows; i++)
            {
                for (int j = _startColumn; j < GetInventoryColumns; j++)
                {
                    Debug.WriteLine(String.Format("== Inventory[{0},{1}] ==", i, j));
                    float _MatchConfidence = CompareImages(
                        ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_INVENTORY[i, j]), 
                        ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_HOTBAR[IMAGEMATCH_HOTBAR_EMPTYCELL])
                    );

                    if (_MatchConfidence >= IMAGEMATCH_MINCONFIDENCE)
                    {
                        Debug.WriteLine($"Images match with {_MatchConfidence * 100f:N2}% confidence");
                        return new Tuple<bool, int[]>(true, new[] { i, j });
                    }
                    else
                    {
                        Debug.WriteLine($"Images are not a match with only {_MatchConfidence * 100f:N2}% confidence");
                    }
                }
            }

            return new Tuple<bool, int[]>(false, new int[2]);
        }
        
        internal static Tuple<bool, int[]> ReturnFirstMatchingInventoryCell(int _startRow, int _startColumn, Bitmap _item)
        {
            Bitmap _DebugOne = ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_INVENTORY[1, 3]); // DEBUG
            _DebugOne.Save("inv_13.bmp", ImageFormat.Bmp); // DEBUG
            Bitmap _DebugTwo = ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_INVENTORY[2, 1]); // DEBUG
            _DebugTwo.Save("inv_21.bmp", ImageFormat.Bmp); // DEBUG
            Bitmap _DebugThree = ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_INVENTORY[3, 0]); // DEBUG
            _DebugThree.Save("inv_30.bmp", ImageFormat.Bmp); // DEBUG
            
            for (int i = _startRow; i < GetInventoryRows; i++)
            {
                for (int j = _startColumn; j < GetInventoryColumns; j++)
                {
                    Debug.WriteLine(String.Format("== Inventory[{0},{1}] ==", i, j));
                    float _MatchConfidence = CompareImages(
                        ScreenCaptureAPI.CapturePaxDeiWindow(RECTS_INVENTORY[i, j]), 
                        _item
                    );

                    if (_MatchConfidence >= IMAGEMATCH_MINCONFIDENCE)
                    {
                        Debug.WriteLine($"Images match with {_MatchConfidence * 100f:N2}% confidence");
                        return new Tuple<bool, int[]>(true, new[] { i, j });
                    }
                    else
                    {
                        Debug.WriteLine($"Images are not a match with only {_MatchConfidence * 100f:N2}% confidence");
                    }
                }
            }
            
            return new Tuple<bool, int[]>(false, new int[2]);
        }

        internal static Bitmap DesaturateImage(Bitmap _image, float _threshold)
        {
            using (Graphics gr = Graphics.FromImage(_image)) // SourceImage is a Bitmap object
            {                
                var gray_matrix = new float[][] { 
                    new float[] { 0.299f, 0.299f, 0.299f, 0, 0 }, 
                    new float[] { 0.587f, 0.587f, 0.587f, 0, 0 }, 
                    new float[] { 0.114f, 0.114f, 0.114f, 0, 0 }, 
                    new float[] { 0,      0,      0,      1, 0 }, 
                    new float[] { 0,      0,      0,      0, 1 } 
                };

                var ia = new System.Drawing.Imaging.ImageAttributes();
                ia.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(gray_matrix));
                ia.SetThreshold(_threshold); // Change this threshold as needed
                var rc = new Rectangle(0, 0, _image.Width, _image.Height);
                gr.DrawImage(_image, rc, 0, 0, _image.Width, _image.Height, GraphicsUnit.Pixel, ia);                
            }

            return _image;
        }

        internal static Bitmap InvertImage(Bitmap _image)
        {
            for (int i = 0; i < _image.Width; i++)
            {
                for (int j = 0; j < _image.Height; j++)
                {
                    Color p = _image.GetPixel(i, j);

                    //extract ARGB value from p
                    int a = p.A;
                    int r = p.R;
                    int g = p.G;
                    int b = p.B;

                    //find negative value
                    r = 255 - r;
                    g = 255 - g;
                    b = 255 - b;

                    //set new ARGB value in pixel
                    _image.SetPixel(i, j, Color.FromArgb(a, r, g, b));
                }
            }
            
            return _image;
        }
    }
}
