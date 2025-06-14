using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class tempFixCull : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
		mesh.bounds = new Bounds(transform.position, Vector3.one*1000);
    }

    // Update is called once per frame
    void Update()
    {
        
    } 
}
