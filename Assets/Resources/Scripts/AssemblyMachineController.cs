using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyMachineController : AbstractConveyorController
{
    public int inputFirstResourceType;
    public int inputSecondResourceType;
    public int outputResourceType;

    private GameObject outputResourcePrefab;

    private GameObject GetOutputResourcePrefab()
    {
        outputResourcePrefab = outputResourcePrefab ?? ResourceContainer.GetInstance().resources[outputResourceType];
        return outputResourcePrefab;
    }

    public override void PostAction(float simulationTimeSeconds)
    {
        base.PostAction(simulationTimeSeconds);

        GameObject firstResource = null, secondResource = null;
        foreach (var resource in resourceInConveyorSet.Keys) {
            var type = resource.GetComponent<ResourceController>().Type;
            if (firstResource == null && resource.transform.localPosition == Vector3.zero && type == inputFirstResourceType)
                firstResource = resource;
            else if (secondResource == null && resource.transform.localPosition == Vector3.zero && type == inputSecondResourceType)
                secondResource = resource;
        }

        if (firstResource != null && secondResource != null) {
            var product = Instantiate(GetOutputResourcePrefab(), positionObject.transform);
            product.transform.localPosition = Vector3.zero;

            DestroyResource(firstResource);
            DestroyResource(secondResource);
            AddResource(product, true);
        }

        SendResources(true);
    }

    protected override bool CanSendResource(GameObject resource)
    {
        return base.CanSendResource(resource) && resource.GetComponent<ResourceController>().Type == outputResourceType;
    }
}
