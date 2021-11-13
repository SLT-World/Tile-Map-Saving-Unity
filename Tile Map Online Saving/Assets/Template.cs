using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[Serializable]
public class Template
{
    [JsonProperty("name")] public string Name;
    [JsonProperty("tiles")] public List<int> Tiles = new List<int>();
    [JsonProperty("positions")] public List<Vector3> Positions = new List<Vector3>();
    [JsonProperty("rotations")] public List<Vector3> Rotations = new List<Vector3>();
}
