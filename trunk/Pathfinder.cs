using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GridAStar
{
    /// <summary>
    /// Resolves pathfinding queries on graph of states.
    /// </summary>
    /// <typeparam name="TState">The type of the states in the graph.</typeparam>
    public sealed class Pathfinder<TState>
    {
        #region Fields
        private readonly Dictionary<TState, PathNode<TState>> nodes
            = new Dictionary<TState, PathNode<TState>>();
        private readonly List<PathNode<TState>> openNodes
            = new List<PathNode<TState>>();
        private readonly List<AdjacentState<TState>> tempAdjacentStateList
            = new List<AdjacentState<TState>>();

        /// <remarks>
        /// Lazy initialized.
        /// </remarks>
        private List<TState> tempStateList;
        #endregion

        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Methods
        public void Find(IPathfindingQuery<TState> query, List<TState> path)
        {
            TState source = query.Source;
            float estimatedCostFromSourceToDestination = query.EstimateCostToDestination(source);
            if (estimatedCostFromSourceToDestination == 0)
            {
                path.Add(source);
                return;
            }

            PathNode<TState> sourceNode = AllocatePathNode();
            sourceNode.Reset(source, null, 0, estimatedCostFromSourceToDestination);
            nodes.Add(source, sourceNode);
            sourceNode.Close();
            OpenAdjacentNodes(query, sourceNode);

            PathNode<TState> bestNode = sourceNode;
            while (true)
            {
                if (openNodes.Count == 0)
                {
                    // No solution found.
                    ClearCollections();
                    return;
                }

                bestNode = PopBestOpenNode();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Visiting {0}", bestNode.State);
                Console.ResetColor();

                if (bestNode.EstimatedCostToDestination == 0)
                    break;

                bestNode.Close();
                OpenAdjacentNodes(query, bestNode);
            }

            CreatePath(bestNode, path);
            ClearCollections();
        }

        private void ClearCollections()
        {
            openNodes.Clear();
            foreach (PathNode<TState> node in nodes.Values)
                ReleasePathNode(node);
            nodes.Clear();
        }

        public TState[] Find(IPathfindingQuery<TState> query)
        {
            if (tempStateList == null) tempStateList = new List<TState>();

            Find(query, tempStateList);
            TState[] path = tempStateList.ToArray();
            tempStateList.Clear();
            return path;
        }

        private PathNode<TState> AllocatePathNode()
        {
            return new PathNode<TState>();
        }

        private void ReleasePathNode(PathNode<TState> node) { }

        private void OpenAdjacentNodes(IPathfindingQuery<TState> query, PathNode<TState> node)
        {
            query.GetAdjacentStates(node, tempAdjacentStateList);
            foreach (AdjacentState<TState> adjacentState in tempAdjacentStateList)
            {
                float costFromSource = node.CostFromSource + adjacentState.MovementCost;
                PathNode<TState> adjacentNode;
                if (nodes.TryGetValue(adjacentState.State, out adjacentNode))
                {
                    if (adjacentNode.IsOpen && costFromSource < adjacentNode.CostFromSource)
                        adjacentNode.UpdatePreviousNode(node, costFromSource);
                }
                else
                {
                    adjacentNode = AllocatePathNode();
                    float estimatedCostToDestination = query.EstimateCostToDestination(adjacentState.State);
                    adjacentNode.Reset(adjacentState.State, node, costFromSource, estimatedCostToDestination);

                    nodes.Add(adjacentState.State, adjacentNode);
                    openNodes.Add(adjacentNode);
                }
            }

            tempAdjacentStateList.Clear();
        }

        private PathNode<TState> PopBestOpenNode()
        {
            PathNode<TState> bestOpenNode = null;
            int bestOpenNodeIndex = -1;
            float lowestEstimatedTotalCost = float.PositiveInfinity;

            for (int i = 0; i < openNodes.Count; ++i)
            {
                PathNode<TState> node = openNodes[i];
                if (node.EstimatedTotalCost < lowestEstimatedTotalCost)
                {
                    bestOpenNode = node;
                    bestOpenNodeIndex = i;
                    lowestEstimatedTotalCost = bestOpenNode.EstimatedTotalCost;
                }
            }

            openNodes.RemoveAt(bestOpenNodeIndex);
            return bestOpenNode;
        }

        private void CreatePath(PathNode<TState> lastNode, List<TState> path)
        {
            PathNode<TState> node = lastNode;
            do
            {
                path.Add(node.State);
                node = node.PreviousNode;
            } while (node != null);

            path.Reverse();
        }
        #endregion
    }
}
