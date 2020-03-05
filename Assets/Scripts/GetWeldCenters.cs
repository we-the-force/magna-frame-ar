using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetWeldCenters : MonoBehaviour
{
    public List<Vector3> WeldPositionCollection;
    public Transform WeldsTransform;


    private void Start()
    {
        WeldPositionCollection = new List<Vector3>();
        for (int i = 0; i < WeldsTransform.childCount; i++)
        {
            Vector3 _center = new Vector3(0, 0, 0);

            Mesh weldMesh = WeldsTransform.GetChild(i).GetComponent<MeshFilter>().mesh;
            for (int j = 0; j < weldMesh.vertices.Length; j++)
            {
                _center += weldMesh.vertices[j];
            }

            _center = _center / weldMesh.vertices.Length;
            WeldPositionCollection.Add(_center);
        }
    }


}
