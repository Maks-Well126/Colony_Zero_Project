using UnityEngine;

[RequireComponent(typeof(Terrain))]
public class ExtractTreeCollidersFromTerrain : MonoBehaviour
{
    [Header("Capsule Settings")]
    [SerializeField] private float radius = 0.6f;
    [SerializeField] private float height = 6f;

    [ContextMenu("Extract Tree Capsules")]
    public void Extract()
    {
        Terrain terrain = GetComponent<Terrain>();
        TerrainData data = terrain.terrainData;

        TreeInstance[] instances = data.treeInstances;

        Vector3 terrainSize = data.size;
        Vector3 terrainPos = terrain.GetPosition();

        Debug.Log("Creating tree capsules...");

        for (int i = 0; i < instances.Length; i++)
        {
            TreeInstance tree = instances[i];

            Vector3 worldPos = Vector3.Scale(tree.position, terrainSize) + terrainPos;

            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            obj.name = "TreeCapsule_" + i;

            obj.transform.parent = terrain.transform;
            obj.transform.position = worldPos;

            float scale = tree.widthScale;

            obj.transform.localScale = new Vector3(
                radius * scale,
                height * tree.heightScale * 0.5f,
                radius * scale
            );

            obj.isStatic = true;
        }

        Debug.Log("Created capsules: " + instances.Length);
    }
}