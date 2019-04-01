using System;
using UnityEngine;

[Serializable]
struct TilePrefabs
{
    public GameObject[] Floor;
    public GameObject[] Invalid;
    public GameObject[] Outside;
    public GameObject[] Wall0;
    public GameObject[] Wall1;
    public GameObject[] Wall2;
    public GameObject[] Wall2Adjacent;
    public GameObject[] Wall2AdjacentNoCorner;
    public GameObject[] Wall3;
    public GameObject[] Wall3NoCornerSW;
    public GameObject[] Wall3NoCornerSE;
    public GameObject[] Wall3NoCornerSWSE;
    public GameObject[] Wall4;
    public GameObject[] Wall4NoCornerSE;
    public GameObject[] Wall4NoCornerSWSE;
    public GameObject[] Wall4NoCornerNWSE;
}
