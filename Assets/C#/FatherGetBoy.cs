using UnityEngine;
using System.Collections;

public class FatherGetBoy : MonoBehaviour {
	public GameObject father,boy;

	// Use this for initialization
	void Start () {
	boy.transform.parent = father.transform;
	}
	
	// Update is called once per frame
	void Update () {
	father.transform.Rotate(0,0,5);
	}
}
