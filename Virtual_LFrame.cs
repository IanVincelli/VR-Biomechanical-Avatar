using System;
using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Virtual_LFrame : MonoBehaviour {
    
    public static Matrix4x4 VLF_LG;
    private GameObject A0;
    private GameObject A1;
    private GameObject B1;
    private GameObject C1;
    //private Vector3 Headset_Position;
    private Matrix4x4 Headset_TF;
    public static Matrix4x4 TF_VH; // transformation matrix for virtual to headset
    public static Matrix4x4 TF_RH; // transformation matrix for real to headset
    public static Matrix4x4 TF_RV; //transformation matrix from real to virtual i hope to god this works
    private StreamReader sr;
    //private double[,] MarkerPositions; //marker position matrix
    private double[] MarkerPositions; //marker position matrix
    private double mp;
    private int length;
    private List<string> MarkerNames;
    private string line;
    private string[] lineArr;
    private int number_of_markers;
    private int number_of_frames;
    private int index;
    //virtual LFram data
    private Vector3 a;
    private Vector3 b;
    private Vector3 c;
    //real Lframe vectors
    private Vector3 a_r;
    private Vector3 b_r;
    private Vector3 c_r;
    public static Vector3 o_r;
    //real Lframe data
    private Vector3 A_r;
    private Vector3 B_r;
    private Vector3 C_r;
    private Vector3 O_r;
    //transformation matrix
    private Matrix4x4 V_LG;//local to global Virtual L frame
    private Matrix4x4 R_LG;//local to gloabal real L frame
    private Matrix4x4 VR_LL; //local real to virtual transformation matrix

    private string readPath = @"C:\Ian\Thesis\StaticTSVfiles\labsetup_test.tsv";
    private string writePath = @"C:\Ian\Thesis\TextFiles\Debuggingg.txt";// this is used for debugging

    public static float ToFloat(double value)
    {
        return (float)value;
    }
    // public Virtual_LFrame() {
    void Start()
    {
        //initialization
        length = File.ReadAllLines(readPath).Length;
        MarkerNames = new List<string>();
        sr = new StreamReader(readPath);
        File.WriteAllText(writePath, String.Empty);
        
        A0 = GameObject.Find("A0");
        A1 = GameObject.Find("A1");
        B1 = GameObject.Find("B1");
        C1 = GameObject.Find("C1");

        a = Vector3.Cross((A1.transform.position - A0.transform.position), (B1.transform.position - A0.transform.position));
        //a = a.normalized;
        b = Vector3.Cross((A1.transform.position - A0.transform.position),a);
        //b = b.normalized;
        c = Vector3.Cross(a, b);
        a = a.normalized;
        b = b.normalized;
        c = c.normalized;

        VLF_LG.SetColumn(0, new Vector4(a.x, a.y, a.z, 0));
        VLF_LG.SetColumn(1, new Vector4(b.x, b.y, b.z, 0));
        VLF_LG.SetColumn(2, new Vector4(c.x, c.y, c.z, 0));
        VLF_LG.SetColumn(3, new Vector4(A0.transform.position.x, A0.transform.position.y, A0.transform.position.z, 0));

        //if (Headset_Script.Headset_TF[0,0] >= 0 || Headset_Script.Headset_TF[0, 0] <= 0)
        //{
        TF_VH = Headset_TF * V_LG;

        //}
       

        GameObject A = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        A.name = "A";
        A.transform.position = a;
        A.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        GameObject B = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        B.name = "B";
        B.transform.position = b;
        B.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        GameObject C = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        C.name = "C";
        C.transform.position = c;
        C.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        GameObject O = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        O.name = "O";
        O.transform.position = A0.transform.position;
        O.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);


        //.TSV file reading

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
        //initialize MarkerPositions Matrix
        //MarkerPositions = new double[number_of_markers * 3, number_of_frames]; //markers are multiplied by 3 because they each have an x,y and z coordinate
        MarkerPositions = new double[number_of_markers * 3]; //markers are multiplied by 3 because they each have an x,y and z coordinate


       
        lineArr = line.Split('\t'); //splits the position values by tabs

        for (int i = 1; i <= number_of_markers; i++)
        {
            MarkerNames.Add(lineArr[i]);
        }

        for (int i = 0; i < 50; i++)// reads for 50 line to get into the data
        {
            sr.ReadLine();
        }
        line = sr.ReadLine();
        lineArr = line.Split('\t'); //splits the position values by tabs

        for (int i = 0; i < number_of_markers * 3; i++)
        {
            MarkerPositions[i] = double.Parse(lineArr[i], CultureInfo.InvariantCulture.NumberFormat); //marker postions goes in order z,x,y

        }

        for (int i = 0; i < number_of_markers; i++)
        {
            if (MarkerNames[i] == "a0")
            {
                O_r = new Vector3(ToFloat(MarkerPositions[1 + i * 3]) / 1000.00f, ToFloat(MarkerPositions[2 + i * 3]) / 1000.00f, ToFloat(-1*MarkerPositions[i * 3 + 0]) / 1000.00f);
            }
            if (MarkerNames[i] == "a1")
            {
                A_r = new Vector3(ToFloat(MarkerPositions[1 + i * 3]) / 1000.00f, ToFloat(MarkerPositions[2 + i * 3]) /1000.00f, ToFloat(-1*MarkerPositions[i * 3 + 0]) /  1000.00f);
            }
            if (MarkerNames[i] == "b1")
            {
                B_r = new Vector3(ToFloat(MarkerPositions[1 + i * 3]) / 1000.00f, ToFloat(MarkerPositions[2 + i * 3]) / 1000.00f, ToFloat(-1*MarkerPositions[i * 3 + 0]) / 1000.00f);
            }
            if (MarkerNames[i] == "c1")
            {
                C_r = new Vector3(ToFloat(MarkerPositions[1 + i * 3]) / 1000.00f, ToFloat(MarkerPositions[2 + i * 3]) / 1000.00f, ToFloat(-1*MarkerPositions[i * 3 + 0]) / 1000.00f);
            }

        }

        a_r = Vector3.Cross(A_r - O_r, B_r - O_r);
        //a_r = a_r.normalized;
        b_r = Vector3.Cross(A_r - O_r, a_r);
        //b_r = b_r.normalized;
        c_r = Vector3.Cross(a_r, b_r);
        a_r = a_r.normalized;
        b_r = b_r.normalized;
        c_r = c_r.normalized;
        o_r = O_r;

        R_LG.SetColumn(0, new Vector4(a_r.x, a_r.y, a_r.z, 0));
        R_LG.SetColumn(1, new Vector4(b_r.x, b_r.y, b_r.z, 0));
        R_LG.SetColumn(2, new Vector4(c_r.x, c_r.y, c_r.z, 0));
        R_LG.SetColumn(3, new Vector4(o_r.x, o_r.y, o_r.z, 0));
        //V_LG = V_LG.transpose;
        //VR_LL = R_LG*V_LG.transpose;
        V_LG = VLF_LG;
        VR_LL = V_LG * R_LG.transpose;
        TF_RV = VR_LL;
        //TF_RH = Headset_Script.Headset_TF.inverse * R_LG;


        a_r = VR_LL * a_r;
        b_r = VR_LL * b_r;
        c_r = VR_LL * c_r;
        o_r = VR_LL * o_r;

        A_r = VR_LL * A_r;
        B_r = VR_LL * B_r;
        C_r = VR_LL * C_r;
        O_r = VR_LL * O_r;

        GameObject AR = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        AR.name = "AR";
        AR.transform.position = a_r;
        AR.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        GameObject BR = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        BR.name = "BR";
        BR.transform.position = b_r;
        BR.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        GameObject CR = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        CR.name = "CR";
        CR.transform.position = c_r;
        CR.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        GameObject OR = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        OR.name = "OR";
        OR.transform.position = o_r;
        OR.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

        GameObject ATSV = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ATSV.name = "ATSV";
        ATSV.transform.position = A_r - new Vector3(o_r.x - V_LG[0, 3], o_r.y - V_LG[1, 3], o_r.z - V_LG[2, 3]);  
        ATSV.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        ATSV.GetComponent<Renderer>().material.color = new Vector4(1, 0, 0, 1);
        GameObject BTSV = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        BTSV.name = "BTSV";
        BTSV.transform.position = B_r - new Vector3(o_r.x - V_LG[0, 3], o_r.y - V_LG[1, 3], o_r.z - V_LG[2, 3]); 
        BTSV.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        BTSV.GetComponent<Renderer>().material.color = new Vector4(1, 0, 0, 1);
        GameObject CTSV = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        CTSV.name = "CTSV";
        CTSV.transform.position = C_r - new Vector3(o_r.x - V_LG[0, 3], o_r.y - V_LG[1, 3], o_r.z - V_LG[2, 3]);
        CTSV.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        CTSV.GetComponent<Renderer>().material.color = new Vector4(1, 0, 0, 1);
        GameObject OTSV = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        OTSV.name = "OTSV";
        OTSV.transform.position = O_r - new Vector3(o_r.x - V_LG[0, 3], o_r.y - V_LG[1, 3], o_r.z - V_LG[2, 3]);
        OTSV.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        OTSV.GetComponent<Renderer>().material.color = new Vector4(1, 0, 0, 1);
        //for (int j = 0; j < number_of_frames; j++)
        //{
        //    line = sr.ReadLine();
        //    lineArr = line.Split('\t');
        //    for (int i = 0; i < number_of_markers * 3; i++)
        //    {
        //        MarkerPositions[i, j] = double.Parse(lineArr[i], CultureInfo.InvariantCulture.NumberFormat); //marker postions goes in order z,x,y
        //    }
        //}
    }

    void Update()
    {
       //Headset_TF = GameObject.Find("Camera (eye)").transform.localToWorldMatrix;

       // //TF_RH = Headset_TF.inverse * R_LG;
       // TF_VH = Headset_TF * V_LG;

       // A0.transform.position = TF_VH * A0.transform.position;
       // A1.transform.position = TF_VH * A1.transform.position;
       // B1.transform.position = TF_VH * B1.transform.position;
       // C1.transform.position = TF_VH * C1.transform.position;
    }

}
