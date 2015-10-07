using UnityEngine;
using System.Collections;

public class oldRotateArrayObj : MonoBehaviour {
	public GameObject nodeObj;
	public int node = 4, layer = 4 ; //節點數量,階層數 如果系統未給予指定數量時起始為4,4
	public float picNumber = 36; //圖示的張數
	int[,] QuestionNodeType; //設定題目節點的圖示屬性
	bool[,] QuestionNodeSwish; //紀錄目前進行到哪個節點
	GameObject[,] QuestionNodeObj_prefab; //設定題目各節點物件用
	GameObject[,] NodeObj_prefab; //設定各節點物件用
	
	//圍繞試驗用
	//GameObject refObj;
	
	
	int delayedTime = 1; //延遲時間
	float updateTime; //紀錄是否到更新時間
	
	
	
	// Use this for initialization
	void Start () {
		node = node+2;//預留位移空間用
		layer = layer +2;//頭尾排起始與終點顯示用
		QuestionNodeType = new int[node,layer];
		QuestionNodeSwish = new bool[node,layer];
		QuestionNodeObj_prefab = new GameObject [node,layer];
		NodeObj_prefab = new GameObject [node,layer];
		
		//圍繞試驗用
		//refObj = GameObject.Instantiate(nodeObj,new Vector3(0,5,0),Quaternion.identity) as GameObject;
		
		
		//產生所有物件與設定與亂數題目屬性
		//autoCreatQuessionNodeObjAndSetup();
		
		//   上下選一作為起始設定
		
		//產生自訂題目物件與設定
		//creatCustomizationsQuesionAndSet();
		
		nodeObj.transform.position = new Vector3(0,5,0);
	}
	
	// Update is called once per frame
	void Update () 
	{	
		//當超過更新時間就做動作
		if(Time.time >= updateTime)
		{
			//更新下次的更新時間
			updateTime = Time.time+delayedTime;
			
			/*
			Vector3 newPos;
			rotatePoint(new Vector3(0,0,0), nodeObj.transform.position,10,out newPos);
			nodeObj.transform.position = newPos;
			*/
			

		}
		
		//圍繞試驗用
		//圍繞某座標旋轉(軸心座標, 軸心旋轉軸向, 旋轉角度)
		//nodeObj.transform.RotateAround(new Vector3(0,0,0),Vector3.forward,5); 
		//圍繞某座標旋轉並自轉
		//refObj.transform.RotateAround(new Vector3(0,0,0),Vector3.forward,5); 
		//refObj.renderer.enabled = false;
		//nodeObj.transform.position = refObj.transform.position;
		//nodeObj.transform.Rotate(Vector3.forward,30);

	}
	
	//旋轉所有物件座標
	void rotateAllNodePoint(Vector3[,] targetPosArray, GameObject[,] targetNodeObj_prefab, out Vector3[,] newPosArray, out GameObject[,] newNodeObj_prefab, out float[,] rotateZAngleArray)
	{
		newPosArray = new Vector3[node,layer];
		newPosArray = targetPosArray;
		newNodeObj_prefab = new GameObject[node,layer];
		newNodeObj_prefab = targetNodeObj_prefab;
		rotateZAngleArray = new float[node,node]; //紀錄物件的選轉角度
		
		//要旋轉第幾列(上下為同一列)
		for(int j = 0;j<layer;j++)
		{
			for(int i = 1;i<node-1;i++)
			{
				newPosArray[i,j].x = 3+(j*2); //依照階層將節點物件上下距離分開
				rotatePoint(new Vector3(0,0,0), targetPosArray[i,j], (360/(node-2))*(i-1), out targetPosArray[i,j]);
				rotateZAngleArray[i,j] = (360/(node-2))*(i-1);
				print("z="+rotateZAngleArray[i,j].ToString());
			}
		}
	}
	
	//旋轉物件位置(旋轉軸心座標, 要被旋轉的目標座標, 旋轉角度, 旋轉後的座標)
	void rotatePoint(Vector3 center, Vector3 targetPos, float angle, out Vector3 newPos)  
    {  
  		newPos = new Vector3(); //用來記錄旋轉後的座標

		float angleHude = angle * Mathf.PI / 180; //旋轉弧度(角度变成弧度) 
		float x1 = (targetPos.x - center.x) * Mathf.Cos(angleHude) + (targetPos.y - center.y ) * Mathf.Sin(angleHude) + center.x;  
		float y1 = -(targetPos.x - center.x) * Mathf.Sin(angleHude) + (targetPos.y - center.y) * Mathf.Cos(angleHude) + center.y;  
		newPos.x = (int)x1;  
		newPos.y = (int)y1;  
		newPos.z = 0; 
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
		
		
		
		Vector3[,] QuestionNodePos = new Vector3[node,layer]; //設定節點物件位置用
		float[,] rotateZAngleArray = new float[node,layer]; //紀錄z軸旋轉角度
		rotateAllNodePoint(QuestionNodePos,QuestionNodeObj_prefab,out QuestionNodePos,out NodeObj_prefab,out rotateZAngleArray);
	
		//產生節點物件並給予名稱與座標ID
		for(int i = 1;i<node-1;i++)
		{
			for(int j = 0;j<layer;j++)
			{
				QuestionNodeObj_prefab[i,j] = GameObject.Instantiate(nodeObj,QuestionNodePos[i,j],Quaternion.identity) as GameObject;
				QuestionNodeObj_prefab[i,j].name = QuestionNodeObj_prefab[i,j].name + "Question i=" + i.ToString() + ",j=" + j.ToString()+", Tyep="+QuestionNodeType[i,j].ToString();
				NodeObj_prefab[i,j] = QuestionNodeObj_prefab[i,j];
				//將節點依照屬性改變其貼圖圖示
				changTexture(j,QuestionNodeType[i,j],QuestionNodeObj_prefab[i,j]);
				//旋轉物件
				//QuestionNodeObj_prefab[i,j].transform.Rotate(0,0,rotateZAngleArray[i,j]);
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
		Vector3[,] QuestionNodePos = new Vector3[node,layer]; //設定節點物件位置用
		float[,] rotateZAngleArray = new float[node,layer]; //紀錄z軸旋轉角度
		rotateAllNodePoint(QuestionNodePos,NodeObj_prefab,out QuestionNodePos,out NodeObj_prefab, out rotateZAngleArray);
		for(int i = 1; i<node-1; i++)
		{
			for(int j = 0; j<layer; j++)
			{
				//亂數決定節點類型(1~3)(排除產屬性為4的上向下節點)
				int rnd = Random.Range(1,4);
				QuestionNodeType[i,j] = rnd;
				
				//產生節點物件並給予名稱與座標ID
				QuestionNodeObj_prefab[i,j] = GameObject.Instantiate(nodeObj,QuestionNodePos[i,j],Quaternion.identity) as GameObject;
				QuestionNodeObj_prefab[i,j].name = QuestionNodeObj_prefab[i,j].name + "Question i=" + i.ToString() + ",j=" + j.ToString()+", Tyep=" + rnd.ToString();
				NodeObj_prefab[i,j] = QuestionNodeObj_prefab[i,j];
				//將節點依照屬性改變其貼圖圖示
				changTexture(j,rnd,QuestionNodeObj_prefab[i,j]);
				//旋轉物件
				//NodeObj_prefab[i,j].transform.Rotate(0,0,rotateZAngleArray[i,j]);
				
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
}
