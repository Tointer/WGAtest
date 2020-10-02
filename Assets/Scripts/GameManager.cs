using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Camera cam;
    private Tile selectedTile;
    public FieldManager field;
    public bool areYouWinningSon;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0) || areYouWinningSon) return;
        Vector2 mousePos2D = cam.ScreenToWorldPoint(Input.mousePosition);
            
        var hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider == null)
        {
            DeselectCurrentTile();
            return;
        }
        
        var tile = hit.collider.GetComponent<Tile>();
        if (tile == null || tile.selected || selectedTile != null && TrySwap(selectedTile, tile) || tile.inactive)
        {
            DeselectCurrentTile();
            return;
        }

        DeselectCurrentTile();
        tile.Select();
        selectedTile = tile;
    }

    private void DeselectCurrentTile()
    {
        if (selectedTile == null) return;
        selectedTile.Deselect();
        selectedTile = null;
    }

    private bool TrySwap(Tile first, Tile second)
    {
        if (Mathf.Abs(first.fieldPosition.x - second.fieldPosition.x) +
            Mathf.Abs(first.fieldPosition.y - second.fieldPosition.y) != 1
            || second.state != TileState.Empty)
            return false;
        field.SwapTiles(first, second);
        
        if (!field.CheckWinCondition()) return true;
        
        Debug.Log("Win!");
        areYouWinningSon = true;
        field.StartEndgameFirework();
        return true;
    }
}
