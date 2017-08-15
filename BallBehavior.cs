using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallBehavior : MonoBehaviour {

	// ball translation time
    public float time = 0.5f;

	// ball speed
	private float speed;

	// the cube that the ball follows
	public GameObject cubeAttachment;

	// geometries
    public List<GameObject> geos;

	// the cube at the start point
	public GameObject initialCube;

	// geometry mesh
	private Mesh mesh;

	// use this for initialization
	void start(){
		
	}

	// update is called once per frame
    void Update()
    {
		// if clicked, check clicked object and move accordingly
		if (Input.GetMouseButtonDown(0))
		{
			Clicked();
		}

		// translate ball and update geo
		if (cubeAttachment != null) {
			TranslateBall (); 
			UpdateGeo ();
		} 
    }

	// return true if any geometry is rotating
	bool IsRotating(){
		foreach (GameObject go in geos) {
			GeoBehavior geo = go.GetComponent<GeoBehavior> ();
			if (geo.rotating) {
				return true;
			}
		}
		return false;
	}
		
	public void TranslateBall(){
		// if geo rotating, make the ball follow the cubeAttachment
		if (IsRotating()) {
			transform.position = cubeAttachment.transform.position;

		// else translate ball to cubeAttachment position
		} else {
			Vector3 dest = Vector3.MoveTowards (transform.position, cubeAttachment.transform.position, speed);
			GetComponent<Rigidbody>().MovePosition(dest);
		}
		
	}
		
	public void UpdateGeo(){
		
		// when ball moves to the position of cubeAttachment, update geo
		if(transform.position == cubeAttachment.transform.position){
			foreach (GameObject go in geos) {
				var adj = go.GetComponent<GeoBehavior> ();
				adj.update = true;
			}
		}
	}


	public void Clicked(){
		
		RaycastHit hit;

		// if geo not rotating and a cube get clicked
		if (!IsRotating () && Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hit)) {

			// get cube
			GameObject clickedCube = hit.collider.gameObject;

			if (clickedCube != null) {
				
				// make sure the object get clicked is a cube
				var cp = clickedCube.transform.parent.gameObject.GetComponent<CubeProp> ();

				if (cp != null) {

					// make clicked cube the ball's new cubeAttachment 
					cubeAttachment = clickedCube.transform.parent.gameObject;

					// update geo canRotate 
					GeoBehavior ballGeo = cp.controller;
					ballGeo.canRotate = true;
					foreach (GameObject go in geos) {
						if (ballGeo.gameObject != go) {
							GeoBehavior otherGeo = go.GetComponent<GeoBehavior> ();
							otherGeo.canRotate = false;
							print (otherGeo+ "false");
						}
					}

					// make sure every translation takes the same time
					speed = Vector3.Distance (transform.position, cubeAttachment.transform.position) / time;
				}
			}
		}
	}

}