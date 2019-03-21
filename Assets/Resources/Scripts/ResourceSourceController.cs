using UnityEngine;

public class ResourceSourceController : AbstractConveyorController
{
    public GameObject ResourcePrefab;

    private float lastResourceInstantiateTime;

    public new void Start()
    {
        base.Start();
        lastResourceInstantiateTime = Time.timeSinceLevelLoad;
    }

    public new void FixedUpdate()
    {
        base.FixedUpdate();
        if (resourceInConveyorSet.Count > 0)
            lastResourceInstantiateTime = Time.time;

        if (resourceInConveyorSet.Count == 0 && Time.time - lastResourceInstantiateTime > Consts.CREATE_RESOURCE_INTERVAL_SECONDS) {
            lastResourceInstantiateTime += Consts.CREATE_RESOURCE_INTERVAL_SECONDS;
            var resource = Instantiate(ResourcePrefab, positionObject.transform);
            resource.transform.localPosition = Vector3.zero;
            resourceInConveyorSet[resource] = true;
        }
    }
}
