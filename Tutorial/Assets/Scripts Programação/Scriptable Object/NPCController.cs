using UnityEngine;

public class NPCController : MonoBehaviour
{
    
    public NPCDataSO npcData;
    
    private MeshRenderer meshRenderer;
   // private Material material;
    
   // [SerializeField] private Color meshColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        npcData.material = meshRenderer.material;
        npcData.material.color = npcData.meshColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
