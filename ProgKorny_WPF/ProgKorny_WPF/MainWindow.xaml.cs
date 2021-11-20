using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ProgKorny_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        List<Rectangle> itemRemover = new List<Rectangle>();
        bool moveLeft, moveRight;

        Random rnd = new Random();
        int enemyCounter = 100;
        int playerSpeed = 10;
        int enemySpeed = 10;
        int limit = 50;
        int score = 0;
        int damage = 0;

        Rect playerHitBox;

        public MainWindow()
        {
            InitializeComponent();

            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            canvasBackground.Focus();

            ImageBrush bg = new ImageBrush();
            bg.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/space.jpg"));
            bg.TileMode = TileMode.Tile;
            bg.Viewport = new Rect(0,0, 1,1);
            bg.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
            canvasBackground.Background = bg;

            ImageBrush playerImage = new ImageBrush();
            playerImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/player.png"));
            player.Fill = playerImage;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);

            enemyCounter -= 1;

            scoreText.Content = "Score: " + score;
            damageText.Content = "Damage: " + damage;

            if (enemyCounter < 0)
            {
                SpawnEnemies();
                enemyCounter = limit;
            }

            if (moveLeft == true && Canvas.GetLeft(player) > 0)
                Canvas.SetLeft(player, Canvas.GetLeft(player) - playerSpeed);

            if (moveRight == true && Canvas.GetLeft(player) + 70 < Application.Current.MainWindow.Width)
                Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);

            foreach (var x in canvasBackground.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "Bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    Rect bulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (Canvas.GetTop(x) < 10)
                        itemRemover.Add(x);

                    foreach (var y in canvasBackground.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "Enemy")
                        {
                            Rect enemyHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bulletHitBox.IntersectsWith(enemyHit))
                            {
                                itemRemover.Add(x);
                                itemRemover.Add(y);
                                score++;
                            }
                        }
                    }

                }

                if (x is Rectangle && (string)x.Tag == "Enemy")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + enemySpeed);

                    if (Canvas.GetTop(x) > 620)
                    {
                        itemRemover.Add(x);
                        damage += 5;
                    }

                    Rect enemyHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(enemyHitBox))
                    {
                        itemRemover.Add(x);
                        damage += 10;
                    }
                }
            }
            foreach (Rectangle i in itemRemover)
            {
                canvasBackground.Children.Remove(i);
            }

            switch (score)
            {
                case 5:
                    limit = 12;
                    enemySpeed = 11;
                    break;
                case 20:
                    limit = 14;
                    enemySpeed = 12;
                    break;
                case 40:
                    limit = 16;
                    enemySpeed = 13;
                    break;
                case 60:
                    limit = 18;
                    enemySpeed = 14;
                    break;
                case 80:
                    limit = 20;
                    enemySpeed = 15;
                    break;
            }
            if (damage > 99)
            {
                gameTimer.Stop();
                damageText.Content = "Damage: 100";
                damageText.Foreground = Brushes.Red;
                MessageBox.Show("Score: " + score + Environment.NewLine + "Press OK to play again","Game Over");

                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
            if (score > 99)
            {
                gameTimer.Stop();
                scoreText.Content = "Score: 100";
                scoreText.Foreground = Brushes.Green;
                MessageBox.Show("Score: " + score + Environment.NewLine + "Press OK to play again", "You Win");

                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }

        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
                moveLeft = true;

            if (e.Key == Key.Right)
                moveRight = true;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
                moveLeft = false;

            if (e.Key == Key.Right)
                moveRight = false;

            if (e.Key == Key.Space)
            {
                Rectangle newBullet = new Rectangle
                {
                    Tag = "Bullet",
                    Height = 20,
                    Width = 4,
                    Fill = Brushes.White,
                    Stroke = Brushes.Blue
                };

                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2);
                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);

                canvasBackground.Children.Add(newBullet);
            }
        }

        private void SpawnEnemies()
        {
            ImageBrush enemySprite = new ImageBrush();
            enemyCounter = rnd.Next(1, 4);
            switch (enemyCounter)
            {
                case 1:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/enemy1.png"));
                    break;
                case 2:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/enemy2.png"));
                    break;
                case 3:
                    enemySprite.ImageSource = new BitmapImage(new Uri("pack://application:,,,/img/enemy3.png"));
                    break;
            }

            Rectangle newEnemy = new Rectangle
            {
                Tag = "Enemy",
                Height = 50,
                Width = 50,
                Fill = enemySprite
            };

            Canvas.SetTop(newEnemy, -100);
            Canvas.SetLeft(newEnemy, rnd.Next(30, 500));
            canvasBackground.Children.Add(newEnemy);
        }
    }
}