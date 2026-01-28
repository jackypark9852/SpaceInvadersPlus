using UnityEngine;

public class MeshAnimation : MonoBehaviour
{
    public Mesh[] frames;
    public float frameRate = 1f;

    MeshFilter mf;
    int frameIndex;
    float t;

    void Awake()
    {
        mf = GetComponent<MeshFilter>();
        if (frames != null && frames.Length > 0)
            mf.mesh = frames[0];
    }

    void Update()
    {
        if (frames == null || frames.Length == 0) return;

        t += Time.deltaTime;
        if (t >= 1f / frameRate)
        {
            t -= 1f / frameRate;
            frameIndex = (frameIndex + 1) % frames.Length;
            mf.mesh = frames[frameIndex];
        }
    }
}
