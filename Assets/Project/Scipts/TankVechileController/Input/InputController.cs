using UnityEngine;

public abstract class InputController : MonoBehaviour
{
    [field: SerializeField]
    public float DriveInput
    {
        get;
        protected set;
    }

    [field: SerializeField]
    public float TurnInput
    {
        get;
        protected set;
    }
    
    [field: SerializeField]
    public bool BrakeInput
    {
        get;
        protected set;
    }

    public bool HasInput
    {
        get;
        protected set;
    }

    public abstract void GetInput();

    public void Free()
    {
        DriveInput = 0;
        TurnInput = 0;
        BrakeInput = true;
    }
}