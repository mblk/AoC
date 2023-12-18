using System.Diagnostics;
using Map = aoc.common.Maps.Map<int>;

namespace day17;

public class DijkstraPathFinder
{
    private readonly record struct Node(Position Position, Direction Direction, int Line);

    private readonly Map _map;
    private readonly int _minInLineBeforeTurn;
    private readonly int _maxInLine;

    public DijkstraPathFinder(Map map, int minInLineBeforeTurn, int maxInLine)
    {
        _map = map;
        _minInLineBeforeTurn = minInLineBeforeTurn;
        _maxInLine = maxInLine;
    }

    public Position[] FindPath(Position start, Position goal)
    {
        var openList = new PriorityQueue<Node, int>();
        var costSoFar = new Dictionary<Node, int>();
        var cameFrom = new Dictionary<Node, Node>();

        var startNode1 = new Node(start, Direction.Right, 0);
        var startNode2 = new Node(start, Direction.Down, 0);
        openList.Enqueue(startNode1, 0);
        openList.Enqueue(startNode2, 0);
        costSoFar.Add(startNode1, 0);
        costSoFar.Add(startNode2, 0);

        int getCostSoFar(Node node)
            => costSoFar.TryGetValue(node, out var c)
                ? c
                : Int32.MaxValue;

        while (openList.Count > 0)
        {
            var current = openList.Dequeue();

            if (current.Position == goal && current.Line >= _minInLineBeforeTurn)
                return ReconstructPath(cameFrom, current);

            foreach (var next in GetNext(current))
            {
                if (!_map.Contains(next.Position.X, next.Position.Y))
                    continue;

                var newCost = getCostSoFar(current) + GetStepCost(next.Position);
                if (newCost < getCostSoFar(next))
                {
                    openList.Enqueue(next, newCost);
                    costSoFar[next] = newCost;
                    cameFrom[next] = current;
                }
            }
        }

        throw new Exception("No path");
    }

    private int GetStepCost(Position p)
    {
        return _map.Get(p.X, p.Y);
    }

    private IEnumerable<Node> GetNext(Node node)
    {
        var canGoForward = node.Line < _maxInLine;
        if (canGoForward)
        {
            yield return new Node(node.Position.Move(node.Direction), node.Direction, node.Line + 1);
        }

        var canGoLeftOrRight = node.Line >= _minInLineBeforeTurn;
        if (canGoLeftOrRight)
        {
            var left = node.Direction.TurnLeft();
            yield return new Node(node.Position.Move(left), left, 1);

            var right = node.Direction.TurnRight();
            yield return new Node(node.Position.Move(right), right, 1);
        }
    }

    private static Position[] ReconstructPath(IReadOnlyDictionary<Node, Node> cameFrom, Node endNode)
    {
        var path = new List<Position>();
        var current = endNode;
        while (true)
        {
            path.Add(current.Position);
            if (!cameFrom.TryGetValue(current, out var prev))
                break;
            current = prev;
        }

        Debug.Assert(path.Count == path.Distinct().Count());
        return path.AsEnumerable().Reverse().ToArray();
    }
}