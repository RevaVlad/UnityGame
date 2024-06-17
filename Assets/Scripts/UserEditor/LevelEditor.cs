using System;
using UnityEngine;

public class LevelEditor : MonoBehaviour
{
    public ItemController[] ItemButtons;
    public GameObject[] ItemPrefabs;
    public GameObject[] SelectedPefabsImages;
    public int CurrentButtonPressed;

    private void Update()
    {
        var screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        var worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        if (Input.GetMouseButtonDown(0) && ItemButtons[CurrentButtonPressed].Clicked)
        {
            if (!ItemButtons[CurrentButtonPressed].wasUpdated)
            {
                ItemButtons[CurrentButtonPressed].Clicked = false;
                Instantiate(ItemPrefabs[CurrentButtonPressed],
                    new Vector3((float)Math.Round(worldPosition.x), (float)Math.Round(worldPosition.y) - 0.5f, 0),
                    Quaternion.identity);
                Destroy(GameObject.FindGameObjectWithTag("PrefabImage"));
            }
            else
                ItemButtons[CurrentButtonPressed].wasUpdated = false;
        }
    }
}