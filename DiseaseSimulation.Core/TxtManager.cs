using DiseaseSimulation.Core.Models;
using DiseaseSimulation.Core.Rules;
using DiseaseSimulation.Core.Statistics;

namespace DiseaseSimulation.Core;

public class TxtManager
{
    public (SimulationConfig config, World world, IModelRules rules) ReadFromTxtFile(string filename, SimulationConfig config)
    {
        int numberOfLine = 1;
        int idOfEntity = 0;

        World? world = null;
        IModelRules? rules = null;

        using var reader = new StreamReader(filename);
        string? line = reader.ReadLine();

        while (line != null)
        {
            if (numberOfLine == 1)
            {
                line = line.Trim().ToLower();

                if (line == "seir")
                {
                    config.Model = ModelType.SEIR;
                    rules = new SEIRRule();
                }
                else if (line == "sir")
                {
                    config.Model = ModelType.SIR;
                    rules = new SIRRule();
                }
                else if (line == "sis")
                {
                    config.Model = ModelType.SIS;
                    rules = new SISRule();
                }

                numberOfLine++;
                line = reader.ReadLine();
                continue;
            }
            
            if (numberOfLine == 2)
            {
                string[] dims = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                int width = int.Parse(dims[0]);
                int height = int.Parse(dims[1]);

                world = new World(width, height);

                numberOfLine++;
                line = reader.ReadLine();
                continue;
            }
            
            string[] cells = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int y = numberOfLine - 3;

            for (int x = 0; x < cells.Length; x++)
            {
                switch (cells[x])
                {
                    case "S":
                        world.Cells[x, y].Entity = new Entity(idOfEntity++, HealthState.Susceptible, new Position(x, y));
                        config.EntityCount++;
                        break;

                    case "E":
                        world.Cells[x, y].Entity = new Entity(idOfEntity++, HealthState.Exposed, new Position(x, y));
                        config.EntityCount++;
                        break;

                    case "I":
                        world.Cells[x, y].Entity = new Entity(idOfEntity++, HealthState.Infected, new Position(x, y));
                        config.EntityCount++;
                        break;

                    case "R":
                        world.Cells[x, y].Entity = new Entity(idOfEntity++, HealthState.Recovered, new Position(x, y));
                        config.EntityCount++;
                        break;
                    
                    default:
                        break;
                }
            }

            numberOfLine++;
            line = reader.ReadLine();
        }
        
        return (config, world!, rules!);
    }

    public void WriteToTxtFile(string filename, List<SimulationDayStats> stats)
    {
        StreamWriter writer = new StreamWriter(filename);

        for (int i = 0; i < stats.Count; i++)
        {
            writer.WriteLine(stats[i].ToString());
            writer.WriteLine();
        }
        
        writer.Close();
    }
}