using UnityEngine;
using System.Collections;

public class ChangTexture : MonoBehaviour {
	//public Texture2D turn;
	public GameObject cube;
	float CheckTime;
	float picNumber = 36,TimeInterval = 10,i,j;
	// Use this for initialization
	void Start () 
	{
		CheckTime=Time.deltaTime * TimeInterval;
	}
	
	// Update is called once per frame
	void Update () 
	{
		cube.transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*1,(1/(Mathf.Sqrt(picNumber)))*5);
		//changTexture();
	}
	
	void changTexture()
	{
		if(Time.time >= CheckTime)
		{
			if(i<Mathf.Sqrt(picNumber) && j<Mathf.Sqrt(picNumber))
			{
			CheckTime = Time.time + Time.deltaTime * TimeInterval;
			cube.transform.renderer.material.mainTextureOffset = new Vector2((1/(Mathf.Sqrt(picNumber)))*i,(1/(Mathf.Sqrt(picNumber)))*i);
			i++;
			j++;
			}
			else {
				i=0;
				j=0;
			}
		}	
	}
}
