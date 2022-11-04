using System.Reflection;
using System;
using UnityEngine;
using HarmonyLib;
using Landfall.TABS;
using System.Collections.Generic;
using System.Collections;
using TFBGames;
using System.Linq;

namespace ForGlory {

	[HarmonyPatch(typeof(ProjectileHit), "DoGameplayEffects")]
	class TopTenReasonsWhyIAmShinjiIkari {

		[HarmonyPrefix]
		public static void Prefix(ProjectileHit __instance, HitData hit, ref TeamHolder ___teamHolder, ref MoveTransform ___move, ref float ___counter, ref List<Unit> ___unitsHit, ref ProjectileStick ___stick, ref Unit ___ownUnit, float m = 1f, Unit unit = null) {

			if ((bool)hit.rigidbody)
			{
				__instance.AddForceToTarget(hit.point, hit.rigidbody, m);
			}
			for (int i = 0; i < __instance.objectsToSpawn.Length; i++)
			{
				__instance.StartCoroutine((IEnumerator)__instance.InvokeMethod("SpawnAfterDelay", new object[] { __instance.objectsToSpawn[i], hit }));
			}
			if (__instance.onlyCallHitEventsOnRigsLessThan == 0f || ((bool)hit.rigidbody && hit.rigidbody.mass < __instance.onlyCallHitEventsOnRigsLessThan))
			{
				for (int j = 0; j < __instance.hitEvents.Length; j++)
				{
					__instance.StartCoroutine((IEnumerator)__instance.InvokeMethod("DelayHitEvent", new object[] { __instance.hitEvents[j].eventDelay, __instance.hitEvents[j].hitEvent }));
				}
			}
			if ((bool)unit)
			{
				for (int k = 0; k < __instance.onUnitsEvent.Length; k++)
				{
					__instance.StartCoroutine((IEnumerator)__instance.InvokeMethod("DelayHitEvent", new object[] { __instance.onUnitsEvent[k].eventDelay, __instance.onUnitsEvent[k].hitEvent }));
				}
			}
			if ((bool)___stick && (!hit.rigidbody || ___stick.minWeight < hit.rigidbody.mass))
			{
				__instance.DisableProjectile();
				___stick.Stick(hit.transform, __instance.transform.forward + UnityEngine.Random.insideUnitSphere * 0.2f, hit.point, hit.rigidbody ? hit.rigidbody : null);
			}
			ServiceLocator.GetService<SoundPlayer>().PlaySoundEffect(__instance.SoundRef, 1f, __instance.transform.position, SoundEffectVariations.GetMaterialType(hit.transform.gameObject, hit.rigidbody));
			Damagable componentInParent = hit.transform.GetComponentInParent<Damagable>();
			if ((bool)componentInParent && (bool)hit.rigidbody)
			{
				if ((bool)___teamHolder && (bool)___teamHolder.spawner)
				{
					___ownUnit = ___teamHolder.spawner.GetComponentInParent<Unit>();
				}
				float num = 1f;
				DamageMultiplier component = hit.rigidbody.GetComponent<DamageMultiplier>();
				if ((bool)component)
				{
					num = component.multiplier;
				}
				if (unit && unit.unitBlueprint && ___ownUnit && unit.unitType == Unit.UnitType.Meat && !unit.data.Dead) {
					if (__instance.damage > 5f && !(unit.name.Contains("Stiffy") && !FGMain.SkeletonBloodEnabled)) {
						var blood = UnityEngine.Object.Instantiate(FGMain.dismember.LoadAsset<GameObject>("E_BloodDamage"), hit.point, Quaternion.identity, hit.rigidbody.transform);
						if (unit.GetComponent<ParticleTeamColor>()) {
							blood.GetComponent<ParticleTeamColor>().redColor = unit.GetComponent<ParticleTeamColor>().redColor;
							blood.GetComponent<ParticleTeamColor>().blueColor = FGMain.TeamColorEnabled ? unit.GetComponent<ParticleTeamColor>().blueColor : unit.GetComponent<ParticleTeamColor>().redColor;
						}
						var em = blood.GetComponent<ParticleSystem>().emission;
						em.burstCount = (int)__instance.damage / (int)1.25 * (int)FGMain.BloodIntensity;
						var inherit = blood.GetComponent<ParticleSystem>().inheritVelocity;
						inherit.curveMultiplier *= FGMain.BloodIntensity;
						var scale = blood.GetComponent<ParticleSystem>().main;
						scale.startSizeMultiplier *= FGMain.BloodSize;
					}
					if (componentInParent.GetComponentInChildren<DismemberablePart>() && unit.Team != ___ownUnit.Team && __instance.damage >= unit.data.health * 0.2f) {
						var partsOrdered = (
							from DismemberablePart part
								in componentInParent.GetComponentsInChildren<DismemberablePart>()
							where !part.dismembered
							orderby Vector3.Distance(hit.point, part.transform.position)
							select part).ToArray();
						if (partsOrdered.Length > 0)
						{
							partsOrdered[0].DismemberPart();
						}
					}
				}
				componentInParent.TakeDamage(__instance.damage * num * m, __instance.transform.forward, ___ownUnit);
			}
			if (__instance.setSpeedOnHit > 0f && (bool)___move)
			{
				___move.velocity = Vector3.ClampMagnitude(___move.velocity, __instance.setSpeedOnHit);
			}
		}
	}
}