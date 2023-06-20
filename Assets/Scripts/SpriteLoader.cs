using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class SpriteLoader : MonoBehaviour
{
    const string spriteAddress = "Assets/Sprites/1.jpg";
    Image img;

    AsyncOperationHandle<Sprite> spriteHandle;
    void Start()
    {
        img = this.GetComponent<Image>();
        if (img == null)
            return;

        // Load by address
        spriteHandle = Addressables.LoadAssetAsync<Sprite>(spriteAddress);
        
        // Assgin the sprite to the image on complete
        spriteHandle.Completed += (spriteHandle) =>
        {
            if (spriteHandle.Status == AsyncOperationStatus.Succeeded)
            {
                img.sprite = spriteHandle.Result;
            }
            else
            {
                Debug.LogError($"Failed to load asset: {spriteAddress}");
            }
        };
    }

    // Release handle when destroyed
    void OnDestroy()
    {
        if (spriteHandle.IsValid())
        {
            Addressables.Release(spriteHandle);
            Debug.Log("Sprites handle released");
        }
    }
}

