using UnityEngine;
using System.Collections;

public class CreatQuestionAndSloving : MonoBehaviour {
	//public GameObject cube;
	public GameObject nodeObj;
	public int node = 4, layer = 4 ; //節點數量,階層數 如果系統未給予指定數量時起始為4,4
	public float picNumber = 36; //圖示的張數
	int[,] QuestionNodeType; //設定題目節點的圖示屬性
	bool[,] QuestionNodeSwish; //紀錄目前進行到哪個節點
	GameObject[,] QuestionNodeObj_prefab; //設定題目各節點物件用
	GameObject[,] NodeObj_prefab; //設定各節點物件用
	
	int[] typeArray; //紀錄藥用哪些節點功能當作題目
	
	// Use this for initialization
	void Start () 
	{
		node = node+2;//預留位移空間用
		layer = layer +2;//頭尾排起始與終點顯示用
		QuestionNodeType = new int[node,layer];
		QuestionNodeSwish = new bool[node,layer];
		QuestionNodeObj_prefab = new GameObject [node,layer];
		NodeObj_prefab = new GameObject [node,layer];
		
		//設定題目節點功能有哪些
		typeArray = new int[7]{1,2,3,5,6,7,8};
		
		//產生所有物件與設定與亂數題目屬性
		autoCreatQuessionNodeObjAndSetup();
		
		//   上下選一作為起始設定
		
		//產生自訂題目物件與設定
		//creatCustomizationsQuesionAndSet();
		
		
		//檢查起始問題路線是否到達終點
		bool targetSlovingQuestionSwish=false, autoSlovingQuestionSwish = false;
		checkQuestion(QuestionNodeType,QuestionNodeSwish, out targetSlovingQuestionSwish);
		
		//更新圖示
		for(int j=1;j<layer-1;j++)
		{
			for(int i=1;i<node-1;i++)
			{
				changTexture(j,QuestionNodeType[i,j],QuestionNodeObj_prefab[i,j]);
			}
		}
		
		//更新節點透明度
		checkAllNodeSwish(QuestionNodeObj_prefab,QuestionNodeSwish);
		
		//自動解題
		//autoSlovingQuestion(targetSlovingQuestionSwish, QuestionNodeType, out autoSlovingQuestionSwish);
		
		if (targetSlovingQuestionSwish || autoSlovingQuestionSwish){
		print ("成功解題!!!!");
		}

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("space"))
		{
			//亂數產生題目
			rndQuession(QuestionNodeType,QuestionNodeSwish, out QuestionNodeType,out QuestionNodeSwish);
			
			//檢查起始問題路線是否到達終點
			bool targetSlovingQuestionSwish=false, autoSlovingQuestionSwish = false;
			checkQuestion(QuestionNodeType,QuestionNodeSwish, out targetSlovingQuestionSwish);
			
			//更新圖示
			for(int j=1;j<layer-1;j++)
			{
				for(int i=1;i<node-1;i++)
				{
					changTexture(j,QuestionNodeType[i,j],QuestionNodeObj_prefab[i,j]);
				}
			}
		
			//更新節點透明度
			checkAllNodeSwish(QuestionNodeObj_prefab,QuestionNodeSwish);
		
			if (targetSlovingQuestionSwish || autoSlovingQuestionSwish){
			print ("新題目成功解題!!!!");
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
		
		//只產生一個旋轉節點
		rndIDi = Random.Range(1,node-1);
		rndIDj = Random.Range(1,layer-1);
		int rndTurnNodeType = Random.Range(5,9);//亂數產生(5~8)
		newType[rndIDi,rndIDj] = rndTurnNodeType;
		//changTexture(rndIDj,targetType,QuestionNodeObj_prefab[rndIDi,rndIDj]);
		print("亂數節點IDij="+rndIDi.ToString()+rndIDj.ToString());
		
		//只產生一個向下節點
		rndIDi = Random.Range(1,node-1);
		newType[rndIDi,layer-2]=4;
		changTexture(layer-2,4,QuestionNodeObj_prefab[rndIDi,layer-2]);
		newSwish[rndIDi,layer-2]=true;
		QuestionNodeObj_prefab[rndIDi,layer-2].renderer.material.color = new Color(1,1,1,0.5f);
		print("起始向下節點  ij="+rndIDi.ToString()+(layer-2).ToString()+", type="+newSwish[rndIDi,layer-2].ToString());
		
		//防止各層沒有向下節點,每層都產生一個
		for(int j = 1;j<layer-2;j++)
		{
			rndIDi = Random.Range(1,node-1);
			newType[rndIDi,j]=4;
			changTexture(j,4,QuestionNodeObj_prefab[rndIDi,j]);
		}
	}
	
	//起始亂數產生節點與其屬性設定
	void autoCreatQuessionNodeObjAndSetup() 
	{
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
				QuestionNodeType[i,j] = rndType;
				
				//產生節點物件並給予名稱與座標ID
				QuestionNodeObj_prefab[i,j] = GameObject.Instantiate(nodeObj,pos,Quaternion.identity) as GameObject;
				QuestionNodeObj_prefab[i,j].name = QuestionNodeObj_prefab[i,j].name + "Question i=" + i.ToString() + ",j=" + j.ToString()+", Tyep=" + rndType.ToString();
				NodeObj_prefab[i,j] = QuestionNodeObj_prefab[i,j];
				//將節點依照屬性改變其貼圖圖示
				changTexture(j,rndType,QuestionNodeObj_prefab[i,j]);
				
			}
		}
		
		//設定所有節點開關起始屬性為關閉
		turnOffAllNodeSwish(QuestionNodeSwish);
		
		//亂數設定ID用
		int rndIDi,rndIDj;
		
		//只產生一個旋轉節點
		rndIDi = Random.Range(1,node-1);
		rndIDj = Random.Range(1,layer-1);
		int rndTurnNodeType = Random.Range(5,9);//亂數產生(5~8)
		QuestionNodeType[rndIDi,rndIDj] = rndTurnNodeType;
		changTexture(rndIDj,rndTurnNodeType,QuestionNodeObj_prefab[rndIDi,rndIDj]);
		print("亂數節點IDij="+rndIDi.ToString()+rndIDj.ToString());
		
		//只產生一個向下節點
		rndIDi = Random.Range(1,node-1);
		QuestionNodeType[rndIDi,layer-2]=4;
		changTexture(layer-2,4,QuestionNodeObj_prefab[rndIDi,layer-2]);
		QuestionNodeSwish[rndIDi,layer-2]=true;
		QuestionNodeObj_prefab[rndIDi,layer-2].renderer.material.color = new Color(1,1,1,0.5f);
		print("起始向下節點  ij="+rndIDi.ToString()+(layer-2).ToString()+", type="+QuestionNodeSwish[rndIDi,layer-2].ToString());
		
		//防止各層沒有向下節點,每層都產生一個
		for(int j = 1;j<layer-2;j++)
		{
			rndIDi = Random.Range(1,node-1);
			QuestionNodeType[rndIDi,j]=4;
			changTexture(j,4,QuestionNodeObj_prefab[rndIDi,j]);
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
		QuestionNodeType[4,3]=7;
			
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
		
		Vector3 pos= new Vector3(0,0,0) ;//指定物件位置用
		
		//排列擺放節點物件
		for(int i = 1; i<node-1; i++)
		{
			for(int j = 0; j<layer; j++)
			{
				
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
				
				//產生節點物件並給予名稱與座標ID
				QuestionNodeObj_prefab[i,j] = GameObject.Instantiate(nodeObj,pos,Quaternion.identity) as GameObject;
				QuestionNodeObj_prefab[i,j].name = QuestionNodeObj_prefab[i,j].name + "Question i=" + i.ToString() + ",j=" + j.ToString()+", Tyep="+QuestionNodeType[i,j].ToString();
				//將節點依照屬性改變其貼圖圖示
				changTexture(j,QuestionNodeType[i,j],QuestionNodeObj_prefab[i,j]);
				
			}
		}
		changTexture(intoIDj, QuestionNodeType[intoIDi,intoIDj], QuestionNodeObj_prefab[intoIDi,intoIDj]);
		QuestionNodeObj_prefab[intoIDi,intoIDj].renderer.material.color = new Color(1,1,1,0.5f);
		
		//設定亮度與
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
					targetNodeObj[i,j].renderer.material.color = new Color(1,1,1,0.5f);
				}
				else{
					targetNodeObj[i,j].renderer.material.color = new Color(1,1,1,1);
				}
			}
		}
	}
	
	//自動解題-如未達終點則自動位移節點嘗試解題(目標題目解題狀態, 目標節點數值陣列, 目標節點屬性, 目標解題狀態)
	void autoSlovingQuestion(bool targetSlovingQuestionSwish, int[,] targetType, out bool autoSlovingQuestionSwish)
	{
		//設定起始解題狀態
		autoSlovingQuestionSwish=targetSlovingQuestionSwish;
		
		//如果未達成解題各階層位移一次自動解題
		int[,,] saveDisplacementAllLayerType = new int [node,layer,(layer-2)*2]; //用來記錄所有位移的陣列數值結果(節點數, 階層數, 位移階層次數(用來記錄各位移後的階層編號))
		bool[,,] saveDisplacementAllLayerSwish = new bool [node,layer,(layer-2)*2]; //用來記錄所有位移的陣列屬性結果(節點數, 階層數, 位移階層次數(用來記錄各位移後的階層編號)
		if(!targetSlovingQuestionSwish)
		{
			//所有階層節點屬性往右位移
			tryDisplacementAllLayerSlovingType(0, targetSlovingQuestionSwish, targetType, saveDisplacementAllLayerType, saveDisplacementAllLayerSwish, out autoSlovingQuestionSwish, out saveDisplacementAllLayerType, out saveDisplacementAllLayerSwish);
			
			//所有階層節點屬性往左位移
			tryDisplacementAllLayerSlovingType(1, targetSlovingQuestionSwish, targetType, saveDisplacementAllLayerType, saveDisplacementAllLayerSwish, out autoSlovingQuestionSwish, out saveDisplacementAllLayerType, out saveDisplacementAllLayerSwish);
			
			//產生所有陣列的節點物件
			for(int layerID = 0;layerID<(layer-2)*2;layerID++)
			{
				int[,] oldType = new int[node,layer];
				bool[,] oldSwish = new bool[node,layer];
				for(int i =1;i<node-1;i++)
				{
					for(int j =1;j<layer-1;j++)
					{
						oldType[i,j] = saveDisplacementAllLayerType[i,j,layerID];
						oldSwish[i,j] = saveDisplacementAllLayerSwish[i,j,layerID];
					}
				}
				GameObject[,] newNodeObj_prefab = new GameObject[node,layer];
				creatNewNodeObjAndSetup(0, 0, layerID*6+10, oldType, out newNodeObj_prefab);
				//print ("開始更換節點透明度");
				checkAllNodeSwish(newNodeObj_prefab, oldSwish);
			}
		}
		
		//多次解題
		/*
		//如果未達成解題各階層位移二次自動解題
		int[,,] saveDisplacementAllLayerType2 = new int [node,layer,(layer-2)*2]; //用來記錄所有位移的陣列數值結果(節點數, 階層數, 位移階層次數(用來記錄各位移後的階層編號))
		bool[,,] saveDisplacementAllLayerSwish2 = new bool [node,layer,(layer-2)*2]; //用來記錄所有位移的陣列屬性結果(節點數, 階層數, 位移階層次數(用來記錄各位移後的階層編號))
		if(!targetSlovingQuestionSwish)
		{
			for(int ArrayID2 = 0;ArrayID2<(layer-2)*2;ArrayID2++)
			{
				int[,] oldType = new int[node,layer];
				bool[,] oldSwish = new bool[node,layer];
				for(int i =1;i<node-1;i++)
				{
					for(int j =1;j<layer-1;j++)
					{
						oldType[i,j]= saveDisplacementAllLayerType[i,j,ArrayID2];
						oldSwish[i,j]= saveDisplacementAllLayerSwish[i,j,ArrayID2];
					}
				}
				//所有階層節點屬性往右位移
				tryDisplacementAllLayerSlovingType(0, targetSlovingQuestionSwish, oldType, saveDisplacementAllLayerType2, saveDisplacementAllLayerSwish2, out autoSlovingQuestionSwish, out saveDisplacementAllLayerType2, out saveDisplacementAllLayerSwish2);
				
				//所有階層節點屬性往左位移
				tryDisplacementAllLayerSlovingType(1, targetSlovingQuestionSwish, oldType, saveDisplacementAllLayerType2, saveDisplacementAllLayerSwish2, out autoSlovingQuestionSwish, out saveDisplacementAllLayerType2, out saveDisplacementAllLayerSwish2);
				
				//產生所有陣列的節點物件
				for(int ArrayID = 0;ArrayID<(layer-2)*2;ArrayID++)
				{
					int[,] newType = new int[node,layer];
					bool[,] newSwish = new bool[node,layer];
			
					for(int i =1;i<node-1;i++)
					{
						for(int j =1;j<layer-1;j++)
						{
						newType[i,j]= saveDisplacementAllLayerType2[i,j,ArrayID];
						newSwish[i,j]= saveDisplacementAllLayerSwish2[i,j,ArrayID];
						}
					}
				
					GameObject[,] newNodeObj_prefab = new GameObject[node,layer];
					creatNewNodeObjAndSetup(ArrayID*-6-10, 0, ArrayID2*6+10, newType, out newNodeObj_prefab);
					//print ("開始更換節點透明度");
					checkAllNodeSwish(newNodeObj_prefab, newSwish);
				}
			}
		}
		
		
		//如果未達成解題各階層位移三次自動解題
		int[,,] saveDisplacementAllLayerType3A = new int [node,layer,(layer-2)*2]; //用來記錄所有位移的陣列數值結果(節點數, 階層數, 位移階層次數(用來記錄各位移後的階層編號))
		bool[,,] saveDisplacementAllLayerSwish3A = new bool [node,layer,(layer-2)*2]; //用來記錄所有位移的陣列屬性結果(節點數, 階層數, 位移階層次數(用來記錄各位移後的階層編號))
		//int[,,,] saveDisplacementAllLayerType3B = new int [node,layer,(layer-2)*2,(layer-2)*2]; //用來記錄所有位移的陣列數值結果(節點數, 階層數, 位移階層次數(用來記錄各位移後的階層編號))
		//bool[,,,] saveDisplacementAllLayerSwish3B = new bool [node,layer,(layer-2)*2,(layer-2)*2]; //用來記錄所有位移的陣列屬性結果(節點數, 階層數, 位移階層次數(用來記錄各位移後的階層編號))
		
		if(!targetSlovingQuestionSwish)
		{
			for(int ArrayID3 = 0;ArrayID3<(layer-2)*2;ArrayID3++)
			{
				for(int ArrayID2 = 0;ArrayID2<(layer-2)*2;ArrayID2++)
				{
					int[,] oldType = new int[node,layer];
					bool[,] oldSwish = new bool[node,layer];
					for(int i =1;i<node-1;i++)
					{
						for(int j =1;j<layer-1;j++)
						{
							oldType[i,j]= saveDisplacementAllLayerType2[i,j,ArrayID2];
							oldSwish[i,j]= saveDisplacementAllLayerSwish2[i,j,ArrayID2];
						}
					}
			
					//所有階層節點屬性往右位移
					tryDisplacementAllLayerSlovingType(0, targetSlovingQuestionSwish, oldType, saveDisplacementAllLayerType3A, saveDisplacementAllLayerSwish3A, out autoSlovingQuestionSwish, out saveDisplacementAllLayerType3A, out saveDisplacementAllLayerSwish3A);
				
					//所有階層節點屬性往左位移
					tryDisplacementAllLayerSlovingType(1, targetSlovingQuestionSwish, oldType, saveDisplacementAllLayerType3A, saveDisplacementAllLayerSwish3A, out autoSlovingQuestionSwish, out saveDisplacementAllLayerType3A, out saveDisplacementAllLayerSwish3A);
				
					//產生所有陣列的節點物件
					for(int ArrayID = 0;ArrayID<(layer-2)*2;ArrayID++)
					{
						int[,] newType = new int[node,layer];
						bool[,] newSwish = new bool[node,layer];
				
						for(int i =1;i<node-1;i++)
						{
							for(int j =1;j<layer-1;j++)
							{
							newType[i,j]= saveDisplacementAllLayerType3A[i,j,ArrayID];
							newSwish[i,j]= saveDisplacementAllLayerSwish3A[i,j,ArrayID];
							}
						}
				
						GameObject[,] newNodeObj_prefab = new GameObject[node,layer];
						creatNewNodeObjAndSetup(ArrayID*-6-10, ArrayID3*8+10, ArrayID2*6+10, newType, out newNodeObj_prefab);
						//print ("開始更換節點透明度");
						checkAllNodeSwish(newNodeObj_prefab, newSwish);
					}
				}
			}
		}
		*/
		
	}
		
	//嘗試各階層位移解題模式(解題模式 0=往右位移, 1=往左位移, 目標解題狀態, 目標數值陣列, 目標解題狀態)
	void tryDisplacementAllLayerSlovingType(int SlovingType, bool targetSlovingQuestionSwish, int[,] targetType, int[,,] oldDisplacementAllLayerType, bool[,,] oldDisplacementAllLayerSwish, out bool autoSlovingQuestionSwish,out int[,,] newDisplacementAllLayerType, out bool[,,] newDisplacementAllLayerSwish)
	//void tryDisplacementAllLayerSlovingType(int SlovingType, bool targetSlovingQuestionSwish, int[,] targetType, int[,,] oldDisplacementAllLayerType, bool[,,] oldDisplacementAllLayerSwish, int[,,,] oldDisplacementAllLayerType2, bool[,,,] oldDisplacementAllLayerSwish2, out bool autoSlovingQuestionSwish,out int[,,] newDisplacementAllLayerType, out bool[,,] newDisplacementAllLayerSwish, out int[,,,] newDisplacementAllLayerType2, out bool[,,,] newDisplacementAllLayerSwish2)
	{
		//設定起始屬性
		newDisplacementAllLayerType = new int[node,layer,(layer-2)*2];
		newDisplacementAllLayerSwish = new bool[node,layer,(layer-2)*2];
		//設定起始解題狀態
		autoSlovingQuestionSwish=targetSlovingQuestionSwish;
		//紀錄位移解題次數
		int countForCheckQuestion=1;
		//指定位移層ID用
		int layerID=1;
		
		//開始往右位移自動解題
		print("開始位移自動解題,j=" + layerID.ToString());
		//紀錄位移前與新的節點數值陣列
		int[,] newType = new int[node,layer];
		int[,] oldType = new int[node,layer];
			
			
		//當題目未能達到終點則持續做
		while (!targetSlovingQuestionSwish)
		{
			//如果指定位移層沒有超過最上層5
			if(layerID<layer-1)
			{
				print("開始自動往右位移,layerID=" + layerID.ToString());
				//恢復起始題目類型屬性給新陣列
				oldType = targetType;
				newType = targetType;
				bool[,] newSwish = new bool[node,layer];
					
					
				//尋找起始節點是否與題目相同IDij
				//紀錄起始ID
				int newfirstIDi, newfirstIDj,QfirstIDi, QfirstIDj;
				findStartNodeType(oldType, newSwish, out newfirstIDi, out newfirstIDj);
				findStartNodeType(QuestionNodeType, QuestionNodeSwish, out QfirstIDi, out QfirstIDj);
				print ("尋找起始節點是否與題目相同IDij="+newfirstIDi.ToString()+newfirstIDj.ToString()+"起始IDij="+QfirstIDi.ToString()+QfirstIDj.ToString());
					  
				
				if(SlovingType==0)
				{
					//往右位移節點來看嘗試解題可能
					DisplacementOneLayerTypeAndCreatNewNode(0,layerID,oldType,out newType);
				}
				else if(SlovingType==1)
				{
					//往左位移節點來看嘗試解題可能
					DisplacementOneLayerTypeAndCreatNewNode(1,layerID,oldType,out newType);
				}
				
				//重置節點開關屬性
				turnOffAllNodeSwish(newSwish);
				
				//尋找檢查的起始節點IDij
				findStartNodeType(newType, newSwish, out newfirstIDi, out newfirstIDj);
				print ("找完起始節點後ID ij="+newfirstIDi.ToString()+newfirstIDj.ToString());
					
				//檢查題目起始點最終會停到哪
				print("檢查slovingQuestionSwish屬性為"+targetSlovingQuestionSwish.ToString());
				//print("!!     開始檢查第"+countForCheckQuestion.ToString()+"次往右移動完後是否能完成解題");
				checkQuestion(newType, newSwish, out autoSlovingQuestionSwish);
				//print("!!     檢查完後slovingQuestionSwish屬性為"+slovingQuestionSwish.ToString());
					
				//將結果紀下來用來回傳
				for(int i = 1;i<node-1;i++)
				{
					for(int j = 1;j<layer-1;j++)
					{
						if(SlovingType==0)
						{
							oldDisplacementAllLayerType[i,j,layerID-1] = newType[i,j];
							oldDisplacementAllLayerSwish[i,j,layerID-1] = newSwish[i,j];
						}
						else if(SlovingType==1)
						{
							oldDisplacementAllLayerType[i,j,layerID-1+layer-2] = newType[i,j];
							oldDisplacementAllLayerSwish[i,j,layerID-1+layer-2] = newSwish[i,j];
						}
					}	
				}
				newDisplacementAllLayerType = oldDisplacementAllLayerType;
				newDisplacementAllLayerSwish = oldDisplacementAllLayerSwish;
				
				layerID++;
				countForCheckQuestion++;

			}
			else{
				if(SlovingType==0)
				{
					print("!!　　　　超過往右位移自動解題能力範圍,layerID=" + layerID.ToString());
					break;
				}
				else if(SlovingType==1)
				{
					print("!!　　　　超過往左位移自動解題能力範圍,layerID=" + layerID.ToString());
					break;
				}
			}
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
				print("開始往右位移存到新陣列");
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
				print("開始往左位移存到新陣列");
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
	
	//產生新的節點物件陣列(與起始物件不同的位移x,y,z座標)
	void creatNewNodeObjAndSetup(int MoveX,int MoveY,int MoveZ,int[,] targetType, out GameObject[,] newNodeObj_prefab) 
	{
		Vector3 newObjVector3;
		newNodeObj_prefab = new GameObject[node,layer];
		for(int i = 1; i<node-1; i++)
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
				GameObject newObj_prefab  = GameObject.Instantiate(nodeObj,newObjVector3,Quaternion.identity) as GameObject;
				newObj_prefab.name = "newOBJ"+newObj_prefab.name + "i=" + i.ToString() + ",j=" + j.ToString();
				newObj_prefab.transform.renderer.material.color = new Color(1,1,1);
				newNodeObj_prefab[i,j] = newObj_prefab;
				//將節點依照屬性改變其貼圖圖示
				changTexture(j,targetType[i,j],newObj_prefab);
				
			}
		}
	}
	
	/*
	//方法回傳測試
	void returnOut(out int i)
	{
		i = 1;
	}
	
	//使用方法回傳值寫法
	void getOut()
	{
		int i; 
		returnOut(out i); //利用returnOut傳回值給i
		//i 這時會變成1
		print (i.ToString());
	}
	*/
}


