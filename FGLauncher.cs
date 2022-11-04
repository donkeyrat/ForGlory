using BepInEx;
using UnityEngine;

namespace ForGlory {
	[BepInPlugin("teamgrad.forglory", "For Glory", "2.0.1")]
	public class FGLauncher : BaseUnityPlugin {
		public FGLauncher() {
			FGBinder.UnitGlad();
		}
	}
}
