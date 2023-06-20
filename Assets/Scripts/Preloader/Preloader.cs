using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Preloader
{
    const string preloadLabel = "preload";

    public static async Task Preload()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Preload error: No internet connected.");
            return;
        }

        Addressables.InitializeAsync();

        long downloadSize = await DownloadingUtil.GetKeyDownloadSizeSync(preloadLabel);

        if (downloadSize > 0)
        {
            Debug.Log("Clearing Cache...");
            DownloadingUtil.ClearAddressablesDownload();

            Debug.Log("Downloading...");
            await DownloadingUtil.DownloadAssets(preloadLabel);
        }

        Debug.Log("Preload ended...");
    }
}
