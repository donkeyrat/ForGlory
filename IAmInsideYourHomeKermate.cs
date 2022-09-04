using System.Reflection;
using System;
using UnityEngine;
using HarmonyLib;
using Landfall.TABS;
using System.Collections.Generic;
using System.Collections;
using TFBGames;

namespace ForGlory {

	[HarmonyPatch(typeof(ReaperWings), "DoAttack")]
	class IAmInsideYourHomeKermate {

		public class Weenis {
			public static IEnumerator Peefix(ReaperWings __instance, AttackArm attack, Unit targetUnit)
			{
				attack.armState = AttackArm.ArmState.Holding;
				float c = 0f;
				float t = AnimationCurveFunctions.GetAnimLength(__instance.reachCurve);
				if (__instance.swingRef != "")
				{
					ServiceLocator.GetService<SoundPlayer>().PlaySoundEffect(__instance.swingRef, 1f, __instance.transform.position, SoundEffectVariations.MaterialType.Default, null, 1f);
				}
				while (c < t && targetUnit && targetUnit.data.mainRig)
				{
					Vector3 a = targetUnit.data.mainRig.position - attack.restPosObj.transform.position;
					attack.targetPos = attack.restPosObj.transform.position + a * __instance.reachCurve.Evaluate(c);
					c += Time.deltaTime;
					yield return null;
				}
				c = 0f;
				t = AnimationCurveFunctions.GetAnimLength(__instance.goBackToHoldCurve);
				attack.heldUnit = targetUnit;
				attack.counter = 0f;
				if (targetUnit && __instance.hitRef != "")
				{
					ServiceLocator.GetService<SoundPlayer>().PlaySoundEffect(__instance.hitRef, 1f, __instance.transform.position, SoundEffectVariations.MaterialType.Default, null, 1f);
				}
				if (targetUnit && targetUnit.unitType == Unit.UnitType.Meat)
				{

					var componentInParent = targetUnit.data;
					if (!(componentInParent.unit.name.Contains("Stiffy") && !FGMain.SkeletonBloodEnabled)) {
						var blood = UnityEngine.Object.Instantiate(FGMain.dismember.LoadAsset<GameObject>("E_BloodDamage"), componentInParent.mainRig.position, Quaternion.identity, componentInParent.mainRig.transform);
						if (componentInParent.unit.GetComponent<ParticleTeamColor>()) {
							blood.GetComponent<ParticleTeamColor>().redColor = componentInParent.unit.GetComponent<ParticleTeamColor>().redColor;
							blood.GetComponent<ParticleTeamColor>().blueColor = FGMain.TeamColorEnabled ? componentInParent.unit.GetComponent<ParticleTeamColor>().blueColor : componentInParent.unit.GetComponent<ParticleTeamColor>().redColor;
						}
						var inherit = blood.GetComponent<ParticleSystem>().inheritVelocity;
						inherit.curveMultiplier *= FGMain.BloodIntensity;
						var scale = blood.GetComponent<ParticleSystem>().main;
						scale.startSizeMultiplier *= FGMain.BloodSize;
					}
					if (componentInParent.GetComponentInChildren<DismemberablePart>()) {

						var randomPart = componentInParent.GetComponentsInChildren<DismemberablePart>()[UnityEngine.Random.Range(0, componentInParent.GetComponentsInChildren<DismemberablePart>().Length - 1)];
						randomPart.DismemberPart();
					}
				}
				while (c < t && targetUnit && targetUnit.data.mainRig)
				{
					Vector3 a2 = targetUnit.data.mainRig.position - attack.restPosObj.transform.position;
					attack.targetPos = attack.restPosObj.transform.position + a2 * __instance.goBackToHoldCurve.Evaluate(c);
					c += Time.deltaTime;
					__instance.SetField("followMainRigAmount", __instance.goBackToHoldCurveFollowMainRigAmount.Evaluate(c));
					yield return null;
				}
				attack.armState = AttackArm.ArmState.Free;
				yield break;
			}
		}

		[HarmonyPrefix]
		public static bool Prefix(ReaperWings __instance, ref IEnumerator __result, AttackArm attack, Unit targetUnit)
		{
			__result = Weenis.Peefix(__instance, attack, targetUnit);
			return false;
		}
	}
}