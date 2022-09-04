using UnityEngine;
using Landfall.TABS;
using System.Collections.Generic;

namespace ForGlory {
    public class DismemberablePart : MonoBehaviour {
        void Start() {
            unit = transform.root.GetComponent<Unit>();
        }

        public void DismemberPart()
        {
            if (!FGMain.DecapitationEnabled)
            {
                return;
            }
            if (transform == unit.data.leftArm) {
                unit.data.leftHand.gameObject.SetActive(false);
                for (int i = 0; i < unit.data.leftHand.childCount; i++) {
                    unit.data.leftHand.GetChild(i).localScale = Vector3.zero;
                    unit.data.leftHand.SetParent(transform);
                    unit.data.leftHand.localPosition = Vector3.zero;
                }
                if (unit.holdingHandler && unit.holdingHandler.leftObject && unit.holdingHandler.rightHandActivity != HoldingHandler.HandActivity.HoldingLeftObject) {
                    unit.WeaponHandler.fistRefernce = null;
                    unit.holdingHandler.leftObject.GetComponent<Rigidbody>().isKinematic = false;
                    if (unit.holdingHandler.leftObject.GetComponentInChildren<ConditionalEvent>())
                    {
                        unit.holdingHandler.leftObject.GetComponentInChildren<ConditionalEvent>().enabled = false;
                    }
                    unit.holdingHandler.LetGoOfWeapon(unit.holdingHandler.leftObject.gameObject);
                }
                else if (unit.holdingHandler && unit.holdingHandler.leftObject && unit.holdingHandler.rightHandActivity == HoldingHandler.HandActivity.HoldingLeftObject) {
                    unit.WeaponHandler.fistRefernce = null;
                    unit.holdingHandler.leftObject.GetComponent<Rigidbody>().isKinematic = false;
                    if (unit.holdingHandler.leftObject.GetComponentInChildren<ConditionalEvent>())
                    {
                        unit.holdingHandler.leftObject.GetComponentInChildren<ConditionalEvent>().enabled = false;
                    }
                    Destroy((ConfigurableJoint)unit.holdingHandler.GetField("leftHandJoint"));
                }
            }
            if (transform == unit.data.rightArm) {
                unit.data.rightHand.gameObject.SetActive(false);
                for (int i = 0; i < unit.data.rightHand.childCount; i++) {
                    unit.data.rightHand.GetChild(i).localScale = Vector3.zero;
                    unit.data.rightHand.SetParent(transform);
                    unit.data.rightHand.localPosition = Vector3.zero;
                }
                if (unit.holdingHandler && unit.holdingHandler.rightObject && unit.holdingHandler.leftHandActivity != HoldingHandler.HandActivity.HoldingRightObject) {
                    unit.WeaponHandler.fistRefernce = null;
                    unit.holdingHandler.rightObject.GetComponent<Rigidbody>().isKinematic = false;
                    if (unit.holdingHandler.rightObject.GetComponentInChildren<ConditionalEvent>())
                    {
                        unit.holdingHandler.rightObject.GetComponentInChildren<ConditionalEvent>().enabled = false;
                    }
                    unit.holdingHandler.LetGoOfWeapon(unit.holdingHandler.rightObject.gameObject);
                }
                else if (unit.holdingHandler && unit.holdingHandler.rightObject && unit.holdingHandler.leftHandActivity == HoldingHandler.HandActivity.HoldingRightObject) {
                    unit.WeaponHandler.fistRefernce = null;
                    unit.holdingHandler.rightObject.GetComponent<Rigidbody>().isKinematic = false;
                    if (unit.holdingHandler.rightObject.GetComponentInChildren<ConditionalEvent>())
                    {
                        unit.holdingHandler.rightObject.GetComponentInChildren<ConditionalEvent>().enabled = false;
                    }
                    Destroy((ConfigurableJoint)unit.holdingHandler.GetField("rightHandJoint"));
                }
            }
            else if (transform == unit.data.legLeft) {
                unit.data.footLeft.gameObject.SetActive(false);
                for (int i = 0; i < unit.data.footLeft.childCount; i++) {
                    unit.data.footLeft.GetChild(i).localScale = Vector3.zero;
                    unit.data.footLeft.SetParent(transform);
                    unit.data.footLeft.localPosition = Vector3.zero;
                }
            }
            else if (transform == unit.data.legRight) {
                unit.data.footRight.gameObject.SetActive(false);
                for (int i = 0; i < unit.data.footRight.childCount; i++) {
                    unit.data.footRight.GetChild(i).localScale = Vector3.zero;
                    unit.data.footRight.SetParent(transform);
                    unit.data.footRight.localPosition = Vector3.zero;
                }
            }
            else if (transform == unit.data.head && FGMain.KillUnitsAfterDecapitateEnabled)
            {
                unit.data.healthHandler.Die();
            }
            if (unit.data.GetComponent<StandingHandler>()) { unit.data.GetComponent<StandingHandler>().selfOffset += 0.8f; }
            if (!(transform.root.name.Contains("Stiffy") && !FGMain.SkeletonBloodEnabled)) {
                var blood = Instantiate(FGMain.dismember.LoadAsset<GameObject>("E_BloodDismember"), transform.position, transform.rotation, unit.data.mainRig.transform);
                if (unit.GetComponent<ParticleTeamColor>()) {
			    	blood.GetComponent<ParticleTeamColor>().redColor = unit.GetComponent<ParticleTeamColor>().redColor;
			    	blood.GetComponent<ParticleTeamColor>().blueColor = FGMain.TeamColorEnabled ? unit.GetComponent<ParticleTeamColor>().blueColor : unit.GetComponent<ParticleTeamColor>().redColor;
			    }
                var em = blood.GetComponent<ParticleSystem>().emission;
                em.burstCount *= (int)FGMain.BloodIntensity;
                var inherit = blood.GetComponent<ParticleSystem>().inheritVelocity;
                inherit.curveMultiplier *= FGMain.BloodIntensity;
                var scale = blood.GetComponent<ParticleSystem>().main;
                scale.startSizeMultiplier *= FGMain.BloodSize;
            }
            unit.data.mainRig.mass += GetComponent<Rigidbody>().mass;
            for (int i = 0; i < transform.childCount; i++) {
                transform.GetChild(i).localScale = Vector3.zero;
                transform.SetParent(unit.data.mainRig.transform);
            }
            gameObject.SetActive(false);
            if (!unit.data.leftArm.gameObject.activeSelf && !unit.data.rightArm.gameObject.activeSelf)
            {
                if (unit.holdingHandler.leftObject && unit.holdingHandler.leftObject.GetComponentInChildren<ConditionalEvent>())
                {
                    unit.holdingHandler.leftObject.GetComponentInChildren<ConditionalEvent>().enabled = false;
                }
                if (unit.holdingHandler.rightObject && unit.holdingHandler.rightObject.GetComponentInChildren<ConditionalEvent>())
                {
                    unit.holdingHandler.rightObject.GetComponentInChildren<ConditionalEvent>().enabled = false;
                }
                unit.holdingHandler.LetGoOfAll();
            }
            dismembered = true;
        }

        private Unit unit;

        public bool dismembered;
    }
}
