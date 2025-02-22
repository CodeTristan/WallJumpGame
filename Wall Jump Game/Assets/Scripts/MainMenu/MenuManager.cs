using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [Header("Menus")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject shopMenu;
    [SerializeField] GameObject Settings;

    [Header("Buttons")]
    [SerializeField] Button PlayButton;
    [SerializeField] Button ShopButton;
    [SerializeField] Button SettingsButton;
    [SerializeField] Button ShopGoBackButton;
    public void Init()
    {
        mainMenu.SetActive(true);
        shopMenu.SetActive(false);

        PlayButton.onClick.AddListener(() =>
        {
            SahneManager.instance.LoadScene(SceneEnum.GameScene);
        });
        ShopButton.onClick.AddListener(() =>
        {
            mainMenu.SetActive(false);
            shopMenu.SetActive(true);
        });
        ShopGoBackButton.onClick.AddListener(() =>
        {
            mainMenu.SetActive(true);
            shopMenu.SetActive(false);
        });
        SettingsButton.onClick.AddListener(() => {
            mainMenu.SetActive(false);
            Settings.SetActive(true);
        });
    }
}
