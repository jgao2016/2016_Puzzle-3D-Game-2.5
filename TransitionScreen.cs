using UnityEngine;
using System.Collections;

public class TransitionScreen : MonoBehaviour {

	// fade out image
    public Texture2D fadeOut;

	// fade speed
    public float fadeSpeed = 0.5f;

	// draw depth
    private int drawDepth = -1000;

	// alpha in color 
    private float alpha = 1.0f;

	// fade direction
    private int fadeDir = -1;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	}

    void OnGUI()
    {
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOut);
    }
		
    public float BeginFade (int direction)
    {
        fadeDir = direction;
        float fadeTimer = 1 / fadeSpeed;
        return (fadeTimer);
    }

	// when level loaded
    void OnLevelWasLoaded()
    {
		BeginFade(-1);
    }
		

}
