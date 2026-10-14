using System.Runtime.InteropServices.Marshalling;
using System.Text;

namespace DiseaseSimulation.Core.Statistics;

public class SimulationDayStats
{
    public int Day { get; set; }
    
    public int Susceptible { get; set; }
    public int Exposed { get; set; }
    public int Infected { get; set; }
    public int Recovered { get; set; }

    public int TotalPopulation => Susceptible + Exposed + Infected + Recovered;

    public string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"Day: {Day}");
        builder.AppendLine($"Total population: {TotalPopulation}");
        builder.AppendLine($"S: {Susceptible},  E:  {Exposed},  I:  {Infected}, R:  {Recovered}");

        return builder.ToString();
    }
}