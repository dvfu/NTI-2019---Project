using System.Collections.Generic;
using UnityEngine;

abstract public class AbstractConveyorController : MonoBehaviour
{
    public GameObject[] pins;
    public Dictionary<GameObject, bool> resourceInConveyorSet = new Dictionary<GameObject, bool>();

    protected GameObject positionObject;

    public void Start()
    {
        positionObject = GetComponentInChildren<ResourceSourcePositionController>().gameObject;
        Debug.Assert(positionObject != null);
    }

    public void SetInPosition(bool value)
    {
        foreach (var item in GetComponentsInChildren<PinController>()) {
            item.SetInPosition(value);
        }
    }

    public void AddResource(GameObject resource)
    {
        resource.transform.parent = positionObject.transform;
        resourceInConveyorSet[resource] = true;
    }

    protected void ExtractResource(GameObject resource)
    {
        resourceInConveyorSet.Remove(resource);
    }

    protected void DestroyResource(GameObject resource)
    {
        ExtractResource(resource);
        Destroy(resource);
    }

    protected virtual bool CanSendResource(GameObject resource)
    {
        return resource.transform.localPosition == Vector3.zero;
    }

    public void FixedUpdate()
    {
        foreach (var resource in resourceInConveyorSet.Keys) {
            if (resource.transform.localPosition != Vector3.zero) {
                var position = resource.transform.localPosition;
                resource.transform.localPosition = position * Mathf.Max(1 - Time.deltaTime * Consts.CONVEYOR_SPEED / position.magnitude, 0);
            }
        }

        GameObject receiver = null;
        foreach (var pin in pins) {
            var pinController = pin.GetComponent<PinController>();
            receiver = pinController.GetReceiverConveyor();
            if (receiver != null)
                break;
        }

        if (receiver != null) {
            var extractedResources = new List<GameObject>();
            foreach (var resource in resourceInConveyorSet.Keys) {
                if (CanSendResource(resource)) {
                    receiver.GetComponent<AbstractConveyorController>().AddResource(resource);
                    extractedResources.Add(resource);
                }
            }

            foreach (var resource in extractedResources)
                ExtractResource(resource);
        }
    }
}
