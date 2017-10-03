using UnityEngine;
using System.Collections;
using MonobitEngine;
using MonobitEngine.Definitions;

public class OnlineSceneReconnect : MonobitEngine.MonoBehaviour
{
    // ゲームスタートフラグ
    private bool m_bGameStart = false;

    // 制限時間
    private int battleEndFrame = 60 * 60;

    // 自身のオブジェクトを生成したかどうかのフラグ
    private bool spawnMyChara = false;

    // 自身のオブジェクトの出現場所
    GameObject myObject = null;
    private Vector3 myPosition = new Vector3();
    private Quaternion myRotation = new Quaternion();

    // 意図しないタイミングで Disconnect されたかどうかのフラグ
    private bool bSafeDiscoonect = false;
    private bool bDisconnecting = false;
    private bool bReconnecting = false;

    // 再接続する際のルーム名
    private string reconnectRoomName = "";

    // GUI処理
    void OnGUI()
    {
        // GUI用の解像度を調整する
        Vector2 guiScreenSize = new Vector2(800, 480);
        if (Screen.width > Screen.height)
        {
            // landscape
            GUIUtility.ScaleAroundPivot(new Vector2(Screen.width / guiScreenSize.x, Screen.height / guiScreenSize.y), Vector2.zero);
        }
        else
        {
            // portrait
            GUIUtility.ScaleAroundPivot(new Vector2(Screen.width / guiScreenSize.y, Screen.height / guiScreenSize.x), Vector2.zero);
        }

        // プレイヤーIDの表示
        if (MonobitNetwork.player != null)
        {
			GUILayout.Label(string.Format("My Player ID : {0}", MonobitNetwork.player.ID));
        }

        // ルーム情報の取得
        Room room = MonobitNetwork.room;
        if (room != null)
        {
            // ルーム名の表示
            GUILayout.Label(string.Format("Room Name : {0}", room.name));

            // ルーム内に存在するプレイヤー数の表示
            GUILayout.Label(string.Format("PlayerCount : {0}", room.playerCount));

            // ルームがオープンかクローズか
            GUILayout.Label(string.Format("Room IsOpen : {0}", room.open));

            // 制限時間の表示
            if (m_bGameStart)
            {
                GUILayout.Label(string.Format("Rest Frame : {0}", this.battleEndFrame));
            }
        }
        // 部屋からの離脱
        if (GUILayout.Button("Leave Room", GUILayout.Width(100)))
        {
            // 安全なDisconnect
            bSafeDiscoonect = true;

            // 部屋から離脱する
            MonobitNetwork.DisconnectServer();

            // シーンをオフラインシーンへ
            MonobitNetwork.LoadLevel(OfflineSceneReconnect.SceneNameOffline);

            return;
        }

        // ホストの場合
        if (MonobitNetwork.isHost)
        {
            // ゲームスタート前にゲームスタートするかどうか
            if (!m_bGameStart && GUILayout.Button("Start Game", GUILayout.Width(100)))
            {
                // ゲームスタートフラグを立てる
                m_bGameStart = true;

                // バトルスタートを通知
                monobitView.RPC("OnGameStart", MonobitTargets.All, null);
            }

            // バトル終了
            if (battleEndFrame <= 0)
            {
                // 安全なDisconnect
                bSafeDiscoonect = true;

                // ルームを抜ける
                MonobitNetwork.DisconnectServer();

                // シーンをオフラインシーンへ
                MonobitNetwork.LoadLevel(OfflineSceneReconnect.SceneNameOffline);

                return;
            }
        }

        // 意図しないタイミングで切断されたとき
        if (!MonobitNetwork.isConnect && !bSafeDiscoonect)
        {
            GUILayout.Window(0, new Rect(Screen.width / 2 - 100, Screen.height / 2 - 40, 200, 80), WindowGUI, "Disconnect");
        }

    }

    // ウィンドウ表示
    void WindowGUI(int WindowID)
    {
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        GUIStyleState stylestate = new GUIStyleState();
        stylestate.textColor = Color.white;
        style.normal = stylestate;
        if (bDisconnecting)
        {
            GUILayout.Label("MUN is disconnect.\nAre you reconnect?", style);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Yes", GUILayout.Width(50)))
            {
                // もう一度接続処理を実行する
                bDisconnecting = false;
                bReconnecting = true;
                m_bGameStart = false;
                spawnMyChara = false;
                MonobitNetwork.ConnectServer("RandomMatchingReconnect_v1.0");
            }
            if (GUILayout.Button("No", GUILayout.Width(50)))
            {
                bDisconnecting = false;

                // シーンをオフラインシーンへ
                MonobitNetwork.LoadLevel(OfflineSceneReconnect.SceneNameOffline);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.Label("reconnecting...", style);
        }
    }

    // 更新処理
    void Update()
    {
        // 自身のキャラクタ位置を退避する
        if (myObject != null)
        {
            myPosition = myObject.transform.position;
            myRotation = myObject.transform.rotation;
        }

        // ルーム名を退避する
        if (MonobitNetwork.room != null)
        {
            reconnectRoomName = MonobitNetwork.room.name;
        }

        // ゲームスタート後、まだ自分のキャラクタのspawnが終わってない状態であれば、自身のキャラクタをspawnする
        if (m_bGameStart && !spawnMyChara)
        {
            OnGameStart();
        }

        // ゲームスタート後、ホストなら
        if (m_bGameStart && MonobitNetwork.isHost)
        {
            // 制限時間の減少
            if (battleEndFrame > 0)
            {
                battleEndFrame--;
            }
            // 制限時間をRPCで送信
            object[] param = new object[]
            {
                battleEndFrame
            };

            monobitView.RPC("TickCount", MonobitTargets.Others, param);
        }
    }

    // ゲームスタートを受信(RPC)
    [MunRPC]
    void OnGameStart()
    {
        // ゲームスタートフラグを立てる
        m_bGameStart = true;

        if (!bReconnecting)
        {
            // ある程度ランダムな場所にプレイヤーを配置する
            myPosition = Vector3.zero;
            myPosition.x = Random.Range(-10.0f, 10.0f);
            myPosition.z = Random.Range(-10.0f, 10.0f);

            myRotation = Quaternion.AngleAxis(Random.Range(-180.0f, 180.0f), Vector3.up);
        }

        // プレイヤーの配置（他クライアントにも同時にInstantiateする）
        myObject = MonobitNetwork.Instantiate("SD_unitychan_generic_PC", myPosition, myRotation, 0);

        // 出現させたことを確認
        spawnMyChara = true;

        // 再接続処理完了
        bReconnecting = false;

#if false
        // RPCのOthersBufferedの確認用
        monobitView.RPC( "BufferedPlayerId", MonobitTargets.OthersBuffered, MonobitNetwork.player.ID );
#endif
    }

    // 制限時間を受信(RPC)
    [MunRPC]
    void TickCount(int frame)
    {
        // ゲームスタートフラグを立てる（途中参加者のための処置）
        m_bGameStart = true;

        // 制限時間を同期する
        this.battleEndFrame = frame;
    }

#if false
    // RPCのOthersBufferedの確認用(RPC)
    [MunRPC]
    void BufferedPlayerId( int playerId )
    {
        Debug.Log( System.String.Format( "BufferedPlayerId playerId={0}", playerId ) );
    }
#endif

    // 接続が切断されたときの処理
    public void OnDisconnectedFromServer()
    {
        Debug.Log("Disconnected from Monobit");
        bDisconnecting = true;
    }

    // 接続失敗時の処理
    public void OnConnectToServerFailed(object parameters)
    {
        Debug.Log("OnConnectToServerFailed : StatusCode = " + parameters + ", ServerAddress = " + MonobitNetwork.ServerAddress);
        bDisconnecting = true;
    }

    // 接続が確立した時の処理
    public void OnJoinedLobby()
    {
        // 現在残っているルーム情報から再接続を実行する
        MonobitNetwork.JoinRoom(reconnectRoomName);
    }

    // ルームが存在しなかった場合
    // 指定ルーム入室失敗時の処理
    public void OnJoinRoomFailed(object[] parameters)
    {
        Debug.Log("OnJoinRoomFailed : ErrorCode = " + parameters[0] + ", DebugMsg = " + parameters[1]);

        // オフラインシーンに戻す
        MonobitNetwork.LoadLevel(OfflineSceneReconnect.SceneNameOffline);
    }
}
