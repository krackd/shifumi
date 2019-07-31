//-------------------
// Copyright 2019
// Reachable Games, LLC
//-------------------

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;

namespace ReachableGames
{
	public class AssetsPopup : EditorWindow
	{
		static string kNextCheckTime = "ReachableGames_AssetPopup_NextCheckTime";
		static string kHashOfWebPage = "ReachableGames_AssetPopup_Hash";
		static string kAssetsURL = "https://reachablegames.com/unity-asset-updates/";
		static private AssetsPopup _instance = null;

		[MenuItem("Tools/ReachableGames/Check for Updates")]
		static void Init()
		{
			if (_instance==null)
				_instance = EditorWindow.CreateInstance<AssetsPopup>();
			_instance.ShowUtility();
			WaitForWebRequest(CheckContent());
		}

		[InitializeOnLoadMethod]
		static void CheckForUpdates()
		{
			long nextCheckTime = long.Parse(EditorPrefs.GetString(kNextCheckTime, "0"));
			if (DateTime.UtcNow.Ticks > nextCheckTime)
			{
				// Do a daily check for updated web page, unless we're told not to check for 3 months.
				long nextCheck = DateTime.UtcNow.Ticks + TimeSpan.FromDays(1.0).Ticks;
				EditorPrefs.SetString(kNextCheckTime, nextCheck.ToString());
				WaitForWebRequest(CheckContent());
			}
		}

		static private IEnumerator CheckContent()
		{
			using (UnityWebRequest w = UnityWebRequest.Get(kAssetsURL))
			{
				yield return w.SendWebRequest();
				while (!w.isDone)
					yield return null;
				if (!w.isNetworkError && !w.isHttpError && w.downloadProgress==1.0f)
				{
					string hashOfWebPage = Hash128.Compute(w.downloadHandler.text).ToString();
					if (EditorPrefs.GetString(kHashOfWebPage, "")!=hashOfWebPage)
					{
						EditorPrefs.SetString(kHashOfWebPage, hashOfWebPage);
						Init();  // pop up the window, something's new
					}
				}
			}
		}

		static private void WaitForWebRequest(IEnumerator update)
		{
			EditorApplication.CallbackFunction cb = null;
			cb = () => { try { if (!update.MoveNext()) EditorApplication.update -= cb; } catch (Exception ex) { Debug.LogException(ex); EditorApplication.update -= cb; } };
			EditorApplication.update += cb;
		}

		//-------------------

		private WebViewHook _webHook = null;

		void OnEnable()
		{
			if (_webHook==null)
				_webHook = CreateInstance<WebViewHook>();
			_webHook.AllowRightClickMenu(false);
			minSize = new Vector2(840, 800);
		}

		void OnDestroy()
		{
			if (_webHook!=null)
			{
				DestroyImmediate(_webHook);
			}
		}

		Rect bottomBarRect;
		void OnGUI()
		{
			// Put web block here
			if (_webHook.Hook(this))
			{
				_webHook.LoadURL(kAssetsURL);
			}
			if (Event.current.type==EventType.Repaint)
				_webHook.OnGUI(new Rect(0, 0, position.width, bottomBarRect.height));

			GUILayout.FlexibleSpace();
			if (Event.current.type==EventType.Repaint)
				bottomBarRect = GUILayoutUtility.GetLastRect();
			var bigTextArea = new GUIStyle(GUI.skin.textArea);
			bigTextArea.fontSize = 15;
			EditorGUILayout.TextArea("Thank you for using our assets.  We sincerely hope they are making dev life better for you.  If so, please help us keep working on them by giving each a great 5* review and recommend them to your friends.  It really does make a difference!", bigTextArea);
			using (new EditorGUILayout.HorizontalScope())
			{
				if (GUILayout.Button(new GUIContent("Check for updates in 3 months", "You can always check manually at Tools->ReachableGames."), GUILayout.Width(200)))
				{
					long nextCheck = DateTime.UtcNow.Ticks + TimeSpan.FromDays(90.0).Ticks;
					EditorPrefs.SetString(kNextCheckTime, nextCheck.ToString());
				}
				GUILayout.FlexibleSpace();
				GUIStyle blueLabel = new GUIStyle(EditorStyles.whiteMiniLabel);
				blueLabel.normal.textColor = Color.blue;
				blueLabel.fontSize = 12;
				if (GUILayout.Button("support@reachablegames.com", blueLabel))
				{
					Application.OpenURL("mailto:support@reachablegames.com?Subject=Support Request");
				}
				GUILayout.FlexibleSpace();
				var blueTextButton = new GUIStyle(GUI.skin.button);
				blueTextButton.normal.textColor = Color.blue;
				if (GUILayout.Button(new GUIContent("Check for updates regularly", "As a service, this will let you know as soon as bugs are fixed or features are added to one of our assets.  Generally this is not more than once a month."), blueTextButton, GUILayout.Width(200)))
				{
					long nextCheck = DateTime.UtcNow.Ticks + TimeSpan.FromDays(1.0).Ticks;
					EditorPrefs.SetString(kNextCheckTime, nextCheck.ToString());
				}
			}
		}
	}
}