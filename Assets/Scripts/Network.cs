using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MonobitEngine;

public class Network : MonobitEngine.MonoBehaviour
{

	private string roomName = "";
	private string chatWord = "";//自分が送信するチャットワード
	List<string> chatLog = new List<string>();//チャットのリストを格納
	public bool isRedy = false;
	public int boolCount=0;//Redyの数を管理

	public Button []Card;

	public InputField inputF;
	public GameObject MainBtn;
	public GameObject RedyBtn;
	public Text txtHol;
	public Text txtMsg;
	public Text txtBtn;



	public enum STS
	{
		PLAYER,
		ROOM,
		REDY,
		GAME
	}
	public STS gameStatus;

	//チャットを受け取るRPC処理
	[MunRPC]
	void RecvChat(string senderName, string senderWord)
	{
		chatLog.Add(senderName + ":" + senderWord);
		if (chatLog.Count > 10)
		{
			chatLog.RemoveAt(0);
		}
	}

	//全員のRedyを監視するHost専用
	[MunRPC]
	void RecvRedy(bool ok)
	{
		boolCount += 1;
		if(boolCount==2)
		{
			monobitView.RPC("StartGame", MonobitTargets.All, null);
		}

	}

	//Gameをサーバーを介して全員スタートさせる
	[MunRPC]
	void StartGame()
	{
		setGame();
	}

	// Use this for initialization
	void Start()
	{
		setPlayer();
	}
	
	// Update is called once per frame
	void Update()
	{
		
	}
	//Guiでルームに入っているプレイヤーを表示
	private void OnGUI()
	{
		if (MonobitNetwork.isConnect)
		{
			if (MonobitNetwork.inRoom)
			{
				//player表示
				GUILayout.BeginVertical();
				GUILayout.Label("入室中のプレイヤー : ");
				foreach (MonobitPlayer player in MonobitNetwork.playerList)
				{
					GUILayout.Label("・"+player.name);
				}
				GUILayout.EndVertical();

				//チャット表示
				string msg="";
				for(int i = 0; i < 10; ++i )
				{
					msg += ((i < chatLog.Count) ? chatLog[i] : "") + "\r\n";
				}
				GUILayout.TextArea(msg);

				/*if(GUILayout.Button("退室する",GUILayout.Width(150)))
				{
					MonobitNetwork.LeaveRoom();
					chatLog.Clear();
					foreach (Button c in Card)
					{
						c.gameObject.SetActive(false);
					}
					inputF.gameObject.SetActive(true);
					MainBtn.gameObject.SetActive(true);
					RedyBtn.gameObject.SetActive(false);
					inputF.transform.localPosition = new Vector2(0,0);
					MainBtn.transform.localPosition = new Vector2(0,-107);
					setRoom();
				}*/

			}
		}
	}

	public void onClickMainBtn() //mainボタン押下処理
	{
		switch (gameStatus)
		{
			case STS.PLAYER:
				if (inputF.text!="")
				{
					MonobitNetwork.playerName = inputF.text;
					MonobitNetwork.ConnectServer("NetworkSample_v1.0");
					setRoom();
				}
				break;

			case STS.ROOM:
				if (inputF.text != "")
				{
					string Rname = inputF.text;
					MonobitEngine.RoomSettings settings = new MonobitEngine.RoomSettings();
					settings.maxPlayers = 4;//max4人

					MonobitNetwork.JoinOrCreateRoom(Rname, settings, null);
					setRedy();
				}
				break;

			case STS.REDY:
				
				break;

			case STS.GAME:
				if (inputF.text != "")
				{
					chatWord = inputF.text;
					monobitView.RPC("RecvChat", MonobitTargets.All, MonobitNetwork.playerName, chatWord);
					inputF.text = "";
					chatWord = "";
				}
				break;

		}
	}
	public void onClickEnterBtn()
	{
		chatWord = inputF.text;
		monobitView.RPC("RecvChat", MonobitTargets.All, MonobitNetwork.playerName, chatWord);
		inputF.text = "";
		chatWord = "";
	}
	public void onClickRedyBtn()//redyボタン
	{
		isRedy = true;
		RedyBtn.gameObject.SetActive(false);
		txtMsg.text="他のプレイヤーを待っています...";
		monobitView.RPC("RecvRedy", MonobitTargets.Host, isRedy);


	}



	void setPlayer()
	{
		gameStatus = STS.PLAYER;
		foreach (Button c in Card)
		{
			c.gameObject.SetActive(false);
		}
		RedyBtn.gameObject.SetActive(false);
		inputF.text = "";
		txtMsg.text = "あなたの名前を入力してください";
		txtHol.text = "Player Name";
		txtBtn.text = "決定";
		if (!MonobitNetwork.isConnect)
		{
			MonobitNetwork.autoJoinLobby = true;
		}
	}

	void setRoom()
	{
		gameStatus = STS.ROOM;
		inputF.text = "";
		txtMsg.text = "ルームを新たに作成するか、すでにあるルーム名を入力してください";
		txtHol.text = "Room Name";
		txtBtn.text = "ルーム作成、入室";
	}

	void setRedy()
	{
		gameStatus = STS.REDY;
		boolCount = 0;
		txtMsg.text = "準備ができたらRedyボタンを押してください";
		inputF.transform.localPosition = new Vector2(-170, -324);
		MainBtn.transform.localPosition = new Vector2(326,-324);
		inputF.gameObject.SetActive(false);
		MainBtn.gameObject.SetActive(false);
		RedyBtn.gameObject.SetActive(true);


	}

	[MunRPC]
	void setGame()
	{
		gameStatus = STS.GAME;
		chatLog.Clear();
		inputF.gameObject.SetActive(true);
		MainBtn.gameObject.SetActive(true);
		foreach (Button c in Card)
		{
			c.gameObject.SetActive(true);
		}
		inputF.text = "";
		txtMsg.text = "職業カードから好きな職業を選択してください";
		txtHol.text = "Message";
		txtBtn.text = "送信する";
	}

	public void goDestroy(int myNum)
	{
		monobitView.RPC("destroyCard",MonobitTargets.All,myNum);
	}
		

	[MunRPC]
	void destroyCard(int cardNum)
	{
		Card[cardNum].gameObject.SetActive(false);
	}
}
