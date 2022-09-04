using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
//using Unity.Collections.LowLevel.Unsafe;


public class ShrinkingBallSeg : MonoBehaviour
    {
        public int thinning;
        public Vector3[] vertices;
        public Vector3[] normals;
        public float initialRadius = 100.0f;
        public List<Vector3> MedialBallCenters;
        public List<float> MedialBallRadii;
        public GameObject dotred;
        public GameObject dotblue;
        MeshComponent meshComp;
        public MATList list;

        void Start()
        {
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
            getMesh();
                meshComp = new MeshComponent(vertices);
                iterateVertices();
                InstantiatePoints();
            }
        }

        public void getMesh()
        {
            GameObject terrain = GameObject.Find("terrain");
        TerrainGenerator meshGenerator = terrain.GetComponent<TerrainGenerator>();
            Mesh mesh = meshGenerator.mesh;
            vertices = meshGenerator.vertices;
            mesh.RecalculateNormals();
            normals = meshGenerator.normals;
        }


    void iterateVertices()
    {
        for (int i = 0; i < vertices.Length; i = i + thinning)
        {
            if (vertices[i].y != 0f && normals[i].y > 0.4f)
            {
                getMedialBallCenter(i);
            }
        }
    }

    void getMedialBallCenter(int vertexIndex)
    {
        bool empty = false;
        float radius = initialRadius;
        while (empty == false)
        {
            radius -= 3.0f;
            empty = checkRadius(vertexIndex, radius);
            if (radius < 10f)
            {
                return;
            }
        }
        MedialBallCenters.Add((vertices[vertexIndex] + normals[vertexIndex] * radius));
        MedialBallRadii.Add(radius);
    }

    bool checkRadius(int vertexIndex, float radius)
    {
        Vector3 medialBallCenter = vertices[vertexIndex] + normals[vertexIndex] * radius;
        Vector3[] list = meshComp.checkSegment(medialBallCenter, radius);
        for (int i = 0; i < list.Length; i = i + thinning)
        {
            if (Distance(vertices[i], medialBallCenter) < radius)
            {
                return false;
            }
        }
        return true;
    }

    void InstantiatePoints()
        {
            list = new MATList(MedialBallCenters.ToArray());
            list.setScores();
            for (int vertId = 0; vertId < MedialBallCenters.ToArray().Length; vertId++)
            {
                Instantiate(dotblue, list.getLoc3D(vertId), transform.rotation);
                //Instantiate(dotred, list.getLoc2D(vertId, 300f), transform.rotation);
            }
        }
    
    float Distance(Vector3 point1, Vector3 point2)
    {
        return Mathf.Pow(Mathf.Pow(point1.x - point2.x, 2f) + Mathf.Pow(point1.y - point2.y, 2f) + Mathf.Pow(point1.z - point2.z, 2f), 0.5f);
    }

    }

    public class MATList : Component
    {
        public Vector3[] OriginalMATList;
        public MATBall[] NewMATList;


        public MATList(Vector3[] originalMATList) // constructor
        {
            OriginalMATList = originalMATList;
            NewMATList = new MATBall[originalMATList.Length];
            int i = 0;
            foreach (Vector3 ball in OriginalMATList)
            {
                NewMATList[i] = new MATBall(ball);
                i++;
            }
        }
        public void setScores()
        {
            MeshComponent matComp = new MeshComponent(OriginalMATList);
            float radiusForScore = 100f;
            for (int num = 0; num < OriginalMATList.Length; num++)
            {
            Vector3[] listToCheck = matComp.checkSegment(OriginalMATList[num], radiusForScore);
                int score = 0;
                foreach (Vector3 vertex in listToCheck)
                {
                    if (Distance(vertex, OriginalMATList[num]) < radiusForScore)
                    {
                        score++;
                    }
                }
                NewMATList[num].Score = score;
            }
        }
        public Vector3 getLoc3D(int index)
        {
            return NewMATList[index].Loc;
        }
        public Vector3 getLoc2D(int index, float yloc)
        {
            return new Vector3(NewMATList[index].Loc.x, yloc, NewMATList[index].Loc.z);
        }
        public MATBall getBall(int index)
        {
            return NewMATList[index];
        }

    float Distance(Vector3 point1, Vector3 point2)
    {
        return Mathf.Pow(Mathf.Pow(point1.x - point2.x, 2f) + Mathf.Pow(point1.y - point2.y, 2f) + Mathf.Pow(point1.z - point2.z, 2f), 0.5f);
    }

}

    public class MATBall : Component
    {
        public Vector3 Loc;
        public int Score;

        public MATBall(Vector3 ballLoc) // constructor
        {
            Loc = ballLoc;
        }
    }

public class MATBranch : Component
{
    public Vector3 Head;
    public Vector3 Tail;

    public MATBranch(Vector3 head, Vector3 tail)
    {
        Head = head;
        Tail = tail;
    }
}



