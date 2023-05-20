using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Project.Editor
{
    public static class EditorUtilities
    {
        const string PlayFromFirstMenuStr = "Tools/Run from Startup Scene";

        static bool playFromFirstScene
        {
            get { return EditorPrefs.GetBool(PlayFromFirstMenuStr, true); }
            set { EditorPrefs.SetBool(PlayFromFirstMenuStr, value); }
        }

        [MenuItem(PlayFromFirstMenuStr, false, 150)]
        static void PlayFromFirstSceneCheckMenu()
        {
            playFromFirstScene = !playFromFirstScene;
            Menu.SetChecked(PlayFromFirstMenuStr, playFromFirstScene);

            ShowNotifyOrLog(playFromFirstScene ? "Play from startup scene" : "Play from current scene");
        }

        // The menu won't be gray out, we use this validate method for update check state
        [MenuItem(PlayFromFirstMenuStr, true)]
        static bool PlayFromFirstSceneCheckMenuValidate()
        {
            Menu.SetChecked(PlayFromFirstMenuStr, playFromFirstScene);
            return true;
        }

        // This method is called before any Awake. It's the perfect callback for this feature
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void LoadFirstSceneAtGameBegins()
        {
            if (!playFromFirstScene || SceneManager.GetActiveScene().name == "Startup")
                return;

            foreach (GameObject go in Object.FindObjectsOfType<GameObject>())
            {
                go.SetActive(false);
            }

            SceneManager.LoadScene("Startup");
        }

        static void ShowNotifyOrLog(string msg)
        {
            if (Resources.FindObjectsOfTypeAll<SceneView>().Length > 0)
                EditorWindow.GetWindow<SceneView>().ShowNotification(new GUIContent(msg));
            else
                Debug.Log(msg); // When there's no scene view opened, we just print a log
        }
    }
}