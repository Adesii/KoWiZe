using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AORTechTreeMenuManager : MonoBehaviour
{

    [SerializeField]
    private GameObject TechTreeViewer;
    [SerializeField]
    private GameObject TechTreeSrollView;

    [SerializeField]
    private GameObject ColumnPrefab;
    [SerializeField]
    private GameObject ItemPrefab;


    private List<GameObject> techColumns = new List<GameObject>();
    public Dictionary<string, AORTechTreeItem> nodeLinks = new Dictionary<string, AORTechTreeItem>();
    private List<TechLayer> localLayers;

    public bool initialized = false;
    public void OpenTechTree()
    {
        TechTreeViewer.SetActive(true);
        if (!initialized) init();

    }
    public void closeTechTree()
    {
        TechTreeViewer.GetComponent<simpleUIFader>().disableObject();
    }
    private void Update()
    {
    }

    public void startResearch(TechNode own)
    {
        nodeLinks.All((e) =>
        {
            if (!e.Value.ownNode.TechName.Equals(own.TechName)&& e.Value.currResearching)
                e.Value.currResearching = false;
            return true;
        });
    }

    public void init()
    {
        localLayers = TechTreeManager.Instance.saveToChangeTechs[0].techLayers;
        foreach (var item in localLayers)
        {
            var go = Instantiate(ColumnPrefab, TechTreeSrollView.transform);
            techColumns.Add(go);
            foreach (var nodes in item.techNodes)
            {
                var gg = Instantiate(ItemPrefab, go.transform);
                var n = gg.GetComponent<AORTechTreeItem>();
                var line = gg.GetComponent<LineGraphic>();
                line.corners = new List<LineGraphic.dependedGraph>();
                line.color = n.inactive;
                n.ownManager = this;
                foreach (var depends in nodes.dependsIDs)
                {
                    nodeLinks.TryGetValue(depends.techName, out AORTechTreeItem val);
                    line.corners.Add(new LineGraphic.dependedGraph
                    {
                        root = (RectTransform)gg.transform,
                        selfInput = n.INput,
                        dependency = val.OUTput
                    });
                }
                n.ownNode = nodes;
                if (nodes.isAvailable)
                {
                    n.BlackOut.SetActive(false);
                    if (nodes.isUnlocked)
                    {
                        n.LightOut.SetActive(true);
                    }
                }
                n.newNode();
                nodeLinks.Add(nodes.TechName, n);
            }
        }
        initialized = true;
    }
}
