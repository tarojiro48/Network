using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using MonobitEngine.Definitions;

namespace MonobitEngine.Editor
{
    /**
     * ServerSettings のインスペクタ表示用クラス.
     */
    [CustomEditor(typeof(ServerSettings))]
    public class ServerSettingsInspector : UnityEditor.Editor
    {
        /** ServerSettins 本体. */
        ServerSettings m_Settings = null;

        /** 初期化したかどうかのフラグ. */
        bool m_Initialize = false;

        /**
         * Inspector上のGUI表示.
         */
        public override void OnInspectorGUI()
        {
#if UNITY_IPHONE
            Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
            m_Settings = target as ServerSettings;
            if (m_Settings == null) return;

            // 初期設定
            Init();

            // アプリケーション設定
            ApplicationSettings();

            // サーバ設定
            ServerSettings();

            // 認証サーバ設定
            CustomAuthServerSettings();

            // 時間設定
            TimeSettings();

            // データの更新
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(m_Settings);
            }
        }

        /**
         * 初期設定.
         */
        private void Init()
        {
            if (!m_Initialize)
            {
                m_Initialize = true;
                AuthenticationCode m_SaveData = AssetDatabase.LoadAssetAtPath(MonobitNetworkSettings.GetAuthCodeSettingsPath(), (typeof(AuthenticationCode))) as AuthenticationCode;
                if (m_SaveData != null)
                {
                    MonobitNetworkSettings.MonobitServerSettings.AuthID = MonobitNetworkSettings.Decrypt(m_SaveData.saveAuthID);
                }
                AssetDatabase.Refresh();
            }
        }

        /**
         * アプリケーション設定.
         */
        private void ApplicationSettings()
        {
            // 標題の表示
            EditorGUILayout.LabelField("Application Settings", EditorStyles.boldLabel);

            EditorGUI.indentLevel = 2;
            GUILayout.Space(5);

            // 認証コードの表示
            if (m_Settings.AuthID == "") SaveAuthID();
            EditorGUILayout.LabelField("Authentication Code", m_Settings.AuthID);
            
            // ボタン表示）
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Generate Code"))
            {
                SaveAuthID();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel = 0;
            GUILayout.Space(5);
        }

        private void SaveAuthID()
        {
            m_Settings.UpdateAuthID();
            AssetDatabase.DeleteAsset(MonobitNetworkSettings.GetAuthCodeSettingsPath());
            AuthenticationCode m_SaveData = ScriptableObject.CreateInstance<AuthenticationCode>();
            m_SaveData.saveAuthID = MonobitNetworkSettings.Encrypt(m_Settings.AuthID);
            EditorUtility.SetDirty(m_SaveData);
            AssetDatabase.CreateAsset(m_SaveData, MonobitNetworkSettings.GetAuthCodeSettingsPath());
            AssetDatabase.SaveAssets();
        }

        /**
         * サーバ設定.
         */
        private void ServerSettings()
        {
            // 標題の表示
            EditorGUILayout.LabelField("Server Settings", EditorStyles.boldLabel);

            EditorGUI.indentLevel = 2;
            GUILayout.Space(5);

            GUI.enabled = !EditorApplication.isPlaying;

            // ホストタイプの設定
            m_Settings.HostType = (ServerSettings.MunHostingOption)EditorGUILayout.EnumPopup("Host Type", m_Settings.HostType);

            // ポート番号＆アドレスの設定
            switch (m_Settings.HostType)
            {
                case MonobitEngine.ServerSettings.MunHostingOption.None:
                    {
                    }
                    break;
                case MonobitEngine.ServerSettings.MunHostingOption.MunTestServer:
                    {
                        // 接続プロトコルを選択する
                        m_Settings.HostProtocol = (ConnectionProtocol)EditorGUILayout.EnumPopup("Protocol", m_Settings.HostProtocol);
                    }
                    break;
                case MonobitEngine.ServerSettings.MunHostingOption.SelfServer:
                    {
                        // 接続プロトコルを選択する
                        m_Settings.HostProtocol = (ConnectionProtocol)EditorGUILayout.EnumPopup("Protocol", m_Settings.HostProtocol);

                        // アドレス・ポート番号ともに自分で設定する
                        m_Settings.SelfServerAddress = EditorGUILayout.TextField("IP Address", m_Settings.SelfServerAddress).Trim();
                        m_Settings.SelfServerPortString = EditorGUILayout.TextField("Port", m_Settings.SelfServerPortString).Trim();

                        // 入力されたポート番号が正常化をチェック
                        int usPort = m_Settings.SelfServerPort;
                        if (int.TryParse(m_Settings.SelfServerPortString, out usPort) == true && usPort >= 0 && usPort <= 65535)
                        {
							m_Settings.SelfServerPort = usPort;
                        }
                    }
                    break;
                case MonobitEngine.ServerSettings.MunHostingOption.OfflineMode:
                    {
                    }
                    break;
            }

            GUI.enabled = true;

            EditorGUI.indentLevel = 0;
            GUILayout.Space(5);
        }

        /**
         * カスタム認証サーバ設定.
         */
        private void CustomAuthServerSettings()
        {
            // 標題の表示
            EditorGUILayout.LabelField("Custom Authentication Server Settings", EditorStyles.boldLabel);

            EditorGUI.indentLevel = 2;
            GUILayout.Space(5);

            GUI.enabled = !EditorApplication.isPlaying;

            // カスタム認証サーバの利用タイプの設定
            m_Settings.CustomAuthType = (ServerSettings.CustomAuthenticationType)EditorGUILayout.EnumPopup("Type", m_Settings.CustomAuthType);

            // アドレスの設定
            switch (m_Settings.CustomAuthType)
            {
                case MonobitEngine.ServerSettings.CustomAuthenticationType.None:
                    {
                        // 設定しない
                    }
                    break;
                case MonobitEngine.ServerSettings.CustomAuthenticationType.WebServer_AppointClient:
                    {
                        // アドレスを自分で設定する
                        m_Settings.CustomAuthServerAddress = EditorGUILayout.TextField("Address", m_Settings.CustomAuthServerAddress).Trim();

                        // エラーを無視するかどうかのフラグの設定
                        m_Settings.CustomAuth_IgnoreError = EditorGUILayout.ToggleLeft(" Ignore AuthServer Error", m_Settings.CustomAuth_IgnoreError);
                    }
                    break;
                case MonobitEngine.ServerSettings.CustomAuthenticationType.WebServer_AppointServer:
                    {
                        // エラーを無視するかどうかのフラグの設定
                        m_Settings.CustomAuth_IgnoreError = EditorGUILayout.ToggleLeft(" Ignore AuthServer Error", m_Settings.CustomAuth_IgnoreError);
                    }
                    break;
            }

            GUI.enabled = true;

            EditorGUI.indentLevel = 0;
            GUILayout.Space(5);
        }

        /**
         * 時間設定
         */
        private void TimeSettings()
        {
            EditorGUILayout.LabelField("Time Settings(ms)", EditorStyles.boldLabel);
            
            EditorGUI.indentLevel = 2;
            GUILayout.Space(5);
            GUI.enabled = ! EditorApplication.isPlaying;
            
            string oldServerConnectWaitTime = m_Settings.ServerConnectWaitTime.ToString();
            string newServerConnectWaitTime = EditorGUILayout.TextField( "Server Connect", oldServerConnectWaitTime );
            if ( oldServerConnectWaitTime != newServerConnectWaitTime ){
                try{
                    m_Settings.ServerConnectWaitTime = uint.Parse( newServerConnectWaitTime );
                }catch{}
            }
            
            GUI.enabled = true;
            EditorGUI.indentLevel = 0;
            GUILayout.Space(5);
        }
    }
}
