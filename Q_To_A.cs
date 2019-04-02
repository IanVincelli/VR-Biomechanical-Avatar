using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace QualisysRealTime.Unity
{
    public class Q_To_A : MonoBehaviour
    {
        private List<LabeledMarker> markerData;
        private RTClient rtClient;
        private GameObject markerRoot;
        private List<GameObject> markers;
        private Matrix4x4 Headset_TF;
        private Vector3 Headset_Position;
        private string headset_name;
        public bool visibleMarkers = true;
        private Camera[] test;
        [Range(0.001f, 1f)]
        public float markerScale = 0.05f;

        private bool streaming = false;
        private string writePath = @"C:\Ian\Thesis\TextFiles\Debuggingg.txt";// this is used for debugging
        //variables for avatar
        private Vector3 vector_1;
        private Vector3 vector_2;
        private Vector3 vector_3;
        private List<GameObject> Avatar;

        // Use this for initialization
        void Start()
        {
            rtClient = RTClient.GetInstance();
            markers = new List<GameObject>();
            markerRoot = gameObject;
            Avatar = new List<GameObject>();
            Headset_Position = new Vector3();
            test = GameObject.FindObjectsOfType<Camera>();
            Headset_Position = test[0].transform.position;
            headset_name = SteamVR_Camera.FindObjectOfType<GameObject>().name;


            //using (StreamWriter sw = new StreamWriter(writePath))
            //{
            //    sw.WriteLine(test.Length.ToString());
            //    sw.WriteLine(test[0].transform.position);

            //}

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
        private void InitiateAvatar()
        {
            foreach (var part in Avatar)
            {
                Destroy(part);
            }

            for (int i = 0; i < 3; i++)
            {
                GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                if (i == 0)
                {
                    part.name = "midline to mid shoulder";
                    part.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }
                if (i == 1)
                {
                    part.name = "shoulder to elbow";
                    part.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }
                if (i == 2)
                {
                    part.name = "elbow to wrist";
                    part.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                }
                //initializing rendering of a partially visible cylinder
                part.GetComponent<MeshRenderer>().material.shader = Shader.Find("Transparent/Diffuse");
                part.GetComponent<MeshRenderer>().material.SetFloat("_Mode", 3f);
                part.GetComponent<MeshRenderer>().material.color = new Vector4(1f, 1f, 1f, 0.5f);
                part.SetActive(false);
                Avatar.Add(part);
            }

        }

        // Update is called once per frame
        void Update()
        {
            //if (GameObject.FindGameObjectWithTag("Camera (eye)").transform.position.x <= 0 || GameObject.FindGameObjectWithTag("Camera (eye)").transform.position.x >= 0)
            //{
            Headset_Position = GameObject.FindObjectOfType<Camera>().transform.position;
            //}

            if (rtClient == null) rtClient = RTClient.GetInstance();
            if (rtClient.GetStreamingStatus() && !streaming)
            {
                InitiateMarkers();
                InitiateAvatar();
                streaming = true;
            }
            if (!rtClient.GetStreamingStatus() && streaming)
            {
                streaming = false;
                InitiateMarkers();
                InitiateAvatar();
            }

            markerData = rtClient.Markers;

            if (markerData == null && markerData.Count == 0)
                return;

            if (markers.Count != markerData.Count)
            {
                InitiateMarkers();
                InitiateAvatar();
            }

            for (int i = 0; i < markerData.Count; i++)
            {
                if (markerData[i].Position.magnitude > 0)
                {
                    markers[i].name = markerData[i].Label;
                    markers[i].GetComponent<Renderer>().material.color = markerData[i].Color;
                    //markers[i].transform.position = markerData[i].Position;
                    markers[i].transform.position = Virtual_LFrame.TF_RV * markerData[i].Position;
                    markers[i].transform.position = markers[i].transform.position - new Vector3(Virtual_LFrame.o_r.x - Virtual_LFrame.VLF_LG[0,3], Virtual_LFrame.o_r.y - Virtual_LFrame.VLF_LG[1, 3], Virtual_LFrame.o_r.z - Virtual_LFrame.VLF_LG[2, 3]);
                    markers[i].transform.position = markers[i].transform.position + new Vector3(Headset_Position.x - Virtual_LFrame.VLF_LG[0, 3], Headset_Position.y - Virtual_LFrame.VLF_LG[1, 3], Headset_Position.z - Virtual_LFrame.VLF_LG[2, 3]);
                    markers[i].SetActive(true);
                    markers[i].GetComponent<Renderer>().enabled = visibleMarkers;
                    markers[i].transform.localScale = Vector3.one * markerScale;
                }
                else
                {
                    //hide markers if we cant find them.
                    markers[i].SetActive(false);
                }

                vector_1.x = (markers[0].transform.position.x + markers[2].transform.position.x) / 2.0F - markers[1].transform.position.x;
                vector_1.y = -1 * ((markers[0].transform.position.y + markers[2].transform.position.y) / 2.0F - markers[1].transform.position.y);
                vector_1.z = (markers[0].transform.position.z + markers[2].transform.position.z) / 2.0F - markers[1].transform.position.z;
                Avatar[0].transform.rotation = Quaternion.FromToRotation(vector_1, transform.up);
                Avatar[0].transform.position = (markers[1].transform.position + markers[8].transform.position + markers[0].transform.position + markers[2].transform.position) / 4.0F;
                Avatar[0].transform.localScale = new Vector3(0.1f, vector_1.magnitude / 2.0f, 0.1f);
                Avatar[0].SetActive(true);
                Avatar[0].GetComponent<Renderer>().enabled = true;


                vector_2.x = (markers[8].transform.position.x + markers[1].transform.position.x) / 2.0F - ((markers[3].transform.position.x + markers[4].transform.position.x) / 2.0F);
                vector_2.y = -1 * ((markers[8].transform.position.y + markers[1].transform.position.y) / 2.0F - ((markers[3].transform.position.y + markers[4].transform.position.y) / 2.0F));
                vector_2.z = ((markers[8].transform.position.z + markers[1].transform.position.z) / 2.0F - ((markers[3].transform.position.z + markers[4].transform.position.z) / 2.0F));
                Avatar[1].transform.rotation = Quaternion.FromToRotation(vector_2, transform.up);
                Avatar[1].transform.position = (markers[8].transform.position + markers[1].transform.position + markers[3].transform.position + markers[4].transform.position) / 4.0F;
                Avatar[1].transform.localScale = new Vector3(0.1f, vector_2.magnitude / 2.0f, 0.1f);
                Avatar[1].SetActive(true);
                Avatar[1].GetComponent<Renderer>().enabled = true;

                vector_3.x = ((markers[3].transform.position.x + markers[4].transform.position.x) / 2.0F) - ((markers[7].transform.position.x + markers[6].transform.position.x) / 2.0F);
                vector_3.y = -1 * (((markers[3].transform.position.y + markers[4].transform.position.y) / 2.0F) - ((markers[7].transform.position.y + markers[6].transform.position.y) / 2.0F));
                vector_3.z = (((markers[3].transform.position.z + markers[4].transform.position.z) / 2.0F) - ((markers[7].transform.position.z + markers[6].transform.position.z) / 2.0F));
                Avatar[2].transform.rotation = Quaternion.FromToRotation(vector_3, transform.up);
                Avatar[2].transform.position = (markers[3].transform.position + markers[4].transform.position + markers[6].transform.position + markers[7].transform.position) / 4.0F;
                Avatar[2].transform.localScale = new Vector3(0.1f, vector_3.magnitude / 2.0f, 0.1f);
                Avatar[2].SetActive(true);
                Avatar[2].GetComponent<Renderer>().enabled = true;
            }
            //Headset_Position[0] = GameObject.Find("C7");
            //Headset_Position[1] = GameObject.Find("SN");
            //Headset_Position[2] = GameObject.Find("r_acromion");

        }
    }
}