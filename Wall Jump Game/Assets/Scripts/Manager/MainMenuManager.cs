using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    [SerializeField] private MenuManager menuManager;
    [SerializeField] private Shop shop;

    public void Init()
    {
        instance = this;

        //menuManager.Init();
        //shop.Init();
    }
}
