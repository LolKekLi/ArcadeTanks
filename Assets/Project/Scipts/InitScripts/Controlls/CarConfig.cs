using UnityEngine;

[CreateAssetMenu(fileName = "Car Config Settings", menuName = "ScriptableObjects/CarConfigSettings", order = 1)]
public class CarConfig : ScriptableObject
{
    [field: SerializeField]
    public float StartVelocity
    {
        get;
        private set;
    } = 0;

    [field: SerializeField]
    public float MaxVelocity
    {
        get;
        private set;
    } = 10;

    [field: SerializeField]
    public float BrakingToMaxVelocity
    {
        get;
        private set;
    } = 5;

    [field: SerializeField]
    public float AccelerationVelocity
    {
        get;
        private set;
    } = 20;

    [field: SerializeField]
    public float ChangeSpeed
    {
        get;
        private set;
    } = 10;

    [field: SerializeField]
    public float HorizontalSpeed
    {
        get;
        private set;
    } = 30;

    [field: SerializeField]
    public float MaxRotationAngle
    {
        get;
        private set;
    } = 30;

    [field: SerializeField]
    public float RotationSpeed
    {
        get;
        private set;
    } = 15;

    [field: SerializeField]
    public float MaxRotationAngleX
    {
        get;
        private set;
    } = 20;

    public float RotationSpeedX
    {
        get;
        private set;
    } = 5;

    public int RoadLayer
    {
        get;
        private set;
    } = 1 << 10;
}
