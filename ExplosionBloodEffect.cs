using UnityEngine;
using Landfall.TABS;
using System.Collections.Generic;
using System.Linq;

namespace ForGlory {

    public class ExplosionBloodEffect : ExplosionEffect {

        public override void DoEffect(GameObject target) {

			if (target && target.transform.root && target.transform.root.GetComponent<Unit>() && target.transform.root.GetComponent<Unit>().unitType == Unit.UnitType.Meat && !(transform.root.GetComponent<Unit>() && target.transform.root.GetComponent<Unit>().Team == transform.root.GetComponent<Unit>().Team) && !(GetComponent<TeamHolder>() && target.transform.root.GetComponent<Unit>().Team == GetComponent<TeamHolder>().team) && !hitList.Contains(target.transform.root.GetComponent<Unit>())) {

				var componentInParent = target.transform.root.GetComponent<Unit>().data;
				hitList.Add(componentInParent.unit);
				if (GetComponent<Explosion>().damage > 5f && !(componentInParent.unit.name.Contains("Stiffy") && !FGMain.SkeletonBloodEnabled)) {
					var blood = Instantiate(FGMain.dismember.LoadAsset<GameObject>("E_BloodDamage"), componentInParent.mainRig.position, Quaternion.identity, target.transform);
					if (componentInParent.unit.GetComponent<ParticleTeamColor>()) {
						blood.GetComponent<ParticleTeamColor>().redColor = componentInParent.unit.GetComponent<ParticleTeamColor>().redColor;
						blood.GetComponent<ParticleTeamColor>().blueColor = FGMain.TeamColorEnabled ? componentInParent.unit.GetComponent<ParticleTeamColor>().blueColor : componentInParent.unit.GetComponent<ParticleTeamColor>().redColor;
					}
					var em = blood.GetComponent<ParticleSystem>().emission;
					em.burstCount = ((int)GetComponent<Explosion>().damage) / 2 / (int)1.25 * (int)FGMain.BloodIntensity;
					var inherit = blood.GetComponent<ParticleSystem>().inheritVelocity;
					inherit.curveMultiplier *= FGMain.BloodIntensity;
					var scale = blood.GetComponent<ParticleSystem>().main;
					scale.startSizeMultiplier *= FGMain.BloodSize;
				}
				if (componentInParent.GetComponentInChildren<DismemberablePart>() && GetComponent<Explosion>().damage >= componentInParent.health * 0.2f && GetComponent<Explosion>().damage > 80f)
				{
					var partsOrdered = (
						from DismemberablePart part 
							in componentInParent.GetComponentsInChildren<DismemberablePart>()
						where !part.dismembered
						orderby Vector3.Distance(transform.position, part.transform.position)
						select part).ToList();
					if (partsOrdered[0] != null)
					{
						partsOrdered[0].DismemberPart();
					}
				}
			}
        }

        private List<Unit> hitList = new List<Unit>();
    }
}
