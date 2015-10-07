using UnityEngine;
using System.Collections;

public class Q3L1ClickNode : MonoBehaviour {
	//!! 注意 !! 本功能需搭配指定程式用來抓取否有點選到物件用
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	 void OnMouseDown() 
	{
		//將節點物件透明屬性改變用來讓指定程式抓取哪個物件被點選到
		transform.renderer.material.color = new Color(1,1,0.9f,transform.renderer.material.color.a);
		MovieNode.clickNodeSwish = true; //單功能用
		Q3L1GM.clickNodeSwish = true; //遊戲整體用
	}
}
