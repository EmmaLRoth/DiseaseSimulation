using DiseaseSimulation.Core.Models;
using DiseaseSimulation.Core.Rules;
using DiseaseSimulation.Core.Statistics;

namespace DiseaseSimulation.Core;

public class SimulationEngine
{
    public SimulationConfig Config { get; set; }
    public World World { get; set; }
    public IModelRules Rules { get; set; }
    
    public List<SimulationDayStats> Stats { get; set; } = new List<SimulationDayStats>();

    public SimulationEngine(SimulationConfig config)
    {
        TxtManager txtManager = new TxtManager();
        (Config, World, Rules) = txtManager.ReadFromTxtFile(config.MapFilepath, config);
        CountDayStats();
    }
    
    public void PrintStats()
    {
        Console.WriteLine($"Day: {Stats.Count}");
        Console.WriteLine($"Total population: {Stats[Stats.Count - 1].TotalPopulation}");
        Console.WriteLine($"S:  {Stats[Stats.Count - 1].Susceptible},   E:  {Stats[Stats.Count - 1].Exposed},   I:  {Stats[Stats.Count - 1].Infected},  R:  {Stats[Stats.Count - 1].Recovered}");
        Console.WriteLine();
    }
    
    public void PrintWorld()
    {
        for (int y = 0; y < World.Height; y++)
        {
            for (int x = 0; x < World.Width; x++)
            {
                World.Cells[x, y].Print();
            }
            Console.WriteLine();
        }
    }

    public void CountDayStats()
    {
        SimulationDayStats dayStats = new();
        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                Entity? entity = World.Cells[x, y].Entity;

                if (entity == null)
                {
                    continue;
                }

                switch (entity.HealthState)
                {
                    case HealthState.Susceptible:
                        dayStats.Susceptible++;
                        break;
                    case HealthState.Infected:
                        dayStats.Infected++;
                        break;
                    case HealthState.Recovered:
                        dayStats.Recovered++;
                        break;
                    case HealthState.Exposed:
                        dayStats.Exposed++;
                        break;
                }
            }
        }
        
        dayStats.Day = Stats.Count + 1;
        
        Stats.Add(dayStats);
    }

    public void NextStep()
    {
        MoveEntities();

        var nextStates = new Dictionary<int, HealthState>();

        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                Entity? entity = World.Cells[x, y].Entity;
                if (entity != null)
                {
                    var neighbours = World.GetNeighbourCells(x, y);
                    var next = Rules.GetNextState(entity, neighbours, Config);
                    nextStates[entity.Id] = next;
                }
            }
        }

        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                Entity? entity = World.Cells[x, y].Entity;
                if (entity != null)
                {
                    entity.HealthState = nextStates[entity.Id];
                }
            }
        }

        CountDayStats();
    }

    public void MoveEntities()
    {
        var moves = new List<(Entity entity, int fromX, int fromY)>();

        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                if (World.Cells[x, y].Entity != null)
                {
                    moves.Add((World.Cells[x, y].Entity!, x, y));
                }
            }
        }

        var rng = Random.Shared;
        for (int i = moves.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (moves[i], moves[j]) = (moves[j], moves[i]);
        }

        foreach (var (entity, fromX, fromY) in moves)
        {
            if (World.Cells[fromX, fromY].Entity != entity)
            {
                continue;
            }

            if (rng.NextDouble() >= Config.MobilityRate)
            {
                continue;
            }

            var neighbours = World.GetNeighbourCells(fromX, fromY).ToArray();
            if (neighbours.Length == 0)
            {
                continue;
            }

            var target = neighbours[rng.Next(neighbours.Length)];

            // Swap
            var targetEntity = target.Entity;

            World.Cells[fromX, fromY].Entity = targetEntity;
            if (targetEntity != null)
            {
                targetEntity.Position = new Position(fromX, fromY);
            }

            target.Entity = entity;
            entity.Position = new Position(target.Position.X, target.Position.Y);
        }
    }

}