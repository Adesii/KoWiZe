using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class cloudmanager : MonoBehaviour
{

    public int samples = 16;
    public int height = 50;
    public GameObject cloudPrefab;
    public AnimationCurve cloudCurve;
    public ParentConstraint parentConstraint;
    public ParentConstraint parentWaterConstraint;
    public ParentConstraint parentPlaneConstraint;

    private List<GameObject> cloudList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void init(Transform c)
    {
        parentConstraint.SetSource(0,
            new ConstraintSource()
            {
                sourceTransform = c,
                weight = 1
            });
        parentWaterConstraint.SetSource(0, new ConstraintSource() { sourceTransform = c, weight = 1 });
        parentPlaneConstraint.SetSource(0, new ConstraintSource() { sourceTransform = c, weight = 1 });
        if (cloudList.Count <= samples * 2)
        {
            for (int i = -samples; i < samples; i++)
            {
                GameObject gc = Instantiate(cloudPrefab, transform.position + new Vector3(0, Mathf.Lerp(-height, height, (float)i / samples), 0), transform.rotation, transform);

                cloudList.Add(gc);
            }
        }
        int j = -samples;
        foreach (var item in cloudList)
        {
            item.GetComponent<MeshRenderer>().material.SetFloat("_MainHeight", cloudCurve.Evaluate(Mathf.Abs(Mathf.Lerp(-height, height, (float)j / samples)) / height));
            j++;
        }
    }

    // Update is called once per frame
    void Update()
    {


    }


}
