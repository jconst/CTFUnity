using UnityEngine;
using System.Collections.Generic;
 
public class GlowCamera : MonoBehaviour
{
    public Material GlowCameraMaterial;
	public Material DebugOnlySplitscreenMaterial;
	public Material DebugOnlyDisplayGlowMaterial;
	
	public float GlowStrength = 20.0F;
    public float BlurRadius = 3.0F;
    public float Factor = 0.5f;
	private float ScreenBlurFactor;
	
	public static List<GlowObject> GlowObjects = new List<GlowObject>();
	
	public bool DebugMode = true;
	public bool ShowCursor = false;
    private int glowObjectLayer;
	
	private GameObject GlowCamObj;
    private GameObject MiniMainCam;
    private Texture2D MaskShaderTexture;
	
	private float minFactor = 0.055f;
	private float maxFactor = 1.0f;
    private float LastFactor;
    private Vector2 MaskSize;
 
    private bool MaskGrab;
 
    private Vector2 LastScreenResolution, CurrentScreenResolution;
	
	private static bool UseGlow, GlowActive, GlowHalfActive, GlowMaskActive;
	
	public static bool UseMenu, LockCursor;
	
	private int frameCounter;
    public int GlowFrameUpdateRate = 1;
	
	private bool StartupCheck() {
		if(GlowCameraMaterial == null) {
			Debug.LogError("GlowCamera GameObject <" + gameObject.name + "> missing the GlowCameraShaderMaterial.");
			return false;
		}

        glowObjectLayer = LayerMask.NameToLayer("GlowObject");
        if (glowObjectLayer == -1)
        {
            Debug.LogError("Layer 'GlowObject': is not defined. Create the 'GlowObject' Layer at Layers Setup in the Inspector or set a layer at the GlowCamera component!");
			return false;
		}
		return true;
	}
 
    void Awake()
    {
		if(!StartupCheck()) {
			UseGlow = false;
			GlowActive = false;
			Debug.LogError("Indie Glow Startup Check Failed. Check the Console for some Errors.");
			return;
		}
		
		UseGlow = true;
		GlowActive = true;
		if(DebugMode) {
			GlowHalfActive = false;
			if(ShowCursor)
				LockCursor = false;
			else
				LockCursor = true;
		} else {
			LockCursor = false;
		}
		
		LastFactor = Factor;
		
        MiniMainCam = new GameObject("Mini Main Camera");
        MiniMainCam.AddComponent<Camera>();
		MiniMainCam.camera.CopyFrom(gameObject.camera);
        MiniMainCam.camera.depth = -3;
        MiniMainCam.camera.clearFlags = CameraClearFlags.Skybox;
        MiniMainCam.transform.position = gameObject.camera.transform.position;
        MiniMainCam.transform.rotation = gameObject.camera.transform.rotation;
        MiniMainCam.transform.parent = gameObject.camera.gameObject.transform;
		MiniMainCam.camera.renderingPath = RenderingPath.VertexLit;
		MiniMainCam.camera.enabled = false;
        MiniMainCam.camera.pixelRect = new Rect(0, 0, Screen.width * Factor, Screen.height * Factor);
        
        GlowCamObj = new GameObject("Glow Camera");
        GlowCamObj.AddComponent<Camera>();
		GlowCamObj.camera.CopyFrom(gameObject.camera);
        GlowCamObj.camera.depth = -2;
        GlowCamObj.camera.cullingMask = 1 << glowObjectLayer;
        GlowCamObj.camera.clearFlags = CameraClearFlags.Nothing;
        GlowCamObj.transform.position = gameObject.camera.transform.position;
        GlowCamObj.transform.rotation = gameObject.camera.transform.rotation;
        GlowCamObj.transform.parent = gameObject.camera.gameObject.transform;
		GlowCamObj.camera.renderingPath = RenderingPath.VertexLit;
        GlowCamObj.camera.enabled = false;
        GlowCamObj.camera.pixelRect = new Rect(0, 0, Screen.width * Factor, Screen.height * Factor);
		
		LastScreenResolution = GetScreenResolution();
		
		ScreenBlurFactor = (float)Screen.width/1024.0f * BlurRadius;
		
        SetGlowMaskResolution();
		InitGlowMaterials();
    }
	
	void RenderMiniMainCam() {
		MiniMainCam.camera.Render();
	}
	
	void SetGlowMaterials() {
		if(GlowObjects.Count != 0) {
			foreach (GlowObject glo in GlowObjects)
        	{
        		if (glo != null)
      			{
					glo.gameObject.renderer.material = glo.GlowMaterial;
                    glo.gameObject.layer = glowObjectLayer;
     			}
  			}
		}
	}
	
	void RenderGlowCam() {
		GL.Clear(false, true, Color.clear);
  		GlowCamObj.camera.Render();
	}
	
	void GrabGlowMask() {
   		MaskShaderTexture.ReadPixels(new Rect(0, 0, Screen.width * Factor, Screen.height * Factor), 0, 0, false);
		MaskShaderTexture.Apply(false, false);
  		MaskGrab = false;
	}
	
	void SetSourceMaterials() {
		if(GlowObjects.Count != 0) {
			foreach (GlowObject glo in GlowObjects)
   			{
 				if (glo != null)
				{
					glo.gameObject.renderer.material = glo.GetSourceMaterial;
					glo.gameObject.layer = glo.GetSourceLayer;
     			}
 			}
		}
	}
 
    void OnPreRender()
    {
		if(!UseGlow || !GlowActive)
			return;
		
        if (MaskGrab)
        {
			RenderMiniMainCam();
			SetGlowMaterials();
			RenderGlowCam();
			GrabGlowMask();
			SetSourceMaterials();
        }
    }
	
	void FixedUpdate()
    {
		if(DebugMode) {
			if(Input.GetKeyDown(KeyCode.Tab)) {
				UseMenu = !UseMenu;
				LockCursor = !UseMenu;
				Screen.lockCursor = LockCursor;
			}
		}
		
		if(!UseGlow || !GlowActive)
			return;
		
		UpdateCameraSettings();

        CurrentScreenResolution = GetScreenResolution();
        if (LastScreenResolution != CurrentScreenResolution || LastFactor != Factor)
        {
            SetGlowMaskResolution();
            LastScreenResolution = CurrentScreenResolution;
            LastFactor = Factor;
        }
		
		ScreenBlurFactor = (float)Screen.width/1024.0f * BlurRadius;
 
		frameCounter++;
        if (frameCounter >= GlowFrameUpdateRate)
        {
            frameCounter = 0;
            MaskGrab = true;
        }
    }
	
	void UpdateCameraSettings() {
		if(!gameObject.camera.orthographic) {
			MiniMainCam.camera.fov = gameObject.camera.fov;
			MiniMainCam.camera.nearClipPlane = gameObject.camera.nearClipPlane;
			MiniMainCam.camera.farClipPlane = gameObject.camera.farClipPlane;
			
			GlowCamObj.camera.fov = gameObject.camera.fov;
			GlowCamObj.camera.nearClipPlane = gameObject.camera.nearClipPlane;
			GlowCamObj.camera.farClipPlane = gameObject.camera.farClipPlane;
		} else {
			MiniMainCam.camera.orthographicSize = gameObject.camera.orthographicSize;
			MiniMainCam.camera.nearClipPlane = gameObject.camera.nearClipPlane;
			MiniMainCam.camera.farClipPlane = gameObject.camera.farClipPlane;
			
			GlowCamObj.camera.orthographicSize = gameObject.camera.orthographicSize;
			GlowCamObj.camera.nearClipPlane = gameObject.camera.nearClipPlane;
			GlowCamObj.camera.farClipPlane = gameObject.camera.farClipPlane;
		}
	}

    void OnGUI()
    {
		if(!UseGlow)
			return;
		
		if(ShowCursor)
			Screen.lockCursor = false;
		else
			Screen.lockCursor = LockCursor;
		
        if (Event.current.type.Equals(EventType.repaint))
        {
			if(GlowActive) {
				if(DebugMode) {
					if(GlowHalfActive) {
						DebugOnlySplitscreenMaterial.SetTexture("_MSKTex", MaskShaderTexture);
						Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), MaskShaderTexture, new Rect(0, 0, MaskSize.x, MaskSize.y), 0, 0, 0, 0, DebugOnlySplitscreenMaterial);
					} else if(GlowMaskActive) {
						DebugOnlyDisplayGlowMaterial.SetTexture("_MSKTex", MaskShaderTexture);
						Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), MaskShaderTexture, new Rect(0, 0, MaskSize.x, MaskSize.y), 0, 0, 0, 0, DebugOnlyDisplayGlowMaterial);
					} else  {
						GlowCameraMaterial.SetTexture("_MSKTex", MaskShaderTexture);
						Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), MaskShaderTexture, new Rect(0, 0, MaskSize.x, MaskSize.y), 0, 0, 0, 0, GlowCameraMaterial);
					}
				} else {
						GlowCameraMaterial.SetTexture("_MSKTex", MaskShaderTexture);
						Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), MaskShaderTexture, new Rect(0, 0, MaskSize.x, MaskSize.y), 0, 0, 0, 0, GlowCameraMaterial);
				}
			}
        }
 
		if(DebugMode) {
			if(!UseMenu) {
				GUI.Label(new Rect(10, 20, 250, 30), "Press Tab to Toggle Glow Menu");
			} else {
		
        		GUI.Label(new Rect(10, 20, 100, 30), "GlowStrength");
        		GlowStrength = GUI.HorizontalSlider(new Rect(10, 40, 200, 30), GlowStrength, 0.0F, 40.0F);
				DebugOnlySplitscreenMaterial.SetFloat("_GlowStrength", GlowStrength);
				GlowCameraMaterial.SetFloat("_GlowStrength", GlowStrength);
				DebugOnlyDisplayGlowMaterial.SetFloat("_GlowStrength", GlowStrength);
				GUI.Label(new Rect(10, 60, 200, 30), "Actual GlowStrenght: " + GlowStrength);
 
        		GUI.Label(new Rect(10, 100, 100, 30), "BlurRadius");
        		BlurRadius = GUI.HorizontalSlider(new Rect(10, 120, 200, 30), BlurRadius, 0.0F, 20.0f);
				DebugOnlySplitscreenMaterial.SetFloat("_BlurRadius", ScreenBlurFactor);
				GlowCameraMaterial.SetFloat("_BlurRadius", ScreenBlurFactor);
				DebugOnlyDisplayGlowMaterial.SetFloat("_BlurRadius", ScreenBlurFactor);
				GUI.Label(new Rect(10, 140, 200, 30), "Actual BlurRadius: " + ScreenBlurFactor);
		
				GUI.Label(new Rect(10, 180, 100, 30), "Factor");
				Factor = GUI.HorizontalSlider(new Rect(10, 200, 200, 30), Factor, minFactor, maxFactor);
				GUI.Label(new Rect(10, 220, 200, 30), "Actual Factor: " + Factor);
				GUI.Label(new Rect(10, 240, 200, 30), "GlowMaskSize: " + MaskShaderTexture.width + " / " + MaskShaderTexture.height);
		
				if(GUI.Button(new Rect(10, 280, 200, 30), "Toggle 1/2 Screen Glow")) {
					GlowHalfActive = !GlowHalfActive;
					GlowMaskActive = false;
				}
			
				if(GUI.Button(new Rect(10, 320, 200, 30), "Toggle Glow Mask Screen")) {
					GlowMaskActive = !GlowMaskActive;
					GlowHalfActive = false;
				}
		
				if(GUI.Button(new Rect(10, 360, 200, 30), "Toggle Glow")) {
					GlowActive = !GlowActive;
					GlowHalfActive = false;
					GlowMaskActive = false;
				}
			}
		}
    }
	
	void InitGlowMaterials() {
		if(DebugMode) {
			DebugOnlySplitscreenMaterial.SetFloat("_GlowStrength", GlowStrength);
			GlowCameraMaterial.SetFloat("_GlowStrength", GlowStrength);
			DebugOnlyDisplayGlowMaterial.SetFloat("_GlowStrength", GlowStrength);
			DebugOnlySplitscreenMaterial.SetFloat("_BlurRadius", ScreenBlurFactor);
			GlowCameraMaterial.SetFloat("_BlurRadius", ScreenBlurFactor);
			DebugOnlyDisplayGlowMaterial.SetFloat("_BlurRadius", ScreenBlurFactor);
		} else {
			GlowCameraMaterial.SetFloat("_GlowStrength", GlowStrength);
			GlowCameraMaterial.SetFloat("_BlurRadius", ScreenBlurFactor);
		}
	}

    int NearestPowerOfTwo(int value)
    {
        var result = 1;
        while (result < value)
            result *= 2;
        return result;
    }
 
    private Vector2 GetScreenResolution() { return new Vector2(Screen.width, Screen.height); }
 
    private void SetGlowMaskResolution()
    {
		Factor = Mathf.Clamp(Factor, minFactor, maxFactor);
        MaskShaderTexture = new Texture2D(NearestPowerOfTwo((int)(Screen.width * Factor)), NearestPowerOfTwo((int)(Screen.height * Factor)), TextureFormat.ARGB32, false);
        MaskSize = new Vector2((float)(Screen.width * Factor) / MaskShaderTexture.width, (float)(Screen.height * Factor) / MaskShaderTexture.height);
		if(DebugMode) {
        	DebugOnlySplitscreenMaterial.SetFloat("SizeX", MaskSize.x);
        	DebugOnlySplitscreenMaterial.SetFloat("SizeY", MaskSize.y);
        	DebugOnlySplitscreenMaterial.SetVector("_TexelSize", new Vector4(1.0f / MaskShaderTexture.width, 1.0f / MaskShaderTexture.height, 0, 0));
			GlowCameraMaterial.SetFloat("SizeX", MaskSize.x);
        	GlowCameraMaterial.SetFloat("SizeY", MaskSize.y);
        	GlowCameraMaterial.SetVector("_TexelSize", new Vector4(1.0f / MaskShaderTexture.width, 1.0f / MaskShaderTexture.height, 0, 0));
			DebugOnlyDisplayGlowMaterial.SetFloat("SizeX", MaskSize.x);
        	DebugOnlyDisplayGlowMaterial.SetFloat("SizeY", MaskSize.y);
        	DebugOnlyDisplayGlowMaterial.SetVector("_TexelSize", new Vector4(1.0f / MaskShaderTexture.width, 1.0f / MaskShaderTexture.height, 0, 0));
		} else {
			GlowCameraMaterial.SetFloat("SizeX", MaskSize.x);
        	GlowCameraMaterial.SetFloat("SizeY", MaskSize.y);
        	GlowCameraMaterial.SetVector("_TexelSize", new Vector4(1.0f / MaskShaderTexture.width, 1.0f / MaskShaderTexture.height, 0, 0));
		}
 
        MaskShaderTexture.anisoLevel = 1;	// Min 1 Max 9
        MaskShaderTexture.filterMode = FilterMode.Bilinear;
        MaskShaderTexture.wrapMode = TextureWrapMode.Clamp;
 
        GlowCamObj.camera.pixelRect = new Rect(0, 0, Screen.width * Factor, Screen.height * Factor);
        MiniMainCam.camera.pixelRect = new Rect(0, 0, Screen.width * Factor, Screen.height * Factor);
    }
	
	public static void ActivateGlowShader(bool active) {
		GlowActive = active;
	}
	
	public void SetGlobalGlowStrength( float GSValue) {
		GlowStrength = GSValue;
	}
	
	public void SetGlobalBlurRadius( float GRValue) {
		BlurRadius = GRValue;
	}
	
	public void SetGlobalFactor( float FValue) {
		Factor = FValue;
	}
	
	public static bool GlowMenuActive() {
		return UseMenu;
	}
	
	public static void ForceGlow(bool active) {
		UseGlow = active;
		GlowActive = active;
	}
 	
}