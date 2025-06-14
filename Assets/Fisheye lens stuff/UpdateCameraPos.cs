using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCameraPos : MonoBehaviour
{
	
	public int cam;
	//public Shader shader;
	//public Shader two;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalVector("_camPos" + cam.ToString(), new Vector3(transform.position.x, 0,transform.position.z));
    }
}
