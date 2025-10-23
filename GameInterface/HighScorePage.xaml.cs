using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GameLibrary;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace GameInterface
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HighScorePage : Page
    {
        private List<HighScore> highScores;
        private ListView highScoreListView;

        public HighScorePage()
        {
            this.InitializeComponent();

            highScorePage.Background = new SolidColorBrush(Windows.UI.Colors.Black);

            // Root container
            //StackPanel root = new StackPanel
            //{
            //    Margin = new Thickness(0, 0, 0, 0),
            //    Background = new SolidColorBrush(Windows.UI.Colors.Black)
            //};

            // Title

            ImageBrush bg = new ImageBrush();
            bg.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Starfield3.png"));
            bg.Stretch = Stretch.UniformToFill; // Fill the whole page
            highScorePage.Background = bg;

            TextBlock title = new TextBlock
            {
                Text = "High Scores",
                FontSize = 36,
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 130, 0, 0)
            };
            highScorePage.Children.Add(title);

            highScoreListView = new ListView
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 24,
                Margin = new Thickness(0, 210, 0, 0),
                Background = bg,
                ItemContainerStyle = new Style(typeof(ListViewItem))
                {
                    Setters =
                    {
                        new Setter(ListViewItem.ForegroundProperty, new SolidColorBrush(Windows.UI.Colors.White)),
                        new Setter(ListViewItem.BackgroundProperty, new SolidColorBrush(Windows.UI.Colors.Transparent)),
                    }
                },
                IsItemClickEnabled = false,
                SelectionMode = ListViewSelectionMode.None
            };
            highScorePage.Children.Add(highScoreListView);

            // Back button
            Button backButton = new Button
            {
                Content = "Back to Menu",
                Width = 200,
                Foreground = new SolidColorBrush(Windows.UI.Colors.White),
                Height = 50,
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, 50),
            };
            backButton.Click += BackButton_Click;
            highScorePage.Children.Add(backButton);

            this.Content = highScorePage;
        }
        // Updates the ListView with the current high scores
        private void UpdateHighScoreList()
        {
            // Sorts scores in descending order
            highScores.Sort((a, b) => b.Time.CompareTo(a.Time));
            highScoreListView.Items.Clear();

            // Displays top 10 scores
            for (int i = 0; i < Math.Min(10, highScores.Count); i++)
            {
                var entry = highScores[i];
 
                highScoreListView.Items.Add($"{entry.Initials} {entry.Time.Minutes:D2}:{entry.Time.Seconds:D2}");
            }
        }

        // Loads high scores when the page is navigated to
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);
            await LoadHighScoresAsync();

            highScores = App.HighScores ?? new List<HighScore>();
            UpdateHighScoreList();
        }


        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LandingPage));
        }

        // Loads high scores from local storage
        private async Task LoadHighScoresAsync()
        {
            try
            {
                // Get the local folder
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.GetFileAsync("highscores.txt");

                // Clear existing scores
                App.HighScores.Clear();

                // Read scores from file
                using (Stream stream = await file.OpenStreamForReadAsync())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var parts = line.Split(':');
                        if (parts.Length == 2 && double.TryParse(parts[1], out double seconds))
                        {
                            App.HighScores.Add(new HighScore(parts[0], TimeSpan.FromSeconds(seconds)));
                        }
                        else
                        {
                            Debug.WriteLine($"Invalid line in high scores file: {line}");
                        }
                    }
                }

                // Sort descending and keep top 10
                App.HighScores = App.HighScores.OrderByDescending(hs => hs.Time).Take(10).ToList();

                Debug.WriteLine("High scores loaded successfully.");
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("High scores file not found. Starting with empty list.");
                App.HighScores = new List<HighScore>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error loading high scores: " + ex.Message);
            }
        }

        // Saves high scores to local storage
        private async Task SaveHighScoresAsync()
        {
            try
            {
                // Get the local folder
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.CreateFileAsync("highscores.txt", CreationCollisionOption.ReplaceExisting);

                // Write scores to file
                using (Stream stream = await file.OpenStreamForWriteAsync())
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    foreach (var hs in App.HighScores)
                    {
                        // Save as Initials:TotalSeconds
                        await writer.WriteLineAsync($"{hs.Initials}:{hs.Time.TotalSeconds}");
                    }
                }

                Debug.WriteLine("High scores saved successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error saving high scores: " + ex.Message);
            }
        }
    }
}
