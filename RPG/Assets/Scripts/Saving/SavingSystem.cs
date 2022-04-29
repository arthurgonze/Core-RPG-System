using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            
            if (state.ContainsKey("lastSceneBuildIndex"))
                buildIndex = (int) state["lastSceneBuildIndex"];
            
            yield return SceneManager.LoadSceneAsync(buildIndex);
            RestoreState(state);
        }

        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        public void Delete(string saveFile)
        {
            DeleteFile(saveFile);
        }

        private void DeleteFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Deleting: " + path);
            // if(File.Exists(saveFile))
            File.Delete(saveFile);
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Loading: " + path);
            if (!File.Exists(path)) 
                return new Dictionary<string, object>();
            
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Saving to " + path);
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            Dictionary<string, object> stateDict = state;
            
            foreach (SavableEntity savable in FindObjectsOfType<SavableEntity>())
            {
                string uniqueIdentifier = savable.GetUniqueIdentifier();
                if(stateDict.ContainsKey(uniqueIdentifier)) 
                    savable.RestoreState(stateDict[uniqueIdentifier]);
            }
        }

        private void CaptureState(Dictionary<string, object> state)
        { 
            foreach (SavableEntity savable in FindObjectsOfType<SavableEntity>())
                state[savable.GetUniqueIdentifier()] = savable.CaptureState();

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            string[] paths = { Application.persistentDataPath, "Saves", saveFile + ".bin"};
            string fullPath = Path.Combine(paths);
            return fullPath;
        }
    }
}
