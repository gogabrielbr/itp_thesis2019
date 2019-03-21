using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//"When you add a script which uses RequireComponent to a GameObject, the required component will automatically be added to the GameObject. This is useful to avoid setup errors"
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

public class DrawGraph : MonoBehaviour
{
    //define material
    public Material lmat;

    //variable for the mesh
    private Mesh ml;

    //variable for the start point
    private Vector3 s;

    //variable for Line Size. Private for now. Should make it public/customizable..
    private float LineSize = .1f;

    //Check if it is the first quad drawn. The first need all points, while the following need less.
    private bool firstQuad = true;

    private void Start()
    {
        //get track of the mesh
        ml = GetComponent<MeshFilter>().mesh;
        //make mesh visible
        GetComponent<MeshRenderer>().material = lmat;
    }

    //setting the width
    public void setWidth(float width)
    {
        LineSize = width;
    }


    //For the second quad. Same as the "set position" on "Draw.cs". Give us a point and that's the next point in line.
    public void AddPoint(Vector3 point)
    {
        //if the start point is not 0, run function Addline.
        // if(s != Vector3.zero)
        if (s != Vector3.zero)
        {
            AddLine(ml, MakeQuad(s, point, LineSize, firstQuad));
            firstQuad = false;
        }
        //sets the start point
        s = point;
    }

    //When I draw the 2 points, with a start and end. Check if first or subsequent quad. Make the quad
    Vector3[] MakeQuad(Vector3 s, Vector3 e, float w, bool all)
    {
        //set the width to be half. top half and half bottom.
        w = w / 2;
        //q = quad made of an array of vector3s.
        Vector3[] q;
        //if we want all 4 vertices..
        if (all)
            q = new Vector3[4];
        else
            q = new Vector3[2];

        //Vector3.Cross is the result of two vectors. This creates the triangles.https://docs.unity3d.com/ScriptReference/Vector3.Cross.html. 
        Vector3 n = Vector3.Cross(s, e);
        //sets the perpendicular line.
        Vector3 l = Vector3.Cross(n, e - s);
        //normalize the resulting triangles
        l.Normalize();

        //set the Vector3s coordinates for each point. "if"= rules for the first quad. "else"= rules for the subsequente quads.

        if (all)
        {
            //InverseTransformPoints = sets from world to local space coordinates. I believe this is to connect the quads following the coordinates of the last quad. It multiplies by "w" = width. S = start point. E = endpoint 

            q[0] = transform.InverseTransformPoint(s + l * w);
            q[1] = transform.InverseTransformPoint(s + l * -w);
            q[2] = transform.InverseTransformPoint(e + l * w);
            q[3] = transform.InverseTransformPoint(e + l * -w);
        }
        else
        {
            q[0] = transform.InverseTransformPoint(s + l * w);
            q[1] = transform.InverseTransformPoint(s + l * -w);
        }
        //"The return statement terminates execution of the method in which it appears and returns control to the calling method. It can also return an optional value. If the method is a void type, the return statement can be omitted."
        return q;
    }

    //This is actually building the mesh.
    void AddLine(Mesh m, Vector3[] quad)
    {
        //check how many vertice we have
        int vl = m.vertices.Length;
        //save these vertices
        Vector3[] vs = m.vertices;
        vs = resizeVertices(vs, 2 * quad.Length);

        //add them with this For loop.
        for (int i = 0; i < 2 * quad.Length; i += 2)
        {
            vs[vl + i] = quad[i / 2];
            vs[vl + i + 1] = quad[i / 2];
        }

        //UV Maps
        Vector2[] uvs = m.uv;
        uvs = resizeUVs(uvs, 2 * quad.Length);

        if (quad.Length == 4)
        {
            //the bottom corner is 0 0
            uvs[vl] = Vector2.zero;
            uvs[vl + 1] = Vector2.zero;
            uvs[vl + 2] = Vector2.right;
            uvs[vl + 3] = Vector2.right;
            uvs[vl + 4] = Vector2.up;
            uvs[vl + 5] = Vector2.up;
            uvs[vl + 6] = Vector2.one;
            uvs[vl + 7] = Vector2.one;
        }
        else
        {
            if (vl % 8 == 0)
            {
                uvs[vl] = Vector2.zero;
                uvs[vl + 1] = Vector2.zero;
                uvs[vl + 2] = Vector2.right;
                uvs[vl + 3] = Vector2.right;
            }
            else
            {
                uvs[vl] = Vector2.up;
                uvs[vl + 1] = Vector2.up;
                uvs[vl + 2] = Vector2.one;
                uvs[vl + 3] = Vector2.one;
            }
        }

        //create the triangles. Drawing 4 triangles. For each triangle we give the index of the vertice we want to draw.
        int tl = m.triangles.Length;

        int[] ts = m.triangles;
        ts = resizeTriangles(ts, 12);

        if (quad.Length == 2)
        {
            vl -= 4;
        }
        //The order in which the points are drawn will define the direction of the normal! Counter-Clockwise for Forward.
        //front facing quad
        ts[tl] = vl;
        ts[tl + 1] = vl + 2;
        ts[tl + 2] = vl + 4;

        ts[tl + 3] = vl + 2;
        ts[tl + 4] = vl + 6;
        ts[tl + 5] = vl + 4;

        //back facing triangles
        ts[tl + 6] = vl + 5;
        ts[tl + 7] = vl + 3;
        ts[tl + 8] = vl + 1;

        ts[tl + 9] = vl + 5;
        ts[tl + 10] = vl + 7;
        ts[tl + 11] = vl + 3;

        m.vertices = vs;
        m.uv = uvs;
        m.triangles = ts;
        //recalculate the boundaries for adding colliders later
        m.RecalculateBounds();
        m.RecalculateNormals();

    }

    Vector3[] resizeVertices(Vector3[] ovs, int ns)
    {
        Vector3[] nvs = new Vector3[ovs.Length + ns];
        for (int i = 0; i < ovs.Length; i++)
        {
            nvs[i] = ovs[i];
        }
        return nvs;
    }

    Vector2[] resizeUVs(Vector2[] uvs, int ns)
    {
        Vector2[] nvs = new Vector2[uvs.Length + ns];
        for (int i = 0; i < uvs.Length; i++)
        {
            nvs[i] = uvs[i];
        }
        return nvs;
    }

    int[] resizeTriangles(int[] ovs, int ns)
    {
        int[] nvs = new int[ovs.Length + ns];
        for (int i = 0; i < ovs.Length; i++)
        {
            nvs[i] = ovs[i];
        }
        return nvs;
    }



}
