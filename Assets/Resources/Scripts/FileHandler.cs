using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InputData
{
    public List<ResourceSource> resourceSources;
    public List<AssemblyMachine> assemblyMachines;
    public List<Conveyor> conveyors;
    public List<Stock> stocks;
    public int simulationSeconds = -1;
}

public class OutputData
{
    public List<Resource> resources;
}

public class GeometryObject
{
    public float x;
    public float y;
}

public class Resource : GeometryObject
{
    public int type;
}

public class ResourceSource : GeometryObject
{
    public int type;
}

public class AssemblyMachine : GeometryObject
{
    public int resourceTypeInputFirst;
    public int resourceTypeInputSecond;
    public int resourceTypeOutput;
    public float rotation;
}

public class Stock : GeometryObject
{
}

public class Conveyor : GeometryObject
{
    public float rotation;
}

public class FileHandler
{
    private static float DirectionToAngle(float x, float y)
    {
        float rotation = 0;
        if (x == 1 && y == 0)
            rotation = 0;
        else if (x == 0 && y == 1)
            rotation = -90;
        else if (x == -1 && y == 0)
            rotation = 180;
        else if (x == 0 && y == -1)
            rotation = 90;
        else
            Debug.Assert(false);

        return rotation;
    }

    public static InputData ReadInputFile(string filename)
    {
        TextReader reader = File.OpenText(filename);

        var resourceSources = new List<ResourceSource>();
        for (int i = 0, count = int.Parse(reader.ReadLine()); i < count; ++i) {
            var numbers = reader.ReadLine().Split(' ');
            resourceSources.Add(new ResourceSource { x = float.Parse(numbers[0]), y = float.Parse(numbers[1]), type = int.Parse(numbers[2]) });
        }

        var assemblyMachines = new List<AssemblyMachine>();
        for (int i = 0, count = int.Parse(reader.ReadLine()); i < count; ++i) {
            var numbers = reader.ReadLine().Split(' ');
            assemblyMachines.Add(new AssemblyMachine {
                x = float.Parse(numbers[0]),
                y = float.Parse(numbers[1]),
                resourceTypeInputFirst = int.Parse(numbers[2]),
                resourceTypeInputSecond = int.Parse(numbers[3]),
                resourceTypeOutput = int.Parse(numbers[4]),
                rotation = DirectionToAngle(float.Parse(numbers[5]), float.Parse(numbers[6])),
            });
        }

        var conveyors = new List<Conveyor>();
        for (int i = 0, count = int.Parse(reader.ReadLine()); i < count; ++i) {
            var numbers = reader.ReadLine().Split(' ');

            conveyors.Add(new Conveyor{
                x = float.Parse(numbers[0]),
                y = float.Parse(numbers[1]),
                rotation = DirectionToAngle(float.Parse(numbers[2]), float.Parse(numbers[3])),
            });
        }

        var stocks = new List<Stock>();
        for (int i = 0, count = int.Parse(reader.ReadLine()); i < count; ++i) {
            var numbers = reader.ReadLine().Split(' ');

            stocks.Add(new Stock{
                x = float.Parse(numbers[0]),
                y = float.Parse(numbers[1]),
            });
        }

        var simulationSeconds = int.Parse(reader.ReadLine());
        Debug.Assert(simulationSeconds > 0 || simulationSeconds == -1);

        return new InputData {
            resourceSources = resourceSources,
            assemblyMachines = assemblyMachines,
            conveyors = conveyors,
            stocks = stocks,
            simulationSeconds = simulationSeconds,
        };
    }

    public static void WriteOutputFile(OutputData data)
    {
        var writer = File.CreateText(GameState.outputFileName);

        writer.WriteLine(data.resources.Count.ToString());
        foreach (var resource in data.resources)
            writer.WriteLine(string.Format("{0} {1} {2}", resource.x, resource.y, resource.type));

        writer.Close();
    }
}
