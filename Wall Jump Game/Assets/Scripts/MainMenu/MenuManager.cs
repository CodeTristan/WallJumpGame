using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [Header("Menus")]
    [SerializeField] Canvas shopMenu;
    [SerializeField] Canvas Settings;

    [Header("Buttons")]
    [SerializeField] Button PlayButton;
    [SerializeField] Button ShopButton;
    [SerializeField] Button SettingsButton;
    [SerializeField] Button ShopGoBackButton;
    public void Init()
    {
        shopMenu.enabled = false;
        Settings.enabled = false;

        PlayButton.onClick.AddListener(() =>
        {
            SahneManager.instance.LoadScene(SceneEnum.GameScene);
        });
        ShopButton.onClick.AddListener(() =>
        {
            shopMenu.enabled = true;
        });
        ShopGoBackButton.onClick.AddListener(() =>
        {
            shopMenu.enabled = false;
        });
        SettingsButton.onClick.AddListener(() => {
            Settings.enabled = true;
        });
    }
}
