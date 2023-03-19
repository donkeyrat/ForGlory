using UnityEngine;
using Landfall.TABS;
using System.Collections.Generic;

namespace ForGlory {

    public class ExplosionBloodEffect : ExplosionEffect {

        public override void DoEffect(GameObject target)
        {
	        if (!FGMain.ExplosionBloodEnabled) return;
	        
	        var unit = target.transform.root.GetComponent<Unit>();
			if (unit && unit.unitType == Unit.UnitType.Meat && target.GetComponent<Rigidbody>() && !hitList.Contains(target.transform.root.GetComponent<Unit>()))
			{
				hitList.Add(unit);
				if (explosion.damage > 5f && !(unit.name.Contains("Stiffy") && !FGMain.SkeletonBloodEnabled)) 
				{
					var blood = Instantiate(FGMain.dismember.LoadAsset<GameObject>("E_BloodDamage"), unit.data.mainRig.position, Quaternion.identity, target.transform);

					var particleTeamColor = unit.GetComponent<ParticleTeamColor>();
					if (particleTeamColor) 
					{
						blood.GetComponent<ParticleTeamColor>().redColor = particleTeamColor.redColor;
						blood.GetComponent<ParticleTeamColor>().blueColor = FGMain.TeamColorEnabled ? particleTeamColor.blueColor : particleTeamColor.redColor;
					}
					
					var main = blood.GetComponent<ParticleSystem>().main;
					main.startSizeMultiplier *= FGMain.BloodSize;
					main.duration *= FGMain.BloodIntensity;
					main.startSpeedMultiplier *= FGMain.BloodIntensity;
					
					var emission = blood.GetComponent<ParticleSystem>().emission;
					emission.rateOverTimeMultiplier = FGMain.BloodAmount;
						
					var inherit = blood.GetComponent<ParticleSystem>().inheritVelocity;
					inherit.curveMultiplier *= FGMain.BloodIntensity;
				}
				if (unit.GetComponent<RootDismemberment>() && unit.Team != FindOwnTeam() && explosion.damage >= unit.data.health * 0.2f && explosion.damage > 80f && unit.data.immunityForSeconds <= 0)
				{
					unit.GetComponent<RootDismemberment>().TryDismemberPart(transform.position);
				}
			}
        }
        
        private Team FindOwnTeam()
        {
	        if (GetComponent<TeamHolder>()) return GetComponent<TeamHolder>().team;
	        if (transform.root.GetComponent<Unit>()) return transform.root.GetComponent<Unit>().Team;

	        return Team.Red;
        }

        public Explosion explosion;

        private List<Unit> hitList = new List<Unit>();
    }
}
