using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.UI;

using Valve.VR;

using Valve.VR.InteractionSystem;



namespace Valve.VR.InteractionSystem.Sample

{

    public class Draw : MonoBehaviour

    {

        //Create variables for the SteamVR actions

       

       public SteamVR_Action_Boolean triggerPress;





        //Call Rigidbody objects to be created.

        //public Rigidbody paintObject;



        //line drawing

        private LineRenderer currLine;

        //object to be tracked: Hand position

        public GameObject trackedObject;

        //line index

        private int numClicks = 0;



     



        private void Update()

        {

            //get updated position of the trecked object and put it into a value

            Vector3 trackedObjectPosition = trackedObject.transform.position;





            //when trigger pressed create new line renderer game object.

            if (triggerPress.GetStateDown(SteamVR_Input_Sources.LeftHand))

            {

                

                GameObject go = new GameObject();

                currLine = go.AddComponent<LineRenderer>();

                //whent trigger down index numClicks = 0

                numClicks = 0;

                //set size of new line

                currLine.startWidth = 0.1f;

                currLine.endWidth = 0.1f;

              

            }

            //I used GetState instead of GetTouch(couldn't find it). SteamVR2? Changed from Touch to State?

            else if (triggerPress.GetState(SteamVR_Input_Sources.LeftHand))

            {

                //trying to increment the number of positions size of the line renderer.

            Vector3[] positions = new Vector3[numClicks];



                currLine.positionCount = positions.Length +1;

               

              

                //setposition(index, tracked object position). Keep creating new "index" while the trigger is pressed.Add the tracked object position Vector 3 value to each index.

                //To avoid "index out of bounds": must add vertex to the lineRender (currLine).

                currLine.SetPosition(positions.Length, trackedObjectPosition);

                numClicks++;

                

               

                Debug.Log("index = " + numClicks + " "+ "trackedObject position =" + trackedObjectPosition);

            }

         

          

           

        }





    }



}
