using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets.ResourceLocators;
using System.Threading.Tasks;
using TMPro;

public class DownloadPanelWithProgress : MonoBehaviour
{
    public AssetReference assetRef;
    public PredownloadSceneController predownloadSceneController;

    [SerializeField]
    TextMeshProUGUI sizeInfo;

    [SerializeField]
    Slider progressBar;
    [SerializeField]
    TextMeshProUGUI percentageInfo;

    [SerializeField]
    Button downloadBtn;
    TextMeshProUGUI downloadBtnText;

    [SerializeField]
    Button cancelBtn;

    async void Start()
    {
        cancelBtn.onClick.AddListener(ClosePanel);

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Preload error: No internet connected.");
            downloadBtn.interactable = false;
            return;
        }
        
        InitOnDownloadingEvent();
        downloadMB = 0;

        Addressables.InitializeAsync();

        bool downloadable = await UpdateSizeInfo();
        downloadBtn.interactable = downloadable;
        if (downloadable)
        {
            downloadBtn.onClick.AddListener(DownloadAssets);
        }
        else
        {
            downloadBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Download process unenabled";
            Debug.Log("Download process will not be invoke.");
        }
    }

    void LateUpdate()
    {
        if (!progresUpdating)
        {
            if (progressBar.value != currentPercentage / 100f)
            {
                UpdateProgress(currentPercentage);
            }

            return;
        }

        UpdateProgress(currentPercentage);
    }

    float downloadMB = 0;
    async Task<bool> UpdateSizeInfo()
    {
        long downloadBytes = await DownloadingUtil.GetKeyDownloadSizeSync(assetRef);
        bool enoughSpace = false;

        if (downloadBytes <= 0)
        {
            downloadBytes = 0;
            Debug.Log("No need for download...");
        }
        else
        {
            enoughSpace = DownloadingUtil.CheckIfEnoughSpaceToDownload(downloadBytes);
            downloadMB = ConvertBytesToMB(downloadBytes);
        }

        sizeInfo.text = $"Size: {downloadMB}MB";

        return downloadMB > 0 && enoughSpace;
    }

    float ConvertBytesToMB(long bytes)
    {
        return (float)Math.Round(bytes / (1024m * 1024m), 2);
    }

    async void DownloadAssets()
    {
        if (downloadMB <= 0)
            return;

        Debug.Log("Start Downloading assets...");
        downloadBtn.interactable = false;

        Debug.Log("Clearing unused cache...");
        DownloadingUtil.ClearAddressablesDownload();

        Debug.Log("Downloading...");
        await DownloadingUtil.DownloadAssets(assetRef);

        Debug.Log("Download ended...");
    }

    void ClosePanel()
    {
        Addressables.ReleaseInstance(this.gameObject);
    }

    void InitOnDownloadingEvent()
    {
        DownloadingUtil.DownloadEvent -= InteractWithDownloadStatus;
        DownloadingUtil.DownloadEvent += InteractWithDownloadStatus;
    }

    bool progresUpdating;
    float currentPercentage;
    void InteractWithDownloadStatus(object sender, DownloadEventArgs args)
    {
        if (args.Key != assetRef)
            return;

        if (args.Success)
        {
            progresUpdating = false;
            currentPercentage = args.Percentage;

            predownloadSceneController.UpdateDownloadResult(args.Success);
        }
        else
        {
            progresUpdating = true;
            currentPercentage = args.Percentage;
        }

    }

    void UpdateProgress(float percentage)
    {
        Debug.Log($"UpdateProgress: {percentage}");

        float convertedPercentage = (float)Math.Round((double)percentage * 100, 2);
        convertedPercentage = convertedPercentage > 99.5f ? 100f : convertedPercentage;

        progressBar.value = percentage;
        percentageInfo.text = $"{convertedPercentage}%";
    }
}
