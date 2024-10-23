using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Waves : MonoBehaviour
{
    private Material material;

    private MeshRenderer meshRenderer;

    private MaterialPropertyBlock materialPropertyBlock;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        material = meshRenderer.material;
        
        materialPropertyBlock = new MaterialPropertyBlock();
    }

    private void Update()
    {
        materialPropertyBlock.SetVector("_ObjectPosition", transform.position);
        material.SetFloat("_TimeSinceLevelLoad", Time.timeSinceLevelLoad);

        meshRenderer.SetPropertyBlock(materialPropertyBlock);
    }
}