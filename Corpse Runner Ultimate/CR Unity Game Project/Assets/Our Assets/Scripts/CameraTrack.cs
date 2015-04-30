using UnityEngine;
using System.Collections;

public class CameraTrack : CommonBehaviour {


    public CharacterMotor[] Targets;
    //public float MaxZoom = 16, MinZoom = 8, 
    public float PlayerRadius = 6, PanSpeed = 0.1f, ZoomSpeed = 0.2f, RotSmooth = 2.0f, RotSpeed = 5.0f;

    Vector2 DesP; float DesZ, PosZ, Rot, DesRot;

    Camera Cam;
 
	void Awake() {
        base.Awake();
        Cam = camera;

        if(Targets.Length != 2) {
            Debug.LogError("err");
            Destroy(this);
            return;
        }

        DesRot =Rot = Trnsfrm.eulerAngles.z;
	}
	

	void FixedUpdate () {


        var up = (Targets[0].yAx - Targets[1].yAx) * 0.5f;

        if(Vector2.Dot(Targets[0].yAx, Targets[1].yAx) <0.95f ) {
            DesRot = Mathf.Atan2(-up.x, up.y) * Mathf.Rad2Deg;

          //  Debug.Log("dot  " + Vector2.Dot(Targets[0].yAx, Targets[1].yAx));
        } else {
            var r1 =  Mathf.Atan2(-Targets[0].yAx.x, Targets[0].yAx.y) * Mathf.Rad2Deg;
            var r2 =  Mathf.Atan2(Targets[1].yAx.x, -Targets[1].yAx.y) * Mathf.Rad2Deg;
            var r3 = Mathf.Atan2(-Targets[0].yAx.y, -Targets[0].yAx.x) * Mathf.Rad2Deg;
            var r4 = Mathf.Atan2(Targets[1].yAx.y, Targets[1].yAx.x) * Mathf.Rad2Deg;
            
            var d1 =  Mathf.Abs(Mathf.DeltaAngle(r1, DesRot)); 
            var d2 =  Mathf.Abs(Mathf.DeltaAngle(r2, DesRot)); 
            var d3 =  Mathf.Abs(Mathf.DeltaAngle(r3, DesRot)); 
            var d4 =  Mathf.Abs(Mathf.DeltaAngle(r4, DesRot)); 

            if( d1 > d3 ) { d1 = d3; r1 = r3; }
            if( d2 > d4 ) { d2 = d4; r2 = r4; }
            //  Debug.Log("dot  " + Vector2.Dot(Targets[0].yAx, Targets[1].yAx) + "  dr " + DesRot + "  r1 " + r1 + "  r2 " + r2 + "  f1 " + Mathf.DeltaAngle(r1, DesRot) + "  f2 " + Mathf.DeltaAngle(r2, DesRot));
            
            
            DesRot = (d1<d2) ? r1 : r2;
        }
        var angs = Trnsfrm.localEulerAngles;
        Rot = Mathf.MoveTowardsAngle(Rot, DesRot, Time.deltaTime * RotSpeed);

        angs.z = Mathf.LerpAngle(angs.z, Rot, Time.deltaTime * RotSmooth);
        Trnsfrm.localEulerAngles = angs;            


        Vector2 p1 = (Vector2)Targets[0].Trnsfrm.position, p2 = (Vector2)Targets[1].Trnsfrm.position;

        p1 = Trnsfrm.InverseTransformPoint(p1);
        p2 = Trnsfrm.InverseTransformPoint(p2);

        Vector2 trv = new Vector2(PlayerRadius, PlayerRadius);

        Vector2 bl = p1, tr = p1, diffV = p1 - p2;
      //  float diff = Mathf.Max(Mathf.Abs(diffV.x / Cam.aspect), Mathf.Abs(diffV.y));

       /* if(TrackOther)
            TrackOther = diff < (MaxZoom - PlayerRadius) * 2.0f * BreakMod;
        else
            TrackOther = diff < (MaxZoom - PlayerRadius) * 2.0f * BreakMod * 0.75;

        if(TrackOther) { */
            bl -= trv; tr += trv;
          //  trv *= 0.4f;

            bl.x = Mathf.Min(bl.x, p2.x - trv.x);
            bl.y = Mathf.Min(bl.y, p2.y - trv.y);
            tr.x = Mathf.Max(tr.x, p2.x + trv.x);
            tr.y = Mathf.Max(tr.y, p2.y + trv.y);
            // if(c.x > o.x) bl.x = o.x - trv.x; else tr.x = o.x - trv.x; ;
            //if(c.y > o.y) bl.y = o.y - trv.y; else tr.y = o.y + trv.x;

          //  bl.x = Mathf.Max(bl.x, mnX); bl.y = Mathf.Max(bl.y, mnY);
        //    tr.x = Mathf.Min(tr.x, mxX); tr.y = Mathf.Min(tr.y, mxY);
    //    }
        // bl.x = mnX; tr.x = mxX;

        //Debug.Log("bl.x, mnX  " + bl.x + "    " + tr.x + "    " + mnX + "    " + mxX);


            Vector2 size = (tr - bl);// *0.5f;


        //!ortho zoom
      //  size.x /= Cam.aspect;
      //  DesZ = Mathf.Clamp(Mathf.Max(size.x, size.y), MinZoom, MaxZoom);
       // Cam.orthographicSize = Mathf.Lerp(Cam.orthographicSize, DesZ, ZoomSpeed * Time.deltaTime);


       // float effsize = Mathf.Tan(Cam.fieldOfView *Mathf.Deg2Rad *0.5f ) * -Trnsfrm.position.z *2.0f;
  
      //  Debug.Log("effSize  " + effsize);
      //  (GameObject.Find("Cube") as GameObject).transform.localScale = new Vector3(effsize * Cam.aspect, effsize, 1);



        float hHght = Mathf.Max(size.x / Cam.aspect, size.y);
        float hWid = hHght * Cam.aspect;


        DesZ = -hHght / ( Mathf.Tan(Cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * 2.0f);
        //Mathf.Clamp(Mathf.Max(size.x, size.y), MinZoom, MaxZoom);
        var z = Mathf.Lerp(Trnsfrm.position.z, DesZ, ZoomSpeed * Time.deltaTime);
        /*
        mnX += hWid; mxX -= hWid;
        mnY += hHght; mxY -= hHght;
        if(mnX > mxX) mnX = mxX = (mnX + mxX) * 0.5f;
        if(mnY > mxY) mnY = mxY = (mnY + mxY) * 0.5f;

        DesP = (bl + tr) * 0.5f;
        DesP.x = Mathf.Clamp(DesP.x, mnX, mxX);
        DesP.y = Mathf.Clamp(DesP.y, mnY, mxY);
        */
        DesP = (bl + tr) * 0.5f;

        DesP = Trnsfrm.TransformPoint(DesP);

        var p = Vector2.Lerp((Vector2)Trnsfrm.position, DesP, hHght * PanSpeed * Time.deltaTime);
        Trnsfrm.position = new Vector3(p.x, p.y, DesZ);


	}
}
