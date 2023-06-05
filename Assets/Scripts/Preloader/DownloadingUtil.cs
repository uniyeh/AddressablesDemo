using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;
using System.Threading.Tasks;

public class DownloadingUtil
{
    public static async Task<long> GetKeyDownloadSizeSync(object key)
    {
        long downloadSize = 0;
        try
        {
            downloadSize = await Addressables.GetDownloadSizeAsync(key).Task;
            if (downloadSize > 0)
            {
                downloadSuccess = false;
                Debug.Log($"Download size: {key} - {downloadSize}");
            }
            else
            {
                downloadSuccess = true;
                percentageComplete = 0f;

                OnDownload(key, percentageComplete, downloadSuccess);
                Debug.Log($"No need to download.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Get download size erro: {e.Message}");
        }

        return downloadSize;
    }

    static bool downloadSuccess;
    static float percentageComplete;
    public static async Task DownloadAssets(object key)
    {
        downloadSuccess = false;

        AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(key, true);
        downloadHandle.Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                downloadSuccess = true;
                percentageComplete = 100;
                Debug.Log("Download success.");
                OnDownload(key, percentageComplete, downloadSuccess);
            }
            else
            {
                downloadSuccess = false;
                percentageComplete = 0f;

                Debug.Log("Download failed.");
                OnDownload(key, percentageComplete, downloadSuccess);
            }

        };

        await Task.Run(() =>
        {
            while (downloadHandle.Status == AsyncOperationStatus.None)
            {
                downloadSuccess = false;
                percentageComplete = downloadHandle.GetDownloadStatus().Percent;

                OnDownload(key, percentageComplete, downloadSuccess);
            }
        });
    }

    public static void ClearAddressablesDownload()
    {
        if (Caching.ClearCache())
        {
            Debug.Log("Clear cache success");
        }
        else
        {
            Debug.Log("Cache is being used");
        }
    }


    public delegate void DownloadEventHandler(object sender, DownloadEventArgs args);
    public static event DownloadEventHandler DownloadEvent;
    static void OnDownload(object key, float percentage, bool status)
    {
        DownloadEvent?.Invoke(null, new DownloadEventArgs(key, percentage, status));
    }
}
