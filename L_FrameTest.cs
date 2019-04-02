using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace QualisysRealTime.Unity
{
    public class L_FrameTest : MonoBehaviour
    {
        private List<LabeledMarker> markerData;
        private RTClient rtClient;
        private GameObject markerRoot;
        private List<GameObject> markers;
        private List<GameObject> V_Lframe;
        private List<GameObject> Test;
        public bool visibleMarkers = true;
        [Range(0.001f, 1f)]
        public float markerScale = 0.05f;
        
        private bool streaming = false;    
        private Matrix4x4 V_LG;
        private Matrix4x4 R_LG;
        private Matrix4x4 VR_LL;
        private Vector3 a;
        private Vector3 b;
        private Vector3 c;
        private Vector3 o;

        private Vector4 a4;
        private Vector4 af4;

        private Vector3 af;
        private Vector3 bf;
        private Vector3 cf;
        private Vector3 of;

        private bool RUN;
        private Vector3 A;
        private Vector3 B;
        private Vector3 C;
        private Vector3 O;

        private int frame;

        private string writePath = @"C:\Ian\Thesis\TextFiles\Debugging.txt";// this is used for debugging


        // Use this for initialization
        void Start()
        {
            File.WriteAllText(writePath, String.Empty);
            V_LG = new Matrix4x4();
            V_LG = Virtual_LFrame.VLF_LG;
            RUN = true;
            rtClient = RTClient.GetInstance();
            V_Lframe = new List<GameObject>();
            Test = new List<GameObject>();
            markers = new List<GameObject>();
            markerRoot = gameObject;
            frame = 0;

            InitiateCO();

        }


        private void InitiateMarkers()
        {
            foreach (var marker in markers)
            {
                Destroy(marker);
            }


            markers.Clear();
            markerData = rtClient.Markers;

            for (int i = 0; i < markerData.Count; i++)
            {
                GameObject newMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                newMarker.name = markerData[i].Label;
                newMarker.transform.parent = markerRoot.transform;
                newMarker.transform.localScale = Vector3.one * markerScale;
                newMarker.SetActive(false);
                markers.Add(newMarker);
            }
        }

        private void InitiateCO()
        {
            foreach (var m in V_Lframe )
            {
                Destroy(m);
            }


            V_Lframe.Clear();

            GameObject AQ = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            AQ.name = "AQ";
            AQ.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            GameObject BQ = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            BQ.name = "BQ";
            BQ.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            GameObject CQ = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            CQ.name = "CQ";
            CQ.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            GameObject OQ = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            OQ.name = "OQ";
            OQ.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
            AQ.SetActive(false);
            BQ.SetActive(false);
            CQ.SetActive(false);
            OQ.SetActive(false);
            V_Lframe.Add(AQ);
            V_Lframe.Add(BQ);
            V_Lframe.Add(CQ);
            V_Lframe.Add(OQ);
        }

        // Update is called once per frame
        void Update()
        {
            frame = rtClient.GetFrame();
            if (rtClient == null) rtClient = RTClient.GetInstance();
            if (rtClient.GetStreamingStatus() && !streaming)
            {
                InitiateMarkers();
                //InitiateCO();
                streaming = true;
            }
            if (!rtClient.GetStreamingStatus() && streaming)
            {
                streaming = false;
                InitiateMarkers();
                //InitiateCO();
            }

            markerData = rtClient.Markers;

            if (markerData == null && markerData.Count == 0)
                return;

            if (markers.Count != markerData.Count)
            {
                InitiateMarkers();
                //InitiateCO();
            }
            for (int i = 0; i < markerData.Count; i++)
            {
                if (markerData[i].Label == "a0")
                {
                    O = markerData[i].Position;
                    //markers[i].transform.position = VR_LL * O;
                }
                if (markerData[i].Label == "a1")
                {
                    A = markerData[i].Position;
                }
                if (markerData[i].Label == "b1")
                {
                    B = markerData[i].Position;
                }
                if (markerData[i].Label == "c1")
                {
                    C = markerData[i].Position;
                }
            }
            if (frame == 100)
            {

                if (RUN == true)
                {
                    a = Vector3.Cross(A - O, B - O);
                    a = a.normalized;
                    b = Vector3.Cross(A - O, a);
                    b = b.normalized;
                    c = Vector3.Cross(a, b);
                    c = c.normalized;
                    o = O;

                    R_LG.SetColumn(0, new Vector4(a.x, a.y, a.z, 0));
                    R_LG.SetColumn(1, new Vector4(b.x, b.y, b.z, 0));
                    R_LG.SetColumn(2, new Vector4(c.x, c.y, c.z, 0));
                    R_LG.SetColumn(3, new Vector4(O.x, O.y, O.z, 0));
                    //V_LG = V_LG.transpose;
                    //VR_LL = R_LG*V_LG.transpose;
                    VR_LL = V_LG * R_LG.transpose;

                    a4 = new Vector4(a.x, a.y, a.z, 0);
                    af4 = VR_LL * a4;
                    af = VR_LL * a;
                    bf = VR_LL * b;
                    cf = VR_LL * c;
                    of = VR_LL * o;


                    V_Lframe[0].transform.position = af;
                    V_Lframe[1].transform.position = bf;
                    V_Lframe[2].transform.position = cf;
                    V_Lframe[3].transform.position = of;
                    V_Lframe[0].SetActive(true);
                    V_Lframe[1].SetActive(true);
                    V_Lframe[2].SetActive(true);
                    V_Lframe[3].SetActive(true);
                    V_Lframe[0].GetComponent<Renderer>().enabled = visibleMarkers;
                    V_Lframe[1].GetComponent<Renderer>().enabled = visibleMarkers;
                    V_Lframe[2].GetComponent<Renderer>().enabled = visibleMarkers;
                    V_Lframe[3].GetComponent<Renderer>().enabled = visibleMarkers;
                    V_Lframe[0].GetComponent<Renderer>().material.color = new Vector4(0, 1, 0, 1);
                    V_Lframe[1].GetComponent<Renderer>().material.color = new Vector4(0, 0, 1, 1);
                    V_Lframe[2].GetComponent<Renderer>().material.color = new Vector4(1, 0, 0, 1);
                    V_Lframe[3].GetComponent<Renderer>().material.color = new Vector4(0, 0, 0, 1);

                    RUN = false;
                }
            }

            for (int i = 0; i < markerData.Count; i++)
            {

                
                if (markerData[i].Position.magnitude > 0)
                {
                    markers[i].name = markerData[i].Label;
                    markers[i].GetComponent<Renderer>().material.color = markerData[i].Color;
                    markers[i].transform.localPosition = VR_LL * markerData[i].Position; //+ (of - new Vector3(V_LG.m30, V_LG.m31, V_LG.m32));
                    markers[i].transform.localPosition = markers[i].transform.localPosition - new Vector3(of.x - V_LG[0,3], of.y - V_LG[1,3], of.z - V_LG[2,3]);
                    markers[i].SetActive(true);
                    markers[i].GetComponent<Renderer>().enabled = visibleMarkers;
                    markers[i].transform.localScale = Vector3.one * markerScale;
                }
                else
                {
                    //hide markers if we cant find them.
                    markers[i].SetActive(false);
                }
            }
            //using (StreamWriter sw = new StreamWriter(writePath))
            //{
            //    sw.WriteLine(of.ToString());
            //    sw.WriteLine(V_LG.GetColumn(3).ToString());
            //    sw.WriteLine(V_LG.m30.ToString());
            //    sw.WriteLine(V_LG.m31.ToString());
            //    sw.WriteLine(V_LG.m32.ToString());

                
            //}
        }     
    }
}

