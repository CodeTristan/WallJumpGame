using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject MainMenuObject;
    public GameObject ShopObject;
    public GameObject ShopPage1;
    public GameObject ShopPage2;

    public GameObject mainBG;
    public GameObject shopbg1;
    public GameObject shopbg2;
    public void Play()
    {
        SceneManager.LoadScene(1); // Game Scene
    }
    public void Shop()
    {
        MainMenuObject.SetActive(false);
        ShopObject.SetActive(true);
        mainBG.SetActive(false);
        shopbg1.SetActive(true);
        shopbg2.SetActive(false);
    }
    public void ShopToMenu()
    {
        MainMenuObject.SetActive(true);
        ShopObject.SetActive(false);
        mainBG.SetActive(true);
        shopbg1.SetActive(false);
        shopbg2.SetActive(false);
    }

    public void goShopPage1()
    {
        ShopPage1.SetActive(true);
        ShopPage2.SetActive(false);
        shopbg1.SetActive(true);
        shopbg2.SetActive(false);
    }
    public void goShopPage2()
    {
        ShopPage1.SetActive(false);
        ShopPage2.SetActive(true);
        shopbg1.SetActive(false);
        shopbg2.SetActive(true);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);  //MainMenu
    }
}
