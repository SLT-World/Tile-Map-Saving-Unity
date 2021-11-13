using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Template _CurrentTemplate;

    [SerializeField] List<GameObject> RoadTiles;
    [SerializeField] GameObject ButtonPrefab;

    [SerializeField] Transform CurrentBuilding;

    [SerializeField] Material BlueprintMaterial;

    [SerializeField] List<RendererColors> RendererColors;

    [SerializeField] Transform BuildingList;
    [SerializeField] Transform AvailableTemplateList;

    [SerializeField] InputField NameInputField;

    float MouseWheelRotation;
    int CurrentIndex;

    public List<Template> AvailableTemplates;

    private void Start()
    {
        AvailableTemplates = JsonConvert.DeserializeObject<List<Template>>(new WebClient().DownloadString("https://multiplayer-servers-database.sltworlddev.repl.co/templates/"));
        foreach (GameObject RoadTile in RoadTiles)
        {
            var IButton = Instantiate(ButtonPrefab, BuildingList);
            IButton.GetComponentInChildren<Text>().text = RoadTile.name;
            IButton.GetComponent<Button>().onClick.AddListener(delegate { Build(RoadTiles.IndexOf(RoadTile)); });
        }
        foreach (Template _Template in AvailableTemplates)
        {
            var IButton = Instantiate(ButtonPrefab, AvailableTemplateList);
            IButton.GetComponentInChildren<Text>().text = _Template.Name;
            IButton.GetComponent<Button>().onClick.AddListener(delegate { Load(_Template); });
        }
    }

    public void Load(Template _Template)
    {
        FindObjectsOfType<Grid>().ToList().ForEach(item => Destroy(item.gameObject));
        _CurrentTemplate = _Template;
        for (int i = 0; i < _CurrentTemplate.Tiles.Count; i++)
            Instantiate(RoadTiles[_CurrentTemplate.Tiles[i]], _CurrentTemplate.Positions[i], Quaternion.Euler(_CurrentTemplate.Rotations[i]));
    }

    private void Update()
    {
        if (CurrentBuilding)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit _Hit, Mathf.Infinity))
            {
                CurrentBuilding.position = new Vector3(Mathf.Round(_Hit.point.x), Mathf.Round(_Hit.point.y), Mathf.Round(_Hit.point.z));
                MouseWheelRotation = Input.mouseScrollDelta.y;
                CurrentBuilding.Rotate(Vector3.up, MouseWheelRotation * 90f);
                if (Input.GetMouseButtonDown(0))
                {
                    _CurrentTemplate.Tiles.Add(CurrentIndex);
                    _CurrentTemplate.Positions.Add(CurrentBuilding.position);
                    _CurrentTemplate.Rotations.Add(CurrentBuilding.rotation.eulerAngles);
                    GetComponentsInChildren<MeshRenderer>(true).ToList().ForEach(item => item.enabled = true);
                    foreach (RendererColors _RendererColors in RendererColors)
                        _RendererColors._Renderer.materials = _RendererColors.Materials;
                    RendererColors.Clear();
                    CurrentBuilding = null;
                    CurrentIndex = -1;
                }
            }
        }
    }

    public void Build(int Index)
    {
        if (CurrentBuilding)
            return;
        CurrentIndex = Index;
        CurrentBuilding = Instantiate(RoadTiles[Index]).transform;
        GetComponentsInChildren<MeshRenderer>(true).ToList().ForEach(item => item.enabled = false);
        foreach (Renderer _Renderer in CurrentBuilding.GetComponentsInChildren<Renderer>())
        {
            RendererColors.Add(new RendererColors { Materials = _Renderer.materials, _Renderer = _Renderer });

            List<Material> _Materials = _Renderer.materials.ToList();
            var ChangedMaterials = new Material[_Materials.Count];
            for (var i = 0; i < _Materials.Count; i++)
                ChangedMaterials[i] = BlueprintMaterial;
            _Renderer.materials = ChangedMaterials;
        }
    }

    public void SaveTemplate()
    {
        _CurrentTemplate.Name = NameInputField.text;
        StartCoroutine(IEnumerator_SaveTemplate());
    }

    IEnumerator IEnumerator_SaveTemplate()
    {
        string JsonData = JsonUtility.ToJson(_CurrentTemplate);
        string Uri = "https://multiplayer-servers-database.sltworlddev.repl.co/templates/create/";
        using (UnityWebRequest Request = UnityWebRequest.Put(Uri, JsonData.ToLower()))
        {
            Request.method = UnityWebRequest.kHttpVerbPOST;
            Request.SetRequestHeader("Content-Type", "application/json");
            Request.SetRequestHeader("Accept", "application/json");

            yield return Request.SendWebRequest();

            if (Request.isNetworkError)
            {
                Debug.Log(Request.error);
            }
            else
            {
                Debug.Log(Request.downloadHandler.text);
            }
        }

        AvailableTemplates = JsonConvert.DeserializeObject<List<Template>>(new WebClient().DownloadString("https://multiplayer-servers-database.sltworlddev.repl.co/templates/"));
        for (int i = 0; i < AvailableTemplateList.childCount; i++)
        {
            Destroy(AvailableTemplateList.GetChild(i).gameObject);
        }
        foreach (Template _Template in AvailableTemplates)
        {
            var IButton = Instantiate(ButtonPrefab, AvailableTemplateList);
            IButton.GetComponentInChildren<Text>().text = _Template.Name;
            IButton.GetComponent<Button>().onClick.AddListener(delegate { Load(_Template); });
        }
    }
}

[System.Serializable]
class RendererColors
{
    public Renderer _Renderer;
    public Material[] Materials;
}
