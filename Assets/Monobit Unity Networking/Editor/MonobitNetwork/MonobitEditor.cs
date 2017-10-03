using System;
using UnityEngine;
using UnityEditor;
using MonobitEngine.Definitions;

namespace MonobitEngine.Editor
{
	[InitializeOnLoad]
	public class MonobitEditor : EditorWindow
	{
		/**
		 * ロード時に一度だけ呼び出される
		 */
		static MonobitEditor()
		{
		}

		/**
		 * Windowごと
		 */
		public MonobitEditor()
		{

		}

		/**
		 * 
		 */
		[MenuItem("Window/Monobit Unity Networking/Pick Up Settings %#&m", false, 1)]
		protected static void OnMenuItemHighlightServerSettings()
		{
			DoHighlightServerSettings();
		}

		/**
		 * MUNで内部的に生成している秘匿オブジェクトのクリーンアップ.
		 */
		[MenuItem("Window/Monobit Unity Networking/Cleanup MUN internal HideInHierarchy object %#&c")]
		static void CleanupMunHideInHierarchyObject()
		{
			MonobitNetwork.CleanupMunHideInHierarchyObject();
		}

		/**
		 * 
		 */
		protected static void DoHighlightServerSettings()
		{
			Selection.objects = new UnityEngine.Object[] { MonobitNetworkSettings.MonobitServerSettings };
			EditorGUIUtility.PingObject(MonobitNetworkSettings.MonobitServerSettings);
		}
	}
}
