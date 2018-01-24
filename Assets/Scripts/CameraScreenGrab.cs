using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class CameraScreenGrab : MonoBehaviour {
	
	//how chunky to make the screen
	public int pixelSize = 4;
	public FilterMode filterMode = FilterMode.Point;
    Camera camera;
	public Camera[] otherCameras;
	public Material mat;
	Texture2D tex;
	public Material screenMat;
	public Material badGlare;
	public bool baseMat = true;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }
    void Start () {
        //bloomMat = new Material(bloomShader);
        //camera.pixelRect = new Rect(0, 0, Screen.width / pixelSize, Screen.height / pixelSize);
        for (int i = 0; i < otherCameras.Length; i++)
        {
			otherCameras[i].pixelRect = new Rect(0,0,Screen.width/pixelSize,Screen.height/pixelSize);
        }
	}

	public void Init()
	{
        camera.pixelRect = new Rect(0, 0, Screen.width / pixelSize, Screen.height / pixelSize);
    }
	
	void OnGUI()
	{
		if (Event.current.type == EventType.Repaint) {
            Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex, screenMat);
            if (baseMat)
            {
                Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), tex, badGlare);
            }
        }
	}
	

	void OnPostRender()
	{
		// Draw a quad over the whole screen with the above shader
		GL.PushMatrix ();
		GL.LoadOrtho ();
		for (var i = 0; i < mat.passCount; ++i) {
			mat.SetPass (i);
			GL.Begin( GL.QUADS );
			GL.Vertex3( 0, 0, 0.1f );
			GL.Vertex3( 1, 0, 0.1f );
			GL.Vertex3( 1, 1, 0.1f );
			GL.Vertex3( 0, 1, 0.1f );
			GL.End();
		}
		GL.PopMatrix ();	

		DestroyImmediate(tex);
		
		tex = new Texture2D(Mathf.FloorToInt(camera.pixelWidth), Mathf.FloorToInt(camera.pixelHeight));
		tex.filterMode = filterMode;
		tex.ReadPixels(new Rect(0, 0, camera.pixelWidth, camera.pixelHeight), 0, 0);
        tex.Apply();

    }
}