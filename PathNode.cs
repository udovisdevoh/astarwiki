using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridAStar
{
    /// <summary>
    /// Represents a node in the pathfinding graph.
    /// </summary>
    public sealed class PathNode<TState>
    {
        #region Fields
        private TState state;
        private PathNode<TState> previousNode;
        private float costFromSource;
        private float estimatedTotalCost;
        private bool isOpen;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the state associated with this node of the graph.
        /// </summary>
        public TState State
        {
            get { return state; }
        }

        /// <summary>
        /// Gets the previous node, least costly node that lead to this one.
        /// <c>Null</c> if this node is the source node.
        /// </summary>
        public PathNode<TState> PreviousNode
        {
            get { return previousNode; }
        }

        /// <summary>
        /// Gets the movement cost from the source node, passing through all previous nodes.
        /// </summary>
        public float CostFromSource
        {
            get { return costFromSource; }
        }

        /// <summary>
        /// Gets the optimistic estimated cost to the destination.
        /// </summary>
        public float EstimatedCostToDestination
        {
            get { return estimatedTotalCost - costFromSource; }
        }

        /// <summary>
        /// Gets the estimated total cost of this node,
        /// the sum of the cost from the source and the estimated cost to the destination.
        /// </summary>
        public float EstimatedTotalCost
        {
            get { return estimatedTotalCost; }
        }

        /// <summary>
        /// Gets a value indicating if this node is in the open list.
        /// </summary>
        public bool IsOpen
        {
            get { return isOpen; }
        }

        /// <summary>
        /// Gets the value indicating if this node is in the closed list.
        /// </summary>
        public bool IsClosed
        {
            get { return !isOpen; }
        }
        #endregion

        #region Methods
        internal void Reset(TState state, PathNode<TState> previousNode, float costFromSource, float estimatedCostToDestination)
        {
            this.state = state;
            this.previousNode = previousNode;
            this.costFromSource = costFromSource;
            this.estimatedTotalCost = costFromSource + estimatedCostToDestination;
            this.isOpen = true;
        }

        internal void UpdatePreviousNode(PathNode<TState> newPreviousNode, float costFromSource)
        {
            float estimatedCostToDestination = EstimatedCostToDestination;
            this.previousNode = newPreviousNode;
            this.costFromSource = costFromSource;
            this.estimatedTotalCost = costFromSource + estimatedCostToDestination;
        }

        internal void Close()
        {
            isOpen = false;
        }
        #endregion
    }
}
