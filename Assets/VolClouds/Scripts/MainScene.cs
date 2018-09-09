using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Parameter
{
    public string Name;
    public string ShaderParam;
    public float value;
    public float min;
    public float max;

    public Parameter(string _Name, string _ShaderParam, float _value, float _min, float _max)
    {
        Name = _Name;
        ShaderParam = _ShaderParam;
        value = _value;
        min = _min;
        max = _max;
    }

    public void SetValue(Material m)
    {
        m.SetFloat(ShaderParam, value);
    }
    public void Render(int y, int w, int tw, int margin)
    {
        GUI.Label(new Rect(margin, y, tw, 30), Name + ":");
        value = GUI.HorizontalSlider(new Rect(margin + tw + margin, y, w, 30), value, min,max);
    }

}

public class MainScene : MonoBehaviour {

	// Use this for initialization

    private Vector3 cameraAcc = Vector3.zero;
    private Vector3 viewAcc = Vector3.zero;
    private Vector3 view = Vector3.zero;
    private Vector3 sunRotation = new Vector3(25, 300, 0);
    private float moveScale = 2.5f;
    private List<Parameter> parameters = new List<Parameter>();
    private int timeToAutoDetail=100;
    private bool autoDetail = false;
    private bool displayMenu = true;

    public Parameter getParameter(string Name) {
        foreach (Parameter p in parameters) {
            if (p.Name == Name)
                return p;
        }
        return null;
    }

    private static string infoText =
        "LemonSpawn VolClouds 1.0 example scene.\n" +
        "Left mouse button to look, WSAD to move.\n" +
        "Currently not renderable from inside the clouds, so height is constrained to stay below the cloud height.\n" +
        "GPU-heavy but sexy - change the \"Detail\" parameter to tune performance."; 

                                        


	void Start () {
        parameters.Add(new Parameter("Scale", "_CloudScale", 0.3f, 0, 1));
        parameters.Add(new Parameter("Distance", "_CloudDistance", 0.03f, 0, 0.25f));
        parameters.Add(new Parameter("Detail", "_MaxDetail", 0.33f, 0, 1f));
        parameters.Add(new Parameter("Subtract", "_CloudSubtract", 0.6f, 0, 1f));
        parameters.Add(new Parameter("Scattering", "_CloudScattering", 1.5f, 1f, 3f));
        parameters.Add(new Parameter("Y Spread", "_CloudHeightScatter", 1.75f, 0.5f, 6f));
        parameters.Add(new Parameter("Density", "_CloudAlpha", 0.6f, 0f, 1f));
        parameters.Add(new Parameter("Hardness", "_CloudHardness", 0.9f, 0f, 1f));
        parameters.Add(new Parameter("Brightness", "_CloudBrightness", 1.4f, 0f, 2f));
        parameters.Add(new Parameter("Sun Glare", "_SunGlare", 0.6f, 0f, 2f));
       // parameters.Add(new Parameter("Time", "_CloudTime", 0, 0f, 100f));

        parameters.Add(new Parameter("XShift", "_XShift", 0f, 0f, 1f));
        parameters.Add(new Parameter("YShift", "_YShift", 0f, 0f, 1f));
        parameters.Add(new Parameter("ZShift", "_ZShift", 0f, 0f, 1f));

        //FillTerrain();
    }


    void FillTerrain() {
        Terrain t = GameObject.Find("Terrain").GetComponent<Terrain>();
        TerrainData terrainData = t.terrainData;
        int ewidth = terrainData.heightmapWidth;
        int eheight = terrainData.heightmapHeight;
        float[,] data = terrainData.GetHeights (0, 0, ewidth, eheight);

        for (int i=0;i<ewidth;i++)
            for (int j=0;j<eheight;j++) {
                Vector2 pos = new Vector3(i,j);
                float scale = 0.00034f;
                float h = 0;
                float A = 0;
                for (int k=1;k<10;k++) {
                    float f = Mathf.Pow(2,k);
                    float amp = 1.0f/(2*Mathf.Pow(k, 1f));
                    h+=(Mathf.PerlinNoise(pos.x*scale*f, pos.y*scale*f)-0.5f)*amp;
                    A+=amp;
                }
                h/=A;

                data[i,j] = Mathf.Clamp(h*1.7f+ 0.04f, 0,1);

            }

        terrainData.SetHeights (0, 0, data);
        t.terrainData = terrainData;
        t.Flush();

    }

    void SetAutoDetail() {
        if (autoDetail)   {
            float fps = Mathf.Clamp(1.0f/Time.smoothDeltaTime,1,60);
//            Debug.Log(fps);
            float dir = (fps - 25); // Target FPS
            Parameter detail = getParameter("Detail");
            if (detail==null) 
                return;
            
            detail.value +=dir*0.0005f;
        }
    }

    void OnGUI() {
        if (!displayMenu)
            return;
        int w = 150;
        int dy = 27;
        int textWidth = 75;
        int margin = 10;

        GUI.contentColor = Color.black;


        GUI.Label(new Rect(margin, dy, textWidth, dy), "Time of day");
        GUI.Label(new Rect(margin, 2*dy, textWidth, dy), "Sun rotation");
        sunRotation.x = GUI.HorizontalSlider(new Rect(2*margin + textWidth, dy, w, dy), sunRotation.x, 0.0F, 360.0F);
        sunRotation.y = GUI.HorizontalSlider(new Rect(2* margin + textWidth, 2*dy, w, dy), sunRotation.y, 0.0F, 360.0F);
        GUI.Label(new Rect(0, 0, 100, 100), "FPS: "+(int)(1.0f / Time.smoothDeltaTime));

        GUI.Label(new Rect(Screen.width - 380, Screen.height - 100, 360, 100), infoText);
        GUI.contentColor = Color.white;
        GUI.Label(new Rect(Screen.width - 381, Screen.height - 101, 360, 100), infoText);
        GUI.contentColor = Color.black;
        string txt = "On";
        if (autoDetail)
            txt = "Off";

        if (GUI.Button(new Rect(margin, (int)(3.5f*dy), 200, dy), "Auto Detail " + txt +  "  (performance)")){
            autoDetail = !autoDetail;
        }
        int i = 5;
        foreach (Parameter p in parameters) {
            p.Render(i++ * dy, w, textWidth, margin);
        }



    }
	
    void MoveCamera()
    {
        Camera c = GameObject.Find("Main Camera").GetComponent<Camera>();
        if (c == null)
            return;
        if (Input.GetKey(KeyCode.W))
            cameraAcc += c.transform.forward * moveScale;
        if (Input.GetKey(KeyCode.S))
            cameraAcc += c.transform.forward * moveScale * -1;
        if (Input.GetKey(KeyCode.D))
            cameraAcc += c.transform.right * moveScale;
        if (Input.GetKey(KeyCode.A))
            cameraAcc += c.transform.right * moveScale * -1;

        if (Input.GetButton("Fire2"))
            viewAcc += (Vector3.right * Input.GetAxis("Mouse X") + Vector3.up * Input.GetAxis("Mouse Y") * -1) * 0.2f;

        c.transform.position = c.transform.position + cameraAcc;
        Vector3 pos = c.transform.position;
        pos.y = Mathf.Clamp(pos.y, 1f, 100);
        c.transform.position = pos;
        view += viewAcc;
        c.transform.eulerAngles = new Vector3(view.y, view.x, 0.0f);
        cameraAcc *= 0.90f;
        viewAcc *= 0.90f;

    }

    void MoveSun()
    {
        GameObject sun = GameObject.Find("Sun");
        if (sun == null)
            return;

        sun.transform.rotation = Quaternion.Euler(sunRotation);
    }

    void UpdateMaterial()
    {
        Material mat = GameObject.Find("Sphere").GetComponent<MeshRenderer>().material;
        foreach (Parameter p in parameters)
            p.SetValue(mat);

        mat.SetFloat("_CloudTime", Time.time*0.01f);
    }

    void Update () {
        MoveCamera();
        MoveSun();
        UpdateMaterial();
        SetAutoDetail();
        if (Input.GetKeyUp(KeyCode.Space))
            displayMenu = !displayMenu;

	}
}
