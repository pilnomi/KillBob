using KSP.Localization;
using ModuleWheels;
using UnityEngine;

namespace KillBob
{
    class KillBobCustomSuspension : ModuleWheelSubmodule
    {
	    [KSPField]
        public string suspensionTransformName = string.Empty;

        [KSPField]
        public float suspensionOffset;

        [KSPField]
        public float suspensionDistance;

        [SerializeField]
        private float spring;

        [SerializeField]
        private float damper;

        [KSPField(isPersistant = true)]
        public float springFriction = .2f;

        [KSPField]
        public float targetPosition = 1f;

        [KSPField]
        public float springRatio = 50f;

        [UI_FloatRange(scene = UI_Scene.All, minValue = 0.05f, maxValue = 2f, stepIncrement = 0.05f, controlEnabled = true, affectSymCounterparts = UI_Scene.All)]
        [KSPField(isPersistant = true, guiName = "#autoLOC_6001469", guiActive = true, guiActiveEditor = true, guiFormat = "F2")] //looks wrong? guiUnits = "F2"
        public float springTweakable = 1f;

        [KSPField]
        public float damperRatio = 1f;

        [UI_FloatRange(scene = UI_Scene.All, minValue = 0.05f, maxValue = 2f, stepIncrement = 0.05f, controlEnabled = true, affectSymCounterparts = UI_Scene.All)]
        [KSPField(isPersistant = true, guiName = "#autoLOC_6001470", guiActive = true, guiActiveEditor = true, guiFormat = "F2")] //looks wrong? guiUnits = "F2"
        public float damperTweakable = 1f;

        [KSPField]
        public float boostRatio;

        [KSPField]
        public bool suppressModuleInfo;

        [KSPField(isPersistant = true)]
        public Vector3 suspensionPos = -Vector3.one;

        [SerializeField]
        private float damperFudge;

        [SerializeField]
        private float boost;

        [SerializeField]
        private float vesselMass;

        private Transform suspensionTransform;

        [KSPField(isPersistant = true)]
        public float autoBoost;

        [KSPField(isPersistant = true)]
        public bool useAutoBoost = false;

        [KSPField]
        public string suspensionColliderName = string.Empty;

        private Collider suspensionCollider;

        [SerializeField]
        private float springClampMax = 5000f;

        [SerializeField]
        private float damperClampMax = 5000f;

        public override void OnStart(StartState state)
        {
            //Debug.Log("custom suspension onstart");
            base.OnStart(state);
            if (!string.IsNullOrEmpty(suspensionTransformName))
            {
                suspensionTransform = base.part.FindModelTransform(suspensionTransformName);
                if (suspensionTransform == null)
                {
                    Debug.LogError("[ModuleWheelBase]: No transform called " + suspensionTransformName + " found in " + base.part.partName + " hierarchy", base.gameObject);
                }
                if (suspensionPos != -Vector3.one)
                {
                    suspensionTransform.localPosition = suspensionPos;
                }
            }
            if (!string.IsNullOrEmpty(suspensionColliderName))
            {
                Transform transform = base.part.FindModelTransform(suspensionColliderName);
                if (transform != null)
                {
                    suspensionCollider = transform.GetComponent<Collider>();
                    suspensionCollider.enabled = false;
                }
            }
            GameEvents.onVesselPartCountChanged.Add(onVesselPartCountChanged);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            GameEvents.onVesselPartCountChanged.Remove(onVesselPartCountChanged);
        }

        private void onVesselPartCountChanged(Vessel changedVessel)
        {
            if (baseSetup && changedVessel.id == base.vessel.id)
            {
                wheel.wheelCollider.springRate = 0f;
            }
        }

        protected override void OnWheelSetup()
        {
            if (suspensionTransform != null)
            {
                wheel.wheelCollider.suspensionTransform = suspensionTransform;
            }
            wheel.wheelCollider.updateSuspension = !wheelBase.InopSystems.HasType(WheelSubsystem.SystemTypes.Suspension);
            wheel.wheelCollider.suspensionDistance = suspensionDistance * base.part.rescaleFactor;
            wheel.wheelCollider.suspensionAnchor = targetPosition;
            wheel.wheelCollider.suspensionOffset = suspensionOffset * base.part.rescaleFactor;
            wheel.wheelCollider.springRate = spring;
            wheel.wheelCollider.damperRate = damper;
            ResetSuspensionCollider(3f);
        }

        private void FixedUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight && baseSetup && !base.part.packed)
            {
                vesselMass = (float)base.vessel.totalMass;
                SuspensionSpringUpdate(vesselMass);
            }
        }

        protected void LateUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (!baseSetup || base.part.packed)
                {
                    if (suspensionTransform != null && suspensionPos != -Vector3.one)
                        suspensionTransform.localPosition = suspensionPos;
                }
                else if (suspensionTransform != null)
                    suspensionPos = suspensionTransform.localPosition;
            }
        }

        private float BoostCurve(float b, float f)
        {
            return Mathf.Clamp(1f / Mathf.Abs(1f - 2f / Mathf.Pow(Mathf.Clamp(b, 0f, 2f), Mathf.Clamp(f, 0.01f, 1f))), 0.01f, 1E+07f);
        }

        private void SuspensionSpringUpdate(float sprungMass)
        {
            springClampMax = Mathf.Lerp(5000f, 10000f, Mathf.InverseLerp(50f, 200f, sprungMass));
            damperClampMax = Mathf.Lerp(5000f, 10000f, Mathf.InverseLerp(50f, 200f, sprungMass));
            UpdateAutoBoost(wheel.currentState.suspensionCompression);
            boost = 1f + Mathf.Clamp(wheel.currentState.suspensionCompression * 2f, -1f, 1f);
            float num = sprungMass * (float)base.vessel.mainBody.GeeASL * (float)base.vessel.gravityMultiplier;
            damperFudge = Mathf.Lerp(1f, 15f, Mathf.InverseLerp(10f, 100f, num));
            spring = Mathf.Clamp(springTweakable * springRatio * num * BoostCurve(boost, boostRatio + autoBoost), 0.01f, springClampMax);
            damper = Mathf.Sqrt(Mathf.Clamp(spring * damperRatio * damperFudge, 0.01f, damperClampMax)) * damperTweakable;
            damper = Mathf.Round(damper);

            //adding "spring friction" to stop perpetual motion when very little damper
            if (damper < spring * springFriction) damper = spring * springFriction;

            spring = Mathf.Round(spring);
            //Debug.Log(string.Format("spring {0}, springrate {1}, damper {2}, damperrate{3}", spring, wheel.wheelCollider.springRate, damper, wheel.wheelCollider.damperRate));
            //spring 4, springrate 3 (legs 4 for wheels), damper 20 damperrate 0
            if (wheel.wheelCollider.springRate < spring)
            {
                if (spring - wheel.wheelCollider.springRate > 10f)
                {
                    wheel.wheelCollider.springRate = spring;
                }
            }
            else if (wheel.wheelCollider.springRate > spring * 1.5f)
            {
                wheel.wheelCollider.springRate = spring;
            }
            if (Mathf.Abs(damper - wheel.wheelCollider.damperRate) > 2f)
            {
                wheel.wheelCollider.damperRate = damper;

            }
            //Debug.Log(string.Format("use autoboost {0}", useAutoBoost));
            //Debug.Log(string.Format("spring: {0}, damper {1}, springrate {2}, damperrate {3}, damperfudge {4}, boost {5}, clamp {6} {7}, dampertweakable {8}", spring, damper, wheel.wheelCollider.springRate, wheel.wheelCollider.damperRate, damperFudge, boost, springClampMax, damperClampMax, damperTweakable));
            /*
             * legs reasonable numbers
             * [LOG 15:22:47.216] spring: 14, damper 4, springrate 14, damperrate 4, damperfudge 1, boost 1.260681, clamp 5000 5000, dampertweakable 1

             */
        }

        protected void UpdateAutoBoost(float st)
        {
            if (!useAutoBoost)
            {
                autoBoost = 0f;
            }
            else if (st > 0.8f)
            {
                autoBoost = Mathf.Min(0.85f, autoBoost + 0.5f * Time.fixedDeltaTime);
            }
            else if (wheel.IsGrounded && wheel.currentState.suspensionCompression < 0.4f)
            {
                autoBoost = Mathf.Max(0f, autoBoost - 0.2f * Time.fixedDeltaTime);
            }
        }


        public override string OnGatherInfo()
        {
            if (suppressModuleInfo)
            {
                return null;
            }
            return Localizer.Format("#autoLOC_248720", suspensionDistance.ToString("0.0#"));
        }

        protected override void OnSubsystemsModified(WheelSubsystems s)
        {
            if (s == wheelBase.InopSystems)
            {
                wheel.wheelCollider.updateSuspension = !s.HasType(WheelSubsystem.SystemTypes.Suspension);
            }
        }

        protected void ResetSuspensionCollider(float delay)
        {
            if (suspensionCollider != null)
            {
                StartCoroutine(CallbackUtil.DelayedCallback(delay, delegate
                {
                    suspensionCollider.enabled = true;
                    base.part.ScheduleSetCollisionIgnores();
                }));
            }
        }
    }
}
