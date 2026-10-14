namespace DiseaseSimulation.Core.Rules;

using DiseaseSimulation.Core.Models;

public class SEIRRule : IModelRules
{
    public HealthState GetNextState(Entity entity, IEnumerable<Cell> neighbours, SimulationConfig config)
    {
        switch (entity.HealthState)
        {
            case HealthState.Susceptible:
                int numberOfInfectedNeighbours = 0;
                if (neighbours.Count() > 0)
                {
                    foreach (var neighbour in neighbours)
                    {
                        if (neighbour.Entity?.HealthState  == HealthState.Infected)
                        {
                            numberOfInfectedNeighbours++;
                        }
                    }
                }
                
                double random = Random.Shared.NextDouble();
                if (numberOfInfectedNeighbours > 0 && random < config.InfectionProbability)
                {
                    entity.ExposedFor = 0;
                    return HealthState.Exposed;
                }

                return HealthState.Susceptible;

            case HealthState.Exposed:
                entity.ExposedFor++;

                if (entity.ExposedFor >= config.IncubationDuration)
                {
                    entity.InfectedFor = 0;
                    return HealthState.Infected;
                }

                return HealthState.Exposed;

            case HealthState.Infected:
                entity.InfectedFor++;

                if (entity.InfectedFor >= config.InfectionDuration)
                {
                    return HealthState.Recovered;
                }

                return HealthState.Infected;

            case HealthState.Recovered:
                return HealthState.Recovered;

            default:
                return entity.HealthState;
        }
    }
}