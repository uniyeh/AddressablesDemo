using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

public class LocaleUpdater : MonoBehaviour
{
    [SerializeField] public static string language = "en-US";

    void Awake()
    {
        CheckAndUpdateLocale();

        SceneManager.sceneLoaded += LogCurrentLocale;
        //SceneManager.sceneLoaded += ForceInvokeSelectedLocaleChangedEvent;
    }

    private void OnApplicationFocus(bool focus)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (focus)
        {
            CheckAndUpdateLocale();           
        }
#endif
    }

    void CheckAndUpdateLocale()
    {
        language = GetLocaleDisplayLanguage();
        LoadLocale(language);
    }

    Locale selectedLocale;
    private void LoadLocale(string displayingLanguage)
    {
        Debug.Log($"Load locale: {displayingLanguage}");

        LocaleIdentifier localeCode = new LocaleIdentifier(displayingLanguage);

        selectedLocale = null;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            selectedLocale = LocalizationSettings.AvailableLocales.Locales[i];
            LocaleIdentifier identifier = selectedLocale.Identifier;

            if (identifier == localeCode)
            {
                LocalizationSettings.SelectedLocale = selectedLocale;
                break;
            }
        }

        Debug.Log($"Language display: {displayingLanguage}, selected: {selectedLocale}");
    }

    private string GetLocaleDisplayLanguage()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaObject localeList = new AndroidJavaObject("android.os.LocaleList");
        AndroidJavaObject locale = new AndroidJavaObject("java.util.Locale");
        locale = localeList.CallStatic<AndroidJavaObject>("getDefault").Call<AndroidJavaObject>("get", 0);

        string displayingLanguage = locale.Call<string>("getLanguage").Split('-')[0];
        string countryCode = locale.Call<string>("getCountry");

        string result = $"{displayingLanguage}-{countryCode}";

        Debug.Log($"GetLocaleDisplayLanguage: {result}");
        return result;
#endif

        return "here is not the target platform";
    }

    void ForceInvokeSelectedLocaleChangedEvent(Scene scene, LoadSceneMode mode)
    {
        CheckAndUpdateLocale();

        if (selectedLocale != null)
        {
            Debug.Log($"Selected locale: {selectedLocale.LocaleName}");
            LocalizationSettings.Instance.SetSelectedLocale(selectedLocale);
        }
    }

    void LogCurrentLocale(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Current Locale: {LocalizationSettings.Instance.GetSelectedLocale()}");
    }
}
