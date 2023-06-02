using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class AddressableAssetsIniter : MonoBehaviour
{
    [SerializeField]
    [AssetReferenceUILabelRestriction("sprite", "background")]
    AssetReferenceSprite spriteReference;
    [SerializeField]
    Image image;

    [SerializeField]
    AssetReferenceGameObject cubeReference;
    [SerializeField]
    Button materialChanger;

    AsyncOperationHandle<Sprite> spriteHandle;
    AsyncOperationHandle<GameObject> cubeHandle;

    void Start()
    {
        if (spriteReference.RuntimeKeyIsValid())
        {
            spriteHandle = spriteReference.LoadAssetAsync<Sprite>();
            spriteHandle.Completed += (spriteHandle) =>
            {
                if (spriteHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    image.sprite = spriteHandle.Result;
                }
                else
                {
                    Debug.LogError($"Failed to load assets: {spriteReference.AssetGUID}");
                }
            };
        }
        else
        {
            Debug.LogError("Runtime key is valid: SpriteReference");
        }

        if (cubeReference.RuntimeKeyIsValid())
        {
            cubeHandle = cubeReference.InstantiateAsync();
            cubeHandle.Completed += (cubeHandle) =>
            {
                if (cubeHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject cube = cubeHandle.Result;
                    cube.transform.position = new Vector3(0f, 0f, -5f);
                    cube.transform.Rotate(new Vector3(0f, 50f, 35f));
                    cube.SetActive(true);
                    materialChanger.GetComponent<MaterialChanger>().Init(cube);
                }
                else
                {
                    Debug.LogError($"Failed to load assets: {cubeReference.AssetGUID}");
                }
            };
        }
        else
        {
            Debug.LogError("Runtime key is valid: CubeReference");
        }

    }

    void OnDestroy()
    {
        // Release AssetReference
        if (spriteReference != null && spriteHandle.IsValid())
        {
            spriteReference.ReleaseAsset();
        }

        // Release AssetReference
        if (cubeReference != null && cubeHandle.IsValid())
        {
            // Release and destroy the object created via Addressables.InitiateAsync
            Addressables.ReleaseInstance(cubeHandle);
        }
    }
}
