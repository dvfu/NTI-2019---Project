using System.Collections.Generic;
using UnityEngine;

abstract public class AbstractConveyorController : MonoBehaviour
{
    public GameObject[] pins;
    public Dictionary<GameObject, bool> resourceInConveyorSet = new Dictionary<GameObject, bool>();
    public List<GameObject> bufferedResources = new List<GameObject>();

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
        var controller = resource.GetComponent<ResourceController>();
        resource.transform.position = resource.transform.position.normalized * (resource.transform.position.magnitude - controller.Error);
        controller.Error = 0;
        bufferedResources.Add(resource);
        
    }

    public void FlushBuffer()
    {
        foreach (var resource in bufferedResources)
            resourceInConveyorSet[resource] = true;
        bufferedResources.Clear();
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

    public void HandleAction()
    {
        foreach (var resource in resourceInConveyorSet.Keys) {
            var resourceController = resource.GetComponent<ResourceController>();
            if (resource.transform.localPosition != Vector3.zero) {
                var position = resource.transform.localPosition;
                var k = 1.0f - Time.fixedDeltaTime * Consts.CONVEYOR_SPEED / position.magnitude;
                if (k < 0) {
                    resourceController.Error += position.magnitude * (-k);
                    k = 0;
                }
                resource.transform.localPosition = position * k;
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
