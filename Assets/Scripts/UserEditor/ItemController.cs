using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    [SerializeField] private int ID;
    [SerializeField] private int quantity;
    [SerializeField] private Text quantityText;
    public bool Clicked;
    public bool wasUpdated;
    private LevelEditor _editor;
    
    void Start()
    {
        quantityText.text = $"{quantity}";
        _editor = GameObject.FindGameObjectWithTag("LevelEditor").GetComponent<LevelEditor>();
    }

    public void IncreaseQuantity()
    {
        quantity++;
        quantityText.text = $"{quantity}";
    }

    public void ButtonClicked()
    {
        if (quantity > 0)
        {
            var screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            var worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            Clicked = true;
            Instantiate(_editor.SelectedPefabsImages[ID],
                new Vector3((float)Math.Round(worldPosition.x), (float)Math.Round(worldPosition.y), 0),
                Quaternion.identity);
            quantity--;
            quantityText.text = $"{quantity}";
            _editor.CurrentButtonPressed = ID;
        }
    }
    
}
