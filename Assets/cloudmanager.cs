using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloudmanager : MonoBehaviour
{

    public int samples = 16;
    public int height = 50;
    public GameObject cloudPrefab;
    public AnimationCurve cloudCurve;

    private List<GameObject> cloudList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        if (cloudList.Count <= samples * 2)
        {
            for (int i = -samples; i < samples; i++)
            {
                GameObject gc = Instantiate(cloudPrefab, transform.position + new Vector3(0, Mathf.Lerp(-height, height, (float)i / samples), 0), transform.rotation, transform);

                cloudList.Add(gc);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        int j = -samples;
        foreach (var item in cloudList)
        {
            item.GetComponent<MeshRenderer>().material.SetFloat("_MainHeight", cloudCurve.Evaluate(Mathf.Abs(Mathf.Lerp(-height, height, (float)j / samples)) / height));
            j++;
        }
    }


}
