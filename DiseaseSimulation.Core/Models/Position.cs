namespace DiseaseSimulation.Core.Models;

public class Position
{
    public int X { get; }
    public int Y { get; }

    public Position(int x, int y)
    {
        X = x; 
        Y = y;
    }

    public override bool Equals(object? obj) => obj is Position p && p.X == X && p.Y == Y;

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public static bool operator ==(Position? a, Position? b) =>
        a?.Equals(b) ?? b is null;

    public static bool operator !=(Position? a, Position? b) => !(a == b);
}