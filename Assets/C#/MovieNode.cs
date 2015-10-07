using UnityEngine;
using System.Collections;

public class MovieNode : MonoBehaviour {
	public GameObject nodeObj;
	public int node = 4, layer = 4 ; //節點數量,階層數 如果系統未給予指定數量時起始為4,4
	public float picNumber = 36; //圖示的張數
	int[,] QuestionNodeType; //設定題目節點的圖示屬性
	bool[,] QuestionNodeSwish; //紀錄目前進行到哪個節點
	GameObject[,] QuestionNodeObj_prefab; //設定題目各節點物件用
	GameObject[,] NodeObj_prefab; //設定各節點物件用	
	
	int[,] playerSlovingNodeType;//紀錄玩家解題狀態

	
	//移動節點用
	static public bool clickNodeSwish; //判斷是否有滑鼠底選到節點物件
	int clickNodeA_IDi,clickNodeA_IDj,clickNodeB_IDi,clickNodeB_IDj;
	bool getFirstID; //是否已經取得第一個id
	int movieCount; //移動次數
	bool clickTwice=false; //是否移動過
	
	// Use this for initialization
	void Start () {
		node = node+2;//預留位移空間用
		layer = layer +2;//頭尾排起始與終點顯示用
		QuestionNodeType = new int[node,layer];
		QuestionNodeSwish = new bool[node,layer];
		QuestionNodeObj_prefab = new GameObject [node,layer];
		NodeObj_prefab = new GameObject [node,layer];
		
		playerSlovingNodeType = new int[node,layer];

		
		
		//產生所有物件與設定與亂數題目屬性
		//autoCreatQuessionNodeObjAndSetup();
		
		//   上下選一作為起始設定
		
		//產生自訂題目物件與設定
		creatCustomizationsQuesionAndSet();
		
		//設定起始屬性
		playerSlovingNodeType = QuestionNodeType;
		for(int j2=0;j2<layer-2;j2++)
		{
			for(int i2=1;i2<node-1;i2++)
			{
				QuestionNodeObj_prefab[i2,j2].renderer.material.color = new Color(1,1,1,1);
			}
		}

	}
	
	// Update is called once per frame
	void Update () 
	{	
		//找出移動模式與移動階層
		int DisplacementType, DisplacementLayer;
		mouseClickNodeEven_FindDisplacementTypeAndLayer(out DisplacementType, out DisplacementLayer);
		
		//移動節點
		DisplacementOneLayerTypeAndCreatNewNode(DisplacementType,DisplacementLayer,playerSlovingNodeType,out playerSlovingNodeType);
		
		//更新節點圖示
		for(int i=1;i<node-1;i++)
		{
			changTexture(DisplacementLayer,playerSlovingNodeType[i,DisplacementLayer],QuestionNodeObj_prefab[i,DisplacementLayer]);
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
		QuestionNodeType[3,3]=1;
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
				QuestionNodeObj_prefab[i,j] = GameObject.Instantiate(nodeObj,new Vector3(0,2+j,0),Quaternion.identity) as GameObject;
				QuestionNodeObj_prefab[i,j].name = QuestionNodeObj_prefab[i,j].name + "Question i=" + i.ToString() + ",j=" + j.ToString()+", Tyep="+QuestionNodeType[i,j].ToString();
				NodeObj_prefab[i,j] = QuestionNodeObj_prefab[i,j];
				//將節點依照屬性改變其貼圖圖示
				changTexture(j,QuestionNodeType[i,j],QuestionNodeObj_prefab[i,j]);
				//旋轉物件
				NodeObj_prefab[i,j].transform.RotateAround(Vector3.zero, Vector3.forward,360/(node-2)*i);
				
			}
		}		
		QuestionNodeObj_prefab[intoIDi,intoIDj].renderer.material.color = new Color(1,1,1,0.5f);

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
		
				
		if(j==0 || j==layer-1)
		{
			//節點圖示: 禁止節點
			obj.transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*4,(1/(Mathf.Sqrt(picNumber)))*2);
			//print("禁止節點 i="+ i.ToString() +", j=" + j.ToString());
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
				QuestionNodeObj_prefab[i,j] = GameObject.Instantiate(nodeObj,new Vector3(0,2+j*2,0),Quaternion.identity) as GameObject;
				QuestionNodeObj_prefab[i,j].name = QuestionNodeObj_prefab[i,j].name + "Question i=" + i.ToString() + ",j=" + j.ToString()+", Tyep=" + rnd.ToString();
				NodeObj_prefab[i,j] = QuestionNodeObj_prefab[i,j];
				//將節點依照屬性改變其貼圖圖示
				changTexture(j,rnd,QuestionNodeObj_prefab[i,j]);
				//旋轉物件
				NodeObj_prefab[i,j].transform.RotateAround(Vector3.zero, Vector3.forward,360/(node-2)*i);
				
			}
		}
		
		//只產生一個向下節點
		int rnd2 = Random.Range(1,node-1);
		QuestionNodeType[rnd2,layer-2]=4;
		changTexture(layer-2,4,QuestionNodeObj_prefab[rnd2,layer-2]);
		QuestionNodeSwish[rnd2,layer-2]=true;
		QuestionNodeObj_prefab[rnd2,layer-2].renderer.material.color = new Color(1,1,1,0.5f);
		print("起始向下節點  ij="+rnd2.ToString()+(layer-2).ToString()+", type="+QuestionNodeSwish[rnd2,layer-2].ToString());
		
		//防止各層沒有向下節點,每層都產生一個
		for(int j = 1;j<layer-2;j++)
		{
			int rnd3 = Random.Range(1,node-1);
			QuestionNodeType[rnd3,j]=4;
			changTexture(j,4,QuestionNodeObj_prefab[rnd3,j]);
		}
	}
	
	//滑鼠點選節點事件 找出移動方式
	void mouseClickNodeEven_FindDisplacementTypeAndLayer(out int DisplacementType,out int DisplacementLayer)
	{
		DisplacementType=10;
		DisplacementLayer=0;
		//如果有滑鼠點選到節點物件則找出是哪個物件與編號
		if(clickNodeSwish)
		{
			//還原之前點過的透明度
			if(clickTwice)
			{
				QuestionNodeObj_prefab[clickNodeA_IDi,clickNodeA_IDj].renderer.material.color = new Color(1,1,1,1);
				QuestionNodeObj_prefab[clickNodeB_IDi,clickNodeB_IDj].renderer.material.color = new Color(1,1,1,1);
				clickTwice = false;
			}
			
			for(int j = 1;j<layer-1;j++)
			{
				for(int i = 1;i<node-1;i++)
				{
					if(QuestionNodeObj_prefab[i,j].renderer.material.color.a == 0.7f)
					{
						//紀錄被點選的ID
						if(!getFirstID)
						{
							QuestionNodeObj_prefab[i,j].renderer.material.color = new Color(1,1,1,0.2f);
							clickNodeA_IDi = i;
							clickNodeA_IDj = j;
							getFirstID=true;
						}
						else if(getFirstID)
						{
							QuestionNodeObj_prefab[i,j].renderer.material.color = new Color(1,1,1,0.5f);
							clickNodeB_IDi = i;
							clickNodeB_IDj = j;
							getFirstID=false;
							//如果在同一階層
							if(clickNodeA_IDj == clickNodeB_IDj && clickNodeA_IDi != clickNodeB_IDi)
							{
								print("兩次點選 在同一階層");
								//判定位移方向
								if(clickNodeA_IDi-1==clickNodeB_IDi || (clickNodeA_IDi==1 && clickNodeB_IDi ==node-2))
								{
									DisplacementType = 1;
									DisplacementLayer = j;
									print("要往左位移, 數值為A-B "+clickNodeA_IDi.ToString()+"-"+clickNodeB_IDi.ToString()+"="+(clickNodeA_IDi-clickNodeB_IDi).ToString());
								}
								else if(clickNodeA_IDi+1==clickNodeB_IDi || (clickNodeA_IDi==node-2 && clickNodeB_IDi ==1))
								{
									DisplacementType = 0;
									DisplacementLayer = j;
									print("要往右位移, 數值為A-B "+clickNodeA_IDi.ToString()+"-"+clickNodeB_IDi.ToString()+"="+(clickNodeA_IDi-clickNodeB_IDi).ToString());
								}
								movieCount++;//更新移動次數
								print("移動次數="+movieCount.ToString());
							}
							else 
							{
								QuestionNodeObj_prefab[clickNodeA_IDi,clickNodeA_IDj].renderer.material.color = new Color(1,1,1,1);
								QuestionNodeObj_prefab[clickNodeB_IDi,clickNodeB_IDj].renderer.material.color = new Color(1,1,1,1);
								print("兩次點選 不在同一階層 或 重複節點");
							}
							clickTwice = true;
						}
					}
				}
			}
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
}
