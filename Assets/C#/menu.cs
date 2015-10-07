using UnityEngine;
using System.Collections;

public class menu : MonoBehaviour {
	public GUITexture menuHome, menuRestart, menuSetup, menuInformation;
	public Texture2D menuHomeTextureNormal, menuRestartTextureNormal, menuSetupTextureNormal, menuInformationTextureNormal, menuHomeTextureMouseDown, menuRestartTextureMouseDown, menuSetupTextureMouseDown, menuInformationTextureMouseDown;
	
	// Use this for initialization
	void Start () {
		menuHome.texture = menuHomeTextureNormal;
		menuRestart.texture = menuRestartTextureNormal;
		menuSetup.texture = menuSetupTextureNormal;
		menuInformation.texture = menuInformationTextureNormal;
	}
	
	// Update is called once per frame
	void Update () 
	{
		//當滑鼠點擊選單時
		if(Input.GetMouseButtonDown(0))
		{
			if(menuHome.HitTest(Input.mousePosition))
			{
				menuHome.texture = menuHomeTextureMouseDown;
			}
			else if(menuRestart.HitTest(Input.mousePosition))
			{
				menuRestart.texture = menuRestartTextureMouseDown;
			}
			else if(menuSetup.HitTest(Input.mousePosition))
			{
				menuSetup.texture = menuSetupTextureMouseDown;
			}
			else if(menuInformation.HitTest(Input.mousePosition))
			{
				menuInformation.texture = menuInformationTextureMouseDown;
			}
		}
		//滑鼠放開後更換回正常圖示
		else if(Input.GetMouseButtonUp(0))
		{
		 	menuHome.texture = menuHomeTextureNormal;
			menuRestart.texture = menuRestartTextureNormal;
			menuSetup.texture = menuSetupTextureNormal;
			menuInformation.texture = menuInformationTextureNormal;
		}
	}
}
