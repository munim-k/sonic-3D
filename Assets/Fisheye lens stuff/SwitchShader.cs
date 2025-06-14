using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class SwitchShader : MonoBehaviour
{

	public Shader shader; 
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Camera>().SetReplacementShader (shader, "Camera");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
