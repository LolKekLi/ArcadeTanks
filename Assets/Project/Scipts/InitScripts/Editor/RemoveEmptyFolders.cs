using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace Project.Editor
{
	/// <summary>
	/// Remove empty folders automatically.
	/// </summary>
	public class RemoveEmptyFolders : UnityEditor.AssetModificationProcessor
	{
		private const string MenuText = "Tools/Remove Empty Folders";
		private static readonly StringBuilder LOG = new StringBuilder();
		private static readonly List<DirectoryInfo> Results = new List<DirectoryInfo>();

		/// <summary>
		/// Raises the initialize on load method event.
		/// </summary>
		[InitializeOnLoadMethod]
		static void OnInitializeOnLoadMethod()
		{
			EditorApplication.delayCall += () => Valid();
		}

		/// <summary>
		/// Raises the will save assets event.
		/// </summary>
		static string[] OnWillSaveAssets(string[] paths)
		{
			// If menu is unchecked, do nothing.
			if (!EditorPrefs.GetBool(MenuText, false))
			{
				return paths;
			}

			// Get empty directories in Assets directory
			Results.Clear();
			var assetsDir = Application.dataPath + Path.DirectorySeparatorChar;
			GetEmptyDirectories(new DirectoryInfo(assetsDir), Results);

			// When empty directories has detected, remove the directory.
			if (0 < Results.Count)
			{
				LOG.Length = 0;
				LOG.AppendFormat("Remove {0} empty directories as following:\n", Results.Count);
				
				foreach (var d in Results)
				{
					LOG.AppendFormat("- {0}\n", d.FullName.Replace(assetsDir, ""));
					FileUtil.DeleteFileOrDirectory(d.FullName);
				}

				// UNITY BUG: Debug.Log can not set about more than 15000 characters.
				LOG.Length = Mathf.Min(LOG.Length, 15000);
				Debug.Log(LOG.ToString());
				LOG.Length = 0;

				AssetDatabase.Refresh();
			}

			return paths;
		}

		/// <summary>
		/// Toggles the menu.
		/// </summary>
		[MenuItem(MenuText)]
		static void OnClickMenu()
		{
			// Check/Uncheck menu.
			bool isChecked = !Menu.GetChecked(MenuText);
			Menu.SetChecked(MenuText, isChecked);

			// Save to EditorPrefs.
			EditorPrefs.SetBool(MenuText, isChecked);

			OnWillSaveAssets(null);
		}

		[MenuItem(MenuText, true)]
		static bool Valid()
		{
			// Check/Uncheck menu from EditorPrefs.
			Menu.SetChecked(MenuText, EditorPrefs.GetBool(MenuText, false));
			return true;
		}

		/// <summary>
		/// Get empty directories.
		/// </summary>
		static bool GetEmptyDirectories(DirectoryInfo dir, List<DirectoryInfo> results)
		{
			bool isEmpty = true;
			
			try
			{
				isEmpty =
					dir.GetDirectories().Count(x => !GetEmptyDirectories(x, results)) == 0 // Are sub directories empty?
					&& dir.GetFiles("*.*").All(x => x.Extension == ".meta"); // No file exist?
			}
			catch { }

			// Store empty directory to results.
			if (isEmpty)
			{
				results.Add(dir);
			}

			return isEmpty;
		}
	}
}