using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonobitEngine;
public class Card :MonobitEngine. MonoBehaviour {

	int myNum;
	Network net;
	GameObject gameM;
	// Use this for initialization
	void Start () {
		myNum = int.Parse(gameObject.name);
		gameM = GameObject.Find("GameManager");
		net = gameM.GetComponent<Network>();
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void onClickDestroyBtn()
	{
		gameM.SendMessage("goDestroy", myNum);
	}
}
