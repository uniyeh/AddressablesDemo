using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;

public class BackToHomeButton : MonoBehaviour
{
    Button button;
    const string homeScene = "Home (Initailization Scene)";

    void Start()
    {
        button = this.GetComponent<Button>();
        button.onClick.AddListener(BackToHome);
    }

    void BackToHome()
    {
        SceneManager.LoadScene(homeScene, LoadSceneMode.Single);
    }
}
