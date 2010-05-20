using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tek.Graphics;
using System.Diagnostics;

namespace GridAStar
{
    public sealed class Grid
    {
        public readonly int Width;
        public readonly int Height;
        public readonly Point EndPosition;
        public readonly HashSet<Wall> Walls = new HashSet<Wall>();

        public Grid(int width, int height, Point endPosition)
        {
            this.Width = width;
            this.Height = height;
            this.EndPosition = endPosition;
        }
    }

    public struct Wall : IEquatable<Wall>
    {
        public readonly Point FirstPoint;
        public readonly Point SecondPoint;

        public Wall(Point firstPoint, Point secondPoint)
        {
            this.FirstPoint = firstPoint;
            this.SecondPoint = secondPoint;
        }

        public bool Equals(Wall other)
        {
            return (FirstPoint == other.FirstPoint && SecondPoint == other.SecondPoint)
                || (SecondPoint == other.FirstPoint && FirstPoint == other.SecondPoint);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return FirstPoint.GetHashCode() ^ SecondPoint.GetHashCode();
        }
    }

    [DebuggerDisplay("Thesus: {TheseusPosition}, Minotaur: {MinotaurPosition}")]
    public struct State : IEquatable<State>
    {
        public readonly Point TheseusPosition;
        public readonly Point MinotaurPosition;

        public State(Point theseusPosition, Point minotaurPosition)
        {
            this.TheseusPosition = theseusPosition;
            this.MinotaurPosition = minotaurPosition;
        }

        public bool Equals(State other)
        {
            return TheseusPosition == other.TheseusPosition
                && MinotaurPosition == other.MinotaurPosition;
        }

        public override bool Equals(object obj)
        {
            return obj is Point && Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            return TheseusPosition.GetHashCode() ^ (MinotaurPosition.GetHashCode() << 16);
        }
    }

    public sealed class Query : IPathfindingQuery<State>
    {
        private readonly Grid grid;
        private readonly State initialState;

        public Query(Grid grid, State initialState)
        {
            this.grid = grid;
            this.initialState = initialState;
        }

        public State Source
        {
            get { return initialState; }
        }

        public void GetAdjacentStates(PathNode<State> node, List<AdjacentState<State>> adjacentStates)
        {
            State state = node.State;
            TryAddMove(state, new Point(state.TheseusPosition.X - 1, state.TheseusPosition.Y), adjacentStates);
            TryAddMove(state, new Point(state.TheseusPosition.X + 1, state.TheseusPosition.Y), adjacentStates);
            TryAddMove(state, new Point(state.TheseusPosition.X, state.TheseusPosition.Y), adjacentStates);
            TryAddMove(state, new Point(state.TheseusPosition.X, state.TheseusPosition.Y - 1), adjacentStates);
            TryAddMove(state, new Point(state.TheseusPosition.X, state.TheseusPosition.Y + 1), adjacentStates);
        }

        public float EstimateCostToDestination(State state)
        {
            return Math.Abs(state.TheseusPosition.X - grid.EndPosition.X)
                + Math.Abs(state.TheseusPosition.Y - grid.EndPosition.Y);
        }

        private void TryAddMove(State state, Point point, List<AdjacentState<State>> adjacentStates)
        {
            if (point.X < 0 || point.Y < 0 || point.X >= grid.Width || point.Y >= grid.Height
                || !CanMove(state.TheseusPosition, point))
                return;

            Point minotaurPosition = state.MinotaurPosition;
            MoveMinotaur(ref minotaurPosition, point);
            MoveMinotaur(ref minotaurPosition, point);
            if (minotaurPosition == point) return;

            State newState = new State(point, minotaurPosition);
            adjacentStates.Add(new AdjacentState<State>(newState, 1));
        }

        private bool CanMove(Point first, Point second)
        {
            return !grid.Walls.Contains(new Wall(first, second));
        }

        private void MoveMinotaur(ref Point minotaurPosition, Point point)
        {
            if (minotaurPosition.X != point.X)
            {
                Point targetPosition = new Point(minotaurPosition.X + Math.Sign(point.X - minotaurPosition.X), minotaurPosition.Y);
                if (CanMove(minotaurPosition, targetPosition))
                {
                    minotaurPosition = targetPosition;
                    return;
                }
            }

            if (minotaurPosition.Y != point.Y)
            {
                Point targetPosition = new Point(minotaurPosition.X, minotaurPosition.Y + Math.Sign(point.Y - minotaurPosition.Y));
                if (CanMove(minotaurPosition, targetPosition))
                {
                    minotaurPosition = targetPosition;
                    return;
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Grid grid = new Grid(4, 4, new Point(3, 0));

            grid.Walls.Add(new Wall(new Point(0, 1), new Point(0, 2)));
            grid.Walls.Add(new Wall(new Point(2, 1), new Point(2, 2)));
            grid.Walls.Add(new Wall(new Point(0, 1), new Point(1, 1)));
            grid.Walls.Add(new Wall(new Point(1, 1), new Point(2, 1)));
            grid.Walls.Add(new Wall(new Point(2, 1), new Point(3, 1)));
            grid.Walls.Add(new Wall(new Point(2, 2), new Point(3, 2)));

            State initialState = new State(new Point(1, 1), new Point(3, 0));

            Pathfinder<State> pathfinder = new Pathfinder<State>();
            var query = new Query(grid, initialState);
            State[] states = pathfinder.Find(query);
        }
    }
}
