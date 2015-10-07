using UnityEngine;
using System.Collections;

public class CreatArray : MonoBehaviour {
	public GameObject cube;
	int node = 4, layer = 4 ; 
	Vector3[,] NodePOS;
	int [,] NodeType; 
	bool[,] NodeSwish;
	string objname;
	
	// Use this for initialization
	void Start () {
		layer += 2;
		NodePOS = new Vector3[node, layer];
		NodeType = new int[node, layer]; 
		NodeSwish = new bool[node, layer]; 
		
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
				GameObject cube_prefab = GameObject.Instantiate(cube,NodePOS[i,j],Quaternion.identity) as GameObject;
				cube_prefab.name = cube_prefab.name + "i=" + i.ToString() + ",j=" + j.ToString();
				
				if(NodeSwish[i,j])
				{
					cube_prefab.name = cube_prefab.name + "(SwishOn)" + "i=" + i.ToString() + ",j=" + j.ToString();
					//exp: GameObject bullet_prefab = GameObject.Instantiate(bullet,Bullet_Pos.position,Quaternion.identity) as GameObject;
					objname = cube_prefab.name;
				}
				
				NodeType[i,j] = Random.Range(0,3);
				if(NodeType[i,j] == 0)
				{
					cube_prefab.renderer.material.color = Color.black;
				}
				else if (NodeType[i,j] == 1)
				{
					cube_prefab.renderer.material.color = Color.red;
				}
				else if (NodeType[i,j] == 2)
				{
					cube_prefab.renderer.material.color = Color.green;
				}
				else if (NodeType[i,j] == 3)
				{
					cube_prefab.renderer.material.color = Color.blue;
				}
				
				if(j==0 || j==layer-1)
				{
					cube_prefab.renderer.material.color = Color.cyan;
				}

			}
		}
		cube.transform.position =  new Vector3(0, 0, -1);
		GameObject swishon = GameObject.Find(objname);
		swishon.renderer.material.color = Color.white;
	}
	
	// Update is called once per frame
	void Update () {
	}
}