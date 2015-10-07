using UnityEngine;
using System.Collections;

public class GetVoidOut : MonoBehaviour {

	// Use this for initialization
	void Start () {
	getOut();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
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
	
	
}
