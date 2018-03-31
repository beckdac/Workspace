using System.Collections;
using UnityEngine;
using UnityEditor;

public class GenerateBaracade : MonoBehaviour {

    public GameObject floor;
    public GameObject baracadePrefab;
    public float baracadeSpacing = .5f;

    public void generateBaracade()
    {
        if (floor == null || baracadePrefab == null)
        {
            Debug.Log("floor and baracadePrefab objects have to be set");
            return;
        }
        Debug.Log("Generating baracade from " + baracadePrefab + " objects along the boundaries of " + floor);

        GameObject baracadeParent = this.gameObject;

        // Find the size of our terrain
        Terrain floorTerrain = floor.GetComponent<Terrain>();
        Vector3 floorSize = floorTerrain.terrainData.size;
        Debug.Log("The floor has dimensions: " + floorSize);
        // The size of the prefab object
        MeshRenderer baracadeMeshRenderer = baracadePrefab.GetComponent<MeshRenderer>();
        Vector3 baracadeSize = baracadeMeshRenderer.bounds.size;
        Debug.Log("The baracade has dimensions: " + baracadeSize);

        // Determine how many copies to put on each axis
        // Note we subtract the width of the baracade from the floor dimension after multiplying it by two
        // to accomodate for the two copies coming into the corner from the other axis
        int xCount = Mathf.FloorToInt((floorSize.x - baracadeSize.z * 2f) / (baracadeSize.x + baracadeSpacing));
        int zCount = Mathf.FloorToInt((floorSize.z - baracadeSize.z * 2f) / (baracadeSize.x + baracadeSpacing));
        Debug.Log("Placing " + xCount.ToString() + " copies on the x axis and " + zCount.ToString() + " copies on the z axis");

        // Loop over locations and instantiate
        GameObject newBaracade;
        for (int i = 0; i < xCount; ++i)
        {
            for (int j = -1; j <= -1; j+=2)
            {
                newBaracade = PrefabUtility.InstantiatePrefab(baracadePrefab) as GameObject;
                // Set the parent for the new baracade to be this object
                newBaracade.transform.SetParent(baracadeParent.transform);
                newBaracade.transform.localPosition = new Vector3(floorSize.x / 2f - baracadeSize.z + baracadeSize.x / 2f + 
                    (float)i * baracadeSize.x, 0f - floorSize.y / 2f, (float)j * floorSize.z / 2f + j * baracadeSize.z + baracadeSpacing / 2f);
                Debug.Log("Placing baracade " + (i + 1) + " at position " + newBaracade.transform.localPosition + "relative to " + floor);
            }
        }
        for (int i = 0; i < zCount; ++i)
        {
            // Set the parent for the new baracade to be this object
         //   newBaracade.transform.SetParent(baracadeParent);
        }
    }

    public void resetBaracade()
    {
        Debug.Log("Removing baracades");
        GameObject baracadeParent = this.gameObject;
        // iterate over the children of my object that are physical objects and remove them
        foreach (Transform childBaracade in GetComponentsInChildren<Transform>())
        {
            if (childBaracade.gameObject == this.gameObject)
                continue;
            Debug.Log(childBaracade.gameObject.name);
            GameObject.DestroyImmediate(childBaracade.gameObject);
        }
    }
}
