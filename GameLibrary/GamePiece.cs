using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Drawing;

//Project: Lab 1A - UWP Game
//Student Name:
//Date:

namespace GameLibrary
{
    public class GamePiece
    {
        private Thickness objectMargins;            //represents the location of the piece on the game board
        private Image onScreen;                     //the image that is displayed on screen
        public Thickness Location                   //get access only - can not directly modify the location of the piece
        {
            get { return onScreen.Margin; }
        }
        
        public GamePiece(Image img)                 //constructor creates a piece and a reference to its associated image
        {                                           //use this to set up other GamePiece properties
            onScreen = img;
            objectMargins = img.Margin;
        }
        public void Dissappear()
        {
            onScreen.Visibility = Visibility.Collapsed;
        }

        public bool Move(Windows.System.VirtualKey direction)   //calculate a new location for the piece, based on a key press
        {
            switch (direction)
            {
                case Windows.System.VirtualKey.Up:
                    objectMargins.Top -= 10;
                    break;
                case Windows.System.VirtualKey.Down:
                    objectMargins.Top += 10;
                    break;
                case Windows.System.VirtualKey.Left:
                    objectMargins.Left -= 10;
                    break;
                case Windows.System.VirtualKey.Right:
                    objectMargins.Left += 10;
                    break;
                default:
                    return false;
            }
            onScreen.Margin = objectMargins;            //assign the new position to the on-screen image
            return true;
        }

        public Rect GetRect()
        {
            return new Rect(Location.Left, Location.Top, onScreen.Width, onScreen.Height);
        }

    }
}
