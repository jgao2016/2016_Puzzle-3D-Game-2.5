using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class EndPointBehavior : MonoBehaviour {

	// the ball
    public GameObject player;
    
	// max distance from ball to goal point
	public float maxDist = 0.5f;

	// scene name
	public string sceneName;


	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {

		// if ball reach goal point, change level
		if (GetDistance() < maxDist)
		{
			StartCoroutine(ChangeLevel());
		}
	}

    IEnumerator ChangeLevel()
    {
		// get fade time
        float fadeTime = GameObject.Find("GUI manager").GetComponent<TransitionScreen>().BeginFade(1);

		// wait for fade time
        yield return new WaitForSeconds(fadeTime);

		// load new scene
		SceneManager.LoadScene(sceneName);

    }

	float GetDistance(){
		
		// get player position
		Vector3 playerv3 = Camera.main.transform.worldToLocalMatrix.MultiplyPoint3x4(player.transform.position);
		Vector2 player2d = new Vector2(playerv3.x, playerv3.y);

		// get goal position
		Vector3 endv3 = Camera.main.transform.worldToLocalMatrix.MultiplyPoint3x4(transform.position);
		Vector2 endPoint2d = new Vector2(endv3.x, endv3.y);

		// get distance
		float dist = Vector2.Distance(player2d, endPoint2d);
		return dist;
	}

}
