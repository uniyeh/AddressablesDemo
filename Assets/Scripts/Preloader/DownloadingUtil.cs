using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.Exceptions;

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

    public static bool CheckIfEnoughSpaceToDownload(long downloadSize)
    {
        long spaceFree = Caching.currentCacheForWriting.spaceFree;
        if (spaceFree < downloadSize)
        {
            Debug.Log($"No enough space for download: {downloadSize} / {spaceFree}");
            return false;
        }

        return true;
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

    public static async Task UpdateCatalogs()
    {
        Addressables.InitializeAsync();
        
        Debug.Log("Check updates begin.");

        var checkUpdatHandle = Addressables.CheckForCatalogUpdates();
        var catalogs = await checkUpdatHandle.Task;
        Debug.Log($"Catalogs: {catalogs.Count}");

        if (catalogs != null && catalogs.Count > 0)
        {
            Debug.Log("Update catalogs begin.");
            
            // Update the catalog cached locally
            Addressables.UpdateCatalogs(catalogs);

            Debug.Log("Update catalogs ended.");
        }
    }

    public static async Task<bool> DownloadUncachedBundles()
    {
        bool downloadSuccess = false;
        List<IResourceLocation> bundleLocationsNotCached = new List<IResourceLocation>();
        long totalDownloadSize = 0;

        foreach (IResourceLocator locator in Addressables.ResourceLocators)
        {
            var map = locator as ResourceLocationMap; // Simple implementation of IResourceLocator

            if (map == null || map.Locations.Count <= 0)
            {
                Debug.Log("Skipping bundle: map.Locations = 0");
                continue;
            }

            foreach (KeyValuePair<object, IList<IResourceLocation>> mapLocation in map.Locations)
            {
                foreach (var loc in mapLocation.Value)
                {
                    if (loc.Data is ILocationSizeData sizeData && !bundleLocationsNotCached.Contains(loc))
                    {
                        long downloadSize = sizeData.ComputeSize(loc, Addressables.ResourceManager);
                        totalDownloadSize += downloadSize;
                        bundleLocationsNotCached.Add(loc);
                    }
                }
            }
        }

        if (bundleLocationsNotCached.Count <= 0)
        {
            Debug.Log("No available updates.");
            return downloadSuccess;
        }
        else
        {
            if (CheckIfEnoughSpaceToDownload(totalDownloadSize))
            {
                var downloadHandle = Addressables.DownloadDependenciesAsync(bundleLocationsNotCached, true);
                downloadHandle.Completed += (handle) =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        Debug.Log("Download Updates Success.");
                        downloadSuccess = true;
                    }
                    else
                    {
                        Debug.Log($"Download Updates Failed: {GetDownloadError(handle)}");
                    }
                };

                await downloadHandle.Task;
            }
            else
            {
                Debug.Log("No enough space for downloading.");
            }
        }

        return downloadSuccess;
    }

    public static string GetDownloadError(AsyncOperationHandle handle)
    {
        if (handle.Status != AsyncOperationStatus.Failed)
            return null;

        RemoteProviderException remoteException;
        Exception e = handle.OperationException;
        while (e != null)
        {
            remoteException = e as RemoteProviderException;
            if (remoteException != null)
                return remoteException.WebRequestResult.Error;

            e = e.InnerException;
        }

        return null;
    }
}
