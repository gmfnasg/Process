using UnityEngine;
using System.Collections;
//隨機產生陣列並自動檢查是否可以達到終點
//目標-非單次階層左右位移解題模式
public class CreatArray9 : MonoBehaviour {
	public GameObject cube;
	public int node = 4, layer = 4 ; //節點數量,階層數 如果系統未給予指定數量時起始為4,4
	Vector3[,] NodePOS; //紀錄並設定每個節點的座標
	int [,] NodeType; //設定節點的圖示屬性
	bool[,] NodeSwish; //紀錄目前進行到哪個節點
	GameObject[,] Cube_prefab; //設定各節點物件用
	//float CheckTime; //紀錄下次要更新動作的時間
	//int TimeInterval = 10; //每次動作的時間間隔
	//int checki=0,checkj=0; // runAllNodeSwish用來紀錄判定到哪個陣列ID
	int intoIDi=0,intoIDj=0; // 用來記錄目前停留在哪個節點ID
	public float picNumber = 36; //圖示的張數
	
	
	bool checkRouteStatus = false;//檢查題目路線狀態
	int firstI,firstJ;//紀錄起始點座標
	int checkQuestionI,checkQuestionJ;//checkQuestion用來紀錄判定到哪個陣列ID

	//DisplacementAllLayerType 位移整排(或列)節點屬性用
	int [,] DisplacementNodeType;
	
	//autoSlovingQuestion 判定用
	bool slovingQuestionSwish =false; //判定是否走向終點用
	int DisplacementIDj; //第幾層執行位移
	
	//gotoNextNodeForCheckDisplacementNodeType用
	int checkDisplacementNodeI,checkDisplacementNodeJ;//gotoNextNodeForCheckDisplacementNodeType用來紀錄判定到哪個陣列ID
	bool[,] DisplacementNodeSwish; //紀錄位移陣列目前進行到哪個節點
	
	// Use this for initialization
	void Start () {
		
		layer += 2; //+2為頭尾排預留用
		NodePOS = new Vector3[node, layer];
		NodeType = new int[node, layer]; 
		NodeSwish = new bool[node, layer]; 
		Cube_prefab = new GameObject[node, layer];
		DisplacementNodeSwish = new bool[node+2, layer]; 
		//CheckTime=Time.deltaTime * TimeInterval;//設定遊戲開始後多久開始執行第一次動作
		
		//位移整排(或列)節點屬性用,並預留頭尾位移1個空間
		DisplacementNodeType= new int[node+2, layer]; 
		
		//產生所有物件與設定與亂數屬性
		//creatCubeAndSetup();
		
		//   上下選一作為起始設定
		
		//產生自訂題目物件與設定
		creatAndSetCustomizationsQuesion();
	
		
		//檢查起始問題路線是否到達終點
		checkQuestion(0);
		
		//位移節點屬性
		//DisplacementAllLayerType();
		
		//自動解題-如未達終點則自動位移節點嘗試解題
		autoSlovingQuestion();
		
		//設定參考物件
		cube.transform.position =  new Vector3(0, 0, 1.5f); //設定參考物件位置
		cube.renderer.material.color = new Color (1, 235/255, 4/255, 0.5f); //設定參考物件顏色
	}
	
	// Update is called once per frame
	void Update () 
	{
		//更新所有節點透明度
		//checkAllNodeSwish();
		
		//所有節點依序跑一遍
		//runAllNodeSwish();
		
		//判定目前節點功能
		//gotoNextNode();
	}	
	
	//檢查更換貼圖屬性
	void changTexture(int i, int j) 
	{
		if(NodeType[i,j] == 0)
		{
			//節點圖示: 黑洞
			Cube_prefab[i,j].transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*4,(1/(Mathf.Sqrt(picNumber)))*5);
			//print("黑洞 i="+ i.ToString() +", j=" + j.ToString());
		}
		else if (NodeType[i,j] == 1)
		{
			//節點圖示: 方向節點 向左
			Cube_prefab[i,j].transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*0,(1/(Mathf.Sqrt(picNumber)))*5);
			//print("方向節點 向左 i="+ i.ToString() +", j=" + j.ToString());
		}
		else if (NodeType[i,j] == 2)
		{
			//節點圖示: 方向節點 向上
			Cube_prefab[i,j].transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*1,(1/(Mathf.Sqrt(picNumber)))*5);
			//print("方向節點 向上 i="+ i.ToString() +", j=" + j.ToString());
		}
		else if (NodeType[i,j] == 3)
		{
			//節點圖示: 方向節點 向右
			Cube_prefab[i,j].transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*2,(1/(Mathf.Sqrt(picNumber)))*5);
			//print("方向節點 向右 i="+ i.ToString() +", j=" + j.ToString());
		}
		else if (NodeType[i,j] == 4)
		{
			//節點圖示: 方向節點 向下
			Cube_prefab[i,j].transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*3,(1/(Mathf.Sqrt(picNumber)))*5);
			//print("方向節點 向下 i="+ i.ToString() +", j=" + j.ToString());
		}
		
				
		if(j==0 || j==layer-1)
		{
			//節點圖示: 禁止節點
			Cube_prefab[i,j].transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*4,(1/(Mathf.Sqrt(picNumber)))*2);
			//print("禁止節點 i="+ i.ToString() +", j=" + j.ToString());
		}
		
	}
	
	//檢查更換貼圖屬性2 (ID編號，節點類型，指定目標節點物件)
	void changTexture2(int i, int j, int type, GameObject obj) 
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
	
	//檢查所有節點開關屬性改變其透明度
	void checkAllNodeSwish()
	{
		for(int i = 0; i<node; i++)
		{
			for(int j = 0; j<layer; j++)
			{
				if(NodeSwish[i,j])
				{
					Cube_prefab[i,j].renderer.material.color = new Color(1,1,1,0.5f);
				}
				else{
					Cube_prefab[i,j].renderer.material.color = new Color(1,1,1,1);
				}
			}
		}
	}
	
	//將所有節點開關設為關閉
	void turnoffAllSwish() 
	{
		for(int i=0;i<node;i++)
		{
			for(int j = 0;j<layer;j++)
			{
			NodeSwish[i,j] = false;
			}
		}
	}
	
	//將所有新陣列節點開關設為關閉
	void turnoffAllDisplacementNodeSwish() 
	{
		for(int i=0;i<node+2;i++)
		{
			for(int j = 0;j<layer;j++)
			{
			DisplacementNodeSwish[i,j] = false;
			}
		}
	}
	
	//起始產生節點與其屬性設定
	void creatCubeAndSetup() 
	{
		for(int i = 0; i<node; i++)
		{
			for(int j = 0; j<layer; j++)
			{
				//排列擺放節點物件
				//判斷節點數是否是偶數
				if(node%2==0){
					NodePOS[i,j].x=node-i-(node/2)-0.5f;
				}
				else{
					NodePOS[i,j].x=node-i-(node/2)-1;
				}
				//判斷階層數是否是偶數
				if(layer%2==0){
					NodePOS[i,j].y=j-(layer/2)+0.5f;
				}
				else{
					NodePOS[i,j].y=j-(layer/2);
				}
				NodePOS[i,j].z=1;
				//亂數決定節點類型(排除產屬性為4的上向下節點)
				int rnd = Random.Range(1,4);
				NodeType[i,j] = rnd;
			
				//產生節點物件並給予名稱與座標ID
				Cube_prefab[i,j] = GameObject.Instantiate(cube,NodePOS[i,j],Quaternion.identity) as GameObject;
				Cube_prefab[i,j].name = Cube_prefab[i,j].name + "i=" + i.ToString() + ",j=" + j.ToString()+", Tyep=" + rnd.ToString();
				//將節點依照屬性改變其貼圖圖示
				changTexture(i,j);
				
			}
		}
		
		//設定所有節點開關起始屬性為關閉
		turnoffAllSwish();
		
		//防止第一層有兩個向下節點或是沒有產生向下節點
		for(int i = 0;i<node;i++)
		{
			NodeType[i,layer-2]=Random.Range(1,4);
			changTexture(i,layer-2);
		}
		//只產生一個向下節點
		int rnd2 = Random.Range(0,node);
		NodeType[rnd2,layer-2]=4;
		changTexture(rnd2,layer-2);
		NodeSwish[rnd2,layer-2]=true;
		Cube_prefab[rnd2,layer-2].renderer.material.color = new Color(1,1,1,0.5f);
		intoIDi = rnd2;
		intoIDj = layer-2;
		firstI = rnd2;
		firstJ = layer-2;
		print("起始停留點 intoID ij="+intoIDi.ToString()+intoIDj.ToString()+", type="+NodeType[rnd2,layer-2].ToString()+", first I,J=" + firstI.ToString()+firstJ.ToString());
		
		//防止各層沒有向下節點
		
		for(int j = 1;j<layer-2;j++)
		{
			int rnd3 = Random.Range(0,node);
			NodeType[rnd3,j]=4;
			changTexture(rnd3,j);
		}
		
	}
	
	//檢查題目起始點最終會停到哪(檢查類型，0為起始檢查用，1為新陣列節點用)
	void checkQuestion(int checkType)
	{
		//檢查ID用
		checkQuestionI=firstI;
		checkQuestionJ=firstJ;
		int i=0;
		print("checkRouteStatus起始設定為"+checkRouteStatus.ToString());
		
		//如果路線未跑完則不斷執行
		while (!checkRouteStatus)
		{
			i++;//顯示已經執行幾次用
			
			if(checkType==0)
			{ 
				print("第"+i.ToString()+"次執行checkQuestion，狀態為"+checkRouteStatus.ToString());
				gotoNextNodeForCheckQuestion();
				//print ("開始更換節點透明度");
				checkAllNodeSwish();
			}
			else if (checkType==1)
			{
				print("位移後第"+i.ToString()+"次判斷checkQuestion，狀態為"+checkRouteStatus.ToString());
				gotoNextNodeForCheckDisplacementNodeType();
			}
			else{
				checkRouteStatus=true;
				print("checkType判定類型錯誤，屬性="+checkType.ToString());
				break;
			}
			
		}
				
	}
	
	//自動檢查節點功能更新移動到下個節點
	void gotoNextNodeForCheckQuestion()
	{
		if(NodeType[checkQuestionI,checkQuestionJ]==0)
		{
			//print("改之前checkRouteStatus="+checkRouteStatus.ToString());
			checkRouteStatus = true;
			//print("改之後checkRouteStatus="+checkRouteStatus.ToString());
			print("跑進黑洞i="+checkQuestionI.ToString()+", j="+checkQuestionJ.ToString());
		}
		
		//如果是往左節點
		else if(NodeType[checkQuestionI,checkQuestionJ]==1)
		{
			//如果往左沒超過邊界
			if(checkQuestionI>0)
			{
				//檢查左邊的節點
				//無法前進時
				if(NodeType[checkQuestionI-1,checkQuestionJ]==3)
				{
					//print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					//print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print(checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 無法往左前進到i="+(checkQuestionI-1).ToString()+", j="+checkQuestionJ.ToString()+", Type="+NodeType[checkQuestionI-1,checkQuestionJ].ToString());
				}
				else if(NodeType[checkQuestionI-1,checkQuestionJ] == 0)
				{
					//print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					//print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 會跑進黑洞i="+(checkQuestionI-1).ToString()+", j="+checkQuestionJ.ToString());
				}
				
				//可以前進時
				else
				{
					//如果下個此節點沒有重複進入過
					if(!NodeSwish[checkQuestionI-1,checkQuestionJ])
					{
					//將停留節點往左移動
					checkQuestionI--;
					NodeSwish[checkQuestionI,checkQuestionJ] = true;
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 往左移動節點到i="+checkQuestionI.ToString()+", j="+checkQuestionJ.ToString()+", Type="+NodeType[checkQuestionI,checkQuestionJ].ToString());
					}
					else{
						checkRouteStatus = true;
						print ("重複迴圈");
					}
				}
			}
			//如果往左超過邊界
			else if(checkQuestionI == 0)
			{
				//檢查左邊的節點
				//無法前進時
				if(NodeType[node-1,checkQuestionJ] == 3)
				{
					//print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					//print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 無法往左前進到i="+(node-1).ToString()+", j="+checkQuestionJ.ToString()+", Type="+NodeType[node-1,checkQuestionJ].ToString());
				}
				else if(NodeType[node-1,checkQuestionJ] == 0)
				{
					//print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					//print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 會跑進黑洞i="+(node-1).ToString()+", j="+checkQuestionJ.ToString());
				}
				
				//可以前進時
				else
				{
					//如果下個此節點沒有重複進入過
					if(!NodeSwish[node-1,checkQuestionJ])
					{
						//將停留節點往左移動
						checkQuestionI=node-1;
						NodeSwish[checkQuestionI,checkQuestionJ] = true;
						print("目前在i=0, j="+checkQuestionJ.ToString() + ", 往左移動節點到i="+checkQuestionI.ToString()+", j="+checkQuestionJ.ToString()+", Type="+NodeType[checkQuestionI,checkQuestionJ].ToString());
					}
					else{
						checkRouteStatus = true;
						print ("重複迴圈");
					}
				}
			}
			else{
			checkRouteStatus = true;
			print("!! 注意 !!檢查強制結束gotoNextNodeForCheckQuestion 往左節點判定問題");
			print("!! 注意 !!位移後的進入節點類型為"+DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ].ToString());
			print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString());
			}
		}
		
		//如果是往右節點
		else if(NodeType[checkQuestionI,checkQuestionJ]==3)
		{
			//如果往右沒超過邊界
			if(checkQuestionI < node-1)
			{
				//檢查右邊的節點
				//無法前進時
				if(NodeType[checkQuestionI+1,checkQuestionJ] == 1)
				{
					//print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					//print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 無法往右前進到i="+(checkQuestionI+1).ToString()+", j="+checkQuestionJ.ToString()+", Type="+NodeType[checkQuestionI+1,checkQuestionJ].ToString());
				}
				else if(NodeType[checkQuestionI+1,checkQuestionJ] == 0)
				{
					//print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					//print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 會跑進黑洞i="+(checkQuestionI+1).ToString()+", j="+checkQuestionJ.ToString());
				}
				
				//可以前進時
				else
				{
					//如果下個此節點沒有重複進入過
					if(!NodeSwish[checkQuestionI+1,checkQuestionJ])
					{
					//將停留節點往左移動
					checkQuestionI++;
					NodeSwish[checkQuestionI,checkQuestionJ] = true;
					print("目前在i=" +(checkQuestionI-1).ToString()+", j="+checkQuestionJ.ToString() + ", 往右移動節點到i="+checkQuestionI.ToString()+", j="+checkQuestionJ.ToString()+", Type="+NodeType[checkQuestionI,checkQuestionJ].ToString());
					}
					else{
						checkRouteStatus = true;
						print ("重複迴圈");
					}
				}
			}
			//如果往右超過邊界
			else if(checkQuestionI == node-1)
			{
				//檢查右邊的節點
				//無法前進時
				if(NodeType[0,checkQuestionJ] == 1)
				{
					//print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					//print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 無法往右前進到i=0, j="+checkQuestionJ.ToString()+", Type="+NodeType[0,checkQuestionJ].ToString());
				}
				else if(NodeType[0,checkQuestionJ] == 0)
				{
					//print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					//print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 會跑進黑洞i=0, j="+checkQuestionJ.ToString());
				}
				
				//可以前進時
				else
				{
					//如果下個此節點沒有重複進入過
					if(!NodeSwish[0,checkQuestionJ])
					{
						//將停留節點往右移動
						checkQuestionI=0;
						NodeSwish[checkQuestionI,checkQuestionJ] = true;
						print("目前在i=" +(node-1).ToString()+", j="+checkQuestionJ.ToString() + ", 往右移動節點到i="+checkQuestionI.ToString()+", j="+checkQuestionJ.ToString()+", Type="+NodeType[checkQuestionI,checkQuestionJ].ToString());
					}
					else{
						checkRouteStatus = true;
						print ("重複迴圈");
					}
				}
			}
			else{
			checkRouteStatus = true;
			print("!! 注意 !!檢查強制結束gotoNextNodeForCheckQuestion 往右節點判定問題");
			print("!! 注意 !!位移後的進入節點類型為"+DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ].ToString());
			print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString());
			}
		}
		
		//如果是往上節點
		else if(NodeType[checkQuestionI,checkQuestionJ]==2)
		{
			//如果往上沒超過邊界
			if(checkQuestionJ < layer-2)
			{
				//檢查上邊的節點
				
				//無法前進時
				if(NodeType[checkQuestionI,checkQuestionJ+1]== 4)
				{
					//print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					//print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 無法往上前進到i="+checkQuestionI.ToString()+", j="+(checkQuestionJ+1).ToString()+", Type="+NodeType[checkQuestionI,checkQuestionJ+1].ToString());
				}
				else if(NodeType[checkQuestionI,checkQuestionJ+1] == 0)
				{
					//print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					//print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 會跑進黑洞i="+checkQuestionI.ToString()+", j="+(checkQuestionJ+1).ToString());
				}
				
				//可以前進時
				else
				{
					//如果下個此節點沒有重複進入過
					if(!NodeSwish[checkQuestionI,checkQuestionJ+1])
					{
						//將停留節點往上移動
						checkQuestionJ++;
						NodeSwish[checkQuestionI,checkQuestionJ] = true;
						print("目前在i=" +checkQuestionI.ToString()+", j="+(checkQuestionJ-1).ToString() + ", 往上移動節點到i="+checkQuestionI.ToString()+", j="+checkQuestionJ.ToString()+", Type="+NodeType[checkQuestionI,checkQuestionJ].ToString());
					}
					else{
						checkRouteStatus = true;
						print ("重複迴圈");
					}
				}
			}
			
			//如果往上超過邊界
			else if(checkQuestionJ == layer-2)
			{
				//print("改之前checkRouteStatus="+checkRouteStatus.ToString());
				checkRouteStatus = true;
				//print("改之後checkRouteStatus="+checkRouteStatus.ToString());
				print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 會超過邊界到i="+checkQuestionI.ToString()+", j="+(checkQuestionJ+1).ToString());
			}
			else{
			checkRouteStatus = true;
			print("!! 注意 !!檢查強制結束gotoNextNodeForCheckQuestion 往上節點判定問題");
			print("!! 注意 !!位移後的進入節點類型為"+DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ].ToString());
			print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString());
			}
		}
		
		//如果是往下節點
		else if(NodeType[checkQuestionI,checkQuestionJ]==4)
		{
			//如果往下沒超過邊界
			if(checkQuestionJ > 1)
			{
				//檢查下邊的節點
				
				//無法前進時
				if(NodeType[checkQuestionI,checkQuestionJ-1]== 2)
				{
					//print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					//print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 無法往下前進到i="+checkQuestionI.ToString()+", j="+(checkQuestionJ-1).ToString()+", Type="+NodeType[checkQuestionI,checkQuestionJ-1].ToString());
				}
				else if(NodeType[checkQuestionI,checkQuestionJ-1] == 0)
				{
					//print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					//print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 會跑進黑洞i="+checkQuestionI.ToString()+", j="+(checkQuestionJ-1).ToString());
				}
				
				//可以前進時
				else
				{
					//如果下個此節點沒有重複進入過
					if(!NodeSwish[checkQuestionI,checkQuestionJ-1])
					{
						//將停留節點往下移動
						checkQuestionJ--;
						NodeSwish[checkQuestionI,checkQuestionJ] = true;
						print("目前在i=" +checkQuestionI.ToString()+", j="+(checkQuestionJ+1).ToString() + ", 往下移動節點到i="+checkQuestionI.ToString()+", j="+checkQuestionJ.ToString()+", Type="+NodeType[checkQuestionI,checkQuestionJ].ToString());
					}
					else{
						checkRouteStatus = true;
						print ("重複迴圈");
					}
				}
			}
			
			//如果往下超過邊界
			else if(checkQuestionJ == 1)
			{
				//print("改之前checkRouteStatus="+checkRouteStatus.ToString());
				checkRouteStatus = true;
				//print("改之後checkRouteStatus="+checkRouteStatus.ToString());
				slovingQuestionSwish = true;//autoSlovingQuestion用來判定題目已經解完
				print("Question Success!! 目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 下個節點是i="+checkQuestionI.ToString()+", j="+(checkQuestionJ-1).ToString());
			}
			else{
			checkRouteStatus = true;
			print("!! 注意 !!檢查強制結束gotoNextNodeForCheckQuestion 往下節點判定問題");
			print("!! 注意 !!位移後的進入節點類型為"+DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ].ToString());
			print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString());
			}
		}
		else{
			checkRouteStatus = true;
			print("!! 注意 !!檢查強制結束gotoNextNodeForCheckQuestion");
			print("!! 注意 !!位移後的進入節點類型為"+DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ].ToString());
			print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString());
		}
	}
	
	//自動檢查位移後的節點功能更新移動到下個節點
	void gotoNextNodeForCheckDisplacementNodeType()
	{
		if( DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ]==0)
		{
			print("改之前checkRouteStatus="+checkRouteStatus.ToString());
			checkRouteStatus = true;
			print("改之後checkRouteStatus="+checkRouteStatus.ToString());
			print("跑進黑洞i="+checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString());
		}
		
		//如果是往左節點
		else if(DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ]==1)
		{
			//如果往左沒超過邊界
			if(checkDisplacementNodeI>1)
			{
				//檢查左邊的節點
				//無法前進時
				if(DisplacementNodeType[checkDisplacementNodeI-1,checkDisplacementNodeJ]==3)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print(checkRouteStatus.ToString());
					print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 無法往左前進到i="+(checkDisplacementNodeI-1).ToString()+", j="+checkDisplacementNodeJ.ToString()+", Type="+DisplacementNodeType[checkDisplacementNodeI-1,checkDisplacementNodeJ].ToString());
				}
				else if(DisplacementNodeType[checkDisplacementNodeI-1,checkDisplacementNodeJ] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 會跑進黑洞i="+(checkDisplacementNodeI-1).ToString()+", j="+checkDisplacementNodeJ.ToString());
				}
				
				//可以前進時
				else
				{
					//如果下個此節點沒有重複進入過
					if(!DisplacementNodeSwish[checkDisplacementNodeI-1,checkDisplacementNodeJ])
					{
					//將停留節點往左移動
					checkDisplacementNodeI--;
					DisplacementNodeSwish[checkDisplacementNodeI,checkDisplacementNodeJ] = true;
					print("目前在i=" +(checkDisplacementNodeI+1).ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 往左移動節點到i="+checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString()+", Type="+DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ].ToString());
					}
					else{
						checkRouteStatus = true;
						print ("重複迴圈");
					}
				}
			}
			//如果往左超過邊界
			else if(checkDisplacementNodeI == 1)
			{
				//檢查左邊的節點
				//無法前進時
				if(DisplacementNodeType[node,checkDisplacementNodeJ] == 3)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 無法往左前進到i="+(node).ToString()+", j="+checkDisplacementNodeJ.ToString()+", Type="+DisplacementNodeType[node,checkDisplacementNodeJ].ToString());
				}
				else if(DisplacementNodeType[node,checkDisplacementNodeJ] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 會跑進黑洞i="+(node).ToString()+", j="+checkDisplacementNodeJ.ToString());
				}
				
				//可以前進時
				else
				{
					//如果下個此節點沒有重複進入過
					if(!DisplacementNodeSwish[node,checkDisplacementNodeJ])
					{
						//將停留節點往左移動
						checkDisplacementNodeI=node;
						DisplacementNodeSwish[node,checkDisplacementNodeJ] = true;
						print("目前在i=1, j="+checkDisplacementNodeJ.ToString() + ", 往左移動節點到i="+node.ToString()+", j="+checkDisplacementNodeJ.ToString()+", Type="+DisplacementNodeType[node,checkDisplacementNodeJ].ToString());
					}
					else{
						checkRouteStatus = true;
						print ("重複迴圈,目前在i=1, j="+checkDisplacementNodeJ.ToString());
					}
				}
			}
		}
		
		//如果是往右節點
		else if(DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ]==3)
		{ 
			//如果往右沒超過邊界
			if(checkDisplacementNodeI < node)
			{
				//檢查右邊的節點
				//無法前進時
				if(DisplacementNodeType[checkDisplacementNodeI+1,checkDisplacementNodeJ] == 1)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 無法往右前進到i="+(checkDisplacementNodeI+1).ToString()+", j="+checkDisplacementNodeJ.ToString()+", Type="+DisplacementNodeType[checkDisplacementNodeI+1,checkDisplacementNodeJ].ToString());
				}
				else if(DisplacementNodeType[checkDisplacementNodeI+1,checkDisplacementNodeJ] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 會跑進黑洞i="+(checkDisplacementNodeI+1).ToString()+", j="+checkDisplacementNodeJ.ToString());
				}
				
				//可以前進時
				else
				{
					//如果下個此節點沒有重複進入過
					if(!DisplacementNodeSwish[checkDisplacementNodeI+1,checkDisplacementNodeJ])
					{
					//將停留節點往左移動
					checkDisplacementNodeI++;
					DisplacementNodeSwish[checkDisplacementNodeI,checkDisplacementNodeJ] = true;
					print("目前在i=" +(checkDisplacementNodeI-1).ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 往右移動節點到i="+checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString()+", Type="+DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ].ToString());
					}
					else{
						checkRouteStatus = true;
						print ("重複迴圈");
					}
				}
			}
			//如果往右超過邊界
			else if(checkDisplacementNodeI == node)
			{
				//檢查右邊的節點
				//無法前進時
				if(DisplacementNodeType[1,checkDisplacementNodeJ] == 1)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 無法往右前進到i=1, j="+checkDisplacementNodeJ.ToString()+", Type="+DisplacementNodeType[0,checkDisplacementNodeJ].ToString());
				}
				else if(DisplacementNodeType[1,checkDisplacementNodeJ] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 會跑進黑洞i=1, j="+checkDisplacementNodeJ.ToString());
				}
				
				//可以前進時
				else
				{
					//如果下個此節點沒有重複進入過
					if(!DisplacementNodeSwish[1,checkDisplacementNodeJ])
					{
						//將停留節點往右移動
						checkDisplacementNodeI=1;
						DisplacementNodeSwish[checkDisplacementNodeI,checkDisplacementNodeJ] = true;
						print("目前在i=" +node.ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 往右移動節點到i="+checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString()+", Type="+DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ].ToString());
					}
					else{
						checkRouteStatus = true;
						print ("重複迴圈");
					}
				}
			}
		}
		
		//如果是往上節點
		else if(DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ]==2)
		{
			//如果往上沒超過邊界
			if(checkDisplacementNodeJ < layer-2)
			{
				//檢查上邊的節點
				
				//無法前進時
				if(DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ+1]== 4)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 無法往上前進到i="+checkDisplacementNodeI.ToString()+", j="+(checkDisplacementNodeJ+1).ToString()+", Type="+DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ+1].ToString());
				}
				else if(DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ+1] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 會跑進黑洞i="+checkDisplacementNodeI.ToString()+", j="+(checkDisplacementNodeJ+1).ToString());
				}
				
				//可以前進時
				else
				{
					//如果下個此節點沒有重複進入過
					if(!DisplacementNodeSwish[checkDisplacementNodeI,checkDisplacementNodeJ+1])
					{
						//將停留節點往上移動
						checkDisplacementNodeJ++;
						DisplacementNodeSwish[checkDisplacementNodeI,checkDisplacementNodeJ] = true;
						print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+(checkDisplacementNodeJ-1).ToString() + ", 往上移動節點到i="+checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString()+", Type="+DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ].ToString());
					}
					else{
						checkRouteStatus = true;
						print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+(checkDisplacementNodeJ).ToString() + ", 往上節點是i="+checkDisplacementNodeI.ToString()+", j="+(checkDisplacementNodeJ+1).ToString()+", Type="+DisplacementNodeSwish[checkDisplacementNodeI,checkDisplacementNodeJ+1].ToString());
						print ("重複迴圈");
					}
				}
			}
			
			//如果往上超過邊界
			else if(checkDisplacementNodeJ == layer-2)
			{
				print("改之前checkRouteStatus="+checkRouteStatus.ToString());
				checkRouteStatus = true;
				print("改之後checkRouteStatus="+checkRouteStatus.ToString());
				print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 會超過邊界到i="+checkDisplacementNodeI.ToString()+", j="+(checkDisplacementNodeJ+1).ToString());
			}
		}
		
		//如果是往下節點
		else if(DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ]==4)
		{
			//如果往下沒超過邊界
			if(checkDisplacementNodeJ > 1)
			{
				//檢查下邊的節點
				
				//無法前進時
				if(DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ-1]== 2)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 無法往下前進到i="+checkDisplacementNodeI.ToString()+", j="+(checkDisplacementNodeJ-1).ToString()+", Type="+DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ-1].ToString());
				}
				else if(DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ-1] == 0)
				{
					print("改之前checkRouteStatus="+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後checkRouteStatus="+checkRouteStatus.ToString());
					print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 會跑進黑洞i="+checkDisplacementNodeI.ToString()+", j="+(checkDisplacementNodeJ-1).ToString());
				}
				
				//可以前進時
				else
				{
					//如果下個此節點沒有重複進入過
					if(!DisplacementNodeSwish[checkDisplacementNodeI,checkDisplacementNodeJ-1])
					{
						//將停留節點往下移動
						checkDisplacementNodeJ--;
						DisplacementNodeSwish[checkDisplacementNodeI,checkDisplacementNodeJ] = true;
						print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+(checkDisplacementNodeJ+1).ToString() + ", 往下移動節點到i="+checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString()+", Type="+DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ].ToString());
					}
					else{
						checkRouteStatus = true;
						print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+(checkDisplacementNodeJ).ToString() + ", 下個節點是i="+checkDisplacementNodeI.ToString()+", j="+(checkDisplacementNodeJ-1).ToString()+", Type="+DisplacementNodeSwish[checkDisplacementNodeI,checkDisplacementNodeJ-1].ToString());
						print ("重複迴圈");
						
					}
				}
			}
			
			//如果往下超過邊界
			else if(checkDisplacementNodeJ == 1)
			{
				print("改之前checkRouteStatus="+checkRouteStatus.ToString());
				checkRouteStatus = true;
				print("改之後checkRouteStatus="+checkRouteStatus.ToString());
				slovingQuestionSwish = true;//autoSlovingQuestion用來判定題目已經解完
				print("Question Success!! 目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString() + ", 下個節點是i="+checkDisplacementNodeI.ToString()+", j="+(checkDisplacementNodeJ-1).ToString());
			}
		}
		else{
			checkRouteStatus = true;
			print("!! 注意 !!檢查強制結束gotoNextNodeForCheckDisplacementNodeType");
			print("!! 注意 !!位移後的進入節點類型為"+DisplacementNodeType[checkDisplacementNodeI,checkDisplacementNodeJ].ToString());
			print("目前在i=" +checkDisplacementNodeI.ToString()+", j="+checkDisplacementNodeJ.ToString());
		}
	}
	
	//位移所有層整排(或列)節點屬性
	void DisplacementAllLayerType()
	{
		//往右位移
		//將資料記錄到新的陣列
		for(int j = 1;j<layer-1;j++)
		{
			for(int i = 0;i<node;i++)
			{
				//將原始的屬性值記錄到新的陣列右邊那格
				DisplacementNodeType[i+2,j] = NodeType[i,j];
				/*
				狀況如下
				012345 	<-DisplacementNodeType
				  ↑↑↑↑
				  0123 	<-NodeType
				*/
			}
			//將紀新的陣列第一格紀錄為原始最右邊那格屬性
			DisplacementNodeType[1,j] = NodeType[node-1,j];
			/*
			狀況如下
			012345 	<-DisplacementNodeType
			 ↑ 
			 3 		<-NodeType
			*/
		}
		 
		//將位移過資料的新陣列的資料存回
		for(int j = 1;j<layer-1;j++)
		{
			for(int i = 0;i<node;i++)
			{
				// NodeType[i,j] = DisplacementNodeType[i+1,j];
				/*
				狀況如下
				012345 	<-DisplacementNodeType
				 ↓↓↓↓
				 0123 	<-NodeType
				*/
			}
		}
		
		//產生新的節點陣列物件來顯示位移結果
		creatNewCubeAndSetup(0,0,6);
	}
	
	//位移指定J層整排(或列)節點屬性 (指定層,指定方向，0向右,1向左)
	void DisplacementOneLayerType(int layerID,int displacementDirection,int MovieX,int MovieY,int MovieZ)
	{
		//往右位移
		if(displacementDirection==0)
		{
			//將資料記錄到新的陣列
			for(int i = 0;i<node;i++)
			{
				print("開始往右位移存到新陣列");
				//將原始的屬性值記錄到新的陣列右邊那格
				DisplacementNodeType[i+2,layerID] = NodeType[i,layerID];
				//狀況如下
				//012345 	<-DisplacementNodeType
				//  ↑↑↑↑
				//  0123 	<-NodeType
			}
			//將紀新的陣列第一格紀錄為原始最右邊那格屬性
			DisplacementNodeType[1,layerID] = NodeType[node-1,layerID];
			//狀況如下
			//012345 	<-DisplacementNodeType
			// ↑ 
			// 3 		<-NodeType
			
			//產生新的節點陣列物件來顯示位移結果
			print("開始利用新陣列屬性來產生新物件");
			creatNewCubeAndSetup(MovieX,MovieY,MovieZ);
		}
		//往左位移
		else if(displacementDirection==1)
		{
			//將資料記錄到新的陣列
			for(int i = 0;i<node;i++)
			{
				print("開始往左位移存到新陣列");
				//將原始的屬性值記錄到新的陣列左邊那格
				DisplacementNodeType[i,layerID] = NodeType[i,layerID];
				//狀況如下
				//012345 	<-DisplacementNodeType
				//↑↑↑↑
				//0123 	<-NodeType
			}
			//將紀新的陣列第一格紀錄為原始最左邊那格屬性
			DisplacementNodeType[node,layerID] = NodeType[node-1,layerID];
			//狀況如下
			//012345 	<-DisplacementNodeType
			//    ↑ 
			//    0 	<-NodeType
			//產生新的節點陣列物件來顯示位移結果
			
			print("開始利用新陣列屬性來產生新物件");
			creatNewCubeAndSetup(MovieX,MovieY,MovieZ);
		}
		
		 
		//將位移過資料的新陣列的資料存回
		for(int i = 0;i<node;i++)
		{
			// NodeType[i,j] = DisplacementNodeType[i+1,j];
			//狀況如下
			//012345 	<-DisplacementNodeType
			// ↓↓↓↓
			// 0123 	<-NodeType
		}
		
		
	}
	
	//產生非起始設定用，新的節點物件(與起始物件不同的位移x,y,z座標)
	void creatNewCubeAndSetup(int MoveX,int MoveY,int MoveZ) 
	{
		Vector3 newObjVector3;
		for(int i = 0; i<node; i++)
		{
			for(int j = 0; j<layer; j++)
			{
				//排列擺放節點物件
				//判斷節點數是否是偶數
				if(node%2==0){
					newObjVector3.x=node-i-(node/2)-0.5f+MoveX;
				}
				else{
					newObjVector3.x=node-i-(node/2)-1+MoveX;
				}
				//判斷階層數是否是偶數
				if(layer%2==0){
					newObjVector3.y=j-(layer/2)+0.5f+MoveY;
				}
				else{
					newObjVector3.y=j-(layer/2)+MoveY;
				}
				newObjVector3.z=MoveZ;
				
				//產生節點物件並給予名稱與座標ID
				GameObject newCube_prefab  = GameObject.Instantiate(cube,newObjVector3,Quaternion.identity) as GameObject;
				newCube_prefab.name = "newOBJ"+newCube_prefab.name + "i=" + i.ToString() + ",j=" + j.ToString();
				newCube_prefab.transform.renderer.material.color = new Color(1,1,1);
				//將節點依照屬性改變其貼圖圖示
				changTexture2(i,j,DisplacementNodeType[i+1,j],newCube_prefab);
				
			}
		}
	}
	
	//自動解題-如未達終點則自動位移節點嘗試解題
	void autoSlovingQuestion()
	{
		if(!slovingQuestionSwish)
		{
			//往右位移
			tryToDisplacementSlovingType(0);
		}
		
		//再次位移
		
		if(!slovingQuestionSwish)
		{
			//往左位移
			tryToDisplacementSlovingType(1);
		}
	}
	
	//尋找新節點陣列的起始節點
	void findStartNodeType()
	{
		for(int i = 1;i<node+1;i++)
		{
			//print("尋找起點位移陣列i="+i.ToString()+"屬性="+DisplacementNodeType[i,layer-2].ToString());
			if(DisplacementNodeType[i,layer-2]==4)
			{
				checkDisplacementNodeI = i;
				checkDisplacementNodeJ = layer-2;
				print("此次的起始點checkDisplacementNodeI=" + checkDisplacementNodeI.ToString()+"checkDisplacementNodeJ=" + checkDisplacementNodeJ.ToString());
				
				DisplacementNodeSwish[i,layer-2]=true;
			}
		}
	}
	
	//將新節點屬性設為原始題目節點屬性
	void setDisplacementNodeType()
	{
		for(int j = 1;j<layer-1;j++)
		{
			for(int i = 0;i<node;i++)
			{
				//將原始的屬性值記錄到新的陣列右邊那格
				DisplacementNodeType[i+1,j] = NodeType[i,j];
				/*
				狀況如下
				012345 	<-DisplacementNodeType
				 ↑↑↑↑
				 0123 	<-NodeType
				*/
				//print("原始IDij="+i.ToString()+j.ToString()+"屬性="+NodeType[i,j].ToString()+"位移陣列IDij="+(i+1).ToString()+j.ToString()+"屬性="+DisplacementNodeType[i+1,j].ToString());
			}
		}
	}
	
	//自訂題目節點屬性並產生
	void creatAndSetCustomizationsQuesion()
	{	
		//限定節點與階層數量
		node = 4;
		layer =6;
		//手動設定節點屬性
		NodeType[0,4]=1;
		NodeType[1,4]=4;
		NodeType[2,4]=1;
		NodeType[3,4]=3;
			
		NodeType[0,3]=1;
		NodeType[1,3]=1;
		NodeType[2,3]=3;
		NodeType[3,3]=4;
			
		NodeType[0,2]=4;
		NodeType[1,2]=1;
		NodeType[2,2]=1;
		NodeType[3,2]=2;
			
		NodeType[0,1]=1;
		NodeType[1,1]=4;
		NodeType[2,1]=3;
		NodeType[3,1]=1;
		
		intoIDi = 1;
		intoIDj = 4;
		
		firstI = 1;
		firstJ = 4;
		NodeSwish[intoIDi,intoIDj]=true;

		
		//排列擺放節點物件
		for(int i = 0; i<node; i++)
		{
			for(int j = 0; j<layer; j++)
			{
				
				//判斷節點數是否是偶數
				if(node%2==0){
					NodePOS[i,j].x=node-i-(node/2)-0.5f;
				}
				else{
					NodePOS[i,j].x=node-i-(node/2)-1;
				}
				//判斷階層數是否是偶數
				if(layer%2==0){
					NodePOS[i,j].y=j-(layer/2)+0.5f;
				}
				else{
					NodePOS[i,j].y=j-(layer/2);
				}
				NodePOS[i,j].z=1;
				
				//產生節點物件並給予名稱與座標ID
				Cube_prefab[i,j] = GameObject.Instantiate(cube,NodePOS[i,j],Quaternion.identity) as GameObject;
				Cube_prefab[i,j].name = Cube_prefab[i,j].name + "i=" + i.ToString() + ",j=" + j.ToString()+", Tyep=" + NodeType[i,j].ToString();
				//將節點依照屬性改變其貼圖圖示
				changTexture(i,j);
			}
		}
		changTexture(intoIDi,intoIDj);
		Cube_prefab[intoIDi,intoIDj].renderer.material.color = new Color(1,1,1,0.5f);

	}
	
	//嘗試位移解題模式(解題模式 0=往右位移, 1=往左位移)
	void tryToDisplacementSlovingType(int SlovingType)
	{
		//紀錄自動解題次數
		int countForCheckQuestion=1;
		//指定位移層ID用
		int j=1;
		
		if(SlovingType==0)
		{
			//開始往右位移自動解題
			print("開始往右位移自動解題,j=" + j.ToString());
			//當題目未能達到終點則持續做
			while (!slovingQuestionSwish)
			{
				
				//如果指定位移層沒有超過最上層5
				if(j<layer-1)
				{
					print("開始自動往右位移,j=" + j.ToString());
					//恢復起始題目類型屬性給新陣列
					setDisplacementNodeType();
					
					//尋找起始節點是否與題目相同IDij
					findStartNodeType();
					print ("尋找起始節點是否與題目相同IDij="+checkDisplacementNodeI.ToString()+checkDisplacementNodeJ.ToString()+"起始IDij="+(firstI+1).ToString()+firstJ.ToString());
					//print ("!!           改前 IDij42 type="+DisplacementNodeType[4,2].ToString());
					//位移節點來看嘗試解題可能性
					DisplacementOneLayerType(j,0,0,0,6+(j*6));//X座標,Y座標,Z座標(+(往右第幾次位移*每個位移間距))
					//print ("!!           改後 IDij42 type="+DisplacementNodeType[4,2].ToString());

					
					//尋找檢查的起始節點IDij
					findStartNodeType();
					print ("找完起始節點後ID ij="+checkDisplacementNodeI.ToString()+checkDisplacementNodeJ.ToString());
					
					//重置節點開關屬性
					//print ("!! - - - - - -          改前 IDij43 bool="+DisplacementNodeSwish[4,3].ToString());
					turnoffAllDisplacementNodeSwish();
					//print ("!! - - - - - -          改後 IDij43 bool="+DisplacementNodeSwish[4,3].ToString());
					
					//重新判定路線是否跑完
					checkRouteStatus = false;
					//檢查題目起始點最終會停到哪
					print("檢查slovingQuestionSwish屬性為"+slovingQuestionSwish.ToString());
					//print("!!     開始檢查第"+countForCheckQuestion.ToString()+"次往右移動完後是否能完成解題");
					checkQuestion(1);
					//print("!!     檢查完後slovingQuestionSwish屬性為"+slovingQuestionSwish.ToString());
					
					//再次往右位移
					
					j++;
					countForCheckQuestion++;
				
				
				}
				else{
					print("!!　　　　超過往右位移自動解題能力範圍,j=" + j.ToString());
					break;
				}
			}
		}
		
		else if(SlovingType == 1)
		{
			//開始往左位移自動解題
			j=1;//還原起始位移皆層
			print("開始往左位移自動解題,j=" + j.ToString());
			countForCheckQuestion=0;//還原位移次數
		
			//當題目未能達到終點則持續做
			while (!slovingQuestionSwish)
			{	
				//如果指定位移層沒有超過最上層5
				if(j<layer-1)
				{
					print("開始自動往右位移,j=" + j.ToString());
					//恢復起始題目類型屬性給新陣列
					setDisplacementNodeType();
					
					//尋找起始節點是否與題目相同IDij
					findStartNodeType();
					print ("尋找起始節點是否與題目相同IDij="+checkDisplacementNodeI.ToString()+checkDisplacementNodeJ.ToString()+"起始IDij="+(firstI+1).ToString()+firstJ.ToString());
					
					//位移節點來看嘗試解題可能性
					DisplacementOneLayerType(j,1,0,0,6+(j*6)+node*6+5);//X座標,Y座標,Z座標(與起始座標間距+(往右第幾次位移*每個位移間距)+與所有往右位移間物件的距+擴大間距以利區分)

					
					//尋找檢查的起始節點IDij
					findStartNodeType();
					print ("找完起始節點後ID ij="+checkDisplacementNodeI.ToString()+checkDisplacementNodeJ.ToString());
					
					//重置節點開關屬性
					turnoffAllDisplacementNodeSwish();
					
					//重新判定路線是否跑完
					checkRouteStatus = false;
					//檢查題目起始點最終會停到哪
					print("檢查slovingQuestionSwish屬性為"+slovingQuestionSwish.ToString());
					print("開始檢查第"+countForCheckQuestion.ToString()+"次往左移動完後是否能完成解題");
					checkQuestion(1);
					print("檢查完後slovingQuestionSwish屬性為"+slovingQuestionSwish.ToString());
					j++;
					countForCheckQuestion++;
				}
				else{
					print("超過往左位移自動解題能力範圍,j=" + j.ToString());
					break;
				}
			}
		}
		
		else if(SlovingType == 2)
		{
			//開始再次往左位移自動解題
			j=1;//還原起始位移皆層
			print("!!A    再次開始往左位移自動解題,j=" + j.ToString());
			countForCheckQuestion=0;//還原位移次數
		
			//當題目未能達到終點則持續做
			while (!slovingQuestionSwish)
			{	
				//如果指定位移層沒有超過最上層5
				if(j<layer-1)
				{
					print("!!A    再次開始自動往右位移,j=" + j.ToString());
					//位移節點來看嘗試解題可能性(nod+2用作物件整體往右邊排列)
					DisplacementOneLayerType(j,1,node+2,0,0);
					
					//尋找檢查的起始節點IDij
					findStartNodeType();
					print ("!!A    找完起始節點後ID ij="+checkDisplacementNodeI.ToString()+checkDisplacementNodeJ.ToString());
					
					//重置節點開關屬性
					turnoffAllDisplacementNodeSwish();
					
					//重新判定路線是否跑完
					checkRouteStatus = false;
					//檢查題目起始點最終會停到哪
					print("!!A    檢查slovingQuestionSwish屬性為"+slovingQuestionSwish.ToString());
					print("!!A    開始檢查第"+countForCheckQuestion.ToString()+"次再次往左移動完後是否能完成解題");
					checkQuestion(1);
					print("!!A    檢查完後slovingQuestionSwish屬性為"+slovingQuestionSwish.ToString());
					j++;
					countForCheckQuestion++;
				}
				else{
					print("!!A    超過再次往左位移自動解題能力範圍,j=" + j.ToString());
					break;
				}
			}
		}
	}
	
	/*
	未用到的功能 void
	//依序將所有節點開關打開然後關掉
	void runAllNodeSwish() 
	{
		if(checkj == 0){
			checkj = layer-2;
		}
		if(Time.time >= CheckTime)
		{
			CheckTime = Time.time + Time.deltaTime*TimeInterval;

			if(checki<node)
			{
				if(checki >0 )
				{
					NodeSwish[checki-1,checkj] = false;//將上一個節點關閉
				}
				NodeSwish[checki,checkj] = true;//將新的節點打開
				checki++;
				
			}
			else if(checki == node && checkj>1)
			{
				if(checkj<layer)
				{
					NodeSwish[checki-1,checkj] = false;//將上一個節點關閉
				}
				checkj--;
				checki=	0;
				NodeSwish[checki,checkj] = true;//將新的節點打開
				checki++;
			}
			else if(checki == node && checkj == 1 )
			{
				NodeSwish[checki-1,checkj] = true;//將最後一個節點關閉
			}
		}	
	}
	
	//判定目前節點功能
	void gotoNextNode()
	{
		//if(intoIDi>=0 && intoIDi<node && intoIDj>0 && intoIDj<layer-1)
		if(NodeType[intoIDi,intoIDj]==0)
		{
			print("跑進黑洞i="+intoIDi.ToString()+", j="+intoIDj.ToString());
		}
		
		//如果是往左節點
		else if(NodeType[intoIDi,intoIDj]==1)
		{
			//如果往左沒超過邊界
			if(intoIDi>0)
			{
				//檢查左邊的節點
				//無法前進時
				if(NodeType[intoIDi-1,intoIDj]==3)
				{
					print("目前在i=" +intoIDi.ToString()+", j="+intoIDj.ToString() + ", 無法往左前進到i="+(intoIDi-1).ToString()+", j="+intoIDj.ToString()+", Type="+NodeType[intoIDi-1,intoIDj].ToString());
				}
				else if(NodeType[intoIDi-1,intoIDj] == 0)
				{
					print("目前在i=" +intoIDi.ToString()+", j="+intoIDj.ToString() + ", 會跑進黑洞i="+(intoIDi-1).ToString()+", j="+intoIDj.ToString());
				}
				
				//可以前進時
				else
				{
					//將停留節點往左移動
					intoIDi--;
					NodeSwish[intoIDi,intoIDj] = true;
					print("目前在i=" +intoIDi.ToString()+", j="+intoIDj.ToString() + ", 往左移動節點到i="+intoIDi.ToString()+", j="+intoIDj.ToString()+", Type="+NodeType[intoIDi,intoIDj].ToString());
				}
			}
			//如果往左超過邊界
			else if(intoIDi == 0)
			{
				//檢查左邊的節點
				//無法前進時
				if(NodeType[node-1,intoIDj] == 3)
				{
					print("目前在i=" +intoIDi.ToString()+", j="+intoIDj.ToString() + ", 無法往左前進到i="+(node-1).ToString()+", j="+intoIDj.ToString()+", Type="+NodeType[node-1,intoIDj].ToString());
				}
				else if(NodeType[node-1,intoIDj] == 0)
				{
					print("目前在i=" +intoIDi.ToString()+", j="+intoIDj.ToString() + ", 會跑進黑洞i="+(node-1).ToString()+", j="+intoIDj.ToString());
				}
				
				//可以前進時
				else
				{
					//將停留節點往左移動
					intoIDi=node-1;
					NodeSwish[intoIDi,intoIDj] = true;
					print("目前在i=0, j="+intoIDj.ToString() + ", 往左移動節點到i="+intoIDi.ToString()+", j="+intoIDj.ToString()+", Type="+NodeType[intoIDi,intoIDj].ToString());
				}
			}
		}
		
		//如果是往右節點
		else if(NodeType[intoIDi,intoIDj]==3)
		{
			//如果往右沒超過邊界
			if(intoIDi < node-1)
			{
				//檢查右邊的節點
				//無法前進時
				if(NodeType[intoIDi+1,intoIDj] == 1)
				{
					print("目前在i=" +intoIDi.ToString()+", j="+intoIDj.ToString() + ", 無法往右前進到i="+(intoIDi+1).ToString()+", j="+intoIDj.ToString()+", Type="+NodeType[intoIDi+1,intoIDj].ToString());
				}
				else if(NodeType[intoIDi+1,intoIDj] == 0)
				{
					print("目前在i=" +intoIDi.ToString()+", j="+intoIDj.ToString() + ", 會跑進黑洞i="+(intoIDi+1).ToString()+", j="+intoIDj.ToString());
				}
				
				//可以前進時
				else
				{
					//將停留節點往左移動
					intoIDi++;
					NodeSwish[intoIDi,intoIDj] = true;
					print("目前在i=" +(intoIDi-1).ToString()+", j="+intoIDj.ToString() + ", 往右移動節點到i="+intoIDi.ToString()+", j="+intoIDj.ToString()+", Type="+NodeType[intoIDi,intoIDj].ToString());
				}
			}
			//如果往右超過邊界
			else if(intoIDi == node-1)
			{
				//檢查右邊的節點
				//無法前進時
				if(NodeType[0,intoIDj] == 1)
				{
					print("目前在i=" +intoIDi.ToString()+", j="+intoIDj.ToString() + ", 無法往右前進到i=0, j="+intoIDj.ToString()+", Type="+NodeType[0,intoIDj].ToString());
				}
				else if(NodeType[0,intoIDj] == 0)
				{
					print("目前在i=" +intoIDi.ToString()+", j="+intoIDj.ToString() + ", 會跑進黑洞i=0, j="+intoIDj.ToString());
				}
				
				//可以前進時
				else
				{
					//將停留節點往右移動
					intoIDi=0;
					NodeSwish[intoIDi,intoIDj] = true;
					print("目前在i=" +(node-1).ToString()+", j="+intoIDj.ToString() + ", 往右移動節點到i="+intoIDi.ToString()+", j="+intoIDj.ToString()+", Type="+NodeType[intoIDi,intoIDj].ToString());
				}
			}
		}
		
		//如果是往上節點
		else if(NodeType[intoIDi,intoIDj]==2)
		{
			//如果往上沒超過邊界
			if(intoIDj < layer-2)
			{
				//檢查上邊的節點
				
				//無法前進時
				if(NodeType[intoIDi,intoIDj+1]== 4)
				{
					print("目前在i=" +intoIDi.ToString()+", j="+intoIDj.ToString() + ", 無法往上前進到i="+intoIDi.ToString()+", j="+(intoIDj+1).ToString()+", Type="+NodeType[intoIDi,intoIDj+1].ToString());
				}
				else if(NodeType[intoIDi,intoIDj+1] == 0)
				{
					print("目前在i=" +intoIDi.ToString()+", j="+intoIDj.ToString() + ", 會跑進黑洞i="+intoIDi.ToString()+", j="+(intoIDj+1).ToString());
				}
				
				//可以前進時
				else
				{
					//將停留節點往上移動
					intoIDj++;
					NodeSwish[intoIDi,intoIDj] = true;
					print("目前在i=" +intoIDi.ToString()+", j="+(intoIDj-1).ToString() + ", 往上移動節點到i="+intoIDi.ToString()+", j="+intoIDj.ToString()+", Type="+NodeType[intoIDi,intoIDj].ToString());
				}
			}
			
			//如果往上超過邊界
			else if(intoIDj == layer-2)
			{
				print("目前在i=" +intoIDi.ToString()+", j="+intoIDj.ToString() + ", 會超過邊界到i="+intoIDi.ToString()+", j="+(intoIDj+1).ToString());
			}
		}
		
		//如果是往下節點
		else if(NodeType[intoIDi,intoIDj]==4)
		{
			//如果往下沒超過邊界
			if(intoIDj > 1)
			{
				//檢查下邊的節點
				
				//無法前進時
				if(NodeType[intoIDi,intoIDj-1]== 2)
				{
					print("目前在i=" +intoIDi.ToString()+", j="+intoIDj.ToString() + ", 無法往下前進到i="+intoIDi.ToString()+", j="+(intoIDj-1).ToString()+", Type="+NodeType[intoIDi,intoIDj-1].ToString());
				}
				else if(NodeType[intoIDi,intoIDj-1] == 0)
				{
					print("目前在i=" +intoIDi.ToString()+", j="+intoIDj.ToString() + ", 會跑進黑洞i="+intoIDi.ToString()+", j="+(intoIDj-1).ToString());
				}
				
				//可以前進時
				else
				{
					//將停留節點往下移動
					intoIDj--;
					NodeSwish[intoIDi,intoIDj] = true;
					print("目前在i=" +intoIDi.ToString()+", j="+(intoIDj+1).ToString() + ", 往下移動節點到i="+intoIDi.ToString()+", j="+intoIDj.ToString()+", Type="+NodeType[intoIDi,intoIDj].ToString());
				}
			}
			
			//如果往下超過邊界
			else if(intoIDj == 1)
			{
				print("Question Success!! 目前在i=" +intoIDi.ToString()+", j="+intoIDj.ToString() + ", 下個節點是i="+intoIDi.ToString()+", j="+(intoIDj-1).ToString());
			}
		}
	}
	
	
	*/
	

}