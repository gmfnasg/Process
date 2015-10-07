using UnityEngine;
using System.Collections;

public class CreatArray2 : MonoBehaviour {
	public GameObject cube;
	int node = 4, layer = 4 ; 
	Vector3[,] NodePOS;
	int [,] NodeType; 
	bool[,] NodeSwish;
	string objname;
	GameObject[,] Cube_prefab;
	float CheckTime;

	int checki=0,checkj=0;
	
	// Use this for initialization
	void Start () {
		layer += 2;
		NodePOS = new Vector3[node, layer];
		NodeType = new int[node, layer]; 
		NodeSwish = new bool[node, layer]; 
		Cube_prefab = new GameObject[node, layer];
		CheckTime=Time.deltaTime * 15;
		
		for(int i = 0; i<node; i++)
		{
			for(int j = 0; j<layer; j++)
			{
				NodeSwish[i,j] = false;
			}
		}
		
		NodeSwish[Random.Range(0, node-1), layer-2 ] = true;
		
		for(int i = 0; i<node; i++)
		{
			for(int j = 0; j<layer; j++)
			{
				NodePOS[i,j].x=i-1.5f;
				NodePOS[i,j].y=j-2.5f;
				NodePOS[i,j].z=-1;
				Cube_prefab[i,j] = GameObject.Instantiate(cube,NodePOS[i,j],Quaternion.identity) as GameObject;
				Cube_prefab[i,j].name = Cube_prefab[i,j].name + "i=" + i.ToString() + ",j=" + j.ToString();
				
				if(NodeSwish[i,j])
				{
					Cube_prefab[i,j].name = Cube_prefab[i,j].name + "(SwishOn)" + "i=" + i.ToString() + ",j=" + j.ToString();
					//exp: GameObject bullet_prefab = GameObject.Instantiate(bullet,Bullet_Pos.position,Quaternion.identity) as GameObject;
					objname = Cube_prefab[i,j].name;
				}
				
				NodeType[i,j] = Random.Range(0,3);
				
				changToColor(i,j);
			}
		}
		cube.transform.position =  new Vector3(0, 0, -1.5f);
		cube.renderer.material.color = new Color (1, 235/255, 4/255, 0);
		
		GameObject swishon = GameObject.Find(objname);
		swishon.renderer.material.color = Color.white;
	}
	
	// Update is called once per frame
	void Update () 
	{
		changAll2();
		checkAllNodeSwish();
	}
	
	void changAll()
	{
		if(checkj == 0){
			checkj = layer-2;
		}
		if(Time.time >= CheckTime)
		{
			CheckTime = Time.time + Time.deltaTime;

			if(checki<node)
			{
				Cube_prefab[checki,checkj].transform.renderer.material.color = Color.white;
				//print("checki="+checki.ToString()+", checkj="+checkj.ToString());
				checki++;
			}
			else if(checki == node && checkj>1)
			{
				checkj--;
				checki=	0;
				Cube_prefab[checki,checkj].transform.renderer.material.color = Color.white;
				//print("checki="+checki.ToString()+", checkj="+checkj.ToString());
				checki++;
			}
		}	
	}
	
	void changAll2()
	{
		if(checkj == 0){
			checkj = layer-2;
		}
		if(Time.time >= CheckTime)
		{
			CheckTime = Time.time + Time.deltaTime;

			if(checki<node)
			{
				if(checki >0 )
				{
					//print ("- i="+ (checki-1).ToString() +", j=" + checkj.ToString());
					changToColor(checki-1,checkj);// ---------------------------------------------------------------將前一個還原成原始顏色
				}
				Cube_prefab[checki,checkj].transform.renderer.material.color = Color.magenta;
				//print("checki="+checki.ToString()+", checkj="+checkj.ToString());
				checki++;
				
			}
			else if(checki == node && checkj>1)
			{
				if(checkj<layer)
				{
					//print ("i="+ (checki-1).ToString() +", j=" + checkj.ToString());
					changToColor(checki-1,checkj);// ---------------------------------------------------------------將前一個還原成原始顏色
				}
				checkj--;
				checki=	0;
				Cube_prefab[checki,checkj].transform.renderer.material.color = Color.magenta;
				//print("checki="+checki.ToString()+", checkj="+checkj.ToString());
				checki++;
			}
			else if(checki == node && checkj == 1 )
			{
				//print ("i="+ (checki-1).ToString() +", j=" + checkj.ToString());
				changToColor(checki-1,checkj);// ---------------------------------------------------------------將前一個還原成原始顏色
			}
		}	
	}
	
	void changToColor(int i, int j)
	{
		if(NodeType[i,j] == 0)
		{
			Cube_prefab[i,j].renderer.material.color = Color.black;
		}
		else if (NodeType[i,j] == 1)
		{
			Cube_prefab[i,j].renderer.material.color = Color.red;
		}
		else if (NodeType[i,j] == 2)
		{
			Cube_prefab[i,j].renderer.material.color = Color.green;
		}
		else if (NodeType[i,j] == 3)
		{
			Cube_prefab[i,j].renderer.material.color = Color.blue;
		}
				
		if(j==0 || j==layer-1)
		{
			Cube_prefab[i,j].renderer.material.color = Color.cyan;
		}
		
	}
	
	void checkAllNodeSwish()
	{
		for(int i = 0; i<node; i++)
		{
			for(int j = 0; j<layer; j++)
			{
				if(NodeSwish[i,j])
				{
					Cube_prefab[i,j].renderer.material.color = Color.white;
				}
			}
		}
	}
}