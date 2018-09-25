using System;
using UnityEngine;
using System.Collections.Generic;
using KSP.UI.Screens;

namespace KillBob
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    class KillDefaultTweakables: MonoBehaviour
    {
        public void Start()
        {
        }
        public void Awake()
        {
        }

        public void killtweakables()
        {
            
            for (int p = 0; p < EditorLogic.SortedShipList.Count; p++)
            {
                List<ModuleWheels.ModuleWheelSuspension> myParts = EditorLogic.SortedShipList[p].FindModulesImplementing<ModuleWheels.ModuleWheelSuspension>();
                foreach (ModuleWheels.ModuleWheelSuspension mws in myParts)
                {
                    for (int i = 0; i < mws.Fields.Count; i++)
                    {
                        mws.Fields[i].advancedTweakable = false;
                        mws.Fields[i].guiActiveEditor = false;
                        mws.Fields[i].guiActive = false;
                    }
                }
            }
        }
        public void Update()
        {
            killtweakables();
        }
    }
}
