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

namespace ShapePop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        List<Shape> removeThis = new List<Shape>();



        int spawnRate = 60,currentRate,lastScore=0,health=350,posX,posY;
        int score = 0,prevScore=0;
        double growthRate = 0.6;
        

        Random rnd = new Random();
        int shape_choice;


        MediaPlayer playClickSound = new MediaPlayer();
        MediaPlayer playPopSound = new MediaPlayer();

        Uri ClickSound;
        Uri PopSound;



        Brush brush;
        Brush blackBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));

        //shapes 

        //Shape circle = new Ellipse
        //{
        //    Tag = "circle",
        //    Height = 10,
        //    Width = 10,
        //    Stroke = Brushes.Black,
        //    StrokeThickness = 1,
        //};


        //Shape square = new Rectangle
        //{
        //    Tag = "square",
        //    Height = 10,
        //    Width = 10,
        //    Stroke = Brushes.Black,
        //    StrokeThickness = 1,
        //};


        //Shape bomb = new Ellipse
        //{
        //    Tag = "bomb",
        //    Name = "bomb",
        //    Height = 10,
        //    Width = 10,
        //    Stroke = Brushes.Black,
        //    StrokeThickness = 1,
        //};


        public MainWindow()
        {
            InitializeComponent();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();

            currentRate = spawnRate;

            ClickSound = new Uri("pack://siteoforigin:,,,/audio files/clickedpop.mp3");
            PopSound = new Uri("pack://siteoforigin:,,,/audio files/pop.mp3");



        }

        private void GameLoop(object sender, EventArgs e)
        {
            txtScore.Content = "Score: " + score;
            txtLastScore.Content = "Last Score: " + lastScore;


            currentRate -= 2;
            if(currentRate<1)
            {
                currentRate = spawnRate;
                posX = rnd.Next(15, 700);
                posY = rnd.Next(50, 350);


                brush = new SolidColorBrush(Color.FromRgb((byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255), (byte)rnd.Next(1, 255)));

                Shape circle = new Ellipse
                {
                    Tag = "circle",
                    Height = 10,
                    Width = 10,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                };


                Shape square = new Rectangle
                {
                    Tag = "square",
                    Height = 10,
                    Width = 10,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                };


                shape_choice = rnd.Next(1, 6);

                switch(shape_choice)
                {
                    case 1:
                        circle.Fill = brush;
                        Canvas.SetLeft(circle, posX);
                        Canvas.SetTop(circle, posY);
                        MyCanvas.Children.Add(circle);
                        break;
                        
                    case 2:
                        square.Fill = brush;
                        Canvas.SetLeft(square, posX);
                        Canvas.SetTop(square, posY);
                        MyCanvas.Children.Add(square);
                        break;
                    
                    default:
                        circle.Fill = brush;
                        Canvas.SetLeft(circle, posX);
                        Canvas.SetTop(circle, posY);
                        MyCanvas.Children.Add(circle);
                        break;
                }
                int bomb_choice = rnd.Next(1, 20);
                if(bomb_choice==15)
                {

                    Shape bomb = new Ellipse
                    {
                        Tag = "circle",
                        Name = "bomb",
                        Height = 10,
                        Width = 10,
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        Fill = blackBrush
                    };

                    posX = rnd.Next(15, 700);
                    posY = rnd.Next(50, 350);

                    Canvas.SetLeft(bomb, posX);
                    Canvas.SetTop(bomb, posY);
                    MyCanvas.Children.Add(bomb);
                    
                }

            }

            foreach(var item in MyCanvas.Children.OfType<Shape>())
            {
                if (item.Name != "healthBar")
                {
                    item.Height += growthRate;
                    item.Width += growthRate;
                    item.RenderTransformOrigin = new Point(0.5, 0.5);

                    if (item.Width > 70 && item.Name != "bomb")
                    {
                        removeThis.Add(item);
                        health -= 15;
                        playPopSound.Open(PopSound);
                        playPopSound.Play();
                    }
                    else if (item.Width > 70 && item.Name == "bomb")
                    {
                        removeThis.Add(item);
                        playPopSound.Open(PopSound);
                        playPopSound.Play();
                    }
                    
                }
            }
            if (health > 1)
                healthBar.Width = health;
            else
                GameOver();

            foreach(Shape shape in removeThis)
            {
                MyCanvas.Children.Remove(shape);
            }

            if (score - prevScore == 5)
            {
                spawnRate -= 5;
                growthRate += 0.3;
                prevScore = score;
            }
            


        }

        private void ClickOnCanvas(object sender, MouseButtonEventArgs e)
        {
            if(e.OriginalSource is Shape)
            {
                Shape shape = (Shape)e.OriginalSource;
                if(shape.Name!="healthBar")
                {
                    if (shape.Name == "bomb")
                    {
                        MyCanvas.Children.Remove(shape);
                        GameOver();
                    }
                    MyCanvas.Children.Remove(shape);
                    score++;

                    playClickSound.Open(ClickSound);
                    playClickSound.Play();

                }
            }
        }

        private void GameOver()
        {
            gameTimer.Stop();

            MessageBox.Show("Game Over" + Environment.NewLine + "You Scored: " + score + Environment.NewLine + "click ok to play again", "");
            foreach (var item in MyCanvas.Children.OfType<Shape>())
            {
                if(item.Name!="healthBar")
                    removeThis.Add(item);
            }

            foreach (Shape item in removeThis)
            {
                MyCanvas.Children.Remove(item);
            }


            growthRate = .6;
            spawnRate = 60;
            lastScore = score;
            score = 0;
            currentRate = 5;
            health = 350;
            removeThis.Clear();
            gameTimer.Start();
        }
    }
}
