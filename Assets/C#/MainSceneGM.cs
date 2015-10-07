using UnityEngine;
using System.Collections;

public class MainSceneGM : MonoBehaviour {
	public GUITexture GuiSound, GuiExit, GuiLayer1, GuiSkipA, GuiSkipB, GuiBack, GuiStart;
	public Texture2D GuiSoundTextureON, GuiSoundTextureOFF, GuiExitTextureNormal,GuiExitTextureMouseDown, GuiGameStartTexture, GuiRulesTexture, GuiNodeFunctionTexture, GuiQuessionSelectTexture;
	int showType;//目前正在顯示的階段(0遊戲開始, 1遊戲規則, 2節點功能說明, 3選擇關卡)
	float soundVolume;
	float upTime;
	
	public GUITexture GuiQ1L1,GuiQ1L2,GuiQ1L3,GuiQ2L1,GuiQ2L2,GuiQ2L3,GuiQ3L1,GuiQ3L2,GuiQ3L3;
	public Texture2D GuiTextureL1On, GuiTextureL1Off, GuiTextureL2On, GuiTextureL2Off, GuiTextureL3On, GuiTextureL3Off;
	// Use this for initialization
	void Start () {
		GuiSound.texture = GuiSoundTextureON;
		GuiExit.texture = GuiExitTextureNormal;
		GuiLayer1.texture = GuiGameStartTexture;
		showType = 0;
		audio.mute = false;
		soundVolume = 0;
		GuiSkipA.enabled = false;
		GuiSkipB.enabled = false;
		GuiBack.enabled = false;
		GuiStart.enabled = true;
		
		GuiQ1L1.enabled = false;
		GuiQ1L2.enabled = false;
		GuiQ1L3.enabled = false;
		GuiQ2L1.enabled = false;
		GuiQ2L2.enabled = false;
		GuiQ2L3.enabled = false;
		GuiQ3L1.enabled = false;
		GuiQ3L2.enabled = false;
		GuiQ3L3.enabled = false;
		
		GuiQ1L1.texture = GuiTextureL1On;
		GuiQ1L2.texture = GuiTextureL2Off;
		GuiQ1L3.texture = GuiTextureL3Off;
		GuiQ2L1.texture = GuiTextureL1Off;
		GuiQ2L2.texture = GuiTextureL2Off;
		GuiQ2L3.texture = GuiTextureL3Off;
		GuiQ3L1.texture = GuiTextureL1Off;
		GuiQ3L2.texture = GuiTextureL2Off;
		GuiQ3L3.texture = GuiTextureL3Off;
		
		
		//存檔測試
		//PlayerPrefs.SetInt("test",989);
		//PlayerPrefs.SetInt("Q1L1",0);
		//PlayerPrefs.SetInt("Q1L2",0);
		//PlayerPrefs.SetInt("Q1L3",0);
		//print(PlayerPrefs.GetInt("test").ToString());
	}
	
	// Update is called once per frame
	void Update () {
		//起始遊戲時聲音淡入
		if(audio.volume <1)
		{
			soundVolume = soundVolume+0.0001f;
			audio.volume = soundVolume;
		}
		
		//當滑鼠點擊選單時
		if(Input.GetMouseButtonDown(0))
		{
			//聲音開關
			if(GuiSound.HitTest(Input.mousePosition))
			{
				if(GuiSound.texture == GuiSoundTextureON)
				{
					GuiSound.texture = GuiSoundTextureOFF;
					audio.mute = true;
				}
				else
				{
					GuiSound.texture = GuiSoundTextureON;
					audio.mute = false;
				}
			}
			
			/*//離開遊戲
			if(GuiExit.HitTest(Input.mousePosition))
			{
				GuiExit.texture = GuiExitTextureMouseDown;
				Application.Quit();
			}*/
		}
		//滑鼠放開後更換回正常圖示
		else if(Input.GetMouseButtonUp(0))
		{
			GuiExit.texture = GuiExitTextureNormal;
		}
		
		//等待開始遊戲階段
		if(showType==0)
		{
			if(!Input.GetMouseButtonDown(0))
			{
				//當按下任何按鍵時
				if(Input.anyKeyDown)
				{
					GuiLayer1.texture = GuiRulesTexture;
					GuiSkipA.enabled =true;
					showType=1;
					GuiStart.enabled = false;
				}
			}
			else if(Input.GetMouseButtonDown(0))
			{
				if(GuiStart.HitTest(Input.mousePosition))
				{
					GuiLayer1.texture = GuiRulesTexture;
					GuiSkipA.enabled =true;
					showType=1;
					GuiStart.enabled = false;
				}
			}
		}
		
		//遊戲說明階段階段
		if(showType==1)
		{
			if(Input.GetMouseButtonDown(0))
			{
				if(GuiSkipA.HitTest(Input.mousePosition))
				{
					GuiLayer1.texture = GuiNodeFunctionTexture;
					GuiSkipA.enabled =false;
					GuiSkipB.enabled =true;
					GuiBack.enabled =true;
					showType=2;
					upTime = Time.time+0.5f;
				}
			}
		}
		
		//節點功能說明階段
		if(showType==2 && Time.time>upTime)
		{
			if(Input.GetMouseButtonDown(0))
			{
				if(GuiSkipB.HitTest(Input.mousePosition)&& !GuiSkipA.enabled)
				{
					GuiLayer1.texture = GuiQuessionSelectTexture;
					showType=3;
					GuiSkipB.enabled =false;
					GuiBack.enabled = false;
					
					GuiQ1L1.enabled = true;
					GuiQ1L2.enabled = true;
					GuiQ1L3.enabled = true;
					GuiQ2L1.enabled = true;
					GuiQ2L2.enabled = true;
					GuiQ2L3.enabled = true;
					GuiQ3L1.enabled = true;
					GuiQ3L2.enabled = true;
					GuiQ3L3.enabled = true;
		
					GuiQ1L1.texture = GuiTextureL1On;
					//GuiQ1L2.texture = GuiTextureL2Off;
					//GuiQ1L3.texture = GuiTextureL3Off;
					//GuiQ2L1.texture = GuiTextureL1Off;
					//GuiQ2L2.texture = GuiTextureL2Off;
					//GuiQ2L3.texture = GuiTextureL3Off;
					//GuiQ3L1.texture = GuiTextureL1Off;
					//GuiQ3L2.texture = GuiTextureL2Off;
					//GuiQ3L3.texture = GuiTextureL3Off;
					
					if(PlayerPrefs.GetInt("Q1L1")==1)
					{
						GuiQ1L2.texture = GuiTextureL2On;
					}
					if(PlayerPrefs.GetInt("Q1L2")==1)
					{
						GuiQ1L3.texture = GuiTextureL3On;
					}
					if(PlayerPrefs.GetInt("Q1L3")==1)
					{
						GuiQ2L1.texture = GuiTextureL1On;
					}
					
					if(PlayerPrefs.GetInt("Q2L1")==1)
					{
						GuiQ2L2.texture = GuiTextureL2On;
					}
					if(PlayerPrefs.GetInt("Q2L2")==1)
					{
						GuiQ2L3.texture = GuiTextureL3On;
					}
					if(PlayerPrefs.GetInt("Q2L3")==1)
					{
						GuiQ3L1.texture = GuiTextureL1On;
					}
					
					if(PlayerPrefs.GetInt("Q3L1")==1)
					{
						GuiQ3L2.texture = GuiTextureL2On;
					}
					if(PlayerPrefs.GetInt("Q3L2")==1)
					{
						GuiQ3L3.texture = GuiTextureL3On;
					}
				}
				else if(GuiBack.HitTest(Input.mousePosition))
				{
					GuiLayer1.texture = GuiRulesTexture;
					GuiSkipA.enabled =true;
					GuiSkipB.enabled = false;
					GuiBack.enabled = false;
					showType=1;
				}
			}
		}
		
		//關卡選擇階段
		if(showType==3)
		{
			if(Input.GetMouseButtonDown(0))
			{
				if(GuiQ1L1.HitTest(Input.mousePosition))
				{
					Application.LoadLevel("Q1L1GameScene");
				}
				else if(GuiQ1L2.HitTest(Input.mousePosition) && PlayerPrefs.GetInt("Q1L1")==1)
				{
					Application.LoadLevel("Q1L2GameScene");
				}
				else if(GuiQ1L3.HitTest(Input.mousePosition) && PlayerPrefs.GetInt("Q1L2")==1)
				{
					Application.LoadLevel("Q1L3GameScene");
				}
				
				else if(GuiQ2L1.HitTest(Input.mousePosition) && PlayerPrefs.GetInt("Q1L3")==1)
				{
					Application.LoadLevel("Q2L1GameScene");
				}
				else if(GuiQ2L2.HitTest(Input.mousePosition) && PlayerPrefs.GetInt("Q2L1")==1)
				{
					Application.LoadLevel("Q2L2GameScene");
				}
				else if(GuiQ2L3.HitTest(Input.mousePosition) && PlayerPrefs.GetInt("Q2L2")==1)
				{
					Application.LoadLevel("Q2L3GameScene");
				}
				
				else if(GuiQ3L1.HitTest(Input.mousePosition) && PlayerPrefs.GetInt("Q2L3")==1)
				{
					Application.LoadLevel("Q3L1GameScene");
				}
				else if(GuiQ3L2.HitTest(Input.mousePosition) && PlayerPrefs.GetInt("Q3L1")==1)
				{
					Application.LoadLevel("Q3L2GameScene");
				}
				else if(GuiQ3L3.HitTest(Input.mousePosition) && PlayerPrefs.GetInt("Q3L2")==1)
				{
					Application.LoadLevel("Q3L3GameScene");
				}
			}
		}
		
		//還原關卡設定值
		if(Input.GetKeyDown(KeyCode.F12))
		{
			PlayerPrefs.SetInt("Q1L1",0);
			PlayerPrefs.SetInt("Q1L2",0);
			PlayerPrefs.SetInt("Q1L3",0);
			
			PlayerPrefs.SetInt("Q2L1",0);
			PlayerPrefs.SetInt("Q2L2",0);
			PlayerPrefs.SetInt("Q2L3",0);
			
			PlayerPrefs.SetInt("Q3L1",0);
			PlayerPrefs.SetInt("Q3L2",0);
			PlayerPrefs.SetInt("Q3L3",0);
			
			GuiQ1L1.texture = GuiTextureL1On;
			GuiQ1L2.texture = GuiTextureL2Off;
			GuiQ1L3.texture = GuiTextureL3Off;
			GuiQ2L1.texture = GuiTextureL1Off;
			GuiQ2L2.texture = GuiTextureL2Off;
			GuiQ2L3.texture = GuiTextureL3Off;
			GuiQ3L1.texture = GuiTextureL1Off;
			GuiQ3L2.texture = GuiTextureL2Off;
			GuiQ3L3.texture = GuiTextureL3Off;
			
			print ("關卡值已還原");
		}
		
		//全破密技
		if(Input.GetKeyDown(KeyCode.F11))
		{
			PlayerPrefs.SetInt("Q1L1",1);
			PlayerPrefs.SetInt("Q1L2",1);
			PlayerPrefs.SetInt("Q1L3",1);
			
			PlayerPrefs.SetInt("Q2L1",1);
			PlayerPrefs.SetInt("Q2L2",1);
			PlayerPrefs.SetInt("Q2L3",1);
			
			PlayerPrefs.SetInt("Q3L1",1);
			PlayerPrefs.SetInt("Q3L2",1);
			PlayerPrefs.SetInt("Q3L3",1);
			
			GuiQ1L1.texture = GuiTextureL1On;
			GuiQ1L2.texture = GuiTextureL2On;
			GuiQ1L3.texture = GuiTextureL3On;
			GuiQ2L1.texture = GuiTextureL1On;
			GuiQ2L2.texture = GuiTextureL2On;
			GuiQ2L3.texture = GuiTextureL3On;
			GuiQ3L1.texture = GuiTextureL1On;
			GuiQ3L2.texture = GuiTextureL2On;
			GuiQ3L3.texture = GuiTextureL3On;
			
			print ("關卡值已還原");
		}
		
		//各關解鎖
		if(Input.GetKeyDown(KeyCode.F1))
		{
			PlayerPrefs.SetInt("Q1L1",1);
			GuiQ1L1.texture = GuiTextureL1On;
			GuiQ1L2.texture = GuiTextureL2On;
		}
		else if(Input.GetKeyDown(KeyCode.F2))
		{
			PlayerPrefs.SetInt("Q1L1",1);
			PlayerPrefs.SetInt("Q1L2",1);
			GuiQ1L1.texture = GuiTextureL1On;
			GuiQ1L2.texture = GuiTextureL2On;
			GuiQ1L3.texture = GuiTextureL3On;
		}
		else if(Input.GetKeyDown(KeyCode.F3))
		{
			PlayerPrefs.SetInt("Q1L1",1);
			PlayerPrefs.SetInt("Q1L2",1);
			PlayerPrefs.SetInt("Q1L3",1);
			GuiQ1L1.texture = GuiTextureL1On;
			GuiQ1L2.texture = GuiTextureL2On;
			GuiQ1L3.texture = GuiTextureL3On;
			GuiQ2L1.texture = GuiTextureL1On;
		}
		else if(Input.GetKeyDown(KeyCode.F4))
		{
			PlayerPrefs.SetInt("Q1L1",1);
			PlayerPrefs.SetInt("Q1L2",1);
			PlayerPrefs.SetInt("Q1L3",1);
			PlayerPrefs.SetInt("Q2L1",1);
			GuiQ1L1.texture = GuiTextureL1On;
			GuiQ1L2.texture = GuiTextureL2On;
			GuiQ1L3.texture = GuiTextureL3On;
			GuiQ2L1.texture = GuiTextureL1On;
			GuiQ2L2.texture = GuiTextureL2On;
		}
		else if(Input.GetKeyDown(KeyCode.F5))
		{
			PlayerPrefs.SetInt("Q1L1",1);
			PlayerPrefs.SetInt("Q1L2",1);
			PlayerPrefs.SetInt("Q1L3",1);
			PlayerPrefs.SetInt("Q2L1",1);
			PlayerPrefs.SetInt("Q2L2",1);
			GuiQ1L1.texture = GuiTextureL1On;
			GuiQ1L2.texture = GuiTextureL2On;
			GuiQ1L3.texture = GuiTextureL3On;
			GuiQ2L1.texture = GuiTextureL1On;
			GuiQ2L2.texture = GuiTextureL2On;
			GuiQ2L3.texture = GuiTextureL3On;
		}	
		else if(Input.GetKeyDown(KeyCode.F6))
		{
			PlayerPrefs.SetInt("Q1L1",1);
			PlayerPrefs.SetInt("Q1L2",1);
			PlayerPrefs.SetInt("Q1L3",1);
			PlayerPrefs.SetInt("Q2L1",1);
			PlayerPrefs.SetInt("Q2L2",1);
			PlayerPrefs.SetInt("Q2L3",1);
			GuiQ1L1.texture = GuiTextureL1On;
			GuiQ1L2.texture = GuiTextureL2On;
			GuiQ1L3.texture = GuiTextureL3On;
			GuiQ2L1.texture = GuiTextureL1On;
			GuiQ2L2.texture = GuiTextureL2On;
			GuiQ2L3.texture = GuiTextureL3On;
			GuiQ3L1.texture = GuiTextureL1On;
		}	
		else if(Input.GetKeyDown(KeyCode.F7))
		{
			PlayerPrefs.SetInt("Q1L1",1);
			PlayerPrefs.SetInt("Q1L2",1);
			PlayerPrefs.SetInt("Q1L3",1);
			PlayerPrefs.SetInt("Q2L1",1);
			PlayerPrefs.SetInt("Q2L2",1);
			PlayerPrefs.SetInt("Q2L3",1);
			PlayerPrefs.SetInt("Q3L1",1);
			GuiQ1L1.texture = GuiTextureL1On;
			GuiQ1L2.texture = GuiTextureL2On;
			GuiQ1L3.texture = GuiTextureL3On;
			GuiQ2L1.texture = GuiTextureL1On;
			GuiQ2L2.texture = GuiTextureL2On;
			GuiQ2L3.texture = GuiTextureL3On;
			GuiQ3L1.texture = GuiTextureL1On;
			GuiQ3L2.texture = GuiTextureL2On;
		}
		else if(Input.GetKeyDown(KeyCode.F8))
		{
			PlayerPrefs.SetInt("Q1L1",1);
			PlayerPrefs.SetInt("Q1L2",1);
			PlayerPrefs.SetInt("Q1L3",1);
			PlayerPrefs.SetInt("Q2L1",1);
			PlayerPrefs.SetInt("Q2L2",1);
			PlayerPrefs.SetInt("Q2L3",1);
			PlayerPrefs.SetInt("Q3L1",1);
			PlayerPrefs.SetInt("Q3L2",1);
			GuiQ1L1.texture = GuiTextureL1On;
			GuiQ1L2.texture = GuiTextureL2On;
			GuiQ1L3.texture = GuiTextureL3On;
			GuiQ2L1.texture = GuiTextureL1On;
			GuiQ2L2.texture = GuiTextureL2On;
			GuiQ2L3.texture = GuiTextureL3On;
			GuiQ3L1.texture = GuiTextureL1On;
			GuiQ3L2.texture = GuiTextureL2On;
			GuiQ3L3.texture = GuiTextureL3On;
		}
	}
}
