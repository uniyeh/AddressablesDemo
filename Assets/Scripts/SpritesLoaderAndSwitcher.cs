using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class SpritesLoaderAndSwitcher : MonoBehaviour
{
    const string spriteLabel = "sprite";
    List<Sprite> sprites;
    int spritesAmount;

    public Image img;

    Button btn;

    AsyncOperationHandle spritesHandler;
    void Start()
    {
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(RandomSwtichSprites);
        btn.interactable = false;

        sprites = new List<Sprite>();

        spritesHandler = Addressables.LoadAssetsAsync<Sprite>(
            spriteLabel,
            sprite =>
            {
                sprites.Add(sprite);
            },
            true);
        spritesHandler.Completed += (spritesHandler) =>
        {
            if (spritesHandler.Status == AsyncOperationStatus.Succeeded)
            {
                btn.interactable = true;
                spritesAmount = sprites.Count;
            }
            else
            {
                spritesAmount = -1;
                Debug.LogError($"Failed to load assets: {spriteLabel}");
            }
        };
    }

    int prevIndex = -1;
    void RandomSwtichSprites()
    {
        if (spritesAmount < 0) return;

        int index = Random.Range(0, spritesAmount);
        // Avoid getting the same index contiunously
        if (prevIndex != -1 && index == prevIndex)
        {
            if (++index >= spritesAmount)
            {
                index = 0;
            }
        }

        img.sprite = sprites[index];
        
        prevIndex = index;
    }

    // Release all the loaded assets associated with loadHandle when destroyed
    void OnDestroy()
    {
        Addressables.Release(spritesHandler);
    }
}
