// See https://aka.ms/new-console-template for more information

using DiseaseSimulation.CLI;
using DiseaseSimulation.Core;
using DiseaseSimulation.Core.Models;

class Program
{
    public static void Main(string[] args)
    {
        if (args.Contains("--help"))
        {
            Parser.PrintHelp();
            return;
        }
        
        SimulationConfig config = Parser.ParseArgs(args);
        SimulationEngine simulationEngine = new SimulationEngine(config);
        
        simulationEngine.PrintStats();

        RunSimulation(simulationEngine);
    }

    private static void RunSimulation(SimulationEngine engine)
    {
        for (int i = 0; i < engine.Config.Steps - 1; i++)
        {
            engine.NextStep();
            engine.PrintStats();
        }
        
        TxtManager txtManager = new TxtManager();
        txtManager.WriteToTxtFile(engine.Config.OutputFilePath, engine.Stats);
    }
}