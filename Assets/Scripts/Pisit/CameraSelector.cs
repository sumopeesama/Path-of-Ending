using System;
using UnityEngine;

public class CameraSelector : MonoBehaviour
{
    // front 1 , back 2, top 3, god 4
    [SerializeField]
    private GameObject frontCameraGO;
    [SerializeField]
    private GameObject backCameraGO;
    [SerializeField]
    private GameObject topCameraGO;
    [SerializeField]
    private GameObject godCameraGO;

    [SerializeField]
    private KeyCode frontKey = KeyCode.Alpha1;
    [SerializeField]
    private KeyCode backKey = KeyCode.Alpha2;
    [SerializeField]
    private KeyCode topKey = KeyCode.Alpha3;
    [SerializeField]
    private KeyCode godKey = KeyCode.Alpha4;

    private void Update()
    {
        HandleCameraSelector();    
    }

    private void HandleCameraSelector()
    {
        // Front
        if( Input.GetKeyDown(frontKey) )
        {
            DisableAllCameras();
            frontCameraGO.SetActive(true);     
        }else if (Input.GetKeyDown(backKey))
        {
            DisableAllCameras();
            backCameraGO.SetActive(true);
        }else if (Input.GetKeyDown(topKey))
        {
            DisableAllCameras();
            topCameraGO.SetActive(true);
        }else if (Input.GetKeyDown(godKey))
        {
            DisableAllCameras();
            godCameraGO.SetActive(true);
        }
    }

    #region Helpers
    public void DisableAllCameras()
    {
        frontCameraGO.SetActive(false);
        backCameraGO.SetActive(false);
        topCameraGO.SetActive(false);
        godCameraGO.SetActive(false);
    }
    #endregion
}
