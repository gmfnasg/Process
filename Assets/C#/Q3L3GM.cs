using UnityEngine;
using System.Collections;

public class Q3L3GM : MonoBehaviour {
	//!!特別注意 !!如果滑鼠點選有反應但節點沒有位移代表節點物件的clickNode程式沒有對應到本程式中的clickNodeSwish屬性
	
	public GameObject nodeObj;
	public int node = 4, layer = 4 ; //節點數量,階層數 如果系統未給予指定數量時起始為4,4
	public float picNumber = 36; //圖示的張數
	int[,] QuestionNodeType; //設定題目節點的圖示屬性
	bool[,] QuestionNodeSwish; //紀錄目前進行到哪個節點
	GameObject[,] QuestionNodeObj_prefab; //設定題目各節點物件用
	GameObject[,] NodeObj_prefab; //設定各節點物件用	
	
	int[,] playerSlovingNodeType;//紀錄玩家解題狀態
	
	GameObject ZeroNode;//中心點節點物件
	
	bool playerSlovingQuession=false; //玩家解題成功
	
	public GUITexture GuiShowInformation;//顯示遊戲規則資訊
	public GUITexture GuiShowSuccess,GuiNextQuession; //顯示成功圖示前往下一題目
	
	//移動節點用
	static public bool clickNodeSwish; //判斷是否有滑鼠底選到節點物件
	int clickNodeA_IDi,clickNodeA_IDj,clickNodeB_IDi,clickNodeB_IDj;
	bool getFirstID; //是否已經取得第一個id
	int movieCount; //移動次數
	bool clickTwice=false; //是否移動過
	public GameObject clickNodeLight;
	
	//選單用
	public GUITexture menuHome, menuRestart, menuInformation, menuSound;
	public Texture2D menuHomeTextureNormal, menuRestartTextureNormal, menuInformationTextureNormal, menuSoundTextureNormal, menuHomeTextureMouseDown, menuRestartTextureMouseDown, menuInformationTextureMouseDown, menuSoundTextureMouseDown;
	
	
	// Use this for initialization
	void Start () {
		//存檔測試
		//print(PlayerPrefs.GetInt("test"));
		
		node = node+2;//預留位移空間用
		layer = layer +2;//頭尾排起始與終點顯示用
		QuestionNodeType = new int[node,layer];
		QuestionNodeSwish = new bool[node,layer];
		QuestionNodeObj_prefab = new GameObject [node,layer];
		NodeObj_prefab = new GameObject [node,layer];
		
		playerSlovingNodeType = new int[node,layer];
		
		//建立中心點的物件
		ZeroNode = GameObject.Instantiate(nodeObj,new Vector3(0,0,0),Quaternion.identity) as GameObject;
		ZeroNode.transform.localScale = new Vector3(2,2,2);
		ZeroNode.renderer.material.color = new Color(0.5f,0.5f,0.5f,1);
		changTexture(1,24,ZeroNode);
		
		//選單用
		menuHome.texture = menuHomeTextureNormal;
		menuRestart.texture = menuRestartTextureNormal;
		menuInformation.texture = menuInformationTextureNormal;
		menuSound.texture = menuSoundTextureNormal;
		
		//先將點選的燈光關掉
		clickNodeLight.light.enabled = false;
		
		GuiShowInformation.enabled = false;
		GuiShowSuccess.enabled = false;
		GuiNextQuession.enabled = false;
		
		//產生所有物件與設定與亂數題目屬性
		autoCreatQuessionNodeObjAndSetup();
		
		//   上下選一作為起始設定
		
		//產生自訂題目物件與設定
		//creatCustomizationsQuesionAndSet();
		
		//設定起始材質球屬性
		playerSlovingNodeType = QuestionNodeType;
		for(int j2=0;j2<layer-2;j2++)
		{
			for(int i2=1;i2<node-1;i2++)
			{
				QuestionNodeObj_prefab[i2,j2].renderer.material.color = new Color(1,1,1,1);
			}
		}
		
		
		//檢查起始問題路線是否到達終點
		bool targetSlovingQuestionSwish=false;
		checkQuestion(QuestionNodeType,QuestionNodeSwish, out targetSlovingQuestionSwish);
		
		//更新節點透明度
		checkAllNodeSwish(QuestionNodeObj_prefab,QuestionNodeSwish);
		
		//確定題目是未解提狀態的
		if (targetSlovingQuestionSwish)
		{
			print ("初始題目不需解題!!!!");
			//產生新的題目
			while (targetSlovingQuestionSwish)
			{
				//亂數產生題目
				rndQuession(QuestionNodeType,QuestionNodeSwish, out QuestionNodeType,out QuestionNodeSwish);
			
				//檢查起始問題路線是否到達終點
				checkQuestion(QuestionNodeType,QuestionNodeSwish, out targetSlovingQuestionSwish);
			
				//更新圖示
				for(int j=1;j<layer-1;j++)
				{
					for(int i=1;i<node-1;i++)
					{
						changTexture(j,QuestionNodeType[i,j],QuestionNodeObj_prefab[i,j]);
					}
				}
			}
		}

		//將參考用節點物件隱藏
		nodeObj.renderer.enabled = false;
		nodeObj.transform.localPosition = new Vector3(0,0,100);
	}
	
	// Update is called once per frame
	void Update () 
	{	
		//!!特別注意 !!如果滑鼠點選有反應但節點沒有位移代表節點物件的clickNode程式沒有對應到本程式中的clickNodeSwish屬性
		
		//找出移動模式與移動階層
		int DisplacementType, DisplacementLayer;
		bool isDisplacement;
		mouseClickNodeEven_FindDisplacementTypeAndLayer(out DisplacementType, out DisplacementLayer, out isDisplacement);
		
		//移動節點
		DisplacementOneLayerTypeAndCreatNewNode(DisplacementType,DisplacementLayer,playerSlovingNodeType,out playerSlovingNodeType);
		
		 //更新節點圖示
		for(int j=1;j<layer-1;j++)
		{
			for(int i=1;i<node-1;i++)
			{
				changTexture(j,playerSlovingNodeType[i,j],QuestionNodeObj_prefab[i,j]);
			}
		}
		
		//移動更新後檢查解題狀態
		if(isDisplacement)
		{
			//設定所有節點開關起始屬性為關閉
			turnOffAllNodeSwish(QuestionNodeSwish);
		
			//檢查起始問題路線是否到達終點
			bool targetSlovingQuestionSwish=false;
			checkQuestion(playerSlovingNodeType,QuestionNodeSwish, out targetSlovingQuestionSwish);
			
			//更新節點透明度
			checkAllNodeSwish(QuestionNodeObj_prefab,QuestionNodeSwish);

			if (targetSlovingQuestionSwish )
			{
				print ("成功解題!!!!");
				playerSlovingQuession = true;
				GuiShowSuccess.enabled = true;
				//GuiNextQuession.enabled = true;
				PlayerPrefs.SetInt("Q3L3",1);//將Q1L3關卡屬性改為已解題
			}
		}
		
		//旋轉中心節點物件
		ZeroNode.transform.RotateAround(Vector3.zero, Vector3.forward,1);
		//中心節點透明度變換
		float rndAlpha = Random.Range(8,11);
		print (rndAlpha.ToString());
		if (!playerSlovingQuession)
		{
			ZeroNode.renderer.material.color = new Color(0.5f,0.5f,0.5f,0.5f);
		}
		else
		{
			ZeroNode.renderer.material.color = new Color(1,1,1,rndAlpha/10);
		}
		
		//當滑鼠點擊選單時
		if(Input.GetMouseButtonDown(0))
		{
			if(menuHome.HitTest(Input.mousePosition))
			{
				menuHome.texture = menuHomeTextureMouseDown;
				Application.LoadLevel("MainScene");
			}
			else if(menuRestart.HitTest(Input.mousePosition) && !playerSlovingQuession)
			{
				menuRestart.texture = menuRestartTextureMouseDown;
				playerSlovingNodeType = QuestionNodeType;
				//設定所有節點開關起始屬性為關閉
				turnOffAllNodeSwish(QuestionNodeSwish);
		
				//檢查起始問題路線是否到達終點
				bool targetSlovingQuestionSwish=false;
				checkQuestion(playerSlovingNodeType,QuestionNodeSwish, out targetSlovingQuestionSwish);
			
				//更新節點透明度
				checkAllNodeSwish(QuestionNodeObj_prefab,QuestionNodeSwish);
				
			}
			else if(menuInformation.HitTest(Input.mousePosition))
			{
				menuInformation.texture = menuInformationTextureMouseDown;
				if(!GuiShowInformation.enabled)
				{
					GuiShowInformation.enabled = true;
				}
				else
				{
					GuiShowInformation.enabled = false;
				}
			}
			else if(menuSound.HitTest(Input.mousePosition))
			{
				menuSound.texture=menuSoundTextureMouseDown;
				if(!audio.mute)
				{
					audio.mute = true;
				}
				else
				{
					audio.mute = false;
				}
			}
		}
		//滑鼠放開後更換回正常圖示
		else if(Input.GetMouseButtonUp(0))
		{
		 	menuHome.texture = menuHomeTextureNormal;
			menuRestart.texture = menuRestartTextureNormal;
			menuInformation.texture = menuInformationTextureNormal;
			menuSound.texture = menuSoundTextureNormal;
		}
		/*
		//前往下一題目
		if(Input.GetMouseButtonDown(0) && GuiNextQuession.enabled)
		{
			if(GuiNextQuession.HitTest(Input.mousePosition))
			{
			Application.LoadLevel("Q2L1GameScene");  //<-------------------------------------------------------------------------設定到下一個題目場景
			}
		}
		*/
		
		//更新起點圖示
		int FidI,FidJ;
		findStartNodeType(playerSlovingNodeType,QuestionNodeSwish,out FidI,out FidJ);
		changTexture(FidJ, 28 ,NodeObj_prefab[FidI,FidJ]);
		
		//破關後返回主選單
		if(Input.GetKeyDown(KeyCode.Space) && playerSlovingQuession )
		{
			Application.LoadLevel("MainScene");
		}
	} 
	
	//自訂題目節點屬性並產生
	void creatCustomizationsQuesionAndSet()
	{	
		//限定節點與階層數量
		node = 6;
		layer =6;
		//手動設定節點屬性
		QuestionNodeType[1,4]=3;
		QuestionNodeType[2,4]=2;
		QuestionNodeType[3,4]=2;
		QuestionNodeType[4,4]=4;
			
		QuestionNodeType[1,3]=4;
		QuestionNodeType[2,3]=3;
		QuestionNodeType[3,3]=6;
		QuestionNodeType[4,3]=3;
			
		QuestionNodeType[1,2]=4;
		QuestionNodeType[2,2]=1;
		QuestionNodeType[3,2]=2;
		QuestionNodeType[4,2]=2;
			
		QuestionNodeType[1,1]=2;
		QuestionNodeType[2,1]=4;
		QuestionNodeType[3,1]=3;
		QuestionNodeType[4,1]=1;
		
		int intoIDi = 4;
		int intoIDj = 4;
		
		QuestionNodeSwish[intoIDi,intoIDj]=true;
	
		//產生節點物件並給予名稱與座標ID
		for(int i = 1;i<node-1;i++)
		{
			for(int j = 0;j<layer;j++)
			{
				QuestionNodeObj_prefab[i,j] = GameObject.Instantiate(nodeObj,new Vector3(0,1.5f+j+(j*0.2f),0),Quaternion.identity) as GameObject;
				QuestionNodeObj_prefab[i,j].name = QuestionNodeObj_prefab[i,j].name + "Question i=" + i.ToString() + ",j=" + j.ToString()+", Tyep="+QuestionNodeType[i,j].ToString();
				NodeObj_prefab[i,j] = QuestionNodeObj_prefab[i,j];
				//將節點依照屬性改變其貼圖圖示
				changTexture(j,QuestionNodeType[i,j],QuestionNodeObj_prefab[i,j]);
				//旋轉物件
				NodeObj_prefab[i,j].transform.RotateAround(Vector3.zero, Vector3.forward,360/(node-2)*i);
				
			}
		}		
		QuestionNodeObj_prefab[intoIDi,intoIDj].renderer.material.color = new Color(1,1,1,0.5f);
		
		//設定亮度
		for(int j=0;j<layer;j=j+layer-1)
		{
			for(int i=1;i<node-1;i++)
			{
				QuestionNodeObj_prefab[i,j].renderer.material.color = new Color(1,1,1,1);
			}
		}
	}

	//檢查更換貼圖屬性 (編號J，節點類型，指定目標節點物件)
	void changTexture(int j,int type, GameObject obj) 
	{
		if(type == 0)
		{
			//節點圖示: 黑洞
			obj.transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*4,(1/(Mathf.Sqrt(picNumber)))*5);
			//print("黑洞 i="+ i.ToString() +", j=" + j.ToString());
		}
		
		//方向節點類
		else if (type == 1)
		{
			//節點圖示: 方向節點 向左
			obj.transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*0,(1/(Mathf.Sqrt(picNumber)))*5);
			//print("方向節點 向左 i="+ i.ToString() +", j=" + j.ToString());
		}
		else if (type == 2)
		{
			//節點圖示: 方向節點 向上
			obj.transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*1,(1/(Mathf.Sqrt(picNumber)))*5);
			//print("方向節點 向上 i="+ i.ToString() +", j=" + j.ToString());
		}
		else if (type == 3)
		{
			//節點圖示: 方向節點 向右
			obj.transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*2,(1/(Mathf.Sqrt(picNumber)))*5);
			//print("方向節點 向右 i="+ i.ToString() +", j=" + j.ToString());
		}
		else if (type == 4)
		{
			//節點圖示: 方向節點 向下
			obj.transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*3,(1/(Mathf.Sqrt(picNumber)))*5);
			//print("方向節點 向下 i="+ i.ToString() +", j=" + j.ToString());
		}
		
		//逆時針旋轉節點類
		else if (type == 5)
		{
			//節點圖示: 逆時針選轉節點節點 向左
			obj.transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*0,(1/(Mathf.Sqrt(picNumber)))*4);
			//print("逆時針選轉節點節點 向左 i="+ i.ToString() +", j=" + j.ToString());
		}
		else if (type == 6)
		{
			//節點圖示: 逆時針選轉節點節點 向上
			obj.transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*1,(1/(Mathf.Sqrt(picNumber)))*4);
			//print("逆時針選轉節點節點 向上 i="+ i.ToString() +", j=" + j.ToString());
		}
		else if (type == 7)
		{
			//節點圖示: 逆時針選轉節點節點 向右
			obj.transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*2,(1/(Mathf.Sqrt(picNumber)))*4);
			//print("逆時針選轉節點節點 向右 i="+ i.ToString() +", j=" + j.ToString());
		}
		else if (type == 8)
		{
			//節點圖示: 逆時針選轉節點節點 向下
			obj.transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*3,(1/(Mathf.Sqrt(picNumber)))*4);
			//print("逆時針選轉節點節點 向下 i="+ i.ToString() +", j=" + j.ToString());
		}
		
		else if (type == 24)
		{
			//節點圖示: 起始節點
			obj.transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*4,(1/(Mathf.Sqrt(picNumber)))*1);
			//print("起始節點 i="+ i.ToString() +", j=" + j.ToString());
		}
		
		else if (type == 28)
		{
			//節點圖示: 起點節點
			obj.transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*5,(1/(Mathf.Sqrt(picNumber)))*5);
			//print("起點節點 i="+ i.ToString() +", j=" + j.ToString());
		}
		
		//起始終點節點類		
		if(j==0)
		{
			//節點圖示: 終點節點
			obj.transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*4,(1/(Mathf.Sqrt(picNumber)))*1);
			//print("終點節點 i="+ i.ToString() +", j=" + j.ToString());
		}
		if(j==layer-1)
		{
			//節點圖示: 起始節點
			obj.transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*4,(1/(Mathf.Sqrt(picNumber)))*0);
			//print("起始節點 i="+ i.ToString() +", j=" + j.ToString());
		}
	}
	
	//起始亂數產生節點與其屬性設定
	void autoCreatQuessionNodeObjAndSetup() 
	{
		for(int i = 1; i<node-1; i++)
		{
			for(int j = 0; j<layer; j++)
			{
				//亂數決定節點類型(1~3)(排除產屬性為4的上向下節點)
				int rnd = Random.Range(1,4);
				QuestionNodeType[i,j] = rnd;
				
				//產生節點物件並給予名稱與座標ID
				QuestionNodeObj_prefab[i,j] = GameObject.Instantiate(nodeObj,new Vector3(0,1.5f+j+(j*0.2f),0),Quaternion.identity) as GameObject;
				QuestionNodeObj_prefab[i,j].name = QuestionNodeObj_prefab[i,j].name + "Question i=" + i.ToString() + ",j=" + j.ToString()+", Tyep=" + rnd.ToString();
				NodeObj_prefab[i,j] = QuestionNodeObj_prefab[i,j];
				//將節點依照屬性改變其貼圖圖示
				changTexture(j,rnd,QuestionNodeObj_prefab[i,j]);
				//旋轉物件
				NodeObj_prefab[i,j].transform.RotateAround(Vector3.zero, Vector3.forward,360/(node-2)*i);
			}
		}
		
		//設定所有節點開關起始屬性為關閉
		turnOffAllNodeSwish(QuestionNodeSwish);
		
		//亂數設定ID用
		int rndIDi,rndIDj;
		
		/*
		//只產生一個旋轉節點
		rndIDi = Random.Range(1,node-1);
		rndIDj = Random.Range(1,layer-1);
		int rndTurnNodeType = Random.Range(5,9);//亂數產生(5~8)
		QuestionNodeType[rndIDi,rndIDj] = rndTurnNodeType;
		changTexture(rndIDj,rndTurnNodeType,QuestionNodeObj_prefab[rndIDi,rndIDj]);
		print("亂數節點IDij="+rndIDi.ToString()+rndIDj.ToString()+"Type="+rndTurnNodeType.ToString());
		*/
		
		//只產生一個向下節點
		rndIDi = Random.Range(1,node-1);
		QuestionNodeType[rndIDi,layer-2]=4;
		changTexture(layer-2,4,QuestionNodeObj_prefab[rndIDi,layer-2]);
		QuestionNodeSwish[rndIDi,layer-2]=true;
		QuestionNodeObj_prefab[rndIDi,layer-2].renderer.material.color = new Color(1,1,1,0.5f);
		print("起始向下節點  ij="+rndIDi.ToString()+(layer-2).ToString()+", type="+QuestionNodeSwish[rndIDi,layer-2].ToString());
		
		//防止各層沒有向下節點,每層都產生一個旋轉節點
		for(int j = 1;j<layer-2;j++)
		{
			int rndTurnNodeType = Random.Range(5,9);//亂數產生(5~8)
			rndIDi = Random.Range(1,node-1);
			QuestionNodeType[rndIDi,j]=rndTurnNodeType;
			changTexture(j,rndTurnNodeType,QuestionNodeObj_prefab[rndIDi,j]);
		}
		
		//設定亮度
		for(int j=0;j<layer;j=j+layer-1)
		{
			for(int i=1;i<node-1;i++)
			{
				QuestionNodeObj_prefab[i,j].renderer.material.color = new Color(1,1,1,1);
			}
		}
	}
	
	//滑鼠點選節點事件 找出移動方式
	void mouseClickNodeEven_FindDisplacementTypeAndLayer(out int DisplacementType,out int DisplacementLayer, out bool isDisplacement)
	{

		isDisplacement = false;
		DisplacementType=10;
		DisplacementLayer=0;
		//如果有滑鼠點選到節點物件則找出是哪個物件與編號
		if(clickNodeSwish && !GuiShowInformation.enabled && !playerSlovingQuession)
		{
			for(int j = 1;j<layer-1;j++)
			{
				for(int i = 1;i<node-1;i++)
				{
					if(QuestionNodeObj_prefab[i,j].renderer.material.color.b == 0.9f)
					{		
						//還原之前點過的透明度
						if(clickTwice)
						{
							QuestionNodeObj_prefab[clickNodeA_IDi,clickNodeA_IDj].renderer.material.color = new Color(1,1,1,QuestionNodeObj_prefab[clickNodeB_IDi,clickNodeB_IDj].renderer.material.color.a);
							QuestionNodeObj_prefab[clickNodeB_IDi,clickNodeB_IDj].renderer.material.color = new Color(1,1,1,QuestionNodeObj_prefab[clickNodeB_IDi,clickNodeB_IDj].renderer.material.color.a);
							clickTwice = false;
							isDisplacement = false;
							#region 縮放凸顯被點選的點
							QuestionNodeObj_prefab[clickNodeA_IDi,clickNodeA_IDj].transform.localScale = new Vector3(1,1,1);
							#endregion 縮放凸顯被點選的點
						}
						
						//紀錄被點選的ID
						if(!getFirstID)
						{
							QuestionNodeObj_prefab[i,j].renderer.material.color = new Color(1,1,1,QuestionNodeObj_prefab[i,j].renderer.material.color.a);
							clickNodeA_IDi = i;
							clickNodeA_IDj = j;
							getFirstID=true;
							//在點選處顯示燈光
							clickNodeLight.transform.position = QuestionNodeObj_prefab[i,j].transform.position;
							clickNodeLight.light.enabled = true;
							#region 縮放凸顯被點選的點
							QuestionNodeObj_prefab[i,j].transform.localScale = new Vector3(1.5f,1.5f,1.5f);
							#endregion 縮放凸顯被點選的點
						}
						else if(getFirstID)
						{
							QuestionNodeObj_prefab[i,j].renderer.material.color = new Color(1,1,1,QuestionNodeObj_prefab[i,j].renderer.material.color.a);
							clickNodeB_IDi = i;
							clickNodeB_IDj = j;
							getFirstID=false;
							clickNodeLight.light.enabled = false;
							//如果在同一階層
							if(clickNodeA_IDj == clickNodeB_IDj && clickNodeA_IDi != clickNodeB_IDi)
							{
								print("兩次點選 在同一階層");
								//判定位移方向
								if(clickNodeA_IDi-1==clickNodeB_IDi || (clickNodeA_IDi==1 && clickNodeB_IDi ==node-2))
								{
									DisplacementType = 1;
									DisplacementLayer = j;
									isDisplacement = true;//回傳已經被移動 要做檢查
									print("要往左位移, 數值為A-B "+clickNodeA_IDi.ToString()+"-"+clickNodeB_IDi.ToString()+"="+(clickNodeA_IDi-clickNodeB_IDi).ToString());
									movieCount++;//更新移動次數
									print("移動次數="+movieCount.ToString());
								}
								else if(clickNodeA_IDi+1==clickNodeB_IDi || (clickNodeA_IDi==node-2 && clickNodeB_IDi ==1))
								{
									DisplacementType = 0;
									DisplacementLayer = j;
									isDisplacement = true;//回傳已經被移動 要做檢查
									print("要往右位移, 數值為A-B "+clickNodeA_IDi.ToString()+"-"+clickNodeB_IDi.ToString()+"="+(clickNodeA_IDi-clickNodeB_IDi).ToString());
									movieCount++;//更新移動次數
									print("移動次數="+movieCount.ToString());
								}
								else
								{
									QuestionNodeObj_prefab[clickNodeA_IDi,clickNodeA_IDj].renderer.material.color = new Color(1,1,1,QuestionNodeObj_prefab[clickNodeB_IDi,clickNodeB_IDj].renderer.material.color.a);
									QuestionNodeObj_prefab[clickNodeB_IDi,clickNodeB_IDj].renderer.material.color = new Color(1,1,1,QuestionNodeObj_prefab[clickNodeB_IDi,clickNodeB_IDj].renderer.material.color.a);
									print("兩次點選 在同一階層 但 不是隔壁的節點");
								}
							}
							else 
							{
								QuestionNodeObj_prefab[clickNodeA_IDi,clickNodeA_IDj].renderer.material.color = new Color(1,1,1,QuestionNodeObj_prefab[clickNodeB_IDi,clickNodeB_IDj].renderer.material.color.a);
								QuestionNodeObj_prefab[clickNodeB_IDi,clickNodeB_IDj].renderer.material.color = new Color(1,1,1,QuestionNodeObj_prefab[clickNodeB_IDi,clickNodeB_IDj].renderer.material.color.a);
								print("兩次點選 不在同一階層 或 重複節點");
							}
							clickTwice = true;
							#region 縮放凸顯被點選的點
							QuestionNodeObj_prefab[clickNodeA_IDi,clickNodeA_IDj].transform.localScale = new Vector3(1,1,1);
							#endregion 縮放凸顯被點選的點
						}
					}
				}
			}
			clickNodeSwish = false;
		}
		else
		{
			clickNodeSwish = false;
		}
	}
	
	//位移指定J層整排(或列)節點屬性 (指定方向(0向右,1向左), 指定層, 新陣列, 舊陣列, 新的物件陣列)
	void DisplacementOneLayerTypeAndCreatNewNode(int displacementDirection,int layerID, int[,] targetType, out int[,] newType)
	{
		//給予起始屬性
		newType = new int[node,layer];

		//往右位移
		if(displacementDirection==0)
		{
			//將資料記錄到新的陣列
			for(int i = 1;i<node-1;i++)
			{
				//print("開始往右位移存到新陣列");
				//將原始的屬性值記錄到新的陣列右邊那格
				newType[i+1,layerID] = targetType[i,layerID];
				//狀況如下
				//012345 	<-newType
				//  ↑↑↑↑
				// 012345 	<-targetType
			}
			//將紀新的陣列第一格紀錄為原始最右邊那格屬性
			newType[1,layerID] = targetType[node-2,layerID];
			//狀況如下
			//012345 	<-newType
			// ↑ 
			// 4 		<-targetType
		}
		
		//往左位移
		else if(displacementDirection==1)
		{
			//將資料記錄到新的陣列
			for(int i = 1;i<node-1;i++)
			{
				//print("開始往左位移存到新陣列");
				//將原始的屬性值記錄到新的陣列左邊那格
				newType[i-1,layerID] = targetType[i,layerID];
				//狀況如下
				// 012345 	<-newType
				// ↑↑↑↑
				//012345 	<-targetType
			}
			//將紀新的陣列第一格紀錄為原始最左邊那格屬性
			newType[node-2,layerID] = targetType[1,layerID];
			//狀況如下
			//012345 	<-newType
			//    ↑ 
			//    1 	<-targetType
		}
		
		//除了位移階層其餘給予原始屬性
		for(int i = 1;i<node-1;i++)
		{
			for(int j = 1;j<layer-1;j++)
			{
				if(j!=layerID)
				{
					newType[i,j] = targetType[i,j];
				}
			}
		}
	}
	
	//將所有節點開關設為關閉(要關閉的陣列)
	void turnOffAllNodeSwish(bool[,] swish) 
	{
		for(int i=0;i<node;i++)
		{
			for(int j = 0;j<layer;j++)
			{
			swish[i,j] = false;
			}
		}
	}
	
	//檢查題目起始點最終會停到哪，是否完成解題(被檢查的數值陣列, 被檢查的屬性陣列, 用來更新透明值的目標物件陣列, 回傳解題狀態)
	void checkQuestion(int[,] targetType, bool[,] targetSwish, out bool targetSlovingQuestionSwish)
	{
		//檢查ID用
		int firstIDi, firstIDj; //找起始ID用
		int checkIDi,checkIDj; //紀錄要被檢查的ID 
		int nextCheckIDi,nextCheckIDj; //紀錄下一個要被檢查的ID
		
		//檢查次數
		int checkCount=0;

		
		//判定檢查狀態
		bool checkRouteStatus=false; //路線是否不能前進的狀態
		targetSlovingQuestionSwish=false; //題目是否完成狀態
		
		//尋找起始ID
		findStartNodeType(targetType,targetSwish,out firstIDi,out firstIDj);
		checkIDi = firstIDi;
		checkIDj = firstIDj;
		
		//如果路線還未不能前進 與 檢查次數不超過99次 時則不斷執行 
		while (!checkRouteStatus && checkCount<99)
		{
			checkCount++;//顯示已經執行幾次用
			
			print("第"+checkCount.ToString()+"次執行checkQuestion，狀態為"+checkRouteStatus.ToString()+ ", 檢查ID=" + checkIDi.ToString() +", "+ checkIDj.ToString());
			//依照目前所在節點屬性執行動作
			checkNodeTypeAndFunction(targetType,targetSwish,checkIDi,checkIDj, out checkRouteStatus, out targetSlovingQuestionSwish,  out nextCheckIDi, out nextCheckIDj);
			//print("checkIDij= "+checkIDi.ToString()+checkIDj.ToString()+", nextIDij= "+nextCheckIDi.ToString()+nextCheckIDj.ToString());
			checkIDi = nextCheckIDi;
			checkIDj = nextCheckIDj;
		}		
	}
	
	//尋找節點陣列的起始節點(要尋找的目標數值陣列, 回傳起始ID)
	void findStartNodeType(int[,] targetType, bool[,] targetSwish, out int firstIDi, out int firstIDj)
	{
		//起始設定數值用，數值本身不需要用到
		firstIDi = 0;
		firstIDj = 0;
		//是否有找到起始值
		bool findFirst=false;
		
		for(int i = 1;i<node-1;i++)
		{
			//print("尋找起點位移陣列i="+i.ToString()+"屬性="+DisplacementNodeType[i,layer-2].ToString());
			if(targetType[i,layer-2]==4)
			{
				firstIDi = i;
				firstIDj = layer-2;
				print("起始點ID ij=" + i.ToString()+",4") ;
				
				targetSwish[i,layer-2]=true;
				findFirst = true;
			}
		}
		//如果沒有找到起始點
		if(!findFirst)
		{
			print("!!error!!findStartNodeType找不到起始節點");
		}
	}
	
	//檢查節點功能並移動到下個節點(要檢查的陣列數值, 屬性, 目標IDi, j, 回傳路線檢查結果屬性, 回傳題目檢查結果屬性, 更新下個要被檢查的IDi,j)
	void checkNodeTypeAndFunction(int[,]tragetType, bool[,]tragetSwish, int checkIDi, int checkIDj, out bool checkRouteStatus, out bool targetSlovingQuestionSwish, out int nextCheckIDi, out int nextCheckIDj)
	{
		//用作回傳下個要被檢查的ID用,其數值沒有意義
		nextCheckIDi =0;
		nextCheckIDj=0;
		//回傳檢查結果用,其屬性沒有意義
		checkRouteStatus = false;
		targetSlovingQuestionSwish = false;
			
		if( tragetType[checkIDi,checkIDj]==0)
		{
			print("改之前checkRouteStatus="+checkRouteStatus.ToString());
			checkRouteStatus = true;
			print("改之後checkRouteStatus="+checkRouteStatus.ToString());
			print("跑進黑洞i="+checkIDi.ToString()+", j="+checkIDj.ToString());
		}
		
		//如果是往左節點
		else if(tragetType[checkIDi,checkIDj]==1)
		{
			//如果往左沒超過邊界
			if(checkIDi>1)
			{
				//檢查左邊的節點
				
				//無法前進時
				if(tragetType[checkIDi-1,checkIDj]==3)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print(checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 無法往左前進到i="+(checkIDi-1).ToString()+", j="+checkIDj.ToString()+", Type="+tragetType[checkIDi-1,checkIDj].ToString());
				}
				else if(tragetType[checkIDi-1,checkIDj] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 會跑進黑洞i="+(checkIDi-1).ToString()+", j="+checkIDj.ToString()+", Type="+tragetType[checkIDi-1,checkIDj].ToString());
				}
				
				//可以前進時
				else
				{
					//將檢查節點往左移動，更新下個檢查ID
					nextCheckIDi = checkIDi-1;
					nextCheckIDj = checkIDj;
					tragetSwish[nextCheckIDi,nextCheckIDj] = true;
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 往左移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
					/* 
					//如果下個此節點沒有重複進入過
					if(!tragetSwish[checkIDi-1,checkIDj])
					{
					//將檢查節點往左移動，更新下個檢查ID
					nextCheckIDi = checkIDi-1;
					nextCheckIDj = checkIDj;
					tragetSwish[nextCheckIDi,nextCheckIDj] = true;
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 往左移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
					}
					else{
						checkRouteStatus = true;
						print ("重複迴圈");
					}
					*/
				}
			}
			//如果往左超過邊界
			else if(checkIDi == 1)
			{
				//檢查左邊的節點
				//無法前進時
				if(tragetType[node-2,checkIDj] == 3)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 無法往左前進到i="+(node-2).ToString()+", j="+checkIDj.ToString()+", Type="+tragetType[node-2,checkIDj].ToString());
				}
				else if(tragetType[node-2,checkIDj] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 會跑進黑洞i="+(node-2).ToString()+", j="+checkIDj.ToString());
				}
				
				//可以前進時
				else
				{

					//將檢查節點往左移動，更新下個檢查ID
					nextCheckIDi=node-2;
					nextCheckIDj=checkIDj;
					tragetSwish[nextCheckIDi,nextCheckIDj] = true;
					print("目前在i="+checkIDi.ToString()+", j="+checkIDj.ToString() + ", 往左移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
					/*
					//如果下個此節點沒有重複進入過
					if(!tragetSwish[node-2,checkIDj])
					{
						//將檢查節點往左移動，更新下個檢查ID
						nextCheckIDi=node-2;
						nextCheckIDj=checkIDj;
						tragetSwish[nextCheckIDi,nextCheckIDj] = true;
						print("目前在i="+checkIDi.ToString()+", j="+checkIDj.ToString() + ", 往左移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
					}
					else{
						checkRouteStatus = true;
						print ("重複迴圈,目前在i=1, j="+checkIDj.ToString());
					}
					*/
				}
			}
		}
		
		//如果是往右節點
		else if(tragetType[checkIDi,checkIDj]==3)
		{ 
			//如果往右沒超過邊界
			if(checkIDi < node-2)
			{
				//檢查右邊的節點
				//無法前進時
				if(tragetType[checkIDi+1,checkIDj] == 1)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 無法往右前進到i="+(checkIDi+1).ToString()+", j="+checkIDj.ToString()+", Type="+tragetType[checkIDi+1,checkIDj].ToString());
				}
				else if(tragetType[checkIDi+1,checkIDj] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 會跑進黑洞i="+(checkIDi+1).ToString()+", j="+checkIDj.ToString());
				}
				
				//可以前進時
				else
				{
					//將檢查節點往右移動，更新下個檢查ID
					nextCheckIDi=checkIDi+1;
					nextCheckIDj=checkIDj;
					tragetSwish[nextCheckIDi,nextCheckIDj] = true;
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 往右移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
					/*
					//如果下個此節點沒有重複進入過
					if(!tragetSwish[checkIDi+1,checkIDj])
					{
					//將檢查節點往右移動，更新下個檢查ID
					nextCheckIDi=checkIDi+1;
					nextCheckIDj=checkIDj;
					tragetSwish[nextCheckIDi,nextCheckIDj] = true;
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 往右移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
					}
					else{
						checkRouteStatus = true;
						print ("重複迴圈");
					}
					*/
				}
	
			}
			//如果往右超過邊界
			else if(checkIDi == node-2)
			{
				//檢查右邊的節點
				//無法前進時
				if(tragetType[1,checkIDj] == 1)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 無法往右前進到i=1, j="+checkIDj.ToString()+", Type="+tragetType[1,checkIDj].ToString());
				}
				else if(tragetType[1,checkIDj] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 會跑進黑洞i=1, j="+checkIDj.ToString());
				}
				
				//可以前進時
				else
				{
					//將檢查節點往右移動，更新下個檢查ID
					nextCheckIDi=1;
					nextCheckIDj=checkIDj;
					tragetSwish[nextCheckIDi,nextCheckIDj] = true;
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 往右移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
					/*
					//如果下個此節點沒有重複進入過
					if(!tragetSwish[1,checkIDj])
					{
						//將檢查節點往右移動，更新下個檢查ID
						nextCheckIDi=1;
						nextCheckIDj=checkIDj;
						tragetSwish[nextCheckIDi,nextCheckIDi] = true;
						print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 往右移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
					}
					else{
						checkRouteStatus = true;
						print ("重複迴圈");
					}
					*/
				}
			}
		}
		
		//如果是往上節點
		else if(tragetType[checkIDi,checkIDj]==2)
		{
			//如果往上沒超過邊界
			if(checkIDj < layer-2)
			{
				//檢查上邊的節點
				
				//無法前進時
				if(tragetType[checkIDi,checkIDj+1] == 4)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 無法往上前進到i="+checkIDi.ToString()+", j="+(checkIDj+1).ToString()+", Type="+tragetType[checkIDi,checkIDj+1].ToString());
				}
				else if(tragetType[checkIDi,checkIDj+1] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 會跑進黑洞i="+checkIDi.ToString()+", j="+(checkIDj+1).ToString());
				}
				
				//可以前進時
				else
				{
					//將檢查節點往上移動，更新下個檢查ID
					nextCheckIDi=checkIDi;
					nextCheckIDj=checkIDj+1;
					tragetSwish[nextCheckIDi,nextCheckIDj] = true;
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 往上移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
					/*
					//如果下個此節點沒有重複進入過
					if(!tragetSwish[checkIDi,checkIDj+1])
					{
						//將檢查節點往上移動，更新下個檢查ID
						nextCheckIDi=checkIDi;
						nextCheckIDj=checkIDj+1;
						tragetSwish[nextCheckIDi,nextCheckIDj] = true;
						print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 往上移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
					}
					else{
						checkRouteStatus = true;
						print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 往上節點是i="+checkIDi.ToString()+", j="+(checkIDj+1).ToString()+", Type="+tragetSwish[checkIDi,checkIDj+1].ToString());
						print ("重複迴圈");
					}
					*/
				}
			}
			
			//如果往上超過邊界
			else if(checkIDj == layer-2)
			{
				print("改之前checkRouteStatus="+checkRouteStatus.ToString());
				checkRouteStatus = true;
				print("改之後checkRouteStatus="+checkRouteStatus.ToString());
				print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 會超過邊界到i="+checkIDi.ToString()+", j="+(checkIDj+1).ToString());
			}
		}
		
		//如果是往下節點
		else if(tragetType[checkIDi,checkIDj]==4)
		{
			//如果往下沒超過邊界
			if(checkIDj > 1)
			{
				//檢查下邊的節點
				
				//無法前進時
				if(tragetType[checkIDi,checkIDj-1]== 2)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 無法往下前進到i="+checkIDi.ToString()+", j="+(checkIDj-1).ToString()+", Type="+tragetType[checkIDi,checkIDj-1].ToString());
				}
				else if(tragetType[checkIDi,checkIDj-1] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 會跑進黑洞i="+checkIDi.ToString()+", j="+(checkIDj-1).ToString());
				}
				
				//可以前進時
				else
				{
					//將檢查節點往下移動，更新下個檢查ID
					nextCheckIDi=checkIDi;
					nextCheckIDj=checkIDj-1;
					tragetSwish[nextCheckIDi,nextCheckIDj] = true;
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 往下移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
					/*
					//如果下個此節點沒有重複進入過
					if(!tragetSwish[checkIDi,checkIDj-1])
					{
						//將檢查節點往下移動，更新下個檢查ID
						nextCheckIDi=checkIDi;
						nextCheckIDj=checkIDj-1;
						tragetSwish[nextCheckIDi,nextCheckIDj] = true;
						print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 往下移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
					}
					else{
						checkRouteStatus = true;
						print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 下個節點是i="+checkIDi.ToString()+", j="+(checkIDj-1).ToString()+", Type="+tragetSwish[checkIDi,checkIDj-1].ToString());
						print ("重複迴圈");
					}
					*/
				}
			}
			
			//如果往下超過邊界
			else if(checkIDj == 1)
			{
				print("改之前checkRouteStatus="+checkRouteStatus.ToString());
				checkRouteStatus = true;
				print("改之後checkRouteStatus="+checkRouteStatus.ToString());
				targetSlovingQuestionSwish = true;//autoSlovingQuestion用來判定題目已經解完
				print("Question Success!! 目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 下個節點是i="+checkIDi.ToString()+", j="+(checkIDj-1).ToString());
			}
		}
		
		//如果是往左逆時針旋轉節點
		else if(tragetType[checkIDi,checkIDj]==5)
		{
			//如果往下沒超過邊界
			if(checkIDj>1)
			{
				//檢查下邊的節點
				
				//無法前進時
				/*
				if(tragetType[checkIDi,checkIDj-1]==1)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print(checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 逆時針旋轉方向後無法往下前進到i="+(checkIDi-1).ToString()+", j="+checkIDj.ToString()+", Type="+tragetType[checkIDi-1,checkIDj].ToString());
				}
				else */if(tragetType[checkIDi,checkIDj-1] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 會跑進黑洞i="+(checkIDi-1).ToString()+", j="+checkIDj.ToString()+", Type="+tragetType[checkIDi-1,checkIDj].ToString());
				}
				
				//可以前進時
				else
				{
					//將檢查節點往下移動，更新下個檢查ID
					nextCheckIDi = checkIDi;
					nextCheckIDj = checkIDj-1;
					tragetSwish[nextCheckIDi,nextCheckIDj] = true;
					tragetType[checkIDi,checkIDj]=8; //目前先不更新圖是，而是在當檢查題目狀態功能執行完後會再次更新節點圖示
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 逆時針旋轉方向後往下移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
				}
			}
			//如果往下超過邊界
			else if(checkIDj == 1)
			{ 
				//無法前進時
				print("改之前checkRouteStatus="+checkRouteStatus.ToString());
				checkRouteStatus = true;
				print("改之後checkRouteStatus="+checkRouteStatus.ToString());
				targetSlovingQuestionSwish = true;//autoSlovingQuestion用來判定題目已經解完
				print("Question Success!! 目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 下個節點是i="+checkIDi.ToString()+", j="+(checkIDj-1).ToString());
			}
		}
		
		//如果是往上逆時針旋轉節點
		else if(tragetType[checkIDi,checkIDj]==6)
		{
			//如果往左沒超過邊界
			if(checkIDi>1)
			{
				//檢查左邊的節點
				
				//無法前進時
				/*
				if(tragetType[checkIDi-1,checkIDj]==3)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print(checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 逆時針旋轉方向後無法往左前進到i="+(checkIDi-1).ToString()+", j="+checkIDj.ToString()+", Type="+tragetType[checkIDi-1,checkIDj].ToString());
				}
				else */if(tragetType[checkIDi-1,checkIDj] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 會跑進黑洞i="+(checkIDi-1).ToString()+", j="+checkIDj.ToString()+", Type="+tragetType[checkIDi-1,checkIDj].ToString());
				}
				
				//可以前進時
				else
				{
					//將檢查節點往左移動，更新下個檢查ID
					nextCheckIDi = checkIDi-1;
					nextCheckIDj = checkIDj;
					tragetSwish[nextCheckIDi,nextCheckIDj] = true;
					tragetType[checkIDi,checkIDj]=5; //目前先不更新圖是，而是在當檢查題目狀態功能執行完後會再次更新節點圖示
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 逆時針旋轉方向後往左移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
				}
			}
			//如果往左超過邊界
			else if(checkIDi == 1)
			{ 
				//檢查左邊的節點
				//無法前進時
				/*
				if(tragetType[node-2,checkIDj] == 3)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 逆時針旋轉方向後無法往左前進到i="+(node-2).ToString()+", j="+checkIDj.ToString()+", Type="+tragetType[node-2,checkIDj].ToString());
				}
				else */if(tragetType[node-2,checkIDj] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 會跑進黑洞i="+(node-2).ToString()+", j="+checkIDj.ToString());
				}
				
				//可以前進時
				else
				{

					//將檢查節點往左移動，更新下個檢查ID
					nextCheckIDi=node-2;
					nextCheckIDj=checkIDj;
					tragetSwish[nextCheckIDi,nextCheckIDj] = true;
					tragetType[checkIDi,checkIDj]=5; //目前先不更新圖是，而是在當檢查題目狀態功能執行完後會再次更新節點圖示
					print("目前在i="+checkIDi.ToString()+", j="+checkIDj.ToString() + ", 往左移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
				}
			}
		}
		
		//如果是往右逆時針旋轉節點
		else if(tragetType[checkIDi,checkIDj]==7)
		{
			//如果往上沒超過邊界
			if(checkIDj<layer-2)
			{
				//檢查上邊的節點
				
				//無法前進時
				/*
				if(tragetType[checkIDi,checkIDj+1]==4)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print(checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 逆時針旋轉方向後無法往上前進到i="+checkIDi.ToString()+", j="+(checkIDj+1).ToString()+", Type="+tragetType[checkIDi,checkIDj+1].ToString());
				}
				else */if(tragetType[checkIDi,checkIDj-1] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 會跑進黑洞i="+checkIDi.ToString()+", j="+(checkIDj+1).ToString()+", Type="+tragetType[checkIDi-1,checkIDj+1].ToString());
				}
				
				//可以前進時
				else
				{
					//將檢查節點往上移動，更新下個檢查ID
					nextCheckIDi = checkIDi;
					nextCheckIDj = checkIDj+1;
					tragetSwish[nextCheckIDi,nextCheckIDj] = true;
					tragetType[checkIDi,checkIDj]=6; //目前先不更新圖是，而是在當檢查題目狀態功能執行完後會再次更新節點圖示
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 逆時針旋轉方向後往上移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
				}
			}
			//如果往上超過邊界
			else if(checkIDj == layer-2)
			{ 
				print("改之前checkRouteStatus="+checkRouteStatus.ToString());
				checkRouteStatus = true;
				print("改之後checkRouteStatus="+checkRouteStatus.ToString());
				print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 會超過邊界到i="+checkIDi.ToString()+", j="+(checkIDj+1).ToString());
			}
		}
		
		//如果是往下逆時針旋轉節點
		else if(tragetType[checkIDi,checkIDj]==8)
		{
			//如果往右沒超過邊界
			if(checkIDi<node-2)
			{
				//檢查右邊的節點
				
				//無法前進時
				/*
				if(tragetType[checkIDi+1,checkIDj]==1)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print(checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 逆時針旋轉方向後無法往右前進到i="+(checkIDi+1).ToString()+", j="+checkIDj.ToString()+", Type="+tragetType[checkIDi+1,checkIDj].ToString());
				}
				else */if(tragetType[checkIDi+1,checkIDj] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 會跑進黑洞i="+(checkIDi+1).ToString()+", j="+checkIDj.ToString()+", Type="+tragetType[checkIDi+1,checkIDj].ToString());
				}
				
				//可以前進時
				else
				{
					//將檢查節點往左移動，更新下個檢查ID
					nextCheckIDi = checkIDi+1;
					nextCheckIDj = checkIDj;
					tragetSwish[nextCheckIDi,nextCheckIDj] = true;
					tragetType[checkIDi,checkIDj]=7; //目前先不更新圖是，而是在當檢查題目狀態功能執行完後會再次更新節點圖示
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 逆時針旋轉方向後往左移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
				}
			}
			//如果往右超過邊界
			else if(checkIDi == node-2)
			{ 
				//檢查右邊的節點
				//無法前進時
				/*
				if(tragetType[1,checkIDj] == 1)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 逆時針旋轉方向後無法往右前進到i=1, j="+checkIDj.ToString()+", Type="+tragetType[1,checkIDj].ToString());
				}
				else */if(tragetType[1,checkIDj] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString() + ", 會跑進黑洞i=1, j="+checkIDj.ToString());
				}
				
				//可以前進時
				else
				{

					//將檢查節點往左移動，更新下個檢查ID
					nextCheckIDi=1;
					nextCheckIDj=checkIDj;
					tragetSwish[nextCheckIDi,nextCheckIDj] = true;
					tragetType[checkIDi,checkIDj]=7; //目前先不更新圖是，而是在當檢查題目狀態功能執行完後會再次更新節點圖示
					print("目前在i="+checkIDi.ToString()+", j="+checkIDj.ToString() + ", 往左移動節點到i="+nextCheckIDi.ToString()+", j="+nextCheckIDj.ToString()+", Type="+tragetType[nextCheckIDi,nextCheckIDj].ToString());
				}
			}
		}
		
		else{
			checkRouteStatus = true;
			print("!! 注意 !!檢查強制結束gotoNextNodeForCheckDisplacementNodeType");
			print("!! 注意 !!位移後的進入節點類型為"+tragetType[checkIDi,checkIDj].ToString());
			print("目前在i=" +checkIDi.ToString()+", j="+checkIDj.ToString());
		}
	}
	
	//檢查所有節點開關屬性改變其透明度(要被修改的目標物件陣列, 要檢查的目標屬性陣列)
	void checkAllNodeSwish(GameObject[,] targetNodeObj, bool[,] targetSwish)
	{
		for(int i = 1; i<node-1; i++)
		{
			for(int j = 1; j<layer-1; j++)
			{
				if(targetSwish[i,j])
				{
					targetNodeObj[i,j].renderer.material.color = new Color(1,1,1,1);
				}
				else{
					targetNodeObj[i,j].renderer.material.color = new Color(1,1,1,0.5f);
				}
			}
		}
	}
	
	//亂數產生題目
	void rndQuession(int[,] targetType, bool[,] targetSwish, out int[,] newType,  out bool[,] newSwish)
	{
		newType = new int[node,layer];
		newSwish = new bool[node,layer];
		newType = targetType;
		newSwish = targetSwish;
		
		Vector3 pos= new Vector3(0,0,0) ;//指定物件位置用
		for(int i = 1; i<node-1; i++)
		{
			for(int j = 0; j<layer; j++)
			{
				//排列擺放節點物件
				
				//判斷節點數是否是偶數
				if(node%2==0){
					pos.x=node-i-(node/2)-0.5f;
				}
				else{
					pos.x=node-i-(node/2)-1;
				}
				//判斷階層數是否是偶數
				if(layer%2==0){
					pos.y=j-(layer/2)+0.5f;
				}
				else{
					pos.y=j-(layer/2);
				}
				pos.z=1;
				
				//亂數決定節點類型
				//int rnd = Random.Range(0,typeArray.Length);
				//int rndType = typeArray[rnd];
				//QuestionNodeType[i,j] = rndType;
				int rndType = Random.Range(1,4);//(1~3)(排除產屬性為4的上向下節點)
				targetType[i,j] = rndType;
			}
		}
		
		//設定所有節點開關起始屬性為關閉
		turnOffAllNodeSwish(targetSwish);
		
		//亂數設定ID用
		int rndIDi,rndIDj;
		
		/*
		//只產生一個旋轉節點
		rndIDi = Random.Range(1,node-1);
		rndIDj = Random.Range(1,layer-1);
		int rndTurnNodeType = Random.Range(5,9);//亂數產生(5~8)
		newType[rndIDi,rndIDj] = rndTurnNodeType;
		//changTexture(rndIDj,targetType,QuestionNodeObj_prefab[rndIDi,rndIDj]);
		print("亂數節點IDij="+rndIDi.ToString()+rndIDj.ToString());
		*/
		
		//只產生一個向下節點
		rndIDi = Random.Range(1,node-1);
		newType[rndIDi,layer-2]=4;
		changTexture(layer-2,4,QuestionNodeObj_prefab[rndIDi,layer-2]);
		newSwish[rndIDi,layer-2]=true;
		QuestionNodeObj_prefab[rndIDi,layer-2].renderer.material.color = new Color(1,1,1,0.5f);
		print("起始向下節點  ij="+rndIDi.ToString()+(layer-2).ToString()+", type="+newSwish[rndIDi,layer-2].ToString());
		
		//防止各層沒有向下節點,每層都產生一個旋轉節點
		for(int j = 1;j<layer-2;j++)
		{
			int rndTurnNodeType = Random.Range(5,9);//亂數產生(5~8)
			rndIDi = Random.Range(1,node-1);
			newType[rndIDi,j]=rndTurnNodeType;
			changTexture(j,rndTurnNodeType,QuestionNodeObj_prefab[rndIDi,j]);
		}
	}
}
