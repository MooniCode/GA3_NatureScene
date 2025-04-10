using UnityEngine;

public class GrassInteraction : MonoBehaviour
{
    [SerializeField] private Shader grassShader;
    [SerializeField] private Material grassMaterial;

    private int WindNoiseSpeed;

    private void Start()
    {
        WindNoiseSpeed = Shader.PropertyToID("_WindNoiseSpeed");
    }

    void Update()
    {
        grassMaterial.SetFloat(WindNoiseSpeed, 0.5f);
    }
}