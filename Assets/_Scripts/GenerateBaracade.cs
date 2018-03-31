using System.Collections;
using UnityEngine;
using UnityEditor;

public class GenerateBaracade : MonoBehaviour
{

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

        // generate the baracades
        generateBaracadeOnAxis(0, 2, 1, floorSize, baracadeSize, baracadeParent);
        generateBaracadeOnAxis(2, 0, 1, floorSize, baracadeSize, baracadeParent);

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
            //Debug.Log(childBaracade.gameObject.name);
            GameObject.DestroyImmediate(childBaracade.gameObject);
        }
    }

    private void generateBaracadeOnAxis(int axis, int offAxis, int minorAxis, Vector3 floorSize, Vector3 baracadeSize, GameObject baracadeParent)
    {
        // Determine how many copies to put on each axis
        // Note we subtract the width of the baracade from the floor dimension after multiplying it by two
        // to accomodate for the two copies coming into the corner from the other axis
        int count = Mathf.FloorToInt(floorSize[axis] / (baracadeSize.x + baracadeSpacing));
        int spacers = count - 1;
        float baracadeLength = count * baracadeSize.x + spacers * baracadeSpacing;
        float sizeDiff = floorSize[axis] - baracadeLength;
        float startEndGap = sizeDiff / 2f;

        // dump our debug info
        string axisString = axis == 0 ? "x" : (axis == 1 ? "y" : (axis == 2 ? "z" : axis.ToString()));
        Debug.Log("Placing " + count + " copies on the " + axisString + " axis");
        Debug.Log("Baracade is " + baracadeLength + " on the " + axisString + " axis");
        Debug.Log("The " + axisString + " axis start and end gap distance is " + sizeDiff + " / 2 = " + startEndGap);

        // Loop over locations and instantiate
        GameObject newBaracade;
        for (int i = 0; i < count; ++i)
        {
            float iOffset = -floorSize[axis] / 2f + startEndGap + i * (baracadeSize.x + baracadeSpacing);
            for (int j = -1; j <= 1; j += 2)
            {
                newBaracade = PrefabUtility.InstantiatePrefab(baracadePrefab) as GameObject;
                // Set the parent for the new baracade to be this object
                newBaracade.transform.SetParent(baracadeParent.transform);
                Vector3 position = Vector3.zero;
                position[axis] = iOffset;
                position[offAxis] = (float)j * (floorSize[offAxis] / 2f - 1.5f * baracadeSize.z + (j == 1 ? baracadeSize.z: 0f));
                newBaracade.transform.localPosition = position;
                Debug.Log("Placed baracade " + (i + 1) + " at position " + newBaracade.transform.localPosition + "relative to " + floor);
                if (axis == 0)
                    continue;
                // The baracade prefab is built along the x axis, if we are doing any other axis we need to rotate the baracade
                // First, find the center of the object
                Vector3 center = newBaracade.GetComponent<MeshRenderer>().bounds.center;
                // rotate the object about an axis at that center of rotation
                Vector3 rotationAxis = Vector3.zero;
                rotationAxis[minorAxis] = 1f;
                Debug.Log("Rotating around minorAxis (" + minorAxis + ") 90 degrees");
                newBaracade.transform.RotateAround(center, rotationAxis, 90f);
                newBaracade.transform.localPosition -= new Vector3(baracadeSize.z * 2.5f, 0f, -startEndGap * 2.5f);
            }
        }
    }
}
