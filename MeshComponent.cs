using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;

public class MeshComponent : Component
{

    public Vector3[] OriginalPC;
    public List<float> Bounds = new List<float>();
    public List<Segment> Segments = new List<Segment>();
    public int numberofSegmentsX = 30;
    public int numberofSegmentsZ = 30;


    public MeshComponent(Vector3[] originalPC) // constructor
    {
        OriginalPC = originalPC;
        getBounds();
    }

    // function to get bounds
    public void getBounds()
    {
        Bounds.Add(OriginalPC[0].x);
        Bounds.Add(OriginalPC[0].y);
        Bounds.Add(OriginalPC[1].x);
        Bounds.Add(OriginalPC[1].y);

        foreach (Vector3 vertex in OriginalPC)
        {
            if (Bounds[0] > vertex.x)
            {
                Bounds[0] = vertex.x;
            }
            if (Bounds[1] > vertex.z)
            {
                Bounds[1] = vertex.z;
            }
            if (Bounds[2] < vertex.x)
            {
                Bounds[2] = vertex.x;
            }
            if (Bounds[3] < vertex.z)
            {
                Bounds[3] = vertex.z;
            }
        }
        getSegments();
    }

    public void getSegments()
    {
        
        for (int i = 0; i < numberofSegmentsX; i++)
        {
            for (int j = 0; j < numberofSegmentsX; j++)
            {
                float minX = (Bounds[0] + (i * ((Bounds[2] - Bounds[0]) / numberofSegmentsX)));
                float maxX = (Bounds[0] + ((i + 1) * ((Bounds[2] - Bounds[0]) / numberofSegmentsX)));
                float minZ = (Bounds[1] + (j * ((Bounds[3] - Bounds[1]) / numberofSegmentsZ)));
                float maxZ = (Bounds[1] + ((j + 1) * ((Bounds[3] - Bounds[1]) / numberofSegmentsZ)));


                List<Vector3> segmentPoints = new List<Vector3>();
                int index = 0;
                foreach (Vector3 vertex in OriginalPC)
                {

                    if ((vertex.x >= minX) && (vertex.x < maxX) && (vertex.z >= minZ) && (vertex.z < maxZ))
                    {
                        segmentPoints.Add(vertex);
                        index++;
                    }
                }
                var list = new List<float>();
                list.Add(minX);
                list.Add(minZ);
                list.Add(maxX);
                list.Add(maxZ);
                Segments.Add(new Segment(segmentPoints.ToArray(), ((10*j) + (i + 1)), list));
            }
        }
    }
    
    public Vector3[] checkSegment(Vector3 center, float range) {
        List<Vector3> outputList = new List<Vector3>();
        foreach (Segment segment in Segments)
        {
            if (segment.Bounds[0] < (center.x + range) && segment.Bounds[2] > (center.x - range) && segment.Bounds[1] < (center.z + range) && segment.Bounds[3] > (center.z - range))
            {
                outputList.AddRange(segment.SegmentPC);
            }
        }
    return outputList.ToArray();
    }

}

public class Segment : Component
{
    public Vector3[] SegmentPC { get; set; }
    public int ID;
    public List<float> Bounds; // min x, min y, max x, max y
    public Segment(Vector3[] PC, int id, List<float> bounds)
    {
        SegmentPC = PC;
        ID = id;
        Bounds = bounds;
    }
}