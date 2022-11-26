using UnityEngine;
using HarmonyLib;
using Landfall.TABS;
using System.Linq;
using TFBGames;

namespace ForGlory
{
	public class CollisionBloodEffect : CollisionWeaponEffect
		{ 
			public override void DoEffect(Transform hitTransform, Collision collision)
			{
				var unit = collision.transform.root.GetComponent<Unit>();
				if (unit && unit.unitType == Unit.UnitType.Meat)
				{
					if (collisionWeapon.damage > 5f && !(unit.name.Contains("Stiffy") && !FGMain.SkeletonBloodEnabled))
					{
						var blood = Instantiate(FGMain.dismember.LoadAsset<GameObject>("E_BloodDamage"), collision.rigidbody.transform.position, collision.rigidbody.transform.rotation, collision.transform);
						
						var particleTeamColor = unit.GetComponent<ParticleTeamColor>();
						if (particleTeamColor) 
						{
							blood.GetComponent<ParticleTeamColor>().redColor = particleTeamColor.redColor;
							blood.GetComponent<ParticleTeamColor>().blueColor = FGMain.TeamColorEnabled ? particleTeamColor.blueColor : particleTeamColor.redColor;
						}

						var goldenNumber =  Mathf.Clamp(100f / collision.impulse.magnitude * FGMain.BloodIntensity * (Random.value > 0.8f ? Random.Range(1f, 2f) : 0.7f), 0.1f, 1.5f * FGMain.BloodIntensity);

						var main = blood.GetComponent<ParticleSystem>().main;
						main.startSizeMultiplier *= FGMain.BloodSize;
						main.duration *= goldenNumber;
						main.startSpeedMultiplier *= goldenNumber;
						
						var inherit = blood.GetComponent<ParticleSystem>().inheritVelocity;
						inherit.curveMultiplier *= goldenNumber;
					}
	
					if (unit.GetComponent<RootDismemberment>() && unit.Team != FindOwnTeam() && collisionWeapon.damage >= unit.data.health * 0.2f)
					{
						unit.GetComponent<RootDismemberment>().TryDismemberPart(collision.contacts[0].point);
					}
				}
			}

			private Team FindOwnTeam()
			{
				if (GetComponent<TeamHolder>()) return GetComponent<TeamHolder>().team;
				if (transform.root.GetComponent<Unit>()) return transform.root.GetComponent<Unit>().Team;

				return Team.Red;
			}

			public CollisionWeapon collisionWeapon;
		}
}