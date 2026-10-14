using System.Runtime.CompilerServices;

namespace DiseaseSimulation.CLI;

using DiseaseSimulation.Core.Models;

public class Parser
{
    public static SimulationConfig ParseArgs(string[] args)
    {
        var config = new SimulationConfig();

        bool hasMap = false;
        bool hasSteps = false;
        bool hasInfProb = false;
        bool hasInfDur = false;
        bool hasIncDur = false;
        bool hasMobility = false;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--map":
                    config.MapFilepath = args[++i];
                    hasMap = true;
                    break;

                case "--steps":
                    config.Steps = int.Parse(args[++i]);
                    hasSteps = true;
                    break;

                case "--infProb":
                    config.InfectionProbability = double.Parse(args[++i]);
                    hasInfProb = true;
                    break;

                case "--infDur":
                    config.InfectionDuration = int.Parse(args[++i]);
                    hasInfDur = true;
                    break;

                case "--incDur":
                    config.IncubationDuration = int.Parse(args[++i]);
                    hasIncDur = true;
                    break;
                
                case "--mobility":
                    config.MobilityRate = double.Parse(args[++i]);
                    hasMobility = true;
                    break;
                
                case "--output":
                    config.OutputFilePath = args[++i];
                    break;

                case "--help":
                    PrintHelp();
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine($"Unknown argument: {args[i]}");
                    Console.WriteLine("Use --help for usage.");
                    Environment.Exit(1);
                    break;
            }
        }
        
        if (!hasMap || !hasSteps || !hasInfProb || !hasInfDur || !hasIncDur || !hasMobility)
        {
            Console.WriteLine("Missing required arguments!");
            Console.WriteLine("Use --help for usage.");
            Environment.Exit(1);
        }

        return config;
    }


    public static void PrintHelp()
    {
        Console.WriteLine("=== Disease Simulation CLI Help ===");
        Console.WriteLine();
        Console.WriteLine("Use:");
        Console.WriteLine("  dotnet run -- [arguments]");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("  --map <filepath>        path to map file");
        Console.WriteLine("  --steps <n>             number of simulation steps");
        Console.WriteLine("  --infProb <0-1>         probability of infection");
        Console.WriteLine("  --infDur <days>         lenght of infection");
        Console.WriteLine("  --incDur <days>         lenght of incubation (only SEIR, if another tap 0)");
        Console.WriteLine("  --mobility <0-1>        mobility rate");
        Console.WriteLine("  --output <filepath>     path to output file");
        Console.WriteLine("  --help                  show this help");
        Console.WriteLine();
        Console.WriteLine("Example:");
        Console.WriteLine("     dotnet run -- --steps 50 --infProb 0.3 --infDur 5 --incDur 10 --map SEIR_map.txt --output results.txt");
        Console.WriteLine();
    }

}