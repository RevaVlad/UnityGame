using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    [SerializeField] private int ID;
    [SerializeField] private int quantity;
    [SerializeField] private Text quantityText;
    public bool Clicked;
    private LevelEditor editor;
    
    void Start()
    {
        quantityText.text = $"{quantity}";
        editor = GameObject.FindGameObjectWithTag("LevelEditor").GetComponent<LevelEditor>();
    }

    public void ButtonClicked()
    {
        if (quantity > 0)
        {
            var screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            var worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            Clicked = true;
            Instantiate(editor.SelectedPefabsImages[ID],
                new Vector3((float)Math.Round(worldPosition.x), (float)Math.Round(worldPosition.y), 0),
                Quaternion.identity);
            quantity--;
            quantityText.text = $"{quantity}";
            editor.CurrentButtonPressed = ID;
        }
    }
    
}
