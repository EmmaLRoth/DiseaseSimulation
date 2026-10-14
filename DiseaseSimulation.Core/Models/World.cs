namespace DiseaseSimulation.Core.Models;

public class World
{
    public int Width  { get; }
    public int Height  { get; }
    public Cell[,] Cells  { get; }

    public World(int width, int height)
    {
        this.Width = width;
        this.Height = height;
        this.Cells = new Cell[width, height];

        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                this.Cells[x, y] = new Cell(new Position(x, y));
            }
        }
    }
    
    public Cell? GetCell(int x, int y)
    {
        if (x < 0 || x >= Width) {
            return null;
        }

        if (y < 0 || y >= Height)
        {
            return null;
        }
        
        return this.Cells[x, y];
    }

    public IEnumerable<Cell> GetNeighbourCells(int x, int y)
    {
        int[] neighbourPositionsX = { -1, 0, 1 };
        int[] neighbourPositionsY = { -1, 0, 1 };

        foreach (int nx in neighbourPositionsX)
        {
            foreach (int ny in neighbourPositionsY) 
            {
                if (nx == 0 && ny == 0)
                {
                    continue;
                }
                
                int newX =  x + nx;
                int newY =  y + ny;

                if (newX >= 0 && newX < Width && newY >= 0 && newY < Height)
                {
                    yield return this.Cells[newX, newY];
                }
            }
        }
    }
}