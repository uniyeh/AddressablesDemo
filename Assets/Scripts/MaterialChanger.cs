using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MaterialChanger : MonoBehaviour
{
    Button btn;

    GameObject obj;
    Renderer objRenderer;

    [SerializeField]
    AssetReferenceMaterial defaultMaterialReference;

    const string label = "material";

    AsyncOperationHandle<GameObject> objHandle;
    AsyncOperationHandle<Material> defaultMatHandle;
    AsyncOperationHandle matsHandle;

    List<Material> mats;

    void Awake()
    {
        btn = this.GetComponent<Button>();
        btn.interactable = false;
    }

    void OnDestroy()
    {
        Addressables.Release(defaultMatHandle);
        Addressables.Release(matsHandle);
    }

    int index = 0;
    void ChangeMaterial()
    {
        if (++index >= mats.Count)
            index = 0;

        objRenderer.material = mats[index];
    }

    public void Init(GameObject go)
    {
        obj = go;
        objRenderer = go.GetComponent<Renderer>();

        if (objRenderer == null)
            return;

        mats = new List<Material>();

        if (defaultMaterialReference.RuntimeKeyIsValid())
        {
            defaultMatHandle = defaultMaterialReference.LoadAssetAsync<Material>();
            defaultMatHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    objRenderer.material = handle.Result;
                }
                else
                {
                    Debug.LogError($"Failed to load material: {defaultMaterialReference.AssetGUID}");
                }
            };
        }

        matsHandle = Addressables.LoadAssetsAsync<Material>(label,
            mat =>
            {
                mats.Add(mat);
            },
            false);

        matsHandle.Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                if (mats.Count >= 0)
                {
                    btn.onClick.AddListener(ChangeMaterial);
                    btn.interactable = true;
                }
            }
            else
            {
                Debug.LogError($"Failed to load lebel: {label}");
            }
        };
    }
}
