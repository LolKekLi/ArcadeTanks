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

    public abstract void GetInput();
}