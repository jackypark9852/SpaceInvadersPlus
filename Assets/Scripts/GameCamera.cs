using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [Header("Cameras")]
    public Camera topDownCamera;
    public Camera thirdPersonCamera;
    public void SetThirdPersonMode(bool active)
    {
        topDownCamera.gameObject.SetActive(!active);
        if (thirdPersonCamera != null)
            thirdPersonCamera.gameObject.SetActive(active);
    }

    public bool IsFirstPersonMode()
    {
        return thirdPersonCamera != null && thirdPersonCamera.gameObject.activeInHierarchy;
    }
}