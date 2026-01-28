using UnityEngine;

public class TPSCamera : MonoBehaviour
{
    [Header("Over Shoulder")]
    public Transform target;
    public Vector3 shoulderOffset = new Vector3(1.5f, 1f, -2f);
    public Vector3 lookAtOffset = new Vector3(0f, 0.5f, 1.5f);  // Push forward for lead
    public float followSpeed = 10f;
    public float rotationSpeed = 8f;

    void LateUpdate()
    {
        if (target == null) return;

        // Position: shoulder offset from target
        Vector3 targetPos = target.position + target.rotation * shoulderOffset;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        // Look ahead: target forward + look offset
        Vector3 lookPoint = target.position + target.forward * 2f + target.rotation * lookAtOffset;
        Vector3 lookDir = lookPoint - transform.position;
        Quaternion targetRot = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}