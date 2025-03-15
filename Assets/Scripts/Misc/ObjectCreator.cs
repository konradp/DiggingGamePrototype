#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform createdObjsParent;
    [SerializeField] private int amount;
    [SerializeField] private Vector2 randomSizeRange;
    
    private BoxCollider boxCollider;

    public void SpawnObjects()
    {
        boxCollider = GetComponent<BoxCollider>();

        for (int i = 0; i < amount; i++)
        {
            var randomPosition = GetRandomPositionInsideBox();
            var go = Instantiate(prefab, randomPosition, Quaternion.identity);
            go.transform.rotation = Random.rotation;
            go.transform.localScale = GetRandomSize();
            go.transform.SetParent(createdObjsParent,true);
        }
    }
    
    private Vector3 GetRandomPositionInsideBox()
    {
        Vector3 boxSize = boxCollider.size;
        Vector3 boxCenter = boxCollider.transform.position + boxCollider.center;

        float x = Random.Range(boxCenter.x - boxSize.x / 2, boxCenter.x + boxSize.x / 2);
        float y = Random.Range(boxCenter.y - boxSize.y / 2, boxCenter.y + boxSize.y / 2);
        float z = Random.Range(boxCenter.z - boxSize.z / 2, boxCenter.z + boxSize.z / 2);

        return new Vector3(x, y, z);
    }

    private Vector3 GetRandomSize()
    {
        var ret = Vector3.zero;
        
        ret.x = Random.Range(randomSizeRange.x, randomSizeRange.y);
        ret.y = Random.Range(randomSizeRange.x, randomSizeRange.y);
        ret.z = Random.Range(randomSizeRange.x, randomSizeRange.y);

        return ret;
    }
}

[CustomEditor(typeof(ObjectCreator))]
public class ObjectCreatorEditor : Editor
{
    private ObjectCreator objectCreator;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        objectCreator = (ObjectCreator)target;

        if (GUILayout.Button("Spawn Objects"))
        {
            objectCreator.SpawnObjects();
        }
    }
}
#endif
