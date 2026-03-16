using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] Tile[] tiles = new Tile[9];
    GameObject[] playerPieces = new GameObject[9];
    [SerializeField] GameObject playerPieceA, playerPieceB;

    private void Start()
    {
        for (int i = 0; i < tiles.Length; i++)
            tiles[i].ID = i;
    }

    public void MakeMove(int move, Player player)
    {
        playerPieces[move] = Instantiate(player == Player.Human ? playerPieceA : playerPieceB, tiles[move].transform.position + new Vector3(0, 0, -0.1f), Quaternion.identity, this.gameObject.transform);
    }

    public void ClearBoard()
    {
        for(int i = tiles.Length - 1; i >= 0; i--)
        {
            if (playerPieces[i] == null) continue;
            Destroy(playerPieces[i].gameObject);
        }

        playerPieces = new GameObject[9];
    }
}