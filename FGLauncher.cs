using System.Collections;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace ForGlory 
{
	[BepInPlugin("teamgrad.forglory", "For Glory", "2.1.2")]
	public class FGLauncher : BaseUnityPlugin 
	{
		public void Awake()
		{
			DoConfig();
			StartCoroutine(LaunchMod());
		}
		
		private static IEnumerator LaunchMod() 
		{
			yield return new WaitUntil(() => FindObjectOfType<ServiceLocator>() != null);
			yield return new WaitUntil(() => ServiceLocator.GetService<ISaveLoaderService>() != null);
			
			yield return new WaitForSeconds(0.5f);
			
			new FGMain();
		}

		public void DoConfig()
		{
			ConfigWeaponBloodEnabled = Config.Bind("Gameplay", "WeaponBloodEnabled", true, "Enables/disables blood from melee weapons.");
			ConfigProjectileBloodEnabled = Config.Bind("Gameplay", "ProjectileBloodEnabled", true, "Enables/disables blood from projectiles.");
			ConfigExplosionBloodEnabled = Config.Bind("Gameplay", "ExplosionBloodEnabled", false, "Enables/disables blood from explosions.");
			ConfigDismembermentChance = Config.Bind("Gameplay", "DismembermentChance", 15f, "Percent chance of dismemberment every time a unt takes damage.");
			ConfigDecapitationChance = Config.Bind("Gameplay", "DecapitationChance", 30f, "Percent chance of decapitation for every dismemberment.");
			ConfigTeamColorEnabled = Config.Bind("Bug", "TeamColorEnabled", true, "Enables/disables team colored blood.");
			ConfigSkeletonBloodEnabled = Config.Bind("Bug", "SkeletonBloodEnabled", true, "Enables/disables skeleton blood.");
			ConfigKillUnitsAfterDecapitateEnabled = Config.Bind("Bug", "KillUnitsAfterDecapitateEnabled", true, "Enables/disables units dying after decapitation.");
			ConfigBloodAmount = Config.Bind("Bug", "BloodAmount", 1f, "Modifies the blood amount of blood splatters.");
			ConfigBloodIntensity = Config.Bind("Bug", "BloodIntensity", 1f, "Modifies the intensity of blood splatters.");
			ConfigBloodSize = Config.Bind("Bug", "BloodSize", 1f, "Modifies the scale of blood splatters.");
		}
		
		public static ConfigEntry<bool> ConfigWeaponBloodEnabled;
		
		public static ConfigEntry<bool> ConfigProjectileBloodEnabled;
		
		public static ConfigEntry<bool> ConfigExplosionBloodEnabled;

		public static ConfigEntry<float> ConfigDismembermentChance;
		
		public static ConfigEntry<float> ConfigDecapitationChance;
		
		public static ConfigEntry<bool> ConfigTeamColorEnabled;
		
		public static ConfigEntry<bool> ConfigSkeletonBloodEnabled;
		
		public static ConfigEntry<bool> ConfigKillUnitsAfterDecapitateEnabled;
		
		public static ConfigEntry<float> ConfigBloodAmount;
		
		public static ConfigEntry<float> ConfigBloodIntensity;
		
		public static ConfigEntry<float> ConfigBloodSize;
	}
}
