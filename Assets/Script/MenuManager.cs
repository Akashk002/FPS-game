using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Menu[] menus; // Array to hold all menu instances
    public static MenuManager Instance; // Singleton instance of MenuManager

    private void Awake()
    {
        Instance = this;
    }

    public void OpenMenu(string menuName)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == menuName)
            {
                menus[i].Open();
            }
            else
            if (menus[i].open)
            {
                CloseMenu(menus[i]); // Close all other menus
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].open) // Check if any menu is currently open
            {
                CloseMenu(menus[i]); // Close the currently open menu 
            }
        }
        menu.Open(); // Open the menu if the name matches
    }
    public void CloseMenu(Menu menu)
    {
        menu.Close(); // Close all menus
    }

    public void QuitGame()
    {
        Application.Quit(); // Quit the application
    }
}
