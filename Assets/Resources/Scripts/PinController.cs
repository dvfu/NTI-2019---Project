using UnityEngine;

public enum PinType
{
    Neutral,
    Input,
    Output,
}

public class PinController : MonoBehaviour
{
    private bool inPosition;
    private GameObject receiver;
    private GameObject otherPin;

    public PinType type;

    public bool GetInPosition()
    {
        return inPosition;
    }

    public void SetInPosition(bool value)
    {
        inPosition = value;
    }

    public GameObject GetReceiverConveyor()
    {
        return receiver;
    }

    protected bool CanReceiveResource()
    {
        return true;
    }

    public void HandleOtherPin(GameObject otherPin)
    {
        var otherPinController = otherPin.GetComponent<PinController>();
        if (type == PinType.Output && otherPinController.type == PinType.Input)
            receiver = otherPin.GetComponentInParent<AbstractConveyorController>().gameObject;

        if (!GetInPosition()) {
            // какая-то мутная корректировка позиции за 5 минут.
            var parentConveyorScript = gameObject.GetComponentInParent<AbstractConveyorController>();
            parentConveyorScript.SetInPosition(true);
            var parentConveyor = parentConveyorScript.gameObject;
            parentConveyor.transform.position = parentConveyor.transform.position - (transform.position - otherPin.transform.position);
            parentConveyor.transform.rotation = otherPin.transform.rotation;
            parentConveyor.transform.Rotate(new Vector3(0, 180, 0));
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        var otherPinController = other.gameObject.GetComponent<PinController>();
        if (otherPinController != null) {
            otherPin = other.gameObject;
            HandleOtherPin(other.gameObject);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        var otherPinController = other.gameObject.GetComponent<PinController>();
        if (otherPinController != null) {
            receiver = null;
            otherPin = null;
        }
    }
}
