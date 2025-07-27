using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public string menuName; // Name of the menu, can be used for identification
    public bool open;
    public void Open()
    {
        open = true; // Set the menu state to open
        gameObject.SetActive(true); // Activate the menu GameObject to open it
    }

    public void Close()
    {
        open = false; // Set the menu state to closed
        gameObject.SetActive(false); // Deactivate the menu GameObject to close it
    }
}
