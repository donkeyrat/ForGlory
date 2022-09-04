using System.Collections;
using UnityEngine;

namespace ForGlory {
    public class FGBinder : MonoBehaviour {

        public static void UnitGlad() {
            if (!instance) {
                instance = new GameObject {
                    hideFlags = HideFlags.HideAndDontSave
                }.AddComponent<FGBinder>();
            }
            instance.StartCoroutine(StartUnitgradLate());
        }

        private static IEnumerator StartUnitgradLate() {
            yield return new WaitUntil(() => FindObjectOfType<ServiceLocator>() != null);
            yield return new WaitUntil(() => ServiceLocator.GetService<ISaveLoaderService>() != null);
            yield return new WaitForSeconds(1f);
            new FGMain();
            yield break;
        }

        private static FGBinder instance;
    }
}