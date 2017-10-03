using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEditor;

namespace MonobitEngine.Editor
{
	/**
	 * サーバー設定ファイルの作成クラス
	 */
	public class SettingsFactory : UnityEngine.MonoBehaviour
	{
		/**
		 * コンストラクタ
		 */
		public SettingsFactory()
		{
		}

		/**
		 * MonobitServerSettingsの作成メニュー
		 */
		[MenuItem("Assets/Create/MonobitServerSettings")]
		public static void OnCreateMonobitServerSettings()
		{
			CreateAsset<MonobitEngine.ServerSettings>(MonobitNetworkSettings.GetServerSettingsPath());
		}

		/**
		 * ScriptableObjectのシリアライズ
		 */
		public static void CreateAsset<Type>(string settingPath) where Type : ScriptableObject
		{
			Type item = ScriptableObject.CreateInstance<Type>();
			AssetDatabase.CreateAsset(item, settingPath);
			AssetDatabase.SaveAssets();

			EditorUtility.FocusProjectWindow();
			Selection.activeObject = item;
		}

		/**
		 * ScriptableObjectのXMLでのデシリアライズ
		 * @param settingPath デシリアライズするファイルパス
		 * @return デシリアライズしたデータ
		 */
		public static T LoadSettings<T>(string settingPath) where T : class
		{
			var serialize = new XmlSerializer(typeof(T));
			using (var stream = new FileStream(settingPath, FileMode.Open))
			{
				T param = serialize.Deserialize(stream) as T;
				return param;
			}
		}

		/**
		 * ScriptableObjectのXMLでのシリアライズ
		 * @param settingPath ファイルへのパス
		 * @param saveObject シリアライズするデータ
		 */
		public static void SaveSettings<T>(string settingPath, T saveObject) where T : class
		{
			var serialize = new XmlSerializer(typeof(T));
			using (var stream = new FileStream(settingPath, FileMode.Create))
			{
				serialize.Serialize(stream, saveObject);
			}
		}

	}
}
