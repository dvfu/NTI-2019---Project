﻿using System.Collections.Generic;
using UnityEngine;

abstract public class AbstractConveyorController : MonoBehaviour
{
    public GameObject[] pins;

    protected Dictionary<GameObject, bool> resourceInConveyorSet = new Dictionary<GameObject, bool>();
    protected List<GameObject> bufferedResources = new List<GameObject>();

    protected GameObject positionObject;

    public void Start()
    {
        positionObject = GetComponentInChildren<ResourceSourcePositionController>().gameObject;
        Debug.Assert(positionObject != null);
    }

    public void SetInPosition(bool value)
    {
        foreach (var item in GetComponentsInChildren<PinController>())
            item.SetInPosition(value);
    }

    public void AddResource(GameObject resource, bool readyToMove)
    {
        resource.transform.parent = positionObject.transform;
        var controller = resource.GetComponent<ResourceController>();
        controller.Error = 0;
        resource.transform.position = resource.transform.position.normalized * (resource.transform.position.magnitude - controller.Error);
        // грязные хаки...
        if (readyToMove)
            resourceInConveyorSet[resource] = true;
        else
            bufferedResources.Add(resource);
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

    protected void SendResources(bool extractedResourcesReadyToMove)
    {
        GameObject receiver = null;
        GameObject outputPin = null;
        foreach (var pin in pins) {
            var pinController = pin.GetComponent<PinController>();
            if (pinController.type == PinType.Output)
                outputPin = pinController.gameObject;

            receiver = pinController.GetReceiverConveyor();
            if (receiver != null)
                break;
        }

        var extractedResources = new List<GameObject>();
        foreach (var resource in resourceInConveyorSet.Keys) {
            if (CanSendResource(resource)) {
                extractedResources.Add(resource);
                if (receiver != null)
                    receiver.GetComponent<AbstractConveyorController>().AddResource(resource, extractedResourcesReadyToMove);
                else if (outputPin != null) {
                    resource.transform.parent = null;
                    var velocity = outputPin.gameObject.transform.position - gameObject.transform.position;
                    velocity.y = 0;
                    velocity.Normalize();

                    var rigidBody = resource.gameObject.AddComponent<Rigidbody>();
                    rigidBody.useGravity = false;
                    rigidBody.velocity = velocity;
                    Destroy(resource, 1f / Consts.CONVEYOR_SPEED);
                }
            }
        }

        foreach (var resource in extractedResources)
            ExtractResource(resource);
    }

    public virtual void PrepareAction(float simulationTimeSeconds)
    {

    }

    public virtual void HandleAction(float simulationTimeSeconds)
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

        SendResources(false);
    }

    public virtual void PostAction(float simulationTimeSeconds)
    {
        foreach (var resource in bufferedResources)
            resourceInConveyorSet[resource] = true;
        bufferedResources.Clear();
    }
}
