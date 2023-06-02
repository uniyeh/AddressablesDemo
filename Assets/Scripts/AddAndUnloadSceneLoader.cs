using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using TMPro;

public class AddAndUnloadSceneLoader : MonoBehaviour
{
    const string sceneAddress = "AddedScene";
    
    Button btn;
    TextMeshProUGUI text;

    SceneInstance loadedScene;
    bool sceneLoaded = false;

    void Start()
    {
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(LoadOrUnLoadScene);
        
        text = this.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "Load Additive Scene";
    }

    void LoadOrUnLoadScene()
    {
        btn.interactable = false;

        if (sceneLoaded)
        {
            Addressables.UnloadSceneAsync(loadedScene).Completed += OnSceneUnloaded;
        }
        else
        {
            Addressables.LoadSceneAsync(sceneAddress, LoadSceneMode.Additive).Completed += OnSceneLoaded;
        }
    }

    void OnSceneUnloaded(AsyncOperationHandle<SceneInstance> sceneHandle)
    {
        if (sceneHandle.Status == AsyncOperationStatus.Succeeded)
        {
            sceneLoaded = false;
            text.text = "Reload Addictive Scene";
            loadedScene = new SceneInstance();
        }
        else
        {
            Debug.LogError($"Failed to unload scene: {sceneAddress}");
        }

        btn.interactable = true;
    }

    void OnSceneLoaded(AsyncOperationHandle<SceneInstance> sceneHandle)
    {
        if (sceneHandle.Status == AsyncOperationStatus.Succeeded)
        {
            text.text = "Unload Addictive Scene";
            loadedScene = sceneHandle.Result;
            sceneLoaded = true;
        }
        else
        {
            Debug.LogError($"Failed to unload scene: {sceneAddress}");
        }

        btn.interactable = true;
    }
}
