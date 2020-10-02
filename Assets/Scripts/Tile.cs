using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileState state;
    public bool inactive = false;
    public bool selected = false;
    public Vector2Int fieldPosition;
    public SpriteRenderer rend;

    public void ChangeState(TileState newState)
    {
        inactive = false;
        state = newState;
        rend.color = FieldManager.GetTileColor(newState);
        if (newState == TileState.Blocked || newState == TileState.Empty)
        {
            inactive = true;
        }
    }

    public void Select()
    {
        selected = true;
        var rendColor = rend.color;
        rendColor.a = 1f;
        rend.color = rendColor;
    }

    public void Deselect()
    {
        selected = false;
        rend.color = FieldManager.GetTileColor(state);
    }
}

