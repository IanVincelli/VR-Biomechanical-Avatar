using System;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerScaling : MonoBehaviour {
    //this is the code for scaling the makrer data usng the frontal poitions of intial setup and subject marekr sets
    public string SetupPath; //setup markers path 
    public string SubjectPath; //subjects marker path
    private string writePath = @"C:\Ian\Thesis\TextFiles\Debugging.txt";// this is used for debugging
    //setup matrices for markers and the defference between their positions
    private double[] SubjectData;
    private double[] SetupData;
    private Vector3[] subject;
    private Vector3[] setup;
    public static Vector3[] ScalingData;
    //marker names fior both subject and setup data
    private List<string> SetupNames;
    private List<string> SubjectNames;

    private bool MarkerCount;

    private string line;
    private string[] lineArr;

    private int number_of_markers;
    private int number_of_frames;

    StreamReader sr; //initializes sr for reading

    //this is the setup for the sorting stuff
    private bool sorted;//if sorted = false then the data hasnt been sorted yet 
    private bool line_sorted; 
    private double compare;
    private double placeholder;
    private string nameholder;

    public static float ToFloat(double value)
    {
        return (float)value;
    }

    void Start() {
        File.WriteAllText(writePath, String.Empty);
        SetupNames = new List<string>();
        SubjectNames = new List<string>();



        // this section is for the reading of the data sets
        for (int n = 0; n < 2; n++) // runs through the program twice to read from both paths
        {
            MarkerCount = false;
            if (n == 0)//changes the readpath to the setup marker set
            {
                sr = new StreamReader(SetupPath);
            }
            if (n == 1) //changes the readpath to the subject marker set
            {
                sr = new StreamReader(SubjectPath);
            }
            using (StreamWriter sw = File.AppendText(writePath))
            {
                sw.WriteLine("line 54");
            }
            for (int i = 0; i < 10; i++) //runs through the first 10 lines of the file which are not needed. Ends on the line that lists marker names
            {
                line = sr.ReadLine(); // reads line file
                if (i == 0) //this section reads the amount of frames
                {
                    lineArr = line.Split('\t'); //splits the position values by tabs
                    number_of_frames = int.Parse(lineArr[1], CultureInfo.InvariantCulture.NumberFormat);
                }
                if (i == 2)// this section reads the amount of markers
                {
                    lineArr = line.Split('\t'); //splits the position values by tabs
                    number_of_markers = int.Parse(lineArr[1], CultureInfo.InvariantCulture.NumberFormat);
                }
            }
            //initializing the setup and subject data
            if (n == 0)
            {
                SetupData = new double[number_of_markers * 3];
            }
            if (n == 1)
            {
                SubjectData = new double[number_of_markers * 3];
            }

            lineArr = line.Split('\t'); //splits the position values by tabs
            for (int i = 1; i <= number_of_markers; i++)
            {

                if (n == 0)
                {
                    SetupNames.Add(lineArr[i]);
                }
                else
                {
                    SubjectNames.Add(lineArr[i]);
                }
            }
            using (StreamWriter sw = File.AppendText(writePath))
            {
                sw.WriteLine("line 88");
            }
            while (MarkerCount == false)//keeps reading lines till it gets a reeading with the correct number of markers
            {
                using (StreamWriter sw = File.AppendText(writePath))
                {
                    sw.WriteLine("markercount while loop");
                }
                line = sr.ReadLine();
                lineArr = line.Split('\t');
                if (lineArr.Length != number_of_markers * 3)
                {
                    line = sr.ReadLine();
                    lineArr = line.Split('\t');
                }
                else
                {
                    MarkerCount = true;
                }
            }
            using (StreamWriter sw = File.AppendText(writePath))
            {
                sw.WriteLine("line 110");
            }
            for (int i = 0; i < number_of_markers * 3; i++)
            {
                if (n == 0)
                {
                    SetupData[i] = double.Parse(lineArr[i], CultureInfo.InvariantCulture.NumberFormat); //marker postions goes in order z,x,y
                }
                if (n == 1)
                {
                    SubjectData[i] = double.Parse(lineArr[i], CultureInfo.InvariantCulture.NumberFormat); //marker postions goes in order z,x,y
                }
            }

        }
        sorted = false;
        line_sorted = false;
        //this section here puts the data in descending order of height 
        while (sorted == true) //currently not being used
        {
            line_sorted = true;
            for (int i = 0; i < number_of_markers - 1; i++)
            {
                compare = SetupData[i * 3 + 2] - SetupData[i * 3 + 5];
                if (compare < 0) //if the next marker data was larger in the y direction then they swap
                {
                    placeholder = SetupData[i * 3 + 2];
                    SetupData[i * 3 + 2] = SetupData[i * 3 + 5];
                    SetupData[i * 3 + 5] = placeholder;
                    nameholder = SetupNames[i];
                    SetupNames[i] = SetupNames[i + 1];
                    SetupNames[i + 1] = nameholder;
                    line_sorted = false;
                }
            }
            if (line_sorted == true)
            {
                sorted = true;
            }
        }
        //using (StreamWriter sw = File.AppendText(writePath))
        //{
        //sw.WriteLine(SetupNames[i]);
        //}

        //this next section is for the calculation of the scaling 
        ScalingData = new Vector3[9];
        subject = new Vector3[9];
        setup = new Vector3[9];
        //the numbers are in the worong order because we are chanigin the order from z,x,y to x,y,z also the order of the scaling data is the order of the markers 
        subject[0] = new Vector3(ToFloat(SubjectData[1]), ToFloat(SubjectData[2]), ToFloat(SubjectData[0])); // this is used to change c7 to the right position
        subject[1] = new Vector3(ToFloat(SubjectData[4] - SubjectData[1]), ToFloat(SubjectData[5] - SubjectData[2]), ToFloat(SubjectData[3] - SubjectData[0])); // this is the distance to the acromion from the C7 for the subject
        subject[2] = new Vector3(ToFloat(SubjectData[7] - SubjectData[1]), ToFloat(SubjectData[8] - SubjectData[2]), ToFloat(SubjectData[6] - SubjectData[0])); // this is the distance to the SN from the C7 for the subject
        subject[8] = new Vector3(ToFloat(SubjectData[25] - SubjectData[4]), ToFloat(SubjectData[26] - SubjectData[5]), ToFloat(SubjectData[24] - SubjectData[3])); // this is the distance to the Gthum from the Acromion for the subject
        subject[3] = new Vector3(ToFloat(SubjectData[10] - SubjectData[25]), ToFloat(SubjectData[11] - SubjectData[26]), ToFloat(SubjectData[9] - SubjectData[24])); // this is the distance to the elbow_L from the Gthum for the subject
        subject[4] = new Vector3(ToFloat(SubjectData[13] - SubjectData[25]), ToFloat(SubjectData[14] - SubjectData[26]), ToFloat(SubjectData[12] - SubjectData[24])); // this is the distance to the elbow_R from the Gthum for the subject
        subject[6] = new Vector3(ToFloat(SubjectData[19] - SubjectData[10]), ToFloat(SubjectData[20] - SubjectData[11]), ToFloat(SubjectData[18] - SubjectData[9])); // this is the distance to the wrist_L from the elbow_L for the subject
        subject[7] = new Vector3(ToFloat(SubjectData[22] - SubjectData[13]), ToFloat(SubjectData[23] - SubjectData[14]), ToFloat(SubjectData[21] - SubjectData[14])); // this is the distance to the wrist_R from the elbow_R for the subject
        subject[5] = new Vector3(ToFloat(SubjectData[16] - SubjectData[10]), ToFloat(SubjectData[17] - SubjectData[11]), ToFloat(SubjectData[15] - SubjectData[9])); // this is the distance to the forearm from the elbow_L for the subject

        setup[0] = new Vector3(ToFloat(SetupData[1]), ToFloat(SetupData[2]), ToFloat(SetupData[0])); // this is used to change c7 to the right position
        setup[1] = new Vector3(ToFloat(SetupData[4] - SetupData[1]), ToFloat(SetupData[5] - SetupData[2]), ToFloat(SetupData[3] - SetupData[0])); // this is the distance to the acromion from the C7 for the subject
        setup[2] = new Vector3(ToFloat(SetupData[7] - SetupData[1]), ToFloat(SetupData[8] - SetupData[2]), ToFloat(SetupData[6] - SetupData[0])); // this is the distance to the SN from the C7 for the subject
        setup[8] = new Vector3(ToFloat(SetupData[25] - SetupData[4]), ToFloat(SetupData[26] - SetupData[5]), ToFloat(SetupData[24] - SetupData[3])); // this is the distance to the Gthum from the Acromion for the subject
        setup[3] = new Vector3(ToFloat(SetupData[10] - SetupData[25]), ToFloat(SetupData[11] - SetupData[26]), ToFloat(SetupData[9] - SetupData[24])); // this is the distance to the elbow_L from the Gthum for the subject
        setup[4] = new Vector3(ToFloat(SetupData[13] - SetupData[25]), ToFloat(SetupData[14] - SetupData[26]), ToFloat(SetupData[12] - SetupData[24])); // this is the distance to the elbow_R from the Gthum for the subject
        setup[6] = new Vector3(ToFloat(SetupData[19] - SetupData[10]), ToFloat(SetupData[20] - SetupData[11]), ToFloat(SetupData[18] - SetupData[9])); // this is the distance to the wrist_L from the elbow_L for the subject
        setup[7] = new Vector3(ToFloat(SetupData[22] - SetupData[13]), ToFloat(SetupData[23] - SetupData[14]), ToFloat(SetupData[21] - SetupData[14])); // this is the distance to the wrist_R from the elbow_R for the subject
        setup[5] = new Vector3(ToFloat(SetupData[16] - SetupData[10]), ToFloat(SetupData[17] - SetupData[11]), ToFloat(SetupData[15] - SetupData[9])); // this is the distance to the forearm from the elbow_L for the subject
        for (int i = 0; i < 9; i++)
        {
            if (i==0)
            {
                ScalingData[i] = new Vector3(0f, 0f, 0f);
            }
            ScalingData[i] = setup[i] - subject[i];
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
