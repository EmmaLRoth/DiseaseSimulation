namespace DiseaseSimulation.Core.Models;

public enum ModelType
{
    SIR,
    SEIR,
    SIS,
}
public class SimulationConfig
{
    public int EntityCount { get; set; }
    public double InfectionProbability { get; set; }
    public int InfectionDuration { get; set; }
    public int IncubationDuration { get; set; }
    public ModelType Model { get; set; }
    public int Steps { get; set; }
    public string MapFilepath { get; set; }
    
    public double MobilityRate { get; set; }
    
    public string OutputFilePath { get; set; }
}