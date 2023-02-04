using UnityEngine;
using Landfall.TABS;

namespace ForGlory 
{
	public class ProjectileHitBloodEffect : ProjectileHitEffect
	{
		public override bool DoEffect(HitData hit)
		{
			if (!FGMain.ProjectileBloodEnabled) return false;
			
			var unit = hit.transform.root.GetComponent<Unit>();
			if (unit && hit.rigidbody && hit.rigidbody.transform.parent == unit.data.transform && unit.unitType == Unit.UnitType.Meat)
			{
				if (projectileHit.damage > 5f && !(unit.name.Contains("Stiffy") && !FGMain.SkeletonBloodEnabled)) 
				{
					var blood = Instantiate(FGMain.dismember.LoadAsset<GameObject>("E_BloodDamage"), hit.point, Quaternion.identity, hit.transform);
					
					var particleTeamColor = unit.GetComponent<ParticleTeamColor>();
					if (particleTeamColor) 
					{
						blood.GetComponent<ParticleTeamColor>().redColor = particleTeamColor.redColor;
						blood.GetComponent<ParticleTeamColor>().blueColor = FGMain.TeamColorEnabled ? particleTeamColor.blueColor : particleTeamColor.redColor;
					}
					
					var goldenNumber = Mathf.Clamp(100f / (projectileHit.force / projectileHit.lowMassCap) * FGMain.BloodIntensity * Random.Range(0.1f, 0.2f), 0.1f, 1.5f * FGMain.BloodIntensity);
						
					var main = blood.GetComponent<ParticleSystem>().main;
					main.startSizeMultiplier *= FGMain.BloodSize;
					main.duration *= goldenNumber;
					main.startSpeedMultiplier *= goldenNumber;
					
					var emission = blood.GetComponent<ParticleSystem>().emission;
					emission.rateOverTimeMultiplier = FGMain.BloodAmount;
						
					var inherit = blood.GetComponent<ParticleSystem>().inheritVelocity;
					inherit.curveMultiplier *= goldenNumber;
				}
				if (unit.GetComponent<RootDismemberment>() && !(GetComponent<TeamHolder>() && unit.Team == GetComponent<TeamHolder>().team) && projectileHit.damage >= unit.data.health * 0.2f)
				{
					unit.GetComponent<RootDismemberment>().TryDismemberPart(hit.point);
				}
			}

			return false;
		}

		public ProjectileHit projectileHit;
	}
}