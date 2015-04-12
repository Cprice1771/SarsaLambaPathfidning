using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
    /// <summary>
    /// Interaction logic for GridWorldControl.xaml
    /// </summary>
    [Serializable]
    public partial class GridWorldControl : UserControl
    {
        private const int UP_OPTION = 0;
        private const int RIGHT_OPTION = 1;
        private const int LEFT_OPTION = 2;
        private const int DOWN_OPTION = 3;
        private const int MAX_STEPS = 10;

        private int _rows;
        private int _columns;
        
        private List<List<GridUserControl>> _world;
        Random _rand = new Random();
        private Point _currentLocation;
        private List<StateAction> _lastActions;

        private double _lamda = 0.9;
        private double gamma = 1.0;
        private double _alpha = 0.0;

        public GridWorldControl(int rows = 20, int cols = 20)
        {
            InitializeComponent();

            _rows = rows;
            _columns = cols;

            _lastActions = new List<StateAction>();
            _world =  new List<List<GridUserControl>>();

            int x = 0;
            int y = 0;

            for (int i = 0; i < _rows; i++)
            {
                _world.Add(new List<GridUserControl>());
                for (int j = 0; j < _columns; j++)
                {
                    GridUserControl grid = new GridUserControl();
                    grid.Margin = new Thickness(x, y, 0, 0);
                    
                    _world[i].Add(grid);
                    GridWorldGrid.Children.Add(grid);
                    x += GridUserControl.WIDTH;
                }
                x = 0;
                y += GridUserControl.HEIGHT;
            }

            _world[18][18].State = GridState.Goal;


            _world[12][14].State = GridState.Blocked;
            _world[13][14].State = GridState.Blocked;
            _world[14][14].State = GridState.Blocked;
            _world[14][14].State = GridState.Blocked;
            _world[15][14].State = GridState.Blocked;
            _world[15][13].State = GridState.Blocked;
            _world[15][12].State = GridState.Blocked;
            _world[15][10].State = GridState.Blocked;
            _world[15][11].State = GridState.Blocked;
            _world[15][9].State = GridState.Blocked;
            _world[15][8].State = GridState.Blocked;
            _world[10][7].State = GridState.Blocked;
            _world[9][7].State = GridState.Blocked;
            _world[8][7].State = GridState.Blocked;
            _world[7][7].State = GridState.Blocked;
            _world[5][2].State = GridState.Blocked;
            _world[5][3].State = GridState.Blocked;
            _world[6][13].State = GridState.Blocked;
            _world[6][14].State = GridState.Blocked;
            _world[6][15].State = GridState.Blocked;
            _world[6][16].State = GridState.Blocked;
            _world[6][17].State = GridState.Blocked;
            _world[6][18].State = GridState.Blocked;

            _world[16][10].State = GridState.Blocked;
            _world[17][11].State = GridState.Blocked;
            _world[3][5].State = GridState.Blocked;
            _world[3][6].State = GridState.Blocked;


            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    //if (_world[i][j].State == GridState.Blocked || _world[i][j].State == GridState.Goal)
                    //    continue;

                    _world[i][j].UpValueQ = (_rand.NextDouble() + .01) * .1;
                    _world[i][j].LeftValueQ = (_rand.NextDouble() + .01) * .1;
                    _world[i][j].DownValueQ = (_rand.NextDouble() + .01) * .1;
                    _world[i][j].RightValueQ = (_rand.NextDouble() + .01) * .1;
                }
            }

            SpawnPlayer();

            this.Height = GridUserControl.HEIGHT * _rows;
            this.Width = GridUserControl.WIDTH * _columns;
        }

        private void SpawnPlayer()
        {
            int startingRow = _rand.Next(_rows);
            int startingCol = _rand.Next(_columns);

            while (_world[startingRow][startingCol].State == GridState.Blocked ||
                   _world[startingRow][startingCol].State == GridState.Goal)
            {
                startingRow = _rand.Next(_rows);
                startingCol = _rand.Next(_columns);
            }

            _world[startingRow][startingCol].State = GridState.Occupied;
            _currentLocation = new Point(startingRow, startingCol);
        }

        public GridWorldControl(List<List<GridUserControl>> world)
        {
            InitializeComponent();

            _world = world;

            _rows = _world.Count;
            _columns = _world[0].Count;

            this.Height = GridUserControl.HEIGHT * _rows;
            this.Width = GridUserControl.WIDTH * _columns;
        }

        public void MakeMove()
        {
            GridUserControl state = _world[(int) _currentLocation.X][(int) _currentLocation.Y];

            double upConfidence = state.UpValueQ + (.5 * (MAX_STEPS - state.UpValueE));
            double downConfidence = state.DownValueQ + (.5 * (MAX_STEPS - state.DownValueE));
            double leftConfidence = state.LeftValueQ + (.5 * (MAX_STEPS - state.LeftValueE));
            double rightConfidence = state.RightValueQ + (.5 * (MAX_STEPS - state.RightValueE));
            double totalChance = upConfidence + downConfidence + leftConfidence + rightConfidence;

            double upChance = (upConfidence / totalChance) * 100;
            double downChance =(downConfidence / totalChance) * 100;
            double leftChance = (leftConfidence / totalChance) * 100;
            double rightChance = (rightConfidence / totalChance) * 100;

            int output = _rand.Next(1, 101);
            int option = 0;

            if (output < upChance)
                option = UP_OPTION;
            else if (output < upChance + downChance)
                option = DOWN_OPTION;
            else if (output < upChance + downChance + leftChance)
                option = LEFT_OPTION;
            else
                option = RIGHT_OPTION;

            
            bool validMove = false;
            Point lastState = _currentLocation;
            Action action = Action.Up;
            switch (option)
            {
                case UP_OPTION:
                    validMove = MoveUp();
                    action = Action.Up;
                    break;
                case RIGHT_OPTION:
                    validMove = MoveRight();
                    action = Action.Right;
                    break;
                case LEFT_OPTION:
                    validMove = MoveLeft();
                    action = Action.Left;
                    break;
                case DOWN_OPTION:
                    validMove = MoveDown();
                    action = Action.Down;
                    break;
            }

            switch (action)
            {
                case Action.Up:
                    _world[(int)lastState.X][(int)lastState.Y].UpValueE++;
                    break;
                case Action.Right:
                    _world[(int)lastState.X][(int)lastState.Y].RightValueE++;
                    break;
                case Action.Left:
                    _world[(int)lastState.X][(int)lastState.Y].LeftValueE++;
                    break;
                case Action.Down:
                    _world[(int)lastState.X][(int)lastState.Y].DownValueE++;
                    break;
            }

            if (validMove)
            {
                
                StateAction sa = new StateAction();
                sa.State = lastState;
                sa.PlayerAction = action;
                //Only add the last action in each state
                RemoveState(sa);
                _lastActions.Add(sa);
            }

            if (_world[(int) _currentLocation.X][(int) _currentLocation.Y].State == GridState.Goal)
                WinGame();

        }

        private void RemoveState(StateAction sa)
        {
            for(int i = _lastActions.Count - 1; i >= 0; i--)
            {
                if (_lastActions[i].State.X == sa.State.X && _lastActions[i].State.Y == sa.State.Y)
                {
                    _lastActions.RemoveAt(i);
                    return;
                }
            }
        }

        private void WinGame()
        {
            int MaxStepsWeCareAbout = 500;

            if (MaxStepsWeCareAbout > _lastActions.Count)
                MaxStepsWeCareAbout = _lastActions.Count;

            int i = 1;
            for (int j = MaxStepsWeCareAbout; j > 0; j--)
            {
                switch (_lastActions[_lastActions.Count - i].PlayerAction)
                {
                    case Action.Up:
                        _world[(int)_lastActions[_lastActions.Count - i].State.X][(int)_lastActions[_lastActions.Count - i].State.Y].UpValueQ += _lamda * ((double)j / MaxStepsWeCareAbout);
                        //_world[(int) _lastActions[_lastActions.Count - i].State.X][(int) _lastActions[_lastActions.Count - i].State.Y].UpValueE++;
                        break;
                    case Action.Right:
                        _world[(int)_lastActions[_lastActions.Count - i].State.X][(int)_lastActions[_lastActions.Count - i].State.Y].RightValueQ += _lamda * ((double)j / MaxStepsWeCareAbout);
                        //_world[(int)_lastActions[_lastActions.Count - i].State.X][(int)_lastActions[_lastActions.Count - i].State.Y].RightValueE++;
                        break;
                    case Action.Left:
                        _world[(int)_lastActions[_lastActions.Count - i].State.X][(int)_lastActions[_lastActions.Count - i].State.Y].LeftValueQ += _lamda * ((double)j / MaxStepsWeCareAbout);
                        //_world[(int)_lastActions[_lastActions.Count - i].State.X][(int)_lastActions[_lastActions.Count - i].State.Y].LeftValueE++;
                        break;
                    case Action.Down:
                        _world[(int)_lastActions[_lastActions.Count - i].State.X][(int)_lastActions[_lastActions.Count - i].State.Y].DownValueQ += _lamda * ((double)j / MaxStepsWeCareAbout);
                        //_world[(int)_lastActions[_lastActions.Count - i].State.X][(int)_lastActions[_lastActions.Count - i].State.Y].DownValueE++;
                        break;
                }

                i++;

            }
            _lastActions.Clear();
            SpawnPlayer();
        }

        public bool MoveUp()
        {
            if (_currentLocation.X == 0 || _world[(int)_currentLocation.X - 1][(int)_currentLocation.Y].State == GridState.Blocked)
                return false;

            _world[(int)_currentLocation.X][(int)_currentLocation.Y].State = GridState.Empty;

            if(_world[(int)_currentLocation.X - 1][(int)_currentLocation.Y].State != GridState.Goal)
                _world[(int)_currentLocation.X - 1][(int)_currentLocation.Y].State = GridState.Occupied;

            _currentLocation.X--;

            return true;
        }

        public bool MoveDown()
        {
            if (_currentLocation.X == _world.Count - 1 || _world[(int)_currentLocation.X + 1][(int)_currentLocation.Y].State == GridState.Blocked)
                return false;

            _world[(int)_currentLocation.X][(int)_currentLocation.Y].State = GridState.Empty;

            if (_world[(int)_currentLocation.X + 1][(int)_currentLocation.Y].State != GridState.Goal)
                _world[(int)_currentLocation.X + 1][(int)_currentLocation.Y].State = GridState.Occupied;

            _currentLocation.X++;

            return true;
        }

        public bool MoveLeft()
        {
            if (_currentLocation.Y == 0 || _world[(int)_currentLocation.X ][(int)_currentLocation.Y - 1].State == GridState.Blocked)
                return false;


            _world[(int)_currentLocation.X][(int)_currentLocation.Y].State = GridState.Empty;

            if (_world[(int)_currentLocation.X ][(int)_currentLocation.Y - 1].State != GridState.Goal)
                _world[(int)_currentLocation.X][(int)_currentLocation.Y - 1].State = GridState.Occupied;

            _currentLocation.Y--;

            return true;
        }

        public bool MoveRight()
        {
            if (_currentLocation.Y == _world[(int)_currentLocation.X].Count - 1 || _world[(int)_currentLocation.X][(int)_currentLocation.Y + 1].State == GridState.Blocked)
                return false;

            _world[(int)_currentLocation.X][(int)_currentLocation.Y].State = GridState.Empty;

            if (_world[(int)_currentLocation.X][(int)_currentLocation.Y + 1].State != GridState.Goal)
                _world[(int)_currentLocation.X][(int)_currentLocation.Y + 1].State = GridState.Occupied;

            _currentLocation.Y++;

            return true;
        }

        struct StateAction
        {
            public Point State { get; set; }
            public Action PlayerAction { get; set; }
        }

        private void GridWorldGrid_KeyDown(object sender, KeyEventArgs e)
        {
            int option = -1;
            if (e.Key == Key.Right)
                option = 1;
            else if (e.Key == Key.Up)
                option = 0;
            else if (e.Key == Key.Left)
                option = 2;
            else if (e.Key == Key.Down)
                option = 3;

            if (option == -1)
                return;

            bool validMove = false;
            Point lastState = _currentLocation;
            Action action = Action.Up;
            switch (option)
            {
                case 0:
                    validMove = MoveUp();
                    action = Action.Up;
                    break;
                case 1:
                    validMove = MoveRight();
                    action = Action.Right;
                    break;
                case 2:
                    validMove = MoveLeft();
                    action = Action.Left;
                    break;
                case 3:
                    validMove = MoveDown();
                    action = Action.Down;
                    break;
            }

            if (validMove)
            {
                StateAction sa = new StateAction();
                sa.State = lastState;
                sa.PlayerAction = action;
                _lastActions.Add(sa);
            }

            if (_world[(int)_currentLocation.X][(int)_currentLocation.Y].State == GridState.Goal)
                WinGame();
        }

        private void GridWorldGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += GridWorldGrid_KeyDown;
        }

        public void Load(string filePath)
        {
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    SetWorldFromSimple((List<List<SimpleGrid>>)formatter.Deserialize(stream));
                } 
            }
            catch (IOException)
            {
                MessageBox.Show("Error Saving File!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Save(string filePath)
        {
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, GetSimpleWorld());
                }
            }
            catch (IOException)
            {
                MessageBox.Show("Error Saving File!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private List<List<SimpleGrid>> GetSimpleWorld()
        {
            List<List<SimpleGrid>> simpleWorld = new List<List<SimpleGrid>>();

            for (int i = 0; i < _rows; i++)
            {
                simpleWorld.Add(new List<SimpleGrid>());
                for (int j = 0; j < _columns; j++)
                {
                    SimpleGrid grid = new SimpleGrid();
                    grid.UpValueQ = _world[i][j].UpValueQ;
                    grid.DownValueQ = _world[i][j].DownValueQ;
                    grid.LeftValueQ = _world[i][j].LeftValueQ;
                    grid.RightValueQ = _world[i][j].RightValueQ;

                    grid.UpValueE = _world[i][j].UpValueE;
                    grid.DownValueE = _world[i][j].DownValueE;
                    grid.LeftValueE = _world[i][j].LeftValueE;
                    grid.RightValueE = _world[i][j].RightValueE;

                    simpleWorld[i].Add(grid);
                }
            }

            return simpleWorld;
        }

        private void SetWorldFromSimple(List<List<SimpleGrid>> simple)
        {

            for (int i = 0; i < _rows; i++)
            {
                for (int j = 0; j < _columns; j++)
                {
                    if(_world[i][j].RightValueQ > 1.0)
                        Console.WriteLine("Broke!");


                    _world[i][j].UpValueQ = simple[i][j].UpValueQ;
                    _world[i][j].DownValueQ = simple[i][j].DownValueQ;
                    _world[i][j].LeftValueQ = simple[i][j].LeftValueQ;
                    _world[i][j].RightValueQ = simple[i][j].RightValueQ;

                    _world[i][j].UpValueE = simple[i][j].UpValueE;
                    _world[i][j].DownValueE = simple[i][j].DownValueE;
                    _world[i][j].LeftValueE = simple[i][j].LeftValueE;
                    _world[i][j].RightValueE = simple[i][j].RightValueE;

                }
            }

        }
    
    }

    [Serializable]
    struct SimpleGrid
    {
        public double UpValueQ { get; set; }
        public double DownValueQ { get; set; }
        public double LeftValueQ { get; set; }
        public double RightValueQ { get; set; }
        public int UpValueE { get; set; }
        public int DownValueE { get; set; }
        public int LeftValueE { get; set; }
        public int RightValueE { get; set; }

    }

}
