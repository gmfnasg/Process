using UnityEngine;
using System.Collections;
//隨機產生陣列並檢查是否可以達到終點
//目標-每次更新檢查是否有達到終點
public class CreatArray7 : MonoBehaviour {
	public GameObject cube;
	public int node = 4, layer = 4 ; //節點數量,階層數
	Vector3[,] NodePOS; //紀錄並設定每個節點的座標
	int [,] NodeType; //設定節點的圖示屬性
	bool[,] NodeSwish; //紀錄目前進行到哪個節點
	GameObject[,] Cube_prefab; //設定各節點物件用
	float CheckTime; //紀錄下次要更新動作的時間
	int TimeInterval = 10; //每次動作的時間間隔
	int checki=0,checkj=0; // runAllNodeSwish用來紀錄判定到哪個陣列ID
	int intoIDi=0,intoIDj=0; // 用來記錄目前停留在哪個節點ID
	public float picNumber = 36; //圖示的張數
	
	
	bool checkRouteStatus = false;//檢查題目路線狀態
	int firstI,firstJ;//紀錄起始點座標
	int checkQuestionI,checkQuestionJ;//checkQuestion用來紀錄判定到哪個陣列ID

	
	// Use this for initialization
	void Start () {
		
		layer += 2; //+2為頭尾排預留用
		NodePOS = new Vector3[node, layer];
		NodeType = new int[node, layer]; 
		NodeSwish = new bool[node, layer]; 
		Cube_prefab = new GameObject[node, layer];
		CheckTime=Time.deltaTime * TimeInterval;//設定遊戲開始後多久開始執行第一次動作
		
		//產生所有物件與設定
		creatCubeAndSetup();
	
		
		//檢查路線是否到達終點
		checkQuestion();
		
		//設定參考物件
		cube.transform.position =  new Vector3(0, 0, 1.5f); //設定參考物件位置
		cube.renderer.material.color = new Color (1, 235/255, 4/255, 0.5f); //設定參考物件顏色
	}
	
	// Update is called once per frame
	void Update () 
	{
		//更新所有節點透明度
		checkAllNodeSwish();
		
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
					intoIDi++;
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
	
	//檢查題目起始點是否可以到達終點
	void checkQuestion()
	{
		//檢查ID用
		checkQuestionI=firstI;
		checkQuestionJ=firstJ;
		int i=0;
		print("起始設定為"+checkRouteStatus.ToString());
		
		while (!checkRouteStatus)
		{
			i++;
			print("第"+i.ToString()+"次執行，狀態為"+checkRouteStatus.ToString());
			gotoNextNodeForcheckQuestion();
		}
		
	}
	
	//自動檢查更新
	void gotoNextNodeForcheckQuestion()
	{
		if(NodeType[checkQuestionI,checkQuestionJ]==0)
		{
			print("改之前"+checkRouteStatus.ToString());
			checkRouteStatus = true;
			print("改之後"+checkRouteStatus.ToString());
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
					print("改之前"+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後"+checkRouteStatus.ToString());
					print(checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 無法往左前進到i="+(checkQuestionI-1).ToString()+", j="+checkQuestionJ.ToString()+", Type="+NodeType[checkQuestionI-1,checkQuestionJ].ToString());
				}
				else if(NodeType[checkQuestionI-1,checkQuestionJ] == 0)
				{
					print("改之前"+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後"+checkRouteStatus.ToString());
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
					print("改之前"+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後"+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 無法往左前進到i="+(node-1).ToString()+", j="+checkQuestionJ.ToString()+", Type="+NodeType[node-1,checkQuestionJ].ToString());
				}
				else if(NodeType[node-1,checkQuestionJ] == 0)
				{
					print("改之前"+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後"+checkRouteStatus.ToString());
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
					print("改之前"+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後"+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 無法往右前進到i="+(checkQuestionI+1).ToString()+", j="+checkQuestionJ.ToString()+", Type="+NodeType[checkQuestionI+1,checkQuestionJ].ToString());
				}
				else if(NodeType[checkQuestionI+1,checkQuestionJ] == 0)
				{
					print("改之前"+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後"+checkRouteStatus.ToString());
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
					print("改之前"+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後"+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 無法往右前進到i=0, j="+checkQuestionJ.ToString()+", Type="+NodeType[0,checkQuestionJ].ToString());
				}
				else if(NodeType[0,checkQuestionJ] == 0)
				{
					print("改之前"+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後"+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 會跑進黑洞i=0, j="+checkQuestionJ.ToString());
				}
				
				//可以前進時
				else
				{
					//如果下個此節點沒有重複進入過
					if(!NodeSwish[0,checkQuestionJ])
					{
						//將停留節點往右移動
//要同步修改---------------------------------注意!!
						checkQuestionI=0;//<--
//要同步修改---------------------------------注意!!
						NodeSwish[checkQuestionI,checkQuestionJ] = true;
						print("目前在i=" +(node-1).ToString()+", j="+checkQuestionJ.ToString() + ", 往右移動節點到i="+checkQuestionI.ToString()+", j="+checkQuestionJ.ToString()+", Type="+NodeType[checkQuestionI,checkQuestionJ].ToString());
					}
					else{
						checkRouteStatus = true;
						print ("重複迴圈");
					}
				}
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
					print("改之前"+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後"+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 無法往上前進到i="+checkQuestionI.ToString()+", j="+(checkQuestionJ+1).ToString()+", Type="+NodeType[checkQuestionI,checkQuestionJ+1].ToString());
				}
				else if(NodeType[checkQuestionI,checkQuestionJ+1] == 0)
				{
					print("改之前"+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後"+checkRouteStatus.ToString());
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
				print("改之前"+checkRouteStatus.ToString());
				checkRouteStatus = true;
				print("改之後"+checkRouteStatus.ToString());
				print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 會超過邊界到i="+checkQuestionI.ToString()+", j="+(checkQuestionJ+1).ToString());
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
					print("改之前"+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後"+checkRouteStatus.ToString());
					print("目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 無法往下前進到i="+checkQuestionI.ToString()+", j="+(checkQuestionJ-1).ToString()+", Type="+NodeType[checkQuestionI,checkQuestionJ-1].ToString());
				}
				else if(NodeType[checkQuestionI,checkQuestionJ-1] == 0)
				{
					print("改之前"+checkRouteStatus.ToString());
					checkRouteStatus = true;
					print("改之後"+checkRouteStatus.ToString());
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
				print("改之前"+checkRouteStatus.ToString());
				checkRouteStatus = true;
				print("改之後"+checkRouteStatus.ToString());
				print("Question Success!! 目前在i=" +checkQuestionI.ToString()+", j="+checkQuestionJ.ToString() + ", 下個節點是i="+checkQuestionI.ToString()+", j="+(checkQuestionJ-1).ToString());
			}
		}
	}
}