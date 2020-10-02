using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class FieldManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public float spacing = 10f;
    private const int FieldSideSize = 5;

    private Tile[,] currentField;



    public static Color GetTileColor(TileState state)
    {
        switch (state)
        {
            case TileState.Empty:
                return new Color(1f, 1f, 1f, 0.3f);
            case TileState.Blocked:
                return new Color(0.3f, 0.3f, 0.3f, 0f);
            case TileState.FirstColor:
                return new Color(1f, 0f, 0f, 0.6f);
            case TileState.SecondColor:
                return new Color(0f, 1f, 0f, 0.6f);
            case TileState.ThirdColor:
                return new Color(0f, 0f, 1f, 0.6f);
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
    
    void Start()
    {
        currentField = CreateField();
        FillFieldClassicWay(currentField);
    }

    public void SwapTiles(Tile first, Tile second)
    {
        var secondState = second.state;
        second.ChangeState(first.state);
        first.ChangeState(secondState);
    }
    
    public bool CheckWinCondition()
    {
        for (var i = 0; i < FieldSideSize * FieldSideSize; i++)
        {
            var x = i % FieldSideSize;
            var y = i / FieldSideSize;
            switch (x)
            {
                case 0 when currentField[x, y].state != TileState.FirstColor:
                case 2 when currentField[x, y].state != TileState.SecondColor:
                case 4 when currentField[x, y].state != TileState.ThirdColor:
                    return false;
            }
        }

        return true;
    }
    
    private static void FillFieldClassicWay(Tile[,] tiles)
    {
        var tileColors = new List<TileState>();
        
        for (var i = 0; i < FieldSideSize; i++)
        {
            tileColors.Add(TileState.FirstColor);
            tileColors.Add(TileState.SecondColor);
            tileColors.Add(TileState.ThirdColor);
        }
        
        var rand = new Random();
        var randomizedColors = new Stack<TileState>(tileColors.OrderBy(x => rand.Next()).ToArray());  

        for (var i = 0; i < FieldSideSize * FieldSideSize; i++)
        {
            var x = i % FieldSideSize;
            var y = i / FieldSideSize;
            TileState state;
            if (x % 2 == 1)
            {
                state = y % 2 == 1 ? TileState.Empty : TileState.Blocked;
            }
            else state = randomizedColors.Pop();
            tiles[x,y].ChangeState(state);
        }
    }

    private Tile[,] CreateField()
    {
        var cam = Camera.main;
        Debug.Assert(cam!= null, "Camera.main != null");
        Vector2 startPosition = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.4f, 0));
        startPosition -= new Vector2((FieldSideSize / 2)*spacing, (FieldSideSize / 2)*spacing);
        
        var tiles = new Tile[FieldSideSize,FieldSideSize];
        
        //Tiles
        for (var i = 0; i < FieldSideSize; i++)
        {
            for (var j = 0; j < FieldSideSize; j++)
            {
                tiles[j, i] = Instantiate(tilePrefab, new Vector2(startPosition.x + spacing*j, startPosition.y + spacing*i), Quaternion.identity).GetComponent<Tile>();
                tiles[j, i].fieldPosition = new Vector2Int(j, i);
            }
        }
        
        //Headers
        for (var i = 0; i < 3; i++)
        {
            var header = Instantiate(tilePrefab, new Vector2(startPosition.x + spacing*2*i, startPosition.y + spacing*1.05f *FieldSideSize), Quaternion.identity).GetComponent<Tile>();
            header.ChangeState(TileState.FirstColor + i);
            header.GetComponent<Collider2D>().enabled = false;
        }

        return tiles;
    }

    public void StartEndgameFirework()
    {
        StartCoroutine(StartFirework());
    }
    
    private IEnumerator StartFirework()
    {
        var counter = 0;
        var state = (int)TileState.FirstColor;
        while (true)
        {
            for (var i = 0; i < FieldSideSize * FieldSideSize; i++)
            {
                var x = i % FieldSideSize;
                var y = i / FieldSideSize;
                currentField[x,y].ChangeState((TileState)state);
            }

            counter = (counter+1)%3;
            state = 2 + counter;
            yield return new WaitForSeconds(1f);
        }
    }
}
