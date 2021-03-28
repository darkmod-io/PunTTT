using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] Menu[] menus;
    [SerializeField] TMP_InputField username;

    void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menuName)
    {
        if (username.text == "") {
            GetComponent<Launcher>()._username = "Player " + Random.Range(0, 1000).ToString("0000");
        }
        else 
            GetComponent<Launcher>()._username = username.text;

        for (int i = 0; i < menus.Length; i++)
        {
            if(menus[i].menuName == menuName)
            {
                menus[i].Open();
            }
            else if(menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
    }
    public void OpenMenu(Menu menu)
    {
        if (username.text == "") {
            GetComponent<Launcher>()._username = "Player " + Random.Range(0, 1000).ToString("0000");
        }
        else 
            GetComponent<Launcher>()._username = username.text;

        for (int i = 0; i < menus.Length; i++)
        {
            if(menus[i].open)
            {
                CloseMenu(menus[i]);
            }
        }
        menu.Open();
    }
    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }
}
