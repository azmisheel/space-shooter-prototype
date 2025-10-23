using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using System;

namespace GameLibrary
{
    public class EnemyPiece : GamePiece
    {
        public double step = 5;

        private Thickness objectMargins;
        public Image onScreen;
        private static Random rand = new Random(); // static, shared by all enemies
        public bool IsHit { get; set; } = false;

        public EnemyPiece(Image img) : base(img)
        {
            onScreen = img;
            objectMargins = img.Margin;
        }

        //Move down automatically additionally teleport to the top if hit by bullet or hit the bottom of the screen
        public void AutoMoveDown(double boundaryHeight, double boundaryWidth)
        {
            objectMargins.Top += step;

            if (objectMargins.Top - 200 > boundaryHeight || IsHit)
            {
                objectMargins.Top = 0;
                objectMargins.Left = rand.Next(0, (int)(boundaryWidth - onScreen.Width));
                onScreen.Visibility = Visibility.Visible;
                IsHit = false;

            }
            onScreen.Margin = objectMargins;
        }

        public void Hit(double boundaryWidth)
        {
            objectMargins.Top = 0;
            objectMargins.Left = rand.Next(0, (int)(boundaryWidth - onScreen.Width));
            onScreen.Margin = objectMargins;
        }
    }
}
