using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QualysisToAvatar : MonoBehaviour {

    public List<QualisysRealTime.Unity.LabeledMarker> Mdata;
    public Text MdataText;
    private List<GameObject> Minfo;
    private QualisysRealTime.Unity.RTClient rtClient;
    private GameObject mark;
    public InputField MotionNumberInput; //variable for motion number
    public Text InputText;
    private int MotionNumber;

    public bool visibleMarkers = true;

    [Range(0.001f, 1f)]
    public float markerScale = 0.05f;

    //private bool streaming = false;

    // Use this for initialization
    void Start()
    {
        MotionNumber = 0;
        MdataText.text = Mdata.ToString();
        InputText.text = MotionNumberInput.ToString() + MotionNumber.ToString();
        rtClient = QualisysRealTime.Unity.RTClient.GetInstance();
        mark = gameObject;
        MotionNumberInput.text = "Enter Text Here...";
    }

    private void InitiateMarkers()
    {
        foreach (var marker in Minfo)
        {
            Destroy(marker);
        }

        Minfo.Clear();
        Mdata = rtClient.Markers;

        for (int i = 0; i < Mdata.Count; i++)
        {
            GameObject newMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            newMarker.name = Mdata[i].Label;
            newMarker.transform.parent = mark.transform;
            newMarker.transform.localScale = Vector3.one * markerScale;
            newMarker.SetActive(false);
            Minfo.Add(newMarker);
        }
    }
    // Update is called once per frame
    void Update()
    {
        InitiateMarkers();
        Mdata = rtClient.Markers;
        //MotionNumber = Int32.Parse(MotionNumberInput,1);
        if (MotionNumber == 1)
        {
            for (int i = 0; i < Mdata.Count; i++)
            {
                if (Mdata[i].Position.magnitude > 0)
                {
                    Minfo[i].name = Mdata[i].Label;
                    Minfo[i].GetComponent<Renderer>().material.color = Mdata[i].Color;
                    Minfo[i].transform.localPosition = Mdata[i].Position;
                    Minfo[i].SetActive(true);
                    Minfo[i].GetComponent<Renderer>().enabled = visibleMarkers;
                    Minfo[i].transform.localScale = Vector3.one * markerScale;
                }
                else
                {
                    //hide markers if we cant find them.
                    Minfo[i].SetActive(false);
                }
            }
        }
    }
}
