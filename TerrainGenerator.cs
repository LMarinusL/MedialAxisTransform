
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class TerrainGenerator : MonoBehaviour
{

    public Mesh mesh;
    Color[] colors;
    public Color color1 = new Color(0f, 0.65f, 0.95f, 95f);
    public Color color2 = new Color(0f, 0.6f, 0.0f, 0.6f);
    public Color color3 = new Color(0.8f, 0.8f, 0.8f, 1f);
    public Material material;


    public Vector3[] normals;
    public Vector3[] vertices;
    int[] triangles;

    public int xSize;
    public int zSize;
    public int NumOfIt = 0;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        MeshRenderer meshr = this.GetComponent<MeshRenderer>();
        meshr.material = material;

        StartCoroutine(CreateShape());

    }
    void Update()
    {
        UpdateMesh();
        resetColors();
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            mesh.Clear();
            StartCoroutine(CreateShape());
        }
        if (Input.GetKey(KeyCode.Z))
        {
            transform.Rotate(Vector3.up * 30 * Time.deltaTime);
        };
        if (Input.GetKey(KeyCode.X))
        {
            transform.Rotate(Vector3.up * -30 * Time.deltaTime);
        };
        if (Input.GetKey(KeyCode.A))
        {
            Vector3 position = transform.position;
            position.x--;
            transform.position = position;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector3 position = transform.position;
            position.x++;
            transform.position = position;
        }
        if (Input.GetKey(KeyCode.W))
        {
            Vector3 position = transform.position;
            position.z++;
            transform.position = position;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector3 position = transform.position;
            position.z--;
            transform.position = position;
        }

    }

    void resetColors()
    {
        colors = new Color[mesh.normals.ToArray().Length];
        Vector3[] normals = mesh.normals.ToArray();
        Vector3[] Vertices = mesh.vertices.ToArray();
        for (int i = 0; i < normals.Length; i++)
        {
            float angle = normals[i].y;
            float height = Vertices[i].y;
            if (angle > 0.98 && height < 30) { colors[i] = color1; }
            else
            {
                if (angle > 0.87 && height > 70) { colors[i] = color3; }
                else
                {
                    if (angle > 0.87 && height < 50) { colors[i] = color2; }
                    else
                    {
                        colors[i] = new Color((height / 50) * 0.5f, (height / 50) * 0.4f, (height / 50) * 0.4f, (height / 50) * 0.7f);
                    }
                }
            }
        }
        mesh.colors = colors;
    }

    IEnumerator CreateShape()
    {

        NumOfIt = 0;
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        float randomNum1 = Random.Range(0.3f, 2.4f);
        float randomNum2 = Random.Range(0.3f, 2.4f);

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = (Mathf.PerlinNoise(x * .01f * randomNum2, z * .01f * randomNum1) * 80f)+(Mathf.PerlinNoise(x * .01f * randomNum1 + 30, z * .02f * randomNum2 +50) * 40f);
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;

            };
            vert++;
            yield return new WaitForSeconds(.01f);

        }



    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        normals = mesh.normals;

    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], .1f);
        }
    }


}