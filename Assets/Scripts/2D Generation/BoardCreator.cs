using Pathfinding;
using System;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{
    // The type of tile that will be laid in a specific position.
    public enum TileType
    {
        Outside,
        OutsideMap,
        Wall,
        Floor
    }

    private enum TileCountDirections
    {
        Adjacent,
        Diagonal,
        Both
    }

    private const int North = 0;
    private const int South = 1;
    private const int East = 2;
    private const int West = 3;
    private const int NorthWest = 4;
    private const int NorthEast = 5;
    private const int SouthWest = 6;
    private const int SouthEast = 7;

    // The open part of wall1 tiles points south
    // The open parts of wall2 2 tiles point west and south
    // The open parts of wall2 tiles points north and south
    // The open parts of wall3 point south, west, and east
    // Wall 4s don't need to be rotated as they point in all directions

   
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private GameObject enemy;

    [SerializeField]
    private AstarPath path;
    
    [SerializeField]
    private bool removeLooseWalls;

    [SerializeField]
    private GenerationParams gParams;

    [SerializeField]
    private TilePrefabs tilePrefabs;

    private TileType[][] tiles;                               // A jagged array of tile types representing the board, like a grid.
    private Room[] rooms;                                     // All the rooms that are created for this board.
    private Corridor[] corridors;                             // All the corridors that connect the rooms.
    private GameObject boardHolder;                           // GameObject that acts as a container for all other tiles.

    private void Start()
    {
        // Create the board holder.
        boardHolder = new GameObject("BoardHolder");

        SetupTilesArray();

        CreateRoomsAndCorridors();

        SetTileValuesForFloors();
        SetTileValuesForWalls();

        if(removeLooseWalls)
        {
            RemoveLooseWalls();
        }

        InstantiateTiles();
        ConfigureGraph();
        SelectEnemySpawnRoom();
    }

    void SelectEnemySpawnRoom()
    {
        int index = UnityEngine.Random.Range(0, rooms.Length - 1);
        Room room = rooms[index];
        Vector2Int start = room.RoomStart;
        Vector2Int size = room.RoomSize;
        enemy.transform.position = new Vector2(start.x + size.x / 2.0f, start.y + size.y / 2.0f);

        enemy.GetComponent<Seeker>().StartPath(enemy.transform.position, player.transform.position);
    }

    void ConfigureGraph()
    {
        int xMin = int.MaxValue;
        int yMin = int.MaxValue;
        int xMax = int.MinValue;
        int yMax = int.MinValue;

        for(int x = 0; x < gParams.columns; x++)
        {
            for (int y = 0; y < gParams.rows; y++)
            {
                if (tiles[x][y] != TileType.OutsideMap)
                {
                    xMin = Mathf.Min(xMin, x);
                    yMin = Mathf.Min(yMin, y);
                    xMax = Mathf.Max(xMax, x);
                    yMax = Mathf.Max(yMax, y);
                }
            }
        }

        GridGraph graph = path.data.graphs[0] as GridGraph;
        graph.center = new Vector2(
            xMin + (xMax - xMin) / 2.0f,
            yMin + (yMax - yMin) / 2.0f
        );
        graph.SetDimensions(xMax - xMin + 1, yMax - yMin + 1, 1.0f);
        graph.collision.mask = LayerMask.GetMask("World");
        path.Scan();

    }


    void SetupTilesArray()
    {
        // Set the tiles jagged array to the correct width.
        tiles = new TileType[gParams.columns][];

        // Go through all the tile arrays...
        for (int i = 0; i < tiles.Length; i++)
        {
            // ... and set each tile array is the correct height.
            tiles[i] = new TileType[gParams.rows];

            for(int j = 0; j < tiles[i].Length; j++)
            {
                tiles[i][j] = TileType.OutsideMap;
            }
        }
    }


    void CreateRoomsAndCorridors()
    {
        // Create the rooms array with a random size.
        rooms = new Room[gParams.numRooms.Random];

        // There should be one less corridor than there is rooms.
        corridors = new Corridor[rooms.Length - 1];

        // Create the first room and corridor.
        rooms[0] = new Room();
        corridors[0] = new Corridor();

        // Setup the first room, there is no previous corridor so we do not use one.
        rooms[0].SetupRoom(gParams);//roomWidth, roomHeight, columns, rows);
        rooms[0].enteringCorridorInstance = null;
        rooms[0].exitingCorridorInstance = corridors[0];

        // Setup the first corridor using the first room.
        corridors[0].SetupCorridor(gParams, rooms[0], true);

        for (int i = 1; i < rooms.Length; i++)
        {
            // Create a room.
            rooms[i] = new Room();
            rooms[i].enteringCorridorInstance = corridors[i - 1];

            // Setup the room based on the previous corridor.
            rooms[i].SetupRoom(gParams, corridors[i - 1]);

            // If we haven't reached the end of the corridors array...
            if (i < corridors.Length)
            {
                // ... create a corridor.
                corridors[i] = new Corridor();
                rooms[i].exitingCorridorInstance = corridors[i];

                // Setup the corridor based on the room that was just created.
                corridors[i].SetupCorridor(gParams, rooms[i], false);
            }
            else
            {
                rooms[i].exitingCorridorInstance = null;
            }

            if (i == rooms.Length - 1)
            {
                // Pick a random bit of the room to spawn in
                int x = UnityEngine.Random.Range(1, rooms[i].roomWidth - 1) + rooms[i].xPos;
                int y = UnityEngine.Random.Range(1, rooms[i].roomHeight - 1) + rooms[i].yPos;
                player.transform.position = new Vector2(x, y);
            }
        }

    }

    void WriteTile(int x, int y, TileType type, bool overrideCurrent=false)
    {
        if (tiles[x][y] == TileType.OutsideMap || overrideCurrent)
        {
            tiles[x][y] = type;
        }
    }

    void SetTileValuesForFloors()
    {
        // Go through every corridor...
        foreach(Corridor corridor in corridors)
        {
            Vector2Int start = corridor.StartPosition;
            Vector2Int direction = corridor.DirectionVector;

            // and go through it's length.
            for (int j = 0; j < corridor.corridorLength; j++)
            {
                // Start the coordinates at the start of the corridor.
                Vector2Int tile = start + direction * j;
                WriteTile(tile.x, tile.y, TileType.Floor);
            }
        }

        // Go through every room
        foreach(Room room in rooms)
        {
            Vector2Int start = room.RoomStart;
            Vector2Int size = room.RoomSize;

            for(int x = start.x; x < start.x + size.x; x++)
            {
                for(int y = start.y; y < start.y + size.y; y++)
                {
                    WriteTile(x, y, TileType.Floor);
                }
            }
        }
    }

    void SetTileValuesForWalls()
    {
        for(int x = 0; x < gParams.columns; x++)
        {
            for(int y = 0; y < gParams.rows; y++)
            {
                TileType type = tiles[x][y];
                TileType[] neighbours = GetNeighbours(x, y);
                bool isSurroundingFloor = Array.IndexOf(neighbours, TileType.Floor) >= 0;
                bool isEdge = x == 0 || y == 0 || x == gParams.columns - 1 || y == gParams.rows - 1;
                if ((type == TileType.OutsideMap || isEdge) && isSurroundingFloor)
                {
                    WriteTile(x, y, TileType.Wall, overrideCurrent: true);
                }
            }
        }
    }

    void RemoveLooseWalls()
    {
        for(int x = 0; x < gParams.columns; x++)
        {
            for(int y = 0; y < gParams.rows; y++)
            {
                TileType type = tiles[x][y];
                TileType[] neighbours = GetNeighbours(x, y);
                if(type == TileType.Wall && Array.IndexOf(neighbours, TileType.OutsideMap) < 0)
                {
                    WriteTile(x, y, TileType.Floor, overrideCurrent: true);
                }
            }
        }
    }

    TileType GetNeighbour(int x, int y, int direction)
    {
        switch (direction)
        {
            case North:
                return y == gParams.rows - 1 ? TileType.OutsideMap : tiles[x][y + 1];

            case South:
                return y == 0 ? TileType.OutsideMap : tiles[x][y - 1];

            case East:
                return x == gParams.columns - 1 ? TileType.OutsideMap : tiles[x + 1][y];

            case West:
                return x == 0 ? TileType.OutsideMap : tiles[x - 1][y];

            case NorthWest:
                return x == 0 || y == gParams.rows - 1 ? TileType.OutsideMap : tiles[x - 1][y + 1];

            case NorthEast:
                return x == gParams.columns - 1 || y == gParams.rows - 1 ? TileType.OutsideMap : tiles[x + 1][y + 1];

            case SouthWest:
                return x == 0 || y == 0 ? TileType.OutsideMap : tiles[x - 1][y - 1];

            case SouthEast:
                return x == gParams.columns - 1 || y == 0 ? TileType.OutsideMap : tiles[x + 1][y - 1];

            default:
                return TileType.OutsideMap;
        }
    }

    TileType[] GetNeighbours(int x, int y)
    {
        TileType[] neighbours = new TileType[8];

        neighbours[West] = GetNeighbour(x, y, West);
        neighbours[East] = GetNeighbour(x, y, East);
        neighbours[North] = GetNeighbour(x, y, North);
        neighbours[South] = GetNeighbour(x, y, South);
        neighbours[NorthWest] = GetNeighbour(x, y, NorthWest);
        neighbours[NorthEast] = GetNeighbour(x, y, NorthEast);
        neighbours[SouthWest] = GetNeighbour(x, y, SouthWest);
        neighbours[SouthEast] = GetNeighbour(x, y, SouthEast);

        return neighbours;
    }

    int GetCount(TileType[] tiles, TileType type, TileCountDirections directions)
    {
        int start;
        int stop;

        switch(directions)
        {
            case TileCountDirections.Adjacent:
                start = North;
                stop = West;
                break;

            case TileCountDirections.Diagonal:
                start = NorthWest;
                stop = SouthEast;
                break;

            default:
            case TileCountDirections.Both:
                start = North;
                stop = SouthEast;
                break;

        }

        int count = 0;
        for(int i = start; i <= stop; i++)
        {
            if(tiles[i] == type)
            {
                count++;
            }
        }
        return count;
    }

    void InstantiateWall0(int i, int j, TileType[] neighbours)
    {
        // No wall neighbours so just instantiate the wall0
        InstantiateFromArray(tilePrefabs.Wall0, i, j, Quaternion.identity);
    }

    void InstantiateWall1(int i, int j, TileType[] neighbours)
    {
        // Need to find which direction the neighbouring wall tile is in
        Quaternion rotation;

        switch(Array.IndexOf(neighbours, TileType.Wall))
        {
            default:
            case North:
                rotation = Quaternion.Euler(0, 0, 180);
                break;

            case South:
                rotation = Quaternion.Euler(0, 0, 0);
                break;

            case East:
                rotation = Quaternion.Euler(0, 0, 90);
                break;

            case West:
                rotation = Quaternion.Euler(0, 0, -90);
                break;
        }

        InstantiateFromArray(tilePrefabs.Wall1, i, j, rotation);
    }

    void InstantiateWall2(int i, int j, TileType[] neighbours)
    {
        Quaternion rotation;
        GameObject[] tiles;

        if(neighbours[North] == TileType.Wall && neighbours[South] == TileType.Wall)
        {
            rotation = Quaternion.Euler(0, 0, 0);
            tiles = tilePrefabs.Wall2;
        }
        else if(neighbours[East] == TileType.Wall && neighbours[West] == TileType.Wall)
        {
            rotation = Quaternion.Euler(0, 0, 90);
            tiles = tilePrefabs.Wall2;
        }
        else if(neighbours[North] == TileType.Wall && neighbours[East] == TileType.Wall)
        {
            rotation = Quaternion.Euler(0, 0, 180);
            tiles = neighbours[NorthEast] == TileType.Wall ?
                tilePrefabs.Wall2AdjacentNoCorner :
                tilePrefabs.Wall2Adjacent;
        }
        else if(neighbours[North] == TileType.Wall && neighbours[West] == TileType.Wall)
        {
            rotation = Quaternion.Euler(0, 0, -90);
            tiles = neighbours[NorthWest] == TileType.Wall ?
                tilePrefabs.Wall2AdjacentNoCorner:
                tilePrefabs.Wall2Adjacent;
        }
        else if(neighbours[South] == TileType.Wall && neighbours[East] == TileType.Wall)
        {
            rotation = Quaternion.Euler(0, 0, 90);
            tiles = neighbours[SouthEast] == TileType.Wall ?
                tilePrefabs.Wall2AdjacentNoCorner :
                tilePrefabs.Wall2Adjacent;
        }
        else if(neighbours[South] == TileType.Wall && neighbours[West] == TileType.Wall)
        {
            rotation = Quaternion.Euler(0, 0, 0);
            tiles = neighbours[SouthWest] == TileType.Wall ?
                tilePrefabs.Wall2AdjacentNoCorner:
                tilePrefabs.Wall2Adjacent;
        }
        else
        {
            // Invalid
            rotation = Quaternion.identity;
            tiles = tilePrefabs.Invalid;
        }

        InstantiateFromArray(tiles, i, j, rotation);
    }

    GameObject[] SelectWall3Sprite(TileType farLeft, TileType farRight)
    {
        if(farLeft == TileType.Wall && farRight == TileType.Wall)
        {
            return tilePrefabs.Wall3NoCornerSWSE;
        }
        else if(farLeft == TileType.Wall)
        {
            return tilePrefabs.Wall3NoCornerSW;
        }
        else if(farRight == TileType.Wall)
        {
            return tilePrefabs.Wall3NoCornerSE;
        }
        else
        {
            return tilePrefabs.Wall3;
        }
    }

    void InstantiateWall3(int i, int j, TileType[] neighbours)
    {
        Quaternion rotation;
        GameObject[] tiles;

        switch (Array.FindIndex(neighbours, n => n != TileType.Wall))
        {
            default:
            case North:
                tiles = SelectWall3Sprite(neighbours[SouthWest], neighbours[SouthEast]);
                rotation = Quaternion.Euler(0, 0, 0);
                break;

            case South:
                tiles = SelectWall3Sprite(neighbours[NorthEast], neighbours[NorthWest]);
                rotation = Quaternion.Euler(0, 0, 180);
                break;

            case East:
                tiles = SelectWall3Sprite(neighbours[NorthWest], neighbours[SouthWest]);
                rotation = Quaternion.Euler(0, 0, -90);
                break;

            case West:
                tiles = SelectWall3Sprite(neighbours[SouthEast], neighbours[NorthEast]);
                rotation = Quaternion.Euler(0, 0, 90);
                break;
        }

        InstantiateFromArray(tiles, i, j, rotation);
    }

    void InstantiateWall4(int i, int j, TileType[] neighbours)
    {
        InstantiateFromArray(tilePrefabs.Wall4, i, j, Quaternion.identity);
    }

    void InstantiateTiles()
    {
        // Go through all the tiles in the jagged array...
        for (int i = 0; i < tiles.Length; i++)
        {
            for (int j = 0; j < tiles[i].Length; j++)
            {
                switch(tiles[i][j])
                {
                    default:
                    case TileType.OutsideMap:
                        break;

                    case TileType.Floor:
                        // ... and instantiate a floor tile for it.
                        InstantiateFromArray(tilePrefabs.Floor, i, j, Quaternion.identity);
                        break;

                    case TileType.Outside:
                        break;

                    case TileType.Wall:
                        TileType[] neighbours = GetNeighbours(i, j);
                        switch(GetCount(neighbours, TileType.Wall, TileCountDirections.Adjacent))
                        {
                            case 0:
                                InstantiateWall0(i, j, neighbours);
                                break;

                            case 1:
                                InstantiateWall1(i, j, neighbours);
                                break;

                            case 2:
                                InstantiateWall2(i, j, neighbours);
                                break;

                            case 3:
                                InstantiateWall3(i, j, neighbours);
                                break;

                            case 4:
                                InstantiateWall4(i, j, neighbours);
                                break;
                        }
                        break;
                }
            }
        }
    }

    void InstantiateFromArray(GameObject[] prefabs, float xCoord, float yCoord, Quaternion rotation)
    {
        // Create a random index for the array.
        int randomIndex = UnityEngine.Random.Range(0, prefabs.Length);

        // The position to be instantiated at is based on the coordinates.
        Vector3 position = new Vector3(xCoord, yCoord, 0f);

        // Create an instance of the prefab from the random index of the array.
        GameObject tileInstance = Instantiate(prefabs[randomIndex], position, rotation) as GameObject;
        tileInstance.layer = LayerMask.NameToLayer("World");

        // Set the tile's parent to the board holder.
        tileInstance.transform.parent = boardHolder.transform;
    }
}