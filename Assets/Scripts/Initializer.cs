using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Initializer : MonoBehaviour
{
    [SerializeField]
    AssetReferenceGameObject localizationController;

    const string HOME_ADDRESS = "Home";
    async void Start()
    {
        await Preloader.Preload();

        if (GameObject.FindObjectOfType<LocaleUpdater>() == null)
        {
            var localControlHandler = localizationController.InstantiateAsync();
            localControlHandler.Completed += (handle) =>
            {
                DontDestroyOnLoad(handle.Result);
                Debug.Log("Instantiate localization controller");
            };

            await localControlHandler.Task;
        }

        Addressables.LoadSceneAsync(HOME_ADDRESS).Completed += (handle) => 
        {
            Debug.Log("Direct to home...");
        };
    }
}
