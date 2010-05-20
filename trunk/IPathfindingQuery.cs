using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridAStar
{
    /// <summary>
    /// Base interface for pathfinding queries.
    /// </summary>
    /// <typeparam name="TState">The type of the states in the graph.</typeparam>
    public interface IPathfindingQuery<TState>
    {
        /// <summary>
        /// Gets the starting state of the pathfinding.
        /// </summary>
        TState Source { get; }

        /// <summary>
        /// Gets the adjacent states of a state in the graph.
        /// </summary>
        /// <param name="node">The pathfinding node of the state for which adjacent states are to be found.</param>
        /// <param name="adjacentStates">A collection in which to return the adjacent states.</param>
        void GetAdjacentStates(PathNode<TState> node, List<AdjacentState<TState>> adjacentStates);

        /// <summary>
        /// Estimates the cost from a state to the destination state.
        /// For the pathfinder to garantee that the path is optimal, this method must be optimistic:
        /// it should return a value that is lower or equal to the final path cost.
        /// </summary>
        /// <param name="state">The state for which the cost is to be evaluated.</param>
        /// <returns>An optimistic estimated of the cost to the destination.</returns>
        float EstimateCostToDestination(TState state);
    }
}
