using Project.UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private const float CAMERA_COLLISION_FORWARD_HIT_OFFSET = 1.0f;

    private TurretMovement turretMovementScript;

    private Transform tankTransform;

    public float[] zoomStepsNormal = new float[8] { 20, 17.4f, 14.8f, 12.2f, 9.6f, 7f, 4.4f, 1.8f };
    public float MinAngle = 15f;
    public float MaxAngle = 45f;
    public float RotSpeed = 1f;
    public float Height = 3f;

    public LayerMask ObstaclesLayer = default;
    public LayerMask StabilizerLayer = default;
    
    [HideInInspector]
    public Camera ourCamera;
    
    private float camRotY = 0f;
    private float camRotX = 0f;
    private float currentRotX;
    private float currentRotY;
    private float currentDistance;
    private float _mouseY;
    private float _mouseX;


    private Vector3 aimTarget;

    private Vector3 stabilizerPos;
    private Vector3 stabilizerPos2;
    private int MaxDistance = 5000;
    private int zoomPointer = 0;
    private int zoomPointerNormal = 0;

    [Inject]
    private UISystem _uiSystem = null;

    public void Setup(TurretMovement turretMovement, Transform transform1)
    {
        ourCamera = gameObject.GetComponent<Camera>();

        var cameraData = ourCamera.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Add(_uiSystem.Camera);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        turretMovementScript = turretMovement;
        tankTransform = transform1;


        transform.position = (tankTransform.position + -tankTransform.forward) + (Vector3.up * 2);
        transform.LookAt(tankTransform.position + (Vector3.up * Height));
        currentDistance = zoomStepsNormal[zoomPointerNormal];
    }

    private void Update()
    {
        // Refactor later to inputcontroller
        _mouseY = Input.GetAxis("Mouse Y");
        _mouseX = Input.GetAxis("Mouse X");
        
        ControlCamera();
        ControlTurret(aimTarget);

        // Prevent main camera from clipping
        Vector3 _cameraPos = (tankTransform.position - (transform.forward * currentDistance)) + (Vector3.up * Height);
        if (Physics.Linecast(tankTransform.position, _cameraPos, out RaycastHit hit, ObstaclesLayer))
        {
            _cameraPos = (hit.point + transform.forward * CAMERA_COLLISION_FORWARD_HIT_OFFSET);
        }

        transform.position = _cameraPos;
    }

    private void ControlCamera()
    {
        // Get and set out rotation
        currentRotX = _mouseX * RotSpeed;
        currentRotY = -(_mouseY * RotSpeed);
        transform.eulerAngles += new Vector3(currentRotY, currentRotX);
        // Clamp to angles, remember to actually make these be variable based
        Vector3 clampthis = transform.eulerAngles;
        if (clampthis.x > MaxAngle && clampthis.x < 300f)
        {
            clampthis.x = MaxAngle;
        }

        if (clampthis.x < (360f - MinAngle) && clampthis.x > 300f)
        {
            clampthis.x = (360f - MinAngle);
        }

        transform.eulerAngles = clampthis;
        // Set our snipercam rotation while we are not in it
        aimTarget = GetTargetPosition();
    }

    private void ControlTurret(Vector3 target)
    {
        turretMovementScript.TurretTargetPosition = target;
    }

    public Vector3 GetTargetPosition()
    {
        Vector3 _pos;

        _pos = transform.position + (transform.forward * 1000);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit3, MaxDistance,
                ObstaclesLayer))
        {
            _pos = hit3.point;
            Debug.DrawLine(transform.position, hit3.point, Color.blue);
        }

        return _pos;
    }

    public Vector3 GetStabilizerPosition()
    {
        Vector3 _pos = Vector3.zero;

        _pos = transform.position + (transform.forward * 1000);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit3, 100f, StabilizerLayer))
        {
            _pos = hit3.point;
        }

        return _pos;
    }
}