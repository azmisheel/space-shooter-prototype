using GameLibrary;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace GameInterface
{
    public sealed partial class LandingPage : Page
    {
        private string selectedDifficulty = "Normal";
        private string selectedGameMode = "Casual";
        private bool warpEnabled = false;
        private List<HighScore> highScores;

        public LandingPage()
        {
            this.InitializeComponent();
            CreateUI();
        }

        private void CreateUI()
        {
            ImageBrush bg = new ImageBrush();
            bg.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Starfield1.png"));
            bg.Stretch = Stretch.UniformToFill; // Fill the whole page
            landingPage.Background = bg;
            //landingPage.Background = new SolidColorBrush(Windows.UI.Colors.Black);

            // Title
            TextBlock title = new TextBlock
            {
                Text = "AGALAG",
                FontSize = 36,
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 260, 0, 0)
            };
            landingPage.Children.Add(title);


            // Play Button
            Button playButton = new Button
            {
                Content = "Play Game",
                Width = 200,
                Height = 60,
                Background = new SolidColorBrush(Windows.UI.Colors.Black),
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, -30, 0, 50)
            };
            playButton.Click += PlayButton_Click;
            StyleButton(playButton);
            landingPage.Children.Add(playButton);

            // Difficulty Button
            Button difficultyButton = new Button
            {
                Content = $"Difficulty: {selectedDifficulty}",
                Width = 200,
                Height = 50,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 100, 0, 50)
            };
            difficultyButton.Click += DifficultyButton_Click;
            StyleButton(difficultyButton);
            landingPage.Children.Add(difficultyButton);

            // Game Mode Button
            Button modeButton = new Button
            {
                Content = $"Mode: {selectedGameMode}",
                Width = 200,
                Height = 50,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 220, 0, 50)
            };
            modeButton.Click += ModeButton_Click;
            StyleButton(modeButton);
            landingPage.Children.Add(modeButton);

            Button warpButton = new Button
            {
                Content = $"{(warpEnabled ? "Warp" : "No Warp")}",
                Width = 200,
                Height = 50,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 340, 0, 50)
            };
            warpButton.Click += WarpButton_Click;
            StyleButton(warpButton);
            landingPage.Children.Add(warpButton);


            // Highscore Button
            Button scoreButton = new Button
            {
                Content = "Highscore",
                Width = 200,
                Height = 50,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                Margin = new Thickness(0, 460, 0, 50)
            };
            scoreButton.Click += HighScoreButton_Click;
            StyleButton(scoreButton);
            landingPage.Children.Add(scoreButton);

            // Quit Button
            Button quitButton = new Button
            {
                Content = "Quit Game",
                Width = 200,
                Height = 50,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                Margin = new Thickness(0, 580, 0, 50)
            };
            quitButton.Click += QuitGameButton_Click;
            StyleButton(quitButton);
            landingPage.Children.Add(quitButton);

        }

        private async void DifficultyButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a ComboBox inside a ContentDialog
            ComboBox comboBox = new ComboBox
            {
                ItemsSource = new List<string> { "Easy", "Normal", "Hard" },
                SelectedItem = selectedDifficulty,
                Width = 400
            };

            ContentDialog dialog = new ContentDialog
            {
                Title = "Select Difficulty",
                Content = comboBox,
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                Background = new SolidColorBrush(Windows.UI.Colors.White),
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && comboBox.SelectedItem != null)
            {
                selectedDifficulty = comboBox.SelectedItem.ToString();
                ((Button)sender).Content = $"Difficulty: {selectedDifficulty}";
            }
        }

        private async void ModeButton_Click(object sender, RoutedEventArgs e)
        {
            //var dialog = new MessageDialog("Select Game Mode");
            //dialog.Commands.Add(new UICommand("Casual", cmd => selectedGameMode = "Casual"));
            //dialog.Commands.Add(new UICommand("Endless", cmd => selectedGameMode = "Endless"));
            //dialog.CancelCommandIndex = 0;
            //await dialog.ShowAsync();

            ComboBox comboBox = new ComboBox
            {
                ItemsSource = new List<string> { "Casual", "Endless" },
                SelectedItem = selectedGameMode,
                Width = 400
            };

            ContentDialog dialog = new ContentDialog
            {
                Title = "Select Game Mode",
                Content = comboBox,
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                Background = new SolidColorBrush(Windows.UI.Colors.White),
                Foreground = new SolidColorBrush(Windows.UI.Colors.Black),
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && comboBox.SelectedItem != null)
            {
                selectedGameMode = comboBox.SelectedItem.ToString();
                ((Button)sender).Content = $"Mode: {selectedGameMode}";
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to MainPage and pass settings
            Frame.Navigate(typeof(MainPage), new GameSettings
            {
                Difficulty = selectedDifficulty,
                Mode = selectedGameMode,
                Warp = warpEnabled
            });
        }

        private void QuitGameButton_Click(object sender, RoutedEventArgs e)
        {
            CoreApplication.Exit();
        }

        private void HighScoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (highScores == null)
                highScores = new List<HighScore>();
            // Navigate to HighScorePage and pass highscores
            Frame.Navigate(typeof(HighScorePage), highScores);
        }

        private void WarpButton_Click(object sender, RoutedEventArgs e)
        {
            warpEnabled = !warpEnabled;
            ((Button)sender).Content = $"{(warpEnabled ? "Warp" : "No Warp")}";
        }

        private void StyleButton(Button btn)
        {
            btn.Background = new SolidColorBrush(Windows.UI.Colors.Transparent);
            btn.BorderBrush = new SolidColorBrush(Windows.UI.Colors.White);
            btn.BorderThickness = new Thickness(0.5);
            btn.Resources["ButtonBackgroundPointerOver"] = new SolidColorBrush(Windows.UI.Colors.Gray);
            btn.Resources["ButtonForegroundPointerOver"] = new SolidColorBrush(Windows.UI.Colors.White);
        }

    }
    public class GameSettings
    {
        public string Difficulty { get; set; }
        public string Mode { get; set; }
        public bool Warp { get; set; } = true;
    }
}
