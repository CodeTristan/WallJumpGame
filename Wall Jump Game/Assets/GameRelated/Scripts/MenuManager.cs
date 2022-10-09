using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject MainMenuObject;
    public GameObject ShopObject;

    public GameObject mainMenuBg;
    public GameObject shopBg;


    public void Play()
    {
        SceneManager.LoadScene(1); // Game Scene
    }
    public void Shop()
    {
        MainMenuObject.SetActive(false);
        mainMenuBg.SetActive(false);
        ShopObject.SetActive(true);
        shopBg.SetActive(true);
    }
    public void ShopToMenu()
    {
        MainMenuObject.SetActive(true);
        mainMenuBg.SetActive(true);
        ShopObject.SetActive(false);
        shopBg.SetActive(false);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);  //MainMenu
    }
}
