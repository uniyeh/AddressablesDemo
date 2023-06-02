using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SingleSceneLoader : MonoBehaviour
{
    public string sceneAddress;
    
    Button btn;

    void Start()
    {
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(LoadScene);
        btn.interactable = true;
    }

    void LoadScene()
    {
        btn.interactable = false;

        if (string.IsNullOrEmpty(sceneAddress))
            return;

        Addressables.LoadSceneAsync(sceneAddress).Completed += (sceneHandle) => 
        {
            if (sceneHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("Load scene success");
            }
            else
            {
                Debug.Log("Load scene failed");
                btn.interactable = true;
            }
        };
    }
}
