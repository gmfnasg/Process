using UnityEngine;
using System.Collections;

public class ChangColor : MonoBehaviour {
	public GameObject cube;
	// Use this for initialization
	void Start () {
	cube.renderer.material.color = Color.yellow;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
