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
        bool moveLeft, moveRight;
        List<Rectangle> itemRemover = new List<Rectangle>();

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
                }
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
            enemyCounter = rnd.Next(1, 3);
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
                Height = 70,
                Width = 50,
                Fill = enemySprite
            };

            Canvas.SetTop(newEnemy, -100);
            Canvas.SetLeft(newEnemy, rnd.Next(30, 520));
            canvasBackground.Children.Add(newEnemy);
        }
    }
}