using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.ResourceManagement.ResourceProviders;

public class PredownloadSceneController : MonoBehaviour
{
    const string downloadPanel = "Assets/Prefabs/DownloadPanel.prefab";

    Canvas canvas;

    SceneOpener opener;

    void Start()
    {
        canvas = this.GetComponentInParent<Canvas>().rootCanvas;
    }

    public void OpenDownloadPanel(AssetReference asset, SceneOpener opener)
    {
        this.opener = opener;

        var panelHandle = Addressables.InstantiateAsync(downloadPanel, canvas.transform);
        panelHandle.Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject panelGO = handle.Result;
                DownloadPanelWithProgress panel = panelGO.GetComponent<DownloadPanelWithProgress>();
                panel.assetRef = asset;
                panel.predownloadSceneController = this;
                panelGO.SetActive(true);
            }
            else
            {
                Debug.LogError("Open download panel failed.");
            }
        };

        Debug.Log($"OpenDownloadPanel: {opener == null}");
    }

    public void UpdateDownloadResult(bool downloadSuccess)
    {
        Debug.Log($"UpdateDownloadResult: {opener == null}");
        opener.SwitchListener(downloadSuccess);
        opener = null;
    }
}
