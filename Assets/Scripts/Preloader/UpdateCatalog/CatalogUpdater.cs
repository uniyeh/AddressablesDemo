using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatalogUpdater : MonoBehaviour
{
    Button button;
    void Start()
    {
        button = this.GetComponent<Button>();
        button.onClick.AddListener(UpdateCatalogAndDownload);
    }

    async void UpdateCatalogAndDownload()
    {
        button.interactable = false;
        
        await DownloadingUtil.UpdateCatalogs();
        bool updateSuccess = await DownloadingUtil.DownloadUncachedBundles();

        if (updateSuccess)
            OnCachedBundlesUpdated();

        button.interactable = true;
        Debug.Log("Update catalog and download done.");
    }

    public delegate void CachedBundlesUpdateEventHandler();
    public static event CachedBundlesUpdateEventHandler cachedBundlesUpdateEventHandler;

    void OnCachedBundlesUpdated()
    {
        cachedBundlesUpdateEventHandler?.Invoke();
    }
}
