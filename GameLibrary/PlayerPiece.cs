using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GameLibrary
{
    public class PlayerPiece : GamePiece
    {
        private double step = 15;
        public bool WarpEnabled { get; set; } = true;

        private Thickness objectMargins;
        private Image onScreen;

        public PlayerPiece(Image img) : base(img)
        {
            onScreen = img;
            objectMargins = img.Margin;
        }

        public void Move(Windows.System.VirtualKey direction, double boundaryWidth)
        {
            try
            {
                switch (direction)
                {
                    case Windows.System.VirtualKey.Left:
                    case Windows.System.VirtualKey.A:
                        objectMargins.Left -= step;

                        // If player wants warp or not
                        if (WarpEnabled)
                        {
                            if (objectMargins.Left + onScreen.Width - 20 < 0)
                                objectMargins.Left = boundaryWidth - onScreen.Width;
                        }
                        else
                        {
                            if (objectMargins.Left < 0)
                                objectMargins.Left = 0;
                        }
                        break;

                    case Windows.System.VirtualKey.Right:
                    case Windows.System.VirtualKey.D:
                        objectMargins.Left += step;

                        if (WarpEnabled)
                        {
                            if (objectMargins.Left > boundaryWidth - onScreen.Width)
                                objectMargins.Left = 0;
                        }
                        else
                        {
                            if (objectMargins.Left > boundaryWidth - onScreen.Width)
                                objectMargins.Left = boundaryWidth - onScreen.Width;
                        }
                        break;
                }

                onScreen.Margin = objectMargins;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Player moving error: " + ex.Message);
            }

        }

    }
}
