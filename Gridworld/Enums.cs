using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gridworld
{
    public enum GridState
    {
        Empty,
        Goal,
        Blocked,
        Occupied
    }

    public enum GridValue
    {
        Up,
        Left,
        Down,
        Right
    }

    public enum Action
    {
        Up,
        Left,
        Down,
        Right
    }
}
