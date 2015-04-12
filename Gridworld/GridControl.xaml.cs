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

namespace Gridworld
{
    public delegate void GridControlDelegate();

    /// <summary>
    /// Interaction logic for GridUserControl.xaml
    /// </summary>
    [Serializable]
    public partial class GridUserControl : UserControl
    {
        public const int WIDTH = 30;
        public const int HEIGHT = 30;

        public const int ARROW_WIDTH = 30;
        public const int ARROW_HEIGHT = 30;

        private GridState _state;

        private readonly BitmapImage _blockedImage = new BitmapImage(new Uri(@"Resources/Blocked.png", UriKind.RelativeOrAbsolute));
        private readonly BitmapImage _goalImage = new BitmapImage(new Uri(@"Resources/Goal.png", UriKind.RelativeOrAbsolute));
        private readonly BitmapImage _emptyImage = new BitmapImage(new Uri(@"Resources/Empty.png", UriKind.RelativeOrAbsolute));
        private readonly BitmapImage _occupiedImage = new BitmapImage(new Uri(@"Resources/Occupied.png", UriKind.RelativeOrAbsolute));
        private readonly BitmapImage _leftImage = new BitmapImage(new Uri(@"Resources/MovedLeft.png", UriKind.RelativeOrAbsolute));
        private readonly BitmapImage _rightImage = new BitmapImage(new Uri(@"Resources/MovedRight.png", UriKind.RelativeOrAbsolute));
        private readonly BitmapImage _downImage = new BitmapImage(new Uri(@"Resources/MovedDown.png", UriKind.RelativeOrAbsolute));
        private readonly BitmapImage _upImage = new BitmapImage(new Uri(@"Resources/MovedUp.png", UriKind.RelativeOrAbsolute));

        public GridState State
        {
            get { return _state; }
            set
            {
                _state = value;

                if (!Dispatcher.CheckAccess())
                {
                    Dispatcher.Invoke((GridControlDelegate)delegate
                    {
                        switch (_state)
                        {
                            case GridState.Blocked:
                                GridStateImage.Source = _blockedImage;
                                break;
                            case GridState.Goal:
                                GridStateImage.Source = _goalImage;
                                break;
                            case GridState.Empty:
                                GridStateImage.Source = _emptyImage;
                                break;
                            case GridState.Occupied:
                                GridStateImage.Source = _occupiedImage;
                                break;
                        }
                    });

                }
                else
                {
                    switch (_state)
                    {
                        case GridState.Blocked:
                            GridStateImage.Source = _blockedImage;
                            break;
                        case GridState.Goal:
                            GridStateImage.Source = _goalImage;
                            break;
                        case GridState.Empty:
                            GridStateImage.Source = _emptyImage;
                            break;
                        case GridState.Occupied:
                            GridStateImage.Source = _occupiedImage;
                            break;
                    }
                }
                
            }
        }

        public GridValue Value
        {
            get
            {
                if (UpValueQ > DownValueQ)
                {
                    if (UpValueQ > LeftValueQ)
                    {
                        if (UpValueQ > RightValueQ)
                            return GridValue.Up;
                        else
                            return GridValue.Right;
                    }
                    else
                    {
                        if (LeftValueQ > RightValueQ)
                            return GridValue.Left;
                        else
                        {
                            return GridValue.Right;
                        }
                    }
                }
                else
                {
                    if (DownValueQ > LeftValueQ)
                    {
                        if (DownValueQ > RightValueQ)
                            return GridValue.Down;
                        else
                            return GridValue.Right;
                    }
                    else
                    {
                        if (LeftValueQ > RightValueQ)
                            return GridValue.Left;
                        else
                        {
                            return GridValue.Right;
                        }
                    }
                }
            }
        }

        public GridValue LastAction { get; set; }

        public double UpValueQ
        {
            get { return _upValueQ; }
            set
            {
                _upValueQ = value;
                UpdateArrow();
            }
        }

        public double DownValueQ
        {
            get { return _downValueQ; }
            set
            {
                _downValueQ = value;
                UpdateArrow();
            }
        }
        public double LeftValueQ
        {
            get { return _leftValueQ; }
            set
            {
                _leftValueQ = value;
                UpdateArrow();
            }
        }
        public double RightValueQ
        {
            get { return _rightValueQ; }
            set
            {
                _rightValueQ = value;

                UpdateArrow();
            }
        }

        public double TotalQValue
        {
            get { return UpValueQ + LeftValueQ + DownValueQ + RightValueQ; }
        }

        

        public int UpValueE 
        { 
            get { return _upValueE; } 
            set
            {
                if (value > 10)
                    _upValueE = 10;
                else
                    _upValueE = value;

            } 
        }
        public int DownValueE
        {
            get { return _downValueE; }
            set
            {
                if (value > 10)
                    _downValueE = 10;
                else
                    _downValueE = value;

            }
        }
        public int LeftValueE
        {
            get { return _leftValueE; }
            set
            {
                if (value > 10)
                    _leftValueE = 10;
                else
                    _leftValueE = value;

            }
        }
        public int RightValueE
        {
            get { return _rightValueE; }
            set
            {
                if (value > 10)
                    _rightValueE = 10;
                else
                    _rightValueE = value;

            }
        }



        private double _upValueQ;
        private double _downValueQ;
        private double _leftValueQ;
        private double _rightValueQ;

        private int _upValueE;
        private int _downValueE;
        private int _leftValueE;
        private int _rightValueE;

        public GridUserControl(GridState state = GridState.Empty)
        {
            InitializeComponent();
            State = state;
            _upValueQ = 0;
            _downValueQ = 0;
            _leftValueQ = 0;
            _rightValueQ = 0;
        }

        public void UpdateArrow()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke((GridControlDelegate) delegate
                {


                    switch (Value)
                    {
                        case GridValue.Up:
                            GridValueImage.Source = _upImage;
                            GridValueImage.Height = ARROW_HEIGHT * (UpValueQ / TotalQValue + .2);
                            GridValueImage.Width = ARROW_WIDTH * (UpValueQ / TotalQValue + .2);
                            break;
                        case GridValue.Left:
                            GridValueImage.Source = _leftImage;
                            GridValueImage.Height = ARROW_HEIGHT * (LeftValueQ / TotalQValue + .2);
                            GridValueImage.Width = ARROW_WIDTH * (LeftValueQ / TotalQValue + .2);
                            break;
                        case GridValue.Down:
                            GridValueImage.Source = _downImage;
                            GridValueImage.Height = ARROW_HEIGHT * (DownValueQ / TotalQValue + .2);
                            GridValueImage.Width = ARROW_WIDTH * (DownValueQ / TotalQValue + .2);
                            break;
                        case GridValue.Right:
                            GridValueImage.Source = _rightImage;
                            GridValueImage.Height = ARROW_HEIGHT * (RightValueQ / TotalQValue + .2);
                            GridValueImage.Width = ARROW_WIDTH * (RightValueQ / TotalQValue + .2);
                            break;

                    }
                });
            }
            else
            {
                switch (Value)
                    {
                        case GridValue.Up:
                            GridValueImage.Source = _upImage;
                            GridValueImage.Height = ARROW_HEIGHT * (UpValueQ / TotalQValue + .2);
                            GridValueImage.Width = ARROW_WIDTH * (UpValueQ / TotalQValue + .2);
                            break;
                        case GridValue.Left:
                            GridValueImage.Source = _leftImage;
                            GridValueImage.Height = ARROW_HEIGHT * (LeftValueQ / TotalQValue + .2);
                            GridValueImage.Width = ARROW_WIDTH * (LeftValueQ / TotalQValue + .2);
                            break;
                        case GridValue.Down:
                            GridValueImage.Source = _downImage;
                            GridValueImage.Height = ARROW_HEIGHT * (DownValueQ / TotalQValue + .2);
                            GridValueImage.Width = ARROW_WIDTH * (DownValueQ / TotalQValue + .2);
                            break;
                        case GridValue.Right:
                            GridValueImage.Source = _rightImage;
                            GridValueImage.Height = ARROW_HEIGHT * (RightValueQ / TotalQValue + .2);
                            GridValueImage.Width = ARROW_WIDTH * (RightValueQ / TotalQValue + .2);
                            break;

                    }
            }
        }
    }
}
