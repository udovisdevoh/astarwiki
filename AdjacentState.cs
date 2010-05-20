using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GridAStar
{
    /// <summary>
    /// Represents a state that is adjacent to another one.
    /// </summary>
    /// <typeparam name="TState">The type of the states in the pathfinding graph.</typeparam>
    [Serializable]
    [DebuggerDisplay("{State} (MovementCost: {MovementCost})")]
    public struct AdjacentState<TState>
    {
        #region Fields
        public readonly TState State;
        public readonly float MovementCost;
        #endregion

        #region Constructors
        public AdjacentState(TState state, float movementCost)
        {
            this.State = state;
            this.MovementCost = movementCost;
        }
        #endregion
    }
}
