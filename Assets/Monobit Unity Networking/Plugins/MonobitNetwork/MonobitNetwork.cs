using UnityEngine;

namespace MonobitEngine
{
	/// <summary>
	/// MonobitNetworkプラグインで使うためのメインクラスです
	/// </summary>
	public sealed class MonobitNetwork : MonobitEngineBase.MonobitNetwork
    {
		/// <summary>
		/// ルーム内の全クライアントが、ホストと同じゲームシーンをロードすべきかを決めます。
		/// </summary>
		/// <remarks>
		/// 読み込むゲームシーンを同期するためには、ホストはMonobitNetwork.LoadLevelを使っている必要があります。
		/// そうであれば、全てのクライアントは、更新や入室の際に新しいシーンを読み込むことになります。
		/// </remarks>
		public static bool autoSyncScene
        {
            get { return GetAutomaticallySyncScene(); }
            set
            {
                MonobitNetwork.SetAutomaticallySyncScene(value, ActiveSceneBuildIndex, ActiveSceneName);
            }
        }

		/// <summary>
		/// 現在動作しているシーンのファイル名を取得します。
		/// </summary>
		public static string ActiveSceneName
        {
            get
            {
#if UNITY_MIN_5_3
            UnityEngine.SceneManagement.Scene s = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            return s.name;
#else
                return Application.loadedLevelName;
#endif
            }
        }

		/// <summary>
		/// 現在動作しているシーンのインデックスを取得します。
		/// </summary>
		public static int ActiveSceneBuildIndex
        {
            get
            {
#if UNITY_MIN_5_3
            return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
#else
                return Application.loadedLevel;
#endif
            }
        }
    }
}
