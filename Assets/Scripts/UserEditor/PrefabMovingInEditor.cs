using System;
using Unity.VisualScripting;
using UnityEngine;

public class PrefabMovingInEditor : MonoBehaviour
{
    [SerializeField] private int ID;
    private LevelEditor editor;

    private void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "UsersLevel")
            editor = GameObject.FindGameObjectWithTag("LevelEditor").GetComponent<LevelEditor>();
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && editor is not null)
        {
            editor.CurrentButtonPressed = ID;
            var screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            var worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            Instantiate(editor.SelectedPefabsImages[ID],
                new Vector3((float)Math.Round(worldPosition.x), (float)Math.Round(worldPosition.y), 0),
                Quaternion.identity);
            editor.ItemButtons[ID].Clicked = true;
            editor.ItemButtons[ID].wasUpdated = true;
            Destroy(this.GameObject());
            
        }

        if (Input.GetMouseButtonDown(1) && editor is not null)
        {
            Destroy(this.GameObject());
            editor.ItemButtons[ID].IncreaseQuantity();
        }
    }
}