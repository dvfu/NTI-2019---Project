using System.Collections.Generic;
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

public class WorldController : MonoBehaviour
{
    public GameObject[] ResourceSourcePrefabs;
    public GameObject[] ResourcePrefabs;
    public GameObject ConveyorPrefab;
    public GameObject StockPrefab;
    public GameObject AssemblyMachinePrefab;

    public static int MoveCount = 0;

    private const int EXTRA_FRAME_LIMIT = 1;

    private int simulationSecondsLimit = -1;
    private float simulationTimeSeonds = 0;
    private bool isExtraFrames = false;
    private int extraFrameCount = 0;
    private AbstractConveyorController[] conveyorControllers;

    private void CreateSceneObjects(InputData inputObjects)
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
        if (GameState.readInputFileIfExists) {
            var inputData = FileHandler.ReadInputFile(GameState.inputFileName);
            simulationSecondsLimit = inputData.simulationSeconds;
            CreateSceneObjects(inputData);
        }

        var container = ResourceContainer.GetInstance();
        container.resourceSources = new List<GameObject>(ResourceSourcePrefabs);
        container.resources = new List<GameObject>(ResourcePrefabs);
    }

    public void FixedUpdate()
    {
        if (isExtraFrames)
            ++extraFrameCount;

        ++MoveCount;
        simulationTimeSeonds += Time.fixedDeltaTime;
        isExtraFrames = isExtraFrames || simulationSecondsLimit != -1 && simulationTimeSeonds + Consts.FIXED_TIME_DELTA >= simulationSecondsLimit;

        if (isExtraFrames && extraFrameCount >= EXTRA_FRAME_LIMIT)
        {
            Time.timeScale = 0.0f;

            var outputData = new OutputData { resources = new List<Resource>() };
            foreach (var resourceController in FindObjectsOfType<ResourceController>())
            {
                var position = resourceController.gameObject.transform.position;
                outputData.resources.Add(new Resource { x = position.x, y = position.z, type = resourceController.Type });
            }
            FileHandler.WriteOutputFile(outputData);
            Application.Quit();
            return;
        }

        if (conveyorControllers == null)
            conveyorControllers = GetComponentsInChildren<AbstractConveyorController>();

        foreach (var controller in conveyorControllers)
            controller.PrepareAction(simulationTimeSeonds);

        foreach (var controller in conveyorControllers)
            controller.HandleAction(simulationTimeSeonds);

        foreach (var controller in conveyorControllers)
            controller.PostAction(simulationTimeSeonds);
    }
}
