using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ForGlory
{
    public class RootDismemberment : MonoBehaviour
    {
        public void TryDismemberPart(Vector3 positionToCheckFrom)
        {
            GetComponentInChildren<ArmLeft>()?.GetComponent<DismemberablePart>()?.DismemberPart();
            Debug.Log("tried");
            //var partsOrdered = dismemberableParts
            //    .Where(x => !x.dismembered)
            //    .OrderBy(x => Vector3.Distance(positionToCheckFrom, x.transform.position)).ToArray();
			//		
            //if (partsOrdered.Length > 0) partsOrdered[0].DismemberPart();
        }
        
        private void Update()
        {
            counter += Time.deltaTime;
        }

        public List<DismemberablePart> dismemberableParts = new List<DismemberablePart>();

        public float counter;

        public float cooldown = 0.1f;
    }
}