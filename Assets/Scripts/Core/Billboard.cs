using UnityEngine;

public class BillboardToCamera : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main == null) return;

        // Make the sprite face the camera
        transform.forward = Camera.main.transform.forward;
    }
}
