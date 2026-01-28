using UnityEngine;

public class Barrier : MonoBehaviour
{
    public Mesh[] meshes = new Mesh[3];
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    int hitCount;

    void Start()
    {
        if (meshFilter == null) meshFilter = GetComponent<MeshFilter>();
        if (meshRenderer == null) meshRenderer = GetComponent<MeshRenderer>();
        SetState(0);
    }

    public void Hit()
    {
        hitCount++;
        if (hitCount < 3)
        {
            SetState(hitCount);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void SetState(int state)
    {
        if (state < meshes.Length && meshes[state] != null)
        {
            meshFilter.mesh = meshes[state];
        }
    }
}