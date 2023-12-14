using System.Diagnostics;

namespace day14;

public enum Tile
{
    Ground,
    FixedRock,
    LooseRock,
}

public enum TiltDirection
{
    North,
    East,
    South,
    West,
}

public class Map
{
    private readonly int _width;
    private readonly int _height;
    private readonly Tile[,] _tiles;
    private readonly int _hash;

    public Map(Tile[,] tiles)
    {
        _height = tiles.GetLength(0);
        _width = tiles.GetLength(1);
        _tiles = tiles;
        _hash = CalculateHash(tiles);
    }

    private static int CalculateHash(Tile[,] tiles)
    {
        var height = tiles.GetLength(0);
        var width = tiles.GetLength(1);
        
        var h = 17;
        h = h * 31 + height;
        h = h * 31 + width;
        
        for (var row = 0; row < height; row++)
        for (var col = 0; col < width; col++)
            h = h * 31 + tiles[row, col].GetHashCode();
        
        return h;
    }

    public override int GetHashCode()
    {
        return _hash;
    }

    public override bool Equals(object? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is not Map otherMap) return false;
        if (_hash != otherMap._hash) return false;

        if (_width != otherMap._width) return false;
        if (_height != otherMap._height) return false;

        for (var row = 0; row < _height; row++)
        for (var col = 0; col < _width; col++)
            if (_tiles[row, col] != otherMap._tiles[row, col])
                return false;

        return true;
    }

    public Map TiltFullCircle()
    {
        return Tilt(TiltDirection.North)
            .Tilt(TiltDirection.West)
            .Tilt(TiltDirection.South)
            .Tilt(TiltDirection.East);
    }

    public Map Tilt(TiltDirection direction)
    {
        var newTiles = new Tile[_height, _width];
        for (var row = 0; row < _height; row++)
        for (var col = 0; col < _width; col++)
            newTiles[row, col] = _tiles[row, col];

        switch (direction)
        {
            case TiltDirection.North:
            {
                for (var column = 0; column < _width; column++)
                {
                    var nextFree = 0;
                    for (var row = 0; row < _height; row++)
                    {
                        switch (newTiles[row, column])
                        {
                            case Tile.FixedRock:
                                nextFree = row + 1;
                                break;
                            case Tile.LooseRock when nextFree < row:
                                newTiles[nextFree, column] = Tile.LooseRock;
                                newTiles[row, column] = Tile.Ground;
                                nextFree++;
                                break;
                            case Tile.LooseRock:
                                nextFree = row + 1;
                                break;
                        }
                    }
                }
                break;
            }
            case TiltDirection.South:
            {
                for (var column = 0; column < _width; column++)
                {
                    var nextFree = _height - 1;
                    for (var row = _height - 1; row >= 0; row--)
                    {
                        switch (newTiles[row, column])
                        {
                            case Tile.FixedRock:
                                nextFree = row - 1;
                                break;
                            case Tile.LooseRock when nextFree > row:
                                newTiles[nextFree, column] = Tile.LooseRock;
                                newTiles[row, column] = Tile.Ground;
                                nextFree--;
                                break;
                            case Tile.LooseRock:
                                nextFree = row - 1;
                                break;
                        }
                    }
                }
                break;
            }
            case TiltDirection.West:
            {
                for (var row = 0; row < _height; row++)
                {
                    var nextFree = 0;
                    for (var column = 0; column < _width; column++)
                    {
                        switch (newTiles[row, column])
                        {
                            case Tile.FixedRock:
                                nextFree = column + 1;
                                break;
                            case Tile.LooseRock when nextFree < column:
                                newTiles[row, nextFree] = Tile.LooseRock;
                                newTiles[row, column] = Tile.Ground;
                                nextFree++;
                                break;
                            case Tile.LooseRock:
                                nextFree = column + 1;
                                break;
                        }
                    }
                }
                break;
            }
            case TiltDirection.East:
            {
                for (var row = 0; row < _height; row++)
                {
                    var nextFree = _width - 1;
                    for (var column = _width - 1; column >= 0; column--)
                    {
                        switch (newTiles[row, column])
                        {
                            case Tile.FixedRock:
                                nextFree = column - 1;
                                break;
                            case Tile.LooseRock when nextFree > column:
                                newTiles[row, nextFree] = Tile.LooseRock;
                                newTiles[row, column] = Tile.Ground;
                                nextFree--;
                                break;
                            case Tile.LooseRock:
                                nextFree = column - 1;
                                break;
                        }
                    }
                }
                break;
            }
        }

        return new Map(newTiles);
    }

    public int CalculateWeight()
    {
        var totalWeight = 0;

        for (var column = 0; column < _width; column++)
        {
            for (var row = 0; row < _height; row++)
            {
                if (_tiles[row, column] == Tile.LooseRock)
                    totalWeight += _height - row;
            }
        }

        return totalWeight;
    }

    public void Print()
    {
        Console.WriteLine("=== map ===");
        for (var row = 0; row < _height; row++)
        {
            var s = "";
            for (var col = 0; col < _width; col++)
            {
                var c = _tiles[row, col] switch
                {
                    Tile.Ground => '.',
                    Tile.LooseRock => 'O',
                    Tile.FixedRock => '#',
                    _ => throw new UnreachableException(),
                };
                s += c;
            }

            Console.WriteLine(s);
        }
    }
}