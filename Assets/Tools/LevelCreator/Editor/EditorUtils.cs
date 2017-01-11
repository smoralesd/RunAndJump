using UnityEngine;
using UnityEditor.SceneManagement;
using System.Linq;

namespace RunAndJump.LevelCreator
{
    public static class EditorUtils
    {
        public static void NewScene()
        {
            //EditorApplication.SaveCurrentSceneIfUserWantsTo();
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            //EditorApplication.NewScene();
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
        }

        public static void CleanScene()
        {
            var allObjects = Object.FindObjectsOfType<GameObject>().ToList();
            allObjects.ForEach(Object.DestroyImmediate);
        }

        public static void NewLevel()
        {
            NewScene();
            CleanScene();
            var levelGameObject = new GameObject("Level");
            levelGameObject.transform.position = Vector3.zero;
            levelGameObject.AddComponent<Level>();
        }
    }
}