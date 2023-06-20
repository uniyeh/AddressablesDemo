using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using TMPro;
using UnityEngine.Localization.Settings;

[Serializable]
public class LocalizedTMPFont : LocalizedAsset<TMP_FontAsset> { }

[Serializable]
public class UnityEventTMPFont : UnityEvent<TMP_FontAsset> { }

public class FontLocalization : LocalizedAssetEvent<TMP_FontAsset, LocalizedTmpFont, UnityEventTMPFont>
{
    const string ASSET_TABLE = "Asset Table";
    const string ENTRY_FONT = "FONT";
    TextMeshProUGUI tmp;

    void Start()
    {
        tmp = this.GetComponent<TextMeshProUGUI>();
        if (AssetReference != null)
        {
            AssetReference.TableReference = ASSET_TABLE;
            AssetReference.TableEntryReference = ENTRY_FONT;
            tmp.font = LocalizationSettings.AssetDatabase.GetLocalizedAsset<TMP_FontAsset>(ASSET_TABLE, ENTRY_FONT);
            Debug.Log($"Font init: {tmp.font.name}");
        }
    }

    protected override void UpdateAsset(TMP_FontAsset localizedAsset)
    {
        if (localizedAsset != null && tmp != null)
        {
            tmp.font = localizedAsset;
            Debug.Log($"Font updated: {tmp.font.name}");
        }
    }
}
