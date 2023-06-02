using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.ResourceProviders;

public class SceneOpener : MonoBehaviour
{
    Button button;

    [SerializeField]
    AssetReference scene;

    public PredownloadSceneController predownloadSceneController;

    void Start()
    {
        button = this.GetComponent<Button>();
        button.onClick.AddListener(OpenDownloadPanel);
    }

    void OpenDownloadPanel()
    {
        if (scene.RuntimeKeyIsValid())
        {
            predownloadSceneController.OpenDownloadPanel(scene, this);
        }
    }

    AsyncOperationHandle<SceneInstance> handle;
    void LoadScene()
    {
        handle = Addressables.LoadSceneAsync(scene, LoadSceneMode.Single);
    }

    public Image downloadIcon;
    public void SwitchListener(bool sceneLoaded)
    {
        button.onClick.RemoveAllListeners();

        if (sceneLoaded)
        {
            button.onClick.AddListener(LoadScene);
            downloadIcon.gameObject.SetActive(false);
        }
        else
        {
            button.onClick.AddListener(OpenDownloadPanel);
            downloadIcon.gameObject.SetActive(true);
        }
    }
}
