using UnityEngine;

public class ResourceSourceController : AbstractConveyorController
{
    public GameObject ResourcePrefab;

    private float lastResourceInstantiateTime = 0;

    public override void PrepareAction(float simulationTimeSeconds)
    {
        base.PrepareAction(simulationTimeSeconds);

        if (resourceInConveyorSet.Count > 0)
            lastResourceInstantiateTime = Time.time;

        if (resourceInConveyorSet.Count == 0 && simulationTimeSeconds - lastResourceInstantiateTime + Consts.FIXED_TIME_DELTA >= Consts.CREATE_RESOURCE_INTERVAL_SECONDS) {
            lastResourceInstantiateTime += Consts.CREATE_RESOURCE_INTERVAL_SECONDS;
            var resource = Instantiate(ResourcePrefab, positionObject.transform);
            resource.transform.localPosition = Vector3.zero;
            AddResource(resource, true);
        }

        SendResources(false);
    }
}
