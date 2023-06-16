using System;
using System.Collections.Generic;
using Project;
using Project.UI;
using UnityEngine;
using Zenject;

public class InGamePauseHendler : MonoBehaviour
{
    public static string TankControllerKey = "TankControllerKey";
    
    [Inject]
    private UISystem _uiSystem;

    [Inject]
    private LevelData _levelData;

    private Dictionary<string, object> _data;

    private void Start()
    {
        _data = new Dictionary<string, object>()
        {
            {TankControllerKey, _levelData.TankController}
        };
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _uiSystem.ShowWindow<PausePopup>(_data);
            _levelData.TankController.CameraController.ToggleActive(false);
        }
    }
}