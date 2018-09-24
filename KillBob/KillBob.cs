using System;
using UnityEngine;
using System.Collections.Generic;

namespace KillBob
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class KillWheelBob : MonoBehaviour
    {
        public void Start()
        {
        }


        public void Update()
        {
            List<ModuleWheels.ModuleWheelSuspension> myList = FlightGlobals.ActiveVessel.FindPartModulesImplementing<ModuleWheels.ModuleWheelSuspension>();
            foreach (ModuleWheels.ModuleWheelSuspension ms in myList)
            {
                //trackWheelSuspension(ms);
                ms.useAutoBoost = false;
            }
        }




    }
}
