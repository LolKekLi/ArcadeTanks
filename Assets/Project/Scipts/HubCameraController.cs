using Project.UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Zenject;

public class HubCameraController : MonoBehaviour
{
    [Inject]
    private UISystem _uiSystem;
    
    private void Awake()
    {
        var ourCamera = gameObject.GetComponent<Camera>();
        var cameraData = ourCamera.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Add(_uiSystem.Camera);
    }
}
