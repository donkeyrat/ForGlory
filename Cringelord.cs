using System.Reflection;
using System;
using UnityEngine;
using HarmonyLib;
using Landfall.TABS;
using System.Collections.Generic;
using System.Linq;

namespace ForGlory
{
	[HarmonyPatch(typeof(CollisionWeapon), "OnCollisionEnter")]
	class Cringelord
	{
		[HarmonyPrefix]
		public static void Prefix(CollisionWeapon __instance, Collision collision, ref Action<Collision, float> ___CollisionAction, ref Rigidbody ___rig, ref DataHandler ___connectedData, ref Holdable ___holdable, ref Unit ___ownUnit, ref float ___sinceLastDamage, ref List<DataHandler> ___hitDatas, ref TeamHolder ___teamHolder, ref Level ___myLevel, ref Action<Collision, float, Vector3> ___DealDamageAction, ref Counter ___counter, ref CollisionWeaponEffect[] ___meleeWeaponEffects)
		{

			if (__instance.onlyCollideWithRigs && !collision.rigidbody)
			{
				return;
			}
			___CollisionAction?.Invoke(collision, 0f);
			if ((bool)__instance.GetComponent<MeleeWeapon>() && !__instance.GetComponent<MeleeWeapon>().canDealDamage)
			{
				return;
			}
			float num = 0f;
			if ((bool)___rig)
			{
				if ((bool)collision.rigidbody)
				{
					_ = collision.rigidbody.mass;
					_ = __instance.enemyMassPow;
					_ = 1f;
					num = collision.impulse.magnitude / (___rig.mass + 10f) * 0.3f;
				}
				else
				{
					num = collision.impulse.magnitude / ___rig.mass * 0.3f;
				}
			}
			num *= __instance.impactMultiplier;
			num = Mathf.Clamp(num, 0f, 2f);
			if (num < 1f)
			{
				return;
			}
			if (!collision.rigidbody)
			{
				__instance.InvokeMethod("DoScreenShake", new object[] { num, collision, 1f });
				if (__instance.callEffectsOn == CollisionWeapon.CallEffectsOn.All || __instance.callEffectsOn == CollisionWeapon.CallEffectsOn.Ground)
				{
					__instance.InvokeMethod("DoCollisionEffects", new object[] { collision.transform, collision });
				}
				if ((bool)__instance.GetComponent<CollisionSound>() && __instance.playSoundWhenHitNonRigidbodies)
				{
					__instance.GetComponent<CollisionSound>().DoEffect(collision.transform, collision, num);
				}
			}
			if (__instance.minVelocity != 0f && (bool)___rig && ___rig.velocity.magnitude < __instance.minVelocity)
			{
				return;
			}
			if (!___connectedData && (bool)___holdable && ___holdable.held)
			{
				___connectedData = ___holdable.holderData;
			}
			if (collision.transform.root == __instance.transform.root || ((bool)___connectedData && ___connectedData.transform.root == collision.transform.root) || !collision.rigidbody || __instance.protectedRigs.Contains(collision.rigidbody) || ___sinceLastDamage < __instance.cooldown)
			{
				return;
			}
			___sinceLastDamage = 0f;
			DataHandler componentInParent = collision.rigidbody.GetComponentInParent<DataHandler>();
			Damagable componentInParent2 = collision.rigidbody.GetComponentInParent<Damagable>();
			if ((bool)componentInParent2)
			{
				if ((bool)componentInParent && __instance.onlyOncePerData)
				{
					if (___hitDatas.Contains(componentInParent))
					{
						return;
					}
					___hitDatas.Add(componentInParent);
				}
				if ((bool)__instance.GetComponent<MeleeWeapon>() && __instance.lastHitHealth == componentInParent2)
				{
					return;
				}
				__instance.lastHitHealth = componentInParent2;
				if ((bool)__instance.GetComponent<CollisionSound>())
				{
					__instance.GetComponent<CollisionSound>().DoEffect(collision.transform, collision, num);
				}
				Unit unit = (___connectedData ? ___connectedData.GetComponentInParent<Unit>() : __instance.GetComponentInParent<Unit>());
				Unit unit2 = null;
				if ((bool)componentInParent)
				{
					unit2 = componentInParent.GetComponentInParent<Unit>();
				}
				if (!___holdable && (bool)unit && unit.data.Dead)
				{
					UnityEngine.Object.Destroy(__instance);
					return;
				}
				float num2 = num;
				if (__instance.staticDamageValue)
				{
					num2 = 1f;
				}
				if ((bool)__instance.GetComponentInChildren<MeleeWeaponMultiplierPoint>() && Vector3.Distance(collision.contacts[0].point, __instance.GetComponentInChildren<MeleeWeaponMultiplierPoint>().transform.position) < __instance.GetComponentInChildren<MeleeWeaponMultiplierPoint>().range)
				{
					num2 *= __instance.GetComponentInChildren<MeleeWeaponMultiplierPoint>().multiplier;
				}
				if ((bool)unit2 && (bool)___teamHolder && unit2.Team == ___teamHolder.team)
				{
					if (((bool)___myLevel && ___myLevel.ignoreTeam) || ((bool)___counter && !(___counter.counter > 0.2f)))
					{
						return;
					}
					num2 *= 0.1f;
				}
				if ((bool)unit && (bool)unit2 && unit.Team == unit2.Team)
				{
					if (((bool)___myLevel && ___myLevel.ignoreTeam) || __instance.ignoreTeamMates)
					{
						return;
					}
					num2 *= __instance.teamDamage;
				}
				Vector3 vector = __instance.transform.forward;
				if ((bool)__instance.GetComponent<MeleeWeapon>())
				{
					vector = __instance.GetComponent<MeleeWeapon>().swingDirection;
				}
				if (__instance.useHitDirection)
				{
					vector = collision.transform.position - __instance.transform.position;
				}
				if ((bool)componentInParent)
				{
					WilhelmPhysicsFunctions.AddForceWithMinWeight(componentInParent.mainRig, (__instance.staticDamageValue ? 5f : Mathf.Sqrt(num * 50f)) * vector * __instance.onImpactForce, ForceMode.Impulse, __instance.massCap);
					WilhelmPhysicsFunctions.AddForceWithMinWeight(collision.rigidbody, (__instance.staticDamageValue ? 5f : Mathf.Sqrt(num * 50f)) * vector * __instance.onImpactForce, ForceMode.Impulse, __instance.massCap);
				}
				if ((bool)__instance.GetComponent<MeleeWeapon>() && collision.rigidbody.mass < __instance.GetComponent<MeleeWeapon>().rigidbody.mass)
				{
					collision.rigidbody.velocity *= 0.6f;
					if ((bool)componentInParent)
					{
						componentInParent.mainRig.velocity *= 0.6f;
					}
				}
				if (!___ownUnit)
				{
					if ((bool)___connectedData && (bool)___connectedData.unit)
					{
						___ownUnit = ___connectedData.unit;
					}
					else if ((bool)___teamHolder && (bool)___teamHolder.spawner)
					{
						___ownUnit = ___teamHolder.spawner.GetComponentInParent<Unit>();
					}
				}
				if (___ownUnit && componentInParent && !componentInParent.Dead && collision != null && collision.collider && collision.collider.attachedRigidbody && componentInParent.unit && componentInParent.unit.unitBlueprint && componentInParent.unit.unitType == Unit.UnitType.Meat) {
					if (__instance.damage > 5f && !(componentInParent.unit.name.Contains("Stiffy") && !FGMain.SkeletonBloodEnabled)) {
						var blood = UnityEngine.Object.Instantiate(FGMain.dismember.LoadAsset<GameObject>("E_BloodDamage"), collision.collider.attachedRigidbody.position, collision.collider.attachedRigidbody.rotation, collision.collider.attachedRigidbody.transform);
						if (componentInParent.unit.GetComponent<ParticleTeamColor>()) {
							blood.GetComponent<ParticleTeamColor>().redColor = componentInParent.unit.GetComponent<ParticleTeamColor>().redColor;
							blood.GetComponent<ParticleTeamColor>().blueColor = FGMain.TeamColorEnabled ? componentInParent.unit.GetComponent<ParticleTeamColor>().blueColor : componentInParent.unit.GetComponent<ParticleTeamColor>().redColor;
						}
						var em = blood.GetComponent<ParticleSystem>().emission;
						em.burstCount = ((int)__instance.damage) / 2 / (int)1.25 * (int)FGMain.BloodIntensity;
						var inherit = blood.GetComponent<ParticleSystem>().inheritVelocity;
						inherit.curveMultiplier *= FGMain.BloodIntensity;
						var scale = blood.GetComponent<ParticleSystem>().main;
						scale.startSizeMultiplier *= FGMain.BloodSize;
					}
					if (componentInParent.GetComponentInChildren<DismemberablePart>() && componentInParent.team != ___ownUnit.Team && __instance.damage >= componentInParent.health * 0.2f) {
						var partsOrdered = (
							from DismemberablePart part 
								in componentInParent.GetComponentsInChildren<DismemberablePart>()
							where !part.dismembered
							orderby Vector3.Distance(collision.contacts[0].point, part.transform.position)
							select part).ToArray();
						if (partsOrdered.Length > 0)
						{
							partsOrdered[0].DismemberPart();
						}
					}
				}
				___DealDamageAction?.Invoke(collision, __instance.damage * num2, vector);
				__instance.lastHitHealth.TakeDamage(__instance.damage * num2, vector, ___ownUnit);
				_ = __instance.damage;
				if (__instance.selfDamageMultiplier != 0f && (bool)unit)
				{
					unit.data.healthHandler.TakeDamage(__instance.damage * num2 * __instance.selfDamageMultiplier, vector, null);
				}
				if (__instance.dealDamageEvent != null)
				{
					__instance.dealDamageEvent.Invoke();
				}
				if ((bool)__instance.GetComponent<MeleeWeapon>() && __instance.hitFasterAfterDealDamage)
				{
					__instance.GetComponent<MeleeWeapon>().internalCounter += UnityEngine.Random.Range(0f, __instance.GetComponent<MeleeWeapon>().internalCooldown * 0.5f);
				}
				if ((bool)componentInParent && (__instance.callEffectsOn == CollisionWeapon.CallEffectsOn.All || __instance.callEffectsOn == CollisionWeapon.CallEffectsOn.Rigidbodies))
				{
					__instance.InvokeMethod("DoCollisionEffects", new object[] { componentInParent.mainRig.transform, collision });
				}
			}
			else
			{
				if ((bool)__instance.GetComponent<CollisionSound>())
				{
					__instance.GetComponent<CollisionSound>().DoEffect(collision.transform, collision, num);
				}
				if ((bool)__instance.GetComponent<MeleeWeapon>())
				{
					__instance.InvokeMethod("WeaponCollision", new object[] { collision, num });
				}
			}
			__instance.InvokeMethod("DoScreenShake", new object[] { num, collision, 1f });
		}
	}
}