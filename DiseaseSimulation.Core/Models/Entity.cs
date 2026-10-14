namespace DiseaseSimulation.Core.Models;

public class Entity
{
    public int Id { get; }
    public HealthState HealthState { get; set; }
    public Position Position  { get; set; }
    public int InfectedFor { get; set; }
    public int ExposedFor  { get; set; }

    public Entity(int id, HealthState healthState, Position position)
    {
        this.Id = id;
        this.HealthState = healthState;
        this.Position = position;
        this.InfectedFor = 0;
        this.ExposedFor = 0;
    }

    public void Print()
    {
        switch (HealthState)
        {
            case HealthState.Susceptible:
                Console.Write('S');
                break;

            case HealthState.Exposed:
                Console.Write('E');
                break;

            case HealthState.Infected:
                Console.Write('I');
                break;

            case HealthState.Recovered:
                Console.Write('R');
                break;

            default:
                break;
        }
    }
}