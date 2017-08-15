using UnityEngine;
using System.Collections;

public class CameraLookAt : MonoBehaviour {


    // get origin point
    public Vector3 origin = new Vector3(0, 0, 0);


	// Use this for initialization
	void Start () {

        transform.rotation = Quaternion.LookRotation(origin - transform.position);

    }
	
	// Update is called once per frame
	void Update () {

	}
}
