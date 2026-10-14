namespace DiseaseSimulation.Core.Models;

public class Cell
{
    public Position Position { get; set; }
    public Entity? Entity { get; set; }
    
    public Cell(Position position)
    {
        this.Position = position;
        this.Entity = null;
    }

    public void Print()
    {
        if (Entity != null)
        {
            Entity.Print();
            return;
        }
        
        Console.Write('.');
    }

    public bool HasEntity()
    {
        if (Entity != null)
        {
            return true;
        }
        
        return false;
    }
}