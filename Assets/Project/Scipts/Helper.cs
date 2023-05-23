using System;
using Project;
using UnityEngine;
using Zenject;

public class Helper : MonoBehaviour
{
    [Inject]
    private LevelFlowController _levelFlowController;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            _levelFlowController.LoadHub();
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            _levelFlowController.Load();
        }
    }
}
