namespace DiseaseSimulation.Core.Rules;

using DiseaseSimulation.Core.Models;

public interface IModelRules
{
    HealthState GetNextState(Entity entity, IEnumerable<Cell> neighbours, SimulationConfig config);
}