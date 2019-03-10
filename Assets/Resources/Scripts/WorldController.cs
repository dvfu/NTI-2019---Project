using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ResourceContainer
{
    private static ResourceContainer instance;

    public List<GameObject> resourceSources;
    public List<GameObject> resources;

    public static ResourceContainer GetInstance()
    {
        instance = instance ?? new ResourceContainer();
        return instance;
    }
}

public class InputObjects
{
    public List<ResourceSource> resourceSources;
    public List<AssemblyMachine> assemblyMachines;
    public List<Conveyor> conveyors;
    public List<Stock> stocks;
}

public class GeometryObject
{
    public float x;
    public float y;
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

public class WorldController : MonoBehaviour
{
    public GameObject[] ResourceSourcePrefabs;
    public GameObject[] ResourcePrefabs;
    public GameObject ConveyorPrefab;
    public GameObject StockPrefab;
    public GameObject AssemblyMachinePrefab;

    private float DirectionToAngle(float x, float y)
    {
        float rotation = 0;
        if (x == 1 && y == 0)
            rotation = 0;
        else if (x == 0 && y == 1)
            rotation = 90;
        else if (x == -1 && y == 0)
            rotation = 180;
        else if (x == 0 && y == -1)
            rotation = -90;
        else
            Debug.Assert(false);

        return rotation;
    }

    private InputObjects ReadInput()
    {
        TextReader reader = File.OpenText("input.txt");

        var resourceSources = new List<ResourceSource>();
        for (int i = 0, count = int.Parse(reader.ReadLine()); i < count; ++i) {
            var numbers = reader.ReadLine().Split(' ');
            resourceSources.Add(new ResourceSource{ x = float.Parse(numbers[0]), y = float.Parse(numbers[1]), type = int.Parse(numbers[2]) - 1 });
        }

        var assemblyMachines = new List<AssemblyMachine>();
        for (int i = 0, count = int.Parse(reader.ReadLine()); i < count; ++i) {
            var numbers = reader.ReadLine().Split(' ');
            assemblyMachines.Add(new AssemblyMachine{
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

        return new InputObjects{
            resourceSources = resourceSources,
            assemblyMachines = assemblyMachines,
            conveyors = conveyors,
            stocks = stocks,
        };
    }

    private void CreateSceneObjects(InputObjects inputObjects)
    {
        foreach (var inputObj in inputObjects.resourceSources) {
            var obj = Instantiate(ResourceSourcePrefabs[inputObj.type], gameObject.transform);
            obj.transform.position = new Vector3(inputObj.x, 0.5f, inputObj.y);
            obj.GetComponent<AbstractConveyorController>().SetInPosition(true);
        }

        foreach (var inputObj in inputObjects.assemblyMachines) {
            var obj = Instantiate(AssemblyMachinePrefab, gameObject.transform);
            obj.transform.position = new Vector3(inputObj.x, 0.5f, inputObj.y);
            obj.transform.rotation = Quaternion.identity;
            obj.transform.Rotate(new Vector3(0, inputObj.rotation, 0));

            var controller = obj.GetComponent<AssemblyMachineController>();
            controller.SetInPosition(true);
            controller.inputFirstResourceType = inputObj.resourceTypeInputFirst;
            controller.inputSecondResourceType = inputObj.resourceTypeInputSecond;
            controller.outputResourceType = inputObj.resourceTypeOutput;
        }

        foreach (var inputObj in inputObjects.conveyors) {
            var obj = Instantiate(ConveyorPrefab, gameObject.transform);
            obj.transform.position = new Vector3(inputObj.x, 0.5f, inputObj.y);
            obj.transform.rotation = Quaternion.identity;
            obj.transform.Rotate(new Vector3(0, inputObj.rotation, 0));
            obj.GetComponent<AbstractConveyorController>().SetInPosition(true);
        }

        foreach (var inputObj in inputObjects.stocks) {
            var obj = Instantiate(StockPrefab, gameObject.transform);
            obj.transform.position = new Vector3(inputObj.x, 0.5f, inputObj.y);
            obj.transform.rotation = Quaternion.identity;
            obj.GetComponent<AbstractConveyorController>().SetInPosition(true);
        }
    }

    public void Awake()
    {
        CreateSceneObjects(ReadInput());

        var container = ResourceContainer.GetInstance();
        container.resourceSources = new List<GameObject>(ResourceSourcePrefabs);
        container.resources = new List<GameObject>(ResourcePrefabs);
    }
}
