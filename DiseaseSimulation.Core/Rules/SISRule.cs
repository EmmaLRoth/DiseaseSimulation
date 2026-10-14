namespace DiseaseSimulation.Core.Rules;

using DiseaseSimulation.Core.Models;

public class SISRule : IModelRules
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
                    entity.InfectedFor = 0;
                    return HealthState.Infected;
                }

                return HealthState.Susceptible;

            case HealthState.Infected:
                entity.InfectedFor++;

                if (entity.InfectedFor >= config.InfectionDuration)
                {
                    entity.InfectedFor = 0;
                    return HealthState.Susceptible;
                }

                return HealthState.Infected;

            default:
                return entity.HealthState;
        }
    }
}