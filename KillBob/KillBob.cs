using System;
using UnityEngine;
using System.Collections.Generic;

namespace KillBob
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KillWheelBob : MonoBehaviour
    {
        public TimeSpan trackOverTime = new TimeSpan(0, 0, 5);
        public class suspensionTracking
        {
            public int instanceID;
            public decimal minPos, maxPos;
            public DateTime lastDataPoint;
            public float originalDamper, originalSpringRatio;
        }
        public List<suspensionTracking> trackedObjects;
        public Vessel ActiveVessel;

        public void Start()
        {
            Debug.Log("killbob start");
            trackedObjects = new List<suspensionTracking>();
            ActiveVessel = FlightGlobals.ActiveVessel;
        }


        public void OnDestroy()
        {
            /*
            List<ModuleWheels.ModuleWheelSuspension> myList = ActiveVessel.FindPartModulesImplementing<ModuleWheels.ModuleWheelSuspension>();
            foreach (ModuleWheels.ModuleWheelSuspension ms in myList)
            {
                suspensionTracking st = findInTrackedObjects(ms);
                ms.damperRatio = st.originalDamper;
                ms.springRatio = st.originalSpringRatio;
            }
            */
        }

        public void Awake()
        {
            Debug.Log("killbob awake");
        }

        public void OnVesselChange(Vessel v)
        {
            ActiveVessel = FlightGlobals.ActiveVessel;
        }

        public void Update()
        {
            List<ModuleWheels.ModuleWheelSuspension> myList = ActiveVessel.FindPartModulesImplementing<ModuleWheels.ModuleWheelSuspension>();
            foreach (ModuleWheels.ModuleWheelSuspension ms in myList)
            {
                //trackWheelSuspension(ms);
                ms.useAutoBoost = false;
            }
        }
        /*
         * -- other stuff I did before just disabling autoboost
         * 
        public void trackWheelSuspension(ModuleWheels.ModuleWheelSuspension ms)
        {
            if (ms.suspensionPos.y < 0) return;
            suspensionTracking st = findInTrackedObjects(ms);


            if ((decimal)ms.suspensionPos.y < st.minPos)
            {
                //Debug.Log(string.Format("updating minpos {0}, {1}, {2}, {3}", ms.suspensionPos.y, st.minPos, st.maxPos, st.instanceID));
                st.minPos = (decimal)ms.suspensionPos.y;
                st.lastDataPoint = DateTime.Now;
            }
            else if ((decimal)ms.suspensionPos.y > st.maxPos)
            {
                //Debug.Log(string.Format("updating maxpos {0}, {1}, {2}, {3}", ms.suspensionPos.y, st.minPos, st.maxPos, st.instanceID));
                st.maxPos = (decimal)ms.suspensionPos.y;
                st.lastDataPoint = DateTime.Now;
                //Debug.Log(st.maxPos);
            }

            //Debug.Log(string.Format("{0}, {1}, {2}, {3}", trackOverTime, st.lastDataPoint, st.lastDataPoint.Add(trackOverTime), DateTime.Now));

            if (st.lastDataPoint.Add(trackOverTime) < DateTime.Now)
            {
                decimal bobthresh = .000015m;
                float nerfamount = .1f; // 1 = no nerf
                Debug.Log(string.Format("checking for wheel bob {0}, {1}", Math.Abs(st.maxPos) - Math.Abs(st.minPos), Math.Abs(st.maxPos * bobthresh)));
                //if min and max don't change significantly over time then wheel bob is probably occuring
                if (Math.Abs(st.maxPos) - Math.Abs(st.minPos) > 0m  &&
                        Math.Abs(st.maxPos - st.minPos) > Math.Abs(st.maxPos * bobthresh))
                {
                    Debug.Log(string.Format("wheel bob detected {0}, {1}, {2}, {3}, {4}, {5}", st.minPos, st.maxPos, ms.springRatio, ms.damperRatio, ms.useAutoBoost, ms.autoBoost));
                    //ms.damperRatio += ms.damperRatio * nerfamount;
                    ms.useAutoBoost = false;
                    //ms.suspensionPos.y = ms.suspensionPos.y * nerfamount;
                    //ms.transform.localPosition = ms.suspensionPos;
                    //ms.OnUpdate();
                }
                st.minPos = (decimal)ms.suspensionPos.y;
                st.maxPos = (decimal)ms.suspensionPos.y;
                st.lastDataPoint = DateTime.Now;
            }
        }

        public suspensionTracking findInTrackedObjects(ModuleWheels.ModuleWheelSuspension ms)
        {
            int instanceID = ms.GetInstanceID();
            foreach (suspensionTracking m in trackedObjects)
            {
                if (m.instanceID == instanceID)
                {
                    //Debug.Log(string.Format("found st: {0}, {1}, {2}", m.instanceID, m.minPos, m.maxPos));
                    return m;
                }
            }
            suspensionTracking st = new suspensionTracking();
            st.instanceID = ms.GetInstanceID();
            st.maxPos = (decimal)ms.suspensionPos.y;
            st.minPos = (decimal)ms.suspensionPos.y;
            st.originalSpringRatio = ms.springRatio;
            st.originalDamper = ms.damperRatio;
            trackedObjects.Add(st);
            return st;

        }
        */
        /*
        public override void OnUpdate()
        {
            base.OnUpdate();
            if (this.suspensionPos == null) return;

            if (minPos == null) minPos = this.suspensionPos;
            if (maxPos == null) maxPos = this.suspensionPos;

            if (lastDataPoint != null)
            {
                if (this.suspensionPos.y < minPos.y )
                {
                    minPos = this.suspensionPos;
                    lastDataPoint = DateTime.Now;
                }
                else if (this.suspensionPos.y > minPos.y )
                {
                    maxPos = this.suspensionPos;
                    lastDataPoint = DateTime.Now;
                }
            }else lastDataPoint = DateTime.Now;

            if (DateTime.Now.Subtract(trackOverTime) < lastDataPoint)
            {
                Debug.Log("checking for wheel bob");
                //if min and max don't change significantly over time then wheel bob is probably occuring
                if (minPos.y != maxPos.y &&
                        Math.Abs(minPos.y - maxPos.y) > Math.Abs(maxPos.y * .1))
                {
                    Debug.Log(string.Format("wheel bob detected {0}, {1}", minPos.y, minPos.x));
                    this.suspensionPos.y = this.suspensionPos.y * .9f; // nerf last spring reaction by 10%

                }


                lastDataPoint = DateTime.Now;
            }
            */



    }
}
