using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Gridworld
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GridWorldControl _world;
        private Timer _playTimer;
        


        public MainWindow()
        {
            InitializeComponent();

            _world = new GridWorldControl();
            _world.Margin = new Thickness(0,0,0,0);
            
            MainGrid.Children.Add(_world);
            _playTimer = new Timer(50);
            _playTimer.Elapsed += new ElapsedEventHandler(PlayTimerElapsed);

            this.Height = _world.Height + 125;
            this.Width = _world.Width + 50;
        }

        private void PlayTimerElapsed(object sender, ElapsedEventArgs e)
        {
            lock (_world)
            {
                _world.MakeMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            lock (_world)
            {
                _world.MakeMove();
            }
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_playTimer.Enabled)
            {
                _playTimer.Stop();
                PlayPauseButton.Content = "Play";
            }
            else
            {
                _playTimer.Start();
                PlayPauseButton.Content = "Pause";
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            fileDialog.DefaultExt = ".dat";
            if (fileDialog.ShowDialog() == true)
            {
                _world.Load(fileDialog.FileName);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            fileDialog.FileName = "World.dat";
            fileDialog.DefaultExt = ".dat";
            if (fileDialog.ShowDialog() == true)
            {
                _world.Save(fileDialog.FileName);
            }
        }

    }
}
