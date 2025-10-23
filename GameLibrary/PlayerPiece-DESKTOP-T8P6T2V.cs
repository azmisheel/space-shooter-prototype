using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace GameLibrary
{
    public class PlayerPiece : GamePiece
    {
        private double step = 18;

        private Thickness objectMargins;
        private Image onScreen;

        public PlayerPiece(Image img) : base(img)
        {
            onScreen = img;
            objectMargins = img.Margin;
        }

        public void Move(Windows.System.VirtualKey direction, double boundaryWidth)
        {
           
                switch (direction)
            {
                case Windows.System.VirtualKey.Left: objectMargins.Left -= step; 
                    if (objectMargins.Left + onScreen.Width < 0) objectMargins.Left = boundaryWidth - onScreen.Width; 
                    break;

                case Windows.System.VirtualKey.A:
                    objectMargins.Left -= step;
                    if (objectMargins.Left + onScreen.Width < 0) objectMargins.Left = boundaryWidth - onScreen.Width;
                    break;

                case Windows.System.VirtualKey.Right: 
                    objectMargins.Left += step; 
                    if (objectMargins.Left > boundaryWidth - onScreen.Width) objectMargins.Left = 0; 
                    break;

                case Windows.System.VirtualKey.D:
                    objectMargins.Left += step;
                    if (objectMargins.Left > boundaryWidth - onScreen.Width) objectMargins.Left = 0;
                    break;
            }

            onScreen.Margin = objectMargins;
        }
    }
}
