using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadBackground : MonoBehaviour
{
    [SerializeField]
    AssetReferenceSprite background;

    Image img;

    void Start()
    {
        img = this.GetComponent<Image>();

        LoadBackgroundSprite();
        CatalogUpdater.cachedBundlesUpdateEventHandler += LoadBackgroundSprite;
    }

    void OnDestroy()
    {
        if (handle.IsValid())
        {
            background.ReleaseAsset();
        }
    }

    AsyncOperationHandle<Sprite> handle;
    void LoadBackgroundSprite()
    {
        if (img == null)
            return;

        if (handle.IsValid())
        {
            background.ReleaseAsset();
            Caching.ClearCache();
        }
        
        handle = background.LoadAssetAsync();
        handle.Completed += (handle) =>
        {
            img.sprite = handle.Result;
            Debug.Log("Load background complete");
        };
    }
}
