using System;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataToMarker : MonoBehaviour
{
    private double[,] MarkerPositions; //marker position matrix

    private List<GameObject> ExistingMarkers;
    private List<GameObject> Avatar;
    private Vector3 vector_1;
    private Vector3 vector_2;
    private Vector3 vector_3;
    [Range(-360, 360)]
    public int x;
    [Range(-360, 360)]
    public int z;
    [Range(1, 30)]
    public int speed = 10;
    //variables for the  main code
    private int frame;
    private bool Streaming;
    private string line;
    private string[] lineArr;
    private int length;
    private GameObject Root;
    public List<string> MarkerNames; //seperate list for checking marker names
    private int number_of_markers;
    private int number_of_frames;
    StreamReader sr; //initializes sr for reading
    public string readPath;
    private string writePath = @"C:\Ian\Thesis\TextFiles\TSV_Test.txt";// this is used for debuggin

    public static float ToFloat(double value)
    {
        return (float)value;
    }

    // Calculates rotation matrix given euler angles.    

    void Start()
    {
        // initializing void Start variables
        File.WriteAllText(writePath, String.Empty);
        sr = new StreamReader(readPath);
        length = File.ReadAllLines(readPath).Length;
        MarkerNames = new List<string>();
        //initializing the void update variables
        Avatar = new List<GameObject>();
        Streaming = false;
        Root = gameObject;
        ExistingMarkers = new List<GameObject>();
        //program begins
        // this program creates a matrix of size markers*3 by amount of frames 
        for (int i = 0; i < 10; i++) //runs through the first 10 lines of the file which are not needed. Ends on the line that lists marker names
        {
            line = sr.ReadLine(); // reads line file
            if (i == 0) //this section reads the amount of frames
            {
                lineArr = line.Split('\t'); //splits the position values by tabs
                number_of_frames = int.Parse(lineArr[1], CultureInfo.InvariantCulture.NumberFormat);
            }
            if (i==2)// this section reads the amount of markers
            {
                lineArr = line.Split('\t'); //splits the position values by tabs
                number_of_markers = int.Parse(lineArr[1], CultureInfo.InvariantCulture.NumberFormat);
            }
        }

        //initialize MarkerPositions Matrix
        MarkerPositions = new double[number_of_markers * 3, number_of_frames]; //markers are multiplied by 3 because they each have an x,y and z coordinate

        lineArr = line.Split('\t'); //splits the position values by tabs
        for (int i = 1; i <= number_of_markers; i++)
        {
            MarkerNames.Add(lineArr[i]);
        }

        for (int j = 0; j < number_of_frames; j++)
        {
            line = sr.ReadLine();
            lineArr = line.Split('\t');
            for (int i = 0; i < number_of_markers*3; i++)
            {
                MarkerPositions[i, j] = double.Parse(lineArr[i], CultureInfo.InvariantCulture.NumberFormat); //marker postions goes in order z,x,y
            }
        }
    }

    // Update is called once per frame
    // this section is for animating the avatar
    void Update()
    {
        if (frame >= number_of_frames - 1)
        {
            frame = 0;
        }
        if (ExistingMarkers.Count == 0)
        {
            Streaming = true;
        }

        if (Streaming == true)
        {
            foreach (var NewMarker in ExistingMarkers)
            {
                Destroy(NewMarker);
            }

            foreach (var part in Avatar)
            {
                Destroy(part);
            }

            ExistingMarkers.Clear();
            for (int i = 0; i < number_of_markers; i++)
            {
                //this section creates the initial markers and will delete then recreate them after all the frames have been run through
                GameObject NewMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                NewMarker.name = MarkerNames[i];

                NewMarker.transform.parent = Root.transform;
                NewMarker.GetComponent<Renderer>().material.color = UnityEngine.Random.ColorHSV();
                NewMarker.transform.localScale = Vector3.one * 0.05f;
                //ExistingMarkers[i].transform.position = new Vector3(ToFloat((MarkerPositions[0 + 3 * i, 0])), ToFloat((MarkerPositions[1 + 3 * i, 0])), ToFloat((MarkerPositions[2 + 3 * i, 0])));
                NewMarker.transform.localPosition = new Vector3(ToFloat((MarkerPositions[1 + 3 * i, frame]) / 1000), ToFloat((MarkerPositions[2 + 3 * i, frame]) / 1000), ToFloat((MarkerPositions[0 + 3 * i, frame]) / 1000));
                NewMarker.SetActive(false);
                ExistingMarkers.Add(NewMarker);
            }
            //this next section creates the body parts on the avatar, this will need to be reworked for all marker sets later
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
                //part.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                part.SetActive(false);
                Avatar.Add(part);
            }

            Streaming = false;
        }

        for (int i = 0; i < number_of_markers; i++)
        {
            ExistingMarkers[i].transform.localPosition = new Vector3(ToFloat((MarkerPositions[1 + 3 * i, frame]) / 1000), ToFloat((MarkerPositions[2 + 3 * i, frame]) / 1000), ToFloat(-1*(MarkerPositions[0 + 3 * i, frame]) / 1000));
            ExistingMarkers[i].SetActive(true);
            ExistingMarkers[i].GetComponent<Renderer>().enabled = true;
        }

            vector_1.x = (ExistingMarkers[0].transform.position.x + ExistingMarkers[2].transform.position.x) / 2.0F - ExistingMarkers[1].transform.position.x;
            vector_1.y = -1*((ExistingMarkers[0].transform.position.y + ExistingMarkers[2].transform.position.y) / 2.0F - ExistingMarkers[1].transform.position.y);
            vector_1.z = (ExistingMarkers[0].transform.position.z + ExistingMarkers[2].transform.position.z) / 2.0F - ExistingMarkers[1].transform.position.z;
            Avatar[0].transform.rotation = Quaternion.FromToRotation(vector_1, transform.up);
            Avatar[0].transform.position = (ExistingMarkers[1].transform.position + ExistingMarkers[8].transform.position + ExistingMarkers[0].transform.position + ExistingMarkers[2].transform.position) / 4.0F;
            Avatar[0].transform.localScale = new Vector3(0.1f, vector_1.magnitude/2.0f, 0.1f);
            Avatar[0].SetActive(true);
            Avatar[0].GetComponent<Renderer>().enabled = true;


            vector_2.x = (ExistingMarkers[8].transform.position.x + ExistingMarkers[1].transform.position.x) / 2.0F - ((ExistingMarkers[3].transform.position.x + ExistingMarkers[4].transform.position.x) / 2.0F);
            vector_2.y = -1*((ExistingMarkers[8].transform.position.y + ExistingMarkers[1].transform.position.y) / 2.0F - ((ExistingMarkers[3].transform.position.y + ExistingMarkers[4].transform.position.y) / 2.0F));
            vector_2.z = ((ExistingMarkers[8].transform.position.z + ExistingMarkers[1].transform.position.z) / 2.0F - ((ExistingMarkers[3].transform.position.z + ExistingMarkers[4].transform.position.z) / 2.0F));
            Avatar[1].transform.rotation = Quaternion.FromToRotation(vector_2, transform.up );
            Avatar[1].transform.position = (ExistingMarkers[8].transform.position + ExistingMarkers[1].transform.position + ExistingMarkers[3].transform.position + ExistingMarkers[4].transform.position) / 4.0F;
            Avatar[1].transform.localScale = new Vector3(0.1f, vector_2.magnitude / 2.0f, 0.1f);
            Avatar[1].SetActive(true);
            Avatar[1].GetComponent<Renderer>().enabled = true;

            vector_3.x = ((ExistingMarkers[3].transform.position.x + ExistingMarkers[4].transform.position.x) / 2.0F) - ((ExistingMarkers[7].transform.position.x + ExistingMarkers[6].transform.position.x) / 2.0F);
            vector_3.y = -1*(((ExistingMarkers[3].transform.position.y + ExistingMarkers[4].transform.position.y) / 2.0F) - ((ExistingMarkers[7].transform.position.y + ExistingMarkers[6].transform.position.y) / 2.0F));
            vector_3.z = (((ExistingMarkers[3].transform.position.z + ExistingMarkers[4].transform.position.z) / 2.0F) - ((ExistingMarkers[7].transform.position.z + ExistingMarkers[6].transform.position.z) / 2.0F));
            Avatar[2].transform.rotation = Quaternion.FromToRotation(vector_3, transform.up);
            Avatar[2].transform.position = (ExistingMarkers[3].transform.position + ExistingMarkers[4].transform.position + ExistingMarkers[6].transform.position + ExistingMarkers[7].transform.position) / 4.0F;
            Avatar[2].transform.localScale = new Vector3(0.1f, vector_3.magnitude / 2.0f, 0.1f);
            Avatar[2].SetActive(true);
            Avatar[2].GetComponent<Renderer>().enabled = true;

        frame = frame + speed;
    }
      
    
}

