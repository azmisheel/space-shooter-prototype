using GameLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

//Project: Lab 1A - UWP Game
//Student Name:
//Date:

namespace GameInterface
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private PlayerPiece player;
        private DispatcherTimer gameTimer;
        private List<EnemyPiece> enemies = new List<EnemyPiece>();
        private List<Bullet> bullets = new List<Bullet>();
        private int shootCounter = 0;
        private int shootInterval = 5;
        private int lives = 5;
        private TimeSpan gameTime;
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private TextBlock livesText;
        private TextBlock timerText;
        private string gameMode;
        private string difficulty;

        public MainPage()
        {
            this.InitializeComponent();
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            // Create 2 ImageBrushes for main and game grids to simulate moving stars
            // Using DispatcherTimer to shift the background down to make it look animated
            ImageBrush bg = new ImageBrush();
            bg.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Starfield2.png"));
            bg.Stretch = Stretch.UniformToFill;

            ImageBrush bg2 = new ImageBrush();
            bg2.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Starfield2.png"));
            bg2.Stretch = Stretch.UniformToFill;

            // TranslateTransform lets us move the background image down each tick
            TranslateTransform bgTransform = new TranslateTransform();
            bg.Transform = bgTransform;

            gridMain.Background = bg;
            gridGame.Background = bg2;

            // DispatcherTimer to move background to make an animated star field
            DispatcherTimer bgTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(30)
            };
            double offsetY = 0;

            bgTimer.Tick += (s, e) =>
            {
                // Move image down
                offsetY += 3;
                if (offsetY >= gridMain.ActualHeight)
                {
                    offsetY = 0;
                }
                bgTransform.Y = offsetY;
            };

            bgTimer.Start();

            // Use screen height so the player always appears near the bottom regardless of screen size
            double screenHeight = Window.Current.Bounds.Height - 70;
            player = CreatePlayer("Battleship", 30, 510, screenHeight);

            // Spawns 10 random enemies within screen width 
            double screenWidth = gridMain.Width - 80;
            Random rand = new Random();
            for (int i = 0; i <= 10; i++)
            {
                enemies.Add(CreateEnemy("Enemy", 100, rand.Next(50, int.Parse(screenWidth.ToString())), 0));
            }

            // Live Display
            livesText = new TextBlock
            {
                Text = $"Lives: {lives}",
                FontSize = 24,
                Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White),
                Margin = new Thickness(10, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            // Timer Display
            timerText = new TextBlock
            {
                Text = $"Time: {gameTime.TotalSeconds} s",
                FontSize = 24,
                Foreground = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.White),
                Margin = new Thickness(10, 50, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            // Set up the timer for automatic enemy movement
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(50); // 20 times per second
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            gridMain.Children.Add(livesText);
            gridMain.Children.Add(timerText);

        }

        /// <summary>
        /// This method creates the Image object (to display the picture) and sets its properties.
        /// It adds the image to the screen.
        /// Then it calls the GamePiece constructor, passing the Image object as a parameter.
        /// </summary>
        /// <param name="imgSrc">Name of the image file</param>
        /// <param name="size">Size in pixels (used for both dimensions, the images are square)</param>
        /// <param name="left">Left location relative to parent</param>
        /// <param name="top">Top location relative to parent</param>
        /// <returns></returns>
        /// 

        private PlayerPiece CreatePlayer(string imgSrc, int size, int left, double top)
        {
            Image img = new Image();
            img.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{imgSrc}.png"));
            img.Width = size;
            img.Height = size;
            img.Name = $"img{imgSrc}";
            img.Margin = new Thickness(left, top, 0, 0);
            img.VerticalAlignment = VerticalAlignment.Top;
            img.HorizontalAlignment = HorizontalAlignment.Left;

            gridMain.Children.Add(img);

            return new PlayerPiece(img);
        }


        private EnemyPiece CreateEnemy(string imgSrc, int size, double left, int top)
        {
            Image img = new Image();
            img.Source = new BitmapImage(new Uri($"ms-appx:///Assets/{imgSrc}.png"));
            img.Width = size;
            img.Height = size;
            img.Name = $"img{imgSrc}";
            img.Margin = new Thickness(left, top, 0, 0);
            img.VerticalAlignment = VerticalAlignment.Top;
            img.HorizontalAlignment = HorizontalAlignment.Left;

            gridMain.Children.Add(img);

            return new EnemyPiece(img);
        }

        private Bullet CreateBullet()
        {
            Image img = new Image();
            img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Bullet.png"));
            img.Width = 30;
            img.Height = 50;
            img.Margin = new Thickness(0, -100, 0, 0);
            img.VerticalAlignment = VerticalAlignment.Top;
            img.HorizontalAlignment = HorizontalAlignment.Left;

            gridMain.Children.Add(img);

            return new Bullet(img);
        }

        private async void GameLoop(object sender, object e)
        {
            try
            {
                // Track total time since start
                elapsedTime += gameTimer.Interval;

                // Calculate time left (for Casual mode) or survival time (for Endless)
                TimeSpan timeLeft = (gameMode == "Casual") ? (gameTime - elapsedTime) : elapsedTime;
                if (timeLeft < TimeSpan.Zero) timeLeft = TimeSpan.Zero;
                timerText.Text = $"Time: {timeLeft.Minutes:D2}:{timeLeft.Seconds:D2}";

                // Stop game when time runs out in Casual mode
                if (gameMode == "Casual" && elapsedTime >= gameTime)
                {
                    gameTimer.Stop();
                    WinGame();
                    return;
                }

                // Enemy Logic
                // Move enemies down, reduce lives if they reach bottom
                foreach (var enemy in enemies.ToList())
                {
                    if (enemy.Location.Top >= gridMain.ActualHeight - enemy.onScreen.Height)
                    {
                        lives--;
                        PlayLifeLostSound();
                        livesText.Text = $"Lives: {lives}";

                        if (lives <= 0)
                        {
                            gameTimer.Stop();
                            LoseGame();
                            return;
                        }

                        // Reset enemy position after it hits bottom
                        enemy.Hit(gridMain.ActualWidth);
                    }

                    enemy.AutoMoveDown(gridMain.ActualHeight, gridMain.ActualWidth);
                }

                // Bullet logic
                // Move bullets upward, check collision with enemies
                foreach (var bullet in bullets.ToList())
                {
                    bullet.MoveUp();

                    foreach (var enemy in enemies.ToList())
                    {
                        Rect b = bullet.GetRect();
                        Rect en = enemy.GetRect();

                        // Use padding for better hit detection accuracy
                        double padding = 15;
                        Rect bRect = new Rect(b.Left + padding, b.Top + padding, b.Width - 2 * padding, b.Height - 2 * padding);
                        Rect eRect = new Rect(en.Left + padding, en.Top + padding, en.Width - 2 * padding, en.Height - 2 * padding);

                        // Check for overlap between bullet and enemy
                        if (bRect.Left < eRect.Right && bRect.Right > eRect.Left &&
                            bRect.Top < eRect.Bottom && bRect.Bottom > eRect.Top)
                        {
                            // Collision hit - remove bullet, reset enemy, play sound
                            bullet.Dissappear();
                            enemy.Hit(gridMain.ActualWidth);
                            PlayHitSound();

                            // Gradually increase enemy speed for challenge
                            double enemySpeed = enemy.step;
                            if (enemySpeed <= 10)
                                enemy.step += 0.4;
                            break;
                        }
                    }
                }

                // Auto shoot
                shootCounter++;
                if (shootCounter >= shootInterval)
                {
                    shootCounter = 0;

                    // Spawn bullet from player position
                    Bullet b = CreateBullet();
                    bullets.Add(b);
                    b.ShootFrom(player.Location.Left, player.Location.Top - 20);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Game loop error: " + ex.Message);
            }
            
        }


        // Win Message Dialog

        private async void WinGame()
        {
            var dialog = new Windows.UI.Popups.MessageDialog("You survived! You win!");

            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Play Again", cmd =>
            {
                Frame.Navigate(typeof(MainPage), new GameSettings { Difficulty = $"{difficulty}", Mode = $"{gameMode}" });
            }));
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Main Menu", cmd =>
            {
                Frame.Navigate(typeof(LandingPage), new GameSettings { Difficulty = $"{difficulty}", Mode = $"{gameMode}" });
            }));

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;

            await dialog.ShowAsync();
        }
        // Lose Message Dialog
        private async void LoseGame()
        {

            string message = "You lost all your lives! Game over.";

            //Check if the game is endless and if its a new high score
            if (gameMode == "Endless" && IsNewHighScore(elapsedTime))
            {
                await AskForInitialsAndSaveAsync(elapsedTime);
            }

            // Adds high score message
            if (gameMode == "Endless" && IsNewHighScore(elapsedTime))
            {
                message += "\nNew High Score!";
            }

            var dialog = new Windows.UI.Popups.MessageDialog($"{message}");

            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Play Again", cmd =>
            {
                Frame.Navigate(typeof(MainPage), new GameSettings { Difficulty = $"{difficulty}", Mode = $"{gameMode}" });
            }));
            dialog.Commands.Add(new Windows.UI.Popups.UICommand("Main Menu", cmd =>
            {
                Frame.Navigate(typeof(LandingPage), new GameSettings { Difficulty = $"{difficulty}", Mode = $"{gameMode}" });
            }));

            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;


            await dialog.ShowAsync();
        }

        private async void CoreWindow_KeyDown(object sender, Windows.UI.Core.KeyEventArgs e)
        {
            //Calculate new location for the player character
            try
            {
                player.Move(e.VirtualKey, gridMain.ActualWidth);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Player move failed: " + ex.Message);
            }


            // If Escape pressed, go back to landing page
            if (e.VirtualKey == Windows.System.VirtualKey.Escape)
            {
                gameTimer.Stop(); // stop the game loop
                Frame.Navigate(typeof(LandingPage));
            }
            //if (player.Location == collectible.Location)
            //await new MessageDialog("Collision Detected").ShowAsync();
        }

        // Prompt to save initial
        private async Task AskForInitialsAndSaveAsync(TimeSpan survivalTime)
        {
            TextBox inputBox = new TextBox
            {
                PlaceholderText = "Enter 3 initials",
                MaxLength = 3,
                Width = 400,
                Height = 28
            };

            ContentDialog dialog = new ContentDialog
            {
                Title = "Enter your initials",
                Content = inputBox,
                PrimaryButtonText = "OK",
                CloseButtonText = "Skip"
            };

            var result = await dialog.ShowAsync();

            string initials;


            if (result == ContentDialogResult.Primary)
            {
                initials = string.IsNullOrWhiteSpace(inputBox.Text)
                    ? "---"
                    : inputBox.Text.ToUpperInvariant();
            }
            else
            {
                // If they skip the dialog name will be saved as ---
                initials = "---";
            }

            var newScore = new HighScore(initials, survivalTime);

            if (App.HighScores == null) App.HighScores = new List<HighScore>();
            App.HighScores.Add(newScore);

            // Take the top 10
            App.HighScores = App.HighScores
                .OrderByDescending(s => s.Time)
                .Take(10)
                .ToList();

            await SaveHighScoresAsync();
        }

        // Load the highscore txt file
        private async Task LoadHighScoresAsync()
        {
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.GetFileAsync("highscores.txt");

                App.HighScores.Clear();

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

        private bool IsNewHighScore(TimeSpan survivalTime)
        {
            if (App.HighScores == null || App.HighScores.Count < 10)
                return true;

            // Check if it's higher than the lowest score in top 10
            return survivalTime > App.HighScores.Min(h => h.Time);
        }

        // Save the highscore
        private async Task SaveHighScoresAsync()
        {
            try
            {
                var localFolder = ApplicationData.Current.LocalFolder;
                var file = await localFolder.CreateFileAsync("highscores.txt", CreationCollisionOption.ReplaceExisting);

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

        private void PlayHitSound()
        {
            var player = new MediaPlayer();
            player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/Explosion.wav"));
            player.Volume = 0.1;
            player.Play();
            player.MediaEnded += (s, e) => player.Dispose();
        }

        private void PlayLifeLostSound()
        {
            var player = new MediaPlayer();
            player.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/LifeLost.wav"));
            player.Volume = 0.7;
            player.Play();
            player.MediaEnded += (s, e) => player.Dispose();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Load high scores from file
            await LoadHighScoresAsync();
            Debug.WriteLine(ApplicationData.Current.LocalFolder.Path);
            if (e.Parameter is GameSettings settings)
            {
                difficulty = settings.Difficulty;
                gameMode = settings.Mode;
                player.WarpEnabled = settings.Warp;

                //Set gameTime based on difficulty
                switch (difficulty)
                {
                    case "Easy":
                        gameTime = TimeSpan.FromSeconds(30);
                        break;
                    case "Normal":
                        gameTime = TimeSpan.FromMinutes(1);
                        break;
                    case "Hard":
                        gameTime = TimeSpan.FromMinutes(3);
                        break;
                    default:
                        gameTime = TimeSpan.FromMinutes(1);
                        break;
                }
            }
        }
    }
}
