using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeoBehavior : MonoBehaviour {

	// the ball
	public GameObject player;

	// tag name of cubes
	public string tagName;

	// rotating smooth factor
	public float smoothFactor = 5;

	// initialize canRotate (differ in different geometries)
	public bool initialCanRotate = true;

	// update
	public bool update;

	// geometry rotating
	public bool rotating = false;

	// geometry can rotate
	public bool canRotate = true;


	// ball position
	private Vector3 playerPos;

	// geometry mesh
	private Mesh mesh;

	// cubes
	private GameObject[] cubes;

	// empty objects
	private GameObject[] empties;

	// geometry edges
	private List<Edge> edges = new List<Edge>();

	// all edges that are connected to player
	private bool[] connected;

	// cullDistance from cube to player
	private float cullDistance = 0.5f;

	// initialize update
	private bool initialUpdate = false;

	// Quaternion of start rotation
	private Quaternion startRotation;

	//Quaternion of target rotation
	private Quaternion targetRotation;

	// transition time
	private float transitionTime = 0.0f;

	// transition time per frame
	private float transitionT = 0.025f;

	// rotation count around X axis
	private int rotationCountx = 0;

	// rotation count around Y axis
	private int rotationCounty = 0;

	// cube scale factor
	private float scale = 1.5f;

	// max geometry edge length
	private float edgeLengthMax = 1.1f;


	// geometry edge
	struct Edge
	{
		public Edge(int ver0, int ver1)
		{
			v0 = ver0;
			v1 = ver1;
		}
		// start point index
		public int v0;
		// end point index
		public int v1;
	}

	// use this for initialization
	void Start()
	{
		// initial empty game objects and edges
		InitialEmpties ();
		InitialEdges ();

		// update empties' distances to ball and update cubes
		UpdateDistances();

		// initialize update and canRotate
		update = initialUpdate;
		canRotate = initialCanRotate;

	}

	// update is called once per frame
	void Update () {
		
		// rotate geometry
		Rotate();

		// update empties' distances to ball and update cubes
		if (update) {
			UpdateDistances ();
		}
	}
		
	public void UpdateDistances()
    {
        // player position
        playerPos = player.transform.position;
		Vector3 playerpp = Camera.main.transform.worldToLocalMatrix.MultiplyPoint3x4(playerPos);
        Vector2 player2d = new Vector2(playerpp.x, playerpp.y);

		//initialize connected
		connected = new bool[mesh.vertices.Length];

		// update distances to player, and update connected
		for (int i = 0; i < edges.Count; i++)
		{
			Vector3 startPt = transform.TransformPoint(mesh.vertices[edges[i].v0]);
			Vector3 spLocal = Camera.main.transform.worldToLocalMatrix.MultiplyPoint3x4(startPt);
			Vector2 sp2 = new Vector2(spLocal.x, spLocal.y);

			Vector3 endPt = transform.TransformPoint(mesh.vertices[edges[i].v1]);
			Vector3 epLocal = Camera.main.transform.worldToLocalMatrix.MultiplyPoint3x4(endPt);
			Vector2 ep2 = new Vector2(epLocal.x, epLocal.y);

			if (((player2d.x - sp2.x) * (player2d.x - sp2.x) + (player2d.y - sp2.y) * (player2d.y - sp2.y)) < (cullDistance * cullDistance))
			{
				connected[edges[i].v1] = true;
			}

			if (((player2d.x - ep2.x) * (player2d.x - ep2.x) + (player2d.y - ep2.y) * (player2d.y - ep2.y)) < (cullDistance * cullDistance))
			{
				connected[edges[i].v0] = true;
			}

		}

		// destroy previous cubes
		cubes = GameObject.FindGameObjectsWithTag (tagName);
		for (int i = 0; i < cubes.Length; i++) {
			Destroy (cubes [i]);
		}

		// create new cubes
		for (int i = 0; i < connected.Length; i++) {
			if (connected [i] == true) {
				GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
				cube.transform.localScale = new Vector3 (scale, scale, scale);
				Renderer rend = cube.GetComponent<Renderer>();
				rend.enabled = false;
				cube.tag = tagName;
				cube.transform.position = transform.TransformPoint (mesh.vertices[i]);
				GameObject parent = empties[i];
				cube.transform.parent = parent.transform;
			}
		}
		// update update
		update = false;
    }

	void InitialEmpties(){
		
		// get geo mesh
		mesh = GetComponent<MeshFilter> ().mesh;

		//get geo
		GeoBehavior geo = GetComponent<GeoBehavior> ();

		// initialize empties
		empties = new GameObject[mesh.vertices.Length];

		// create empty game objects to keep track of all vertices
		for (int i = 0; i < mesh.vertices.Length; i++) {
			Vector3 vertice = transform.TransformPoint (mesh.vertices [i]);
			GameObject emptyOb = new GameObject ();
			emptyOb.transform.position = vertice;
			emptyOb.transform.parent = transform;
			CubeProp cp = emptyOb.AddComponent<CubeProp> ();
			cp.controller = geo;
			empties [i] = emptyOb;
		}
	}

	void InitialEdges(){
		
		// iterate over every vertice and get all edges
		for (int i = -0; i < mesh.triangles.Length; i += 3)
		{
			for (int j = 0; j < 3; j++)
			{
				// get edge length 
				int startPtIndex = mesh.triangles[i + j];
				int endPtIndex = mesh.triangles[i + ((j + 1) % 3)];
				float edgeLength = Vector3.Distance(mesh.vertices[startPtIndex], mesh.vertices[endPtIndex]);

				// make sure no diagonals are added
				if (edgeLength < edgeLengthMax)
				{
					Edge newEdge = new Edge(startPtIndex, endPtIndex);
					edges.Add(newEdge);
				}
			}
		}
	}
	public void Rotate(){

		// if is rotating, time the rotation and end rotation after 90 degrees
		if (rotating)
		{
			transform.rotation = Quaternion.Slerp(startRotation, targetRotation, Mathf.Sin(transitionTime * Mathf.PI * 0.5f));
			transitionTime += transitionT;
			if (transitionTime >= 1.0f)
			{
				EndRotation();
			}
		}

		// if not rotating & can rotate, get rotation Quaternion and init rotation
		else if(canRotate)
		{
			bool rotate = false;
			if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				rotationCounty += 1;
				rotate = true;
			}
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				rotationCounty -= 1;
				rotate = true;
			}
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				rotationCountx += 1;
				rotate = true;
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				rotationCountx -= 1;
				rotate = true;
			}
			if (rotate)
			{
				Quaternion Rotation = Quaternion.Euler(90 * rotationCountx, 90 * rotationCounty, 0);
				InitRotation(Rotation);
			}
		}
	}

	public void InitRotation(Quaternion Rotation)
	{
		transitionTime = 0.0f;
		rotating = true;
		startRotation = transform.rotation;
		targetRotation = Rotation;
	}

	public void EndRotation()
	{
		transitionTime = 0.0f;
		rotating = false;
		transform.rotation = targetRotation;
		update = true;
	}

}
