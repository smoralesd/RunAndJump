using UnityEngine;
using UnityEditor.SceneManagement;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace RunAndJump.LevelCreator
{
    public static class EditorUtils
    {
        public static void NewScene()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
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

        public static List<T> GetAssestsWithScript<T>(string path) where T : MonoBehaviour
        {
            var assetList = new List<T>();

            var guids = AssetDatabase.FindAssets("t:Prefab", new string[] { path }).ToList();

            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                var TComponent = asset.GetComponent<T>();

                if (TComponent != null)
                {
                    assetList.Add(TComponent);
                }
            }

            return assetList;
        }
    }
}