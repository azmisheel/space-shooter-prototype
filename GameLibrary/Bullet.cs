using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace GameLibrary
{
    public class Bullet : GamePiece
    {
        private double step = 15; // speed of the bullet

        private Thickness objectMargins;
        public Image onScreen;

        public bool IsActive { get; private set; } = true;

        public Bullet(Image img) : base(img)
        {
            onScreen = img;
            objectMargins = img.Margin;
        }

        // Move the bullet upwards
        public void MoveUp()
        {
            if (!IsActive) return;

            objectMargins.Top -= step;

            if (objectMargins.Top < 0)
            {
                //remove from screen when off top
                objectMargins.Top = -100;
                onScreen.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                IsActive = false;
            }

            onScreen.Margin = objectMargins;
        }

        // Position the bullet at specified coordinates and activate it
        public void ShootFrom(double left, double top)
        {
            objectMargins.Left = left;
            objectMargins.Top = top;
            onScreen.Margin = objectMargins;
            onScreen.Visibility = Windows.UI.Xaml.Visibility.Visible;
            IsActive = true;
        }
    }
}
