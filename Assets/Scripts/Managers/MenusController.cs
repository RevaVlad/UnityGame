using UnityEngine;
using UnityEngine.EventSystems;

public static class MenusController
{
    public static void OpenMenu(GameObject menuCanvas, GameObject firstSelected)
    {
        menuCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
    public static void SwitchMenu(GameObject currentMenuCanvas, GameObject newMenuCanvas, GameObject firstSelected)
    {
        currentMenuCanvas.SetActive(false);
        OpenMenu(newMenuCanvas, firstSelected);
    }

    public static void CloseAllMenus(params GameObject[] menus)
    {
        foreach (var menu in menus)
            menu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }
}