using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PointListSource  {

    public static PointListSource CurrentEdit;
    public static uint CurrentNodeInd = uint.MaxValue;
    public static uint LastNodeInd = uint.MaxValue;
    bool Rebuild = false;

    public Transform Trnsfrm;
    MeshFilter MF;
    PolygonCollider2D Col;
   
    [System.Serializable]
    public class Node {
        public Node() { }
        public Vector2 P, N;
        public void setN(Vector2 nrm) {
            var sm = nrm.sqrMagnitude;
            if(sm < 0.01f) nrm = Vector2.up; else nrm /= Mathf.Sqrt(sm);
            N = nrm;
        }
        public Vector3 getP() { return new Vector3(P.x, P.y, 0); }
    }
    public List<Node> Nodes = new List<Node>();


    public void select(GameObject host) {
        Trnsfrm = host.transform;
        MF = host.GetComponent<MeshFilter>();
        Col = host.GetComponent<PolygonCollider2D>();
        CurrentEdit = this;
        CurrentNodeInd = LastNodeInd = uint.MaxValue;
    }

    public bool pick(Vector3 mp3) {

        bool ret = false;
        var mp = getP(mp3 - Trnsfrm.position);
        var d = 0.1f;
        uint i = 0;
        foreach(Node n in Nodes) {
            var mg = (n.P - mp).sqrMagnitude;
            if(mg < d) {
                d = mg;
                LastNodeInd = CurrentNodeInd = i;
                ret = true;
            }
            i++;
        }
        return ret;
    }
    public void add(Vector3 p, EventModifiers mod) {
        
        if(LastNodeInd == 0 && Nodes.Count > 1) {
            CurrentNodeInd = 0;
            Nodes.Insert(0, new Node());
        } else {
            LastNodeInd = CurrentNodeInd = (uint)Nodes.Count;
            Nodes.Add(new Node());
        }

        move(p, (int)CurrentNodeInd, mod);
    }
    public void move(Vector3 p3, int ni, EventModifiers mod) {
        var p = getP(p3 - Trnsfrm.position);
        var n = Nodes[ni];
        if((mod & EventModifiers.Control) != 0 && ni > 0) {
            p = snap(p, Nodes[ni - 1].P, (mod & EventModifiers.Alt) != 0);
        } else if((mod & EventModifiers.Shift) != 0 && ni <= Nodes.Count - 2) {
            p = snap(p, Nodes[ni + 1].P, (mod & EventModifiers.Alt) != 0);
        }
        n.P = p;

        var nrm = Vector2.zero; bool gotNrm = false;
        if(ni > 0) {
            var on = Nodes[ni - 1];
            var nrm1 = wind(p - on.P);
            nrm = nrm1;
            gotNrm = true;

            if(ni > 1) {
                var pn = Nodes[ni - 2];
                var nrm2 = wind(on.P - pn.P);
                if(nrm1.sqrMagnitude > 0.001f) {
                    if(nrm2.sqrMagnitude > 0.001f) {
                        on.setN(nrm1.normalized + nrm2.normalized);
                    } else {
                        on.setN(nrm1);
                    }
                } else if(nrm2.sqrMagnitude > 0.001f) {
                    on.setN(nrm2);
                }
            } else on.setN(nrm1);
        }
        if(ni <= Nodes.Count - 2) {
            var on = Nodes[ni + 1];
            var nrm1 = wind(on.P - p);

            if(gotNrm) {
                if(nrm.sqrMagnitude > 0.001f) {
                    if(nrm1.sqrMagnitude > 0.001f) {
                        nrm = nrm.normalized + nrm1.normalized;
                    }
                } else if(nrm1.sqrMagnitude > 0.001f) {
                    nrm = nrm1;
                }
            } else
                nrm = nrm1;

            if(ni <= Nodes.Count - 3) {
                var pn = Nodes[ni + 2];
                var nrm2 = wind(pn.P - on.P);
                if(nrm1.sqrMagnitude > 0.001f) {
                    if(nrm2.sqrMagnitude > 0.001f) {
                        on.setN(nrm1.normalized + nrm2.normalized);
                    } else {
                        on.setN(nrm1);
                    }
                } else if(nrm2.sqrMagnitude > 0.001f) {
                    on.setN(nrm2);
                }
            } else on.setN(nrm1);
        }

        n.setN(nrm);

        Rebuild = true;
    }
    Vector2 wind(Vector2 v) { return new Vector2(-v.y, v.x); }
    Vector3 snap(Vector2 p, Vector2 op, bool snapDis) {
        var vec = p - op; var mag = vec.sqrMagnitude;
        if(mag > 0.001f) {
            mag = Mathf.Sqrt(mag); vec /= mag;
            var ang = Mathf.Rad2Deg * Mathf.Atan2(vec.y, vec.x);
            float step = 22.5f;// *Mathf.Deg2Rad;
            ang = Mathf.Round(ang / step) * step;
            float step2 = 2.0f;
            if(snapDis) mag = Mathf.Round(mag / step2) * step2;
            p = op + new Vector2(Mathf.Cos(ang * Mathf.Deg2Rad), Mathf.Sin(ang * Mathf.Deg2Rad)) * mag;
            // Debug.Log("snap "+ ang );
        }
        return p;
    }

    static Vector3 getP(Vector2 p) { return new Vector3(p.x, p.y, 0); }
    static Vector2 getP(Vector3 p) { return new Vector2(p.x, p.y); }

    void rebuild() {
        Rebuild = false;

        if(MF.sharedMesh != null) GameObject.DestroyImmediate(MF.sharedMesh);

        var mesh = MF.sharedMesh = new Mesh();

        if(Nodes.Count < 2) return;

        int triCnt = (Nodes.Count - 1) * 6;
        int vrtCnt = Nodes.Count * 2;

        var tris = new int[triCnt];
        var vrts = new Vector3[vrtCnt];
        var nrms = new Vector3[vrtCnt];
        var uv = new Vector2[vrtCnt];

        int ti = 0;

        var path = new Vector2[vrtCnt];
        //
        for(int i = Nodes.Count; i-- > 0; ) {
            var n = Nodes[i];
            var nrm = n.N; //nrm /= Mathf.Max(Mathf.Abs(nrm.x), Mathf.Abs(nrm.y));

            if(i > 0 && i < Nodes.Count - 2) {
                Vector2 v1 = n.P - Nodes[i - 1].P, v2 = Nodes[i + 1].P - n.P;
                nrm /= Mathf.Abs(Vector2.Dot(nrm, wind(v1.normalized)));
            }

            var p = n.getP(); var na = getP(nrm) * 0.3f;

            int vi = i * 2;

            if(i > 0) {
                tris[ti++] = vi; tris[ti++] = vi + 1; tris[ti++] = vi - 2;
                tris[ti++] = vi - 2; tris[ti++] = vi + 1; tris[ti++] = vi - 1;
            }

            vrts[vi] = p + na; vrts[vi + 1] = p - na;
            nrms[vi] = nrms[vi + 1] = Vector3.back;


            path[i] = vrts[vi];
            path[vrtCnt-1-i] = vrts[vi+1];
        }

        mesh.vertices = vrts;
        mesh.normals = nrms;
        mesh.triangles = tris;
        mesh.uv = uv;

        if( Col) 
            Col.SetPath(0, path);
    }


    struct CubicSpline {
        public CubicSpline( Vector2 a, Vector2 b, Vector2 c, Vector2 d) {

            M0 = 0.5f * d - 1.5f * c - 0.5f * a + 1.5f * b;
            M1 = a - 2.5f * b + 2.0f * c - 0.5f * d;
            M2 = 0.5f * c - 0.5f * a;
            M3 = b;
        }
        public Vector2 get(float delta) {
            float delta2 = delta * delta;
            return M0 * delta * delta2 + M1 * delta2 + M2 * delta + M3;
        }
        Vector2 M0, M1, M2, M3;
    };

    public void draw() {
        if(Rebuild) rebuild();

        if(!Trnsfrm) return;
        //if(CurrentEdit != this) return;
        var off = Trnsfrm.position;


        if(Trnsfrm.name == "Mid") {  //todo -- lazy..

    
            for( int i = 1; i < Nodes.Count; i++ ) {
                Node n = Nodes[i], l  = Nodes[i-1];

                var p = off + n.getP();


                Gizmos.color = Color.blue;
                var nrm = getP(n.N * 0.5f);
            //    Gizmos.DrawLine(p + nrm, p - nrm);

                Vector2 b = l.getP(), c = n.getP(), a,d;
                float mag = (b-c).magnitude;
                if(i >= 2) {
                    a = Nodes[i - 2].getP();
                    a = b + (a - b).normalized * mag;
                }  else a = b * 2 - c;
                if(i < Nodes.Count - 1) {
                    d = Nodes[i + 1].getP();
                    d = c + (d - c).normalized * mag;
                } else d = c * 2 - b;

                Gizmos.color = Color.red;

              //  Vector2 m0 = d - c - a + b,  m1 = a - b - m0,
              //    m2 = c - a,  m3 = b;

                var spline = new CubicSpline(a, b, c, d);

                var col = Color.red;


                Vector2 lp = c;

                int iter = Mathf.FloorToInt(mag / 1.0f);

                float step = 1.0f / (float)(iter + 1);

                for(int j = iter; --j > 0; ) {

                    float delta = (float)j * step;

                    p = spline.get(delta);

                    Gizmos.color = col;
                    Gizmos.DrawLine(off + p, off + (Vector3)lp);
                    col.r = col.g; col.g = 1 - col.g;
                    lp = p;
                }

                Gizmos.color = col;
                Gizmos.DrawLine(off + (Vector3)lp, off + (Vector3)b);         
            }
        }        
        if(CurrentEdit == this) { 

            if(LastNodeInd >= (uint)Nodes.Count) LastNodeInd = (uint)(Nodes.Count - 1);
            Node last = null;
            uint i = 0;
            foreach(Node n in Nodes) {
                var p = off + n.getP();

                if(i == CurrentNodeInd) Gizmos.color = Color.red;
                else if(i == LastNodeInd) Gizmos.color = Color.magenta;
                else Gizmos.color = Color.green;

                Gizmos.DrawCube(p, new Vector3(0.5f, 0.5f, 0.5f));

                Gizmos.color = Color.blue;
                var nrm = getP(n.N * 0.5f);
                Gizmos.DrawLine(p + nrm, p - nrm);

                if(last != null) {
                    Gizmos.color = Color.white;
                 //   Gizmos.DrawLine(p, off + last.getP());
                }
                last = n;
                i++;
            }
        }
    }

    public void gravMod(ref Vector2 g, Vector2 at) {  //this function should not really be here


        var off = Trnsfrm.position;

        at -= (Vector2)off;

        int ci = -1; float cd = float.MaxValue, cDelta = 0;
        Vector2 cp = Nodes[0].getP();
        for(int i = 1; i < Nodes.Count; i++) {  //eseaching all nodes.. bad / lazy
            Node n = Nodes[i], l = Nodes[i - 1];

            Vector2 p = n.getP(), lp = l.getP();

            var v = lp - p;
            var mag = v.magnitude;
            var v2 = at - p;
            var dot = Vector2.Dot(v2, v);

            float d; Vector2 p1;
            float delta;
            // Debug.Log( "
            if(dot < 0) {
                d = v2.SqrMagnitude();
                p1 = p;
                delta = 0;
            } else if(dot > mag * mag) {
                d = (at - lp).SqrMagnitude();
                p1 = lp;
                delta = 1;
            } else {

                v /= mag;
                p1 = p + v * dot / mag ;

                delta = dot / (mag * mag);

                Vector2 nrm = new Vector2(v.y, -v.x);

                d = Vector2.Dot(nrm, at) - Vector2.Dot(nrm, p);

                //        Debug.DrawLine(off + (Vector3)p1, off + (Vector3)(p1 + nrm * d), Color.red);
                d *= d;
            }


            if(d < cd) {
                ci = i;
                cd = d;
                cp = p1;
                cDelta = delta;
            }

        }

        Debug.DrawLine(off + (Vector3)cp, off + (Vector3)at, Color.green);
        {
            int i = ci;
            Node n = Nodes[i], l = Nodes[i - 1];
            Vector2 b = l.N, c = n.N, a, d;
            //float mag = (b - c).magnitude;
            if(i >= 2) {
                a = Nodes[i - 2].N;
                //  a = b + (a - b).normalized * mag;
            } else a = b;// *2 - c;
            if(i < Nodes.Count - 1) {
                d = Nodes[i + 1].N;
                // d = c + (d - c).normalized * mag;
            } else d = c;// *2 - b;

            Gizmos.color = Color.red;


            var spline = new CubicSpline(d,c,b,a);

            var sn = spline.get(cDelta).normalized;

            g = sn * g.y;

            Debug.DrawLine(off + (Vector3) (at +sn ), off + (Vector3)at, Color.blue);

         //   Debug.DrawLine(off + (Vector3)dp, off + (Vector3)at, Color.blue);

            /*
            var col = Color.red;
            Vector2 lp = c;

            int iter = Mathf.FloorToInt(mag / 1.0f);

            float step = 1.0f / (float)(iter + 1);

            for(int j = iter; --j > 0; ) {

                Gizmos.color = col;
                Gizmos.DrawLine(off + p, off + (Vector3)lp);
                col.r = col.g; col.g = 1 - col.g;
                lp = p;
            }

            Gizmos.color = col;
            Gizmos.DrawLine(off + (Vector3)lp, off + (Vector3)b); */

        }
    }        


}
