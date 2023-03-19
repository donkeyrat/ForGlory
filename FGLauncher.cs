using BepInEx;
using BepInEx.Configuration;
using TGCore;

namespace ForGlory 
{
	[BepInPlugin("teamgrad.forglory", "For Glory", "2.2.1")]
	[BepInDependency("teamgrad.core")]
	public class FGLauncher : TGMod 
	{
		public override void LateLaunch()
		{
			new FGMain();
		}

		public override void AddSettings()
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
			
			var toggleWeaponBlood = TGAddons.CreateSetting(SettingsInstance.SettingsType.Options, "Toggle melee blood", "Enables/disables blood from melee weapons.", "GAMEPLAY", 0f, FGLauncher.ConfigWeaponBloodEnabled.Value ? 0 : 1, new[] { "Enabled", "Disabled" } );
            toggleWeaponBlood.OnValueChanged += delegate(int value)
            {
                ConfigWeaponBloodEnabled.Value = value == 0;
            };
            
            var toggleProjectileBlood = TGAddons.CreateSetting(SettingsInstance.SettingsType.Options, "Toggle projectile blood", "Enables/disables blood from projectiles.", "GAMEPLAY", 0f, FGLauncher.ConfigProjectileBloodEnabled.Value ? 0 : 1, new[] { "Enabled", "Disabled" } );
            toggleProjectileBlood.OnValueChanged += delegate(int value)
            {
                ConfigProjectileBloodEnabled.Value = value == 0;
            };
            
            var toggleExplosionBlood = TGAddons.CreateSetting(SettingsInstance.SettingsType.Options, "Toggle explosion blood", "Enables/disables blood from explosions.", "GAMEPLAY", 0f, FGLauncher.ConfigExplosionBloodEnabled.Value ? 0 : 1, new[] { "Disabled", "Enabled" } );
            toggleExplosionBlood.OnValueChanged += delegate(int value)
            {
                ConfigExplosionBloodEnabled.Value = value == 1;
            };

            var dismemberment = TGAddons.CreateSetting(SettingsInstance.SettingsType.Slider, "Dismemberment chance", "Percent chance of dismemberment every time a unt takes damage.", "GAMEPLAY", 10f, FGLauncher.ConfigDismembermentChance.Value, null, 0f, 100f);
            dismemberment.OnSliderValueChanged += delegate(float value)
            {
                ConfigDismembermentChance.Value = value;
            };
            
            var decapitation = TGAddons.CreateSetting(SettingsInstance.SettingsType.Slider, "Decapitation chance", "Percent chance of decapitation for every dismemberment.", "GAMEPLAY", 30f, FGLauncher.ConfigDecapitationChance.Value, null, 0f, 100f);
            decapitation.OnSliderValueChanged += delegate(float value)
            {
                ConfigDecapitationChance.Value = value;
            };

            var teamColorBlood = TGAddons.CreateSetting(SettingsInstance.SettingsType.Options, "Toggle team color blood", "Enables/disables team colored blood.", "BUG", 0f, FGLauncher.ConfigTeamColorEnabled.Value ? 0 : 1, new[] { "Enabled", "Disabled" } );
            teamColorBlood.OnValueChanged += delegate(int value)
            {
                ConfigTeamColorEnabled.Value = value == 0;
            };

            var skeletonBlood = TGAddons.CreateSetting(SettingsInstance.SettingsType.Options, "Toggle skeleton blood", "Enables/disables skeleton blood.", "BUG", 0f, FGLauncher.ConfigSkeletonBloodEnabled.Value ? 0 : 1, new[] { "Enabled", "Disabled" } );
            skeletonBlood.OnValueChanged += delegate(int value)
            {
                ConfigSkeletonBloodEnabled.Value = value == 0;
            };
            
            var killAfterDecapitate = TGAddons.CreateSetting(SettingsInstance.SettingsType.Options, "Kill units after decapitation", "Enables/disables units dying after decapitation.", "BUG", 0f, FGLauncher.ConfigKillUnitsAfterDecapitateEnabled.Value ? 0 : 1, new[] { "Enabled", "Disabled" } );
            killAfterDecapitate.OnValueChanged += delegate(int value)
            {
                ConfigKillUnitsAfterDecapitateEnabled.Value = value == 0;
            };
            
            var bloodAmount = TGAddons.CreateSetting(SettingsInstance.SettingsType.Slider, "Blood amount", "Modifies the blood amount of blood splatters.", "BUG", 1f, FGLauncher.ConfigBloodAmount.Value, null, 0f, 10f);
            bloodAmount.OnSliderValueChanged += delegate(float value)
            {
                ConfigBloodAmount.Value = value;
            };
            
            var bloodIntensity = TGAddons.CreateSetting(SettingsInstance.SettingsType.Slider, "Blood intensity", "Modifies the intensity of blood splatters.", "BUG", 1f, FGLauncher.ConfigBloodIntensity.Value, null, 0f, 10f);
            bloodIntensity.OnSliderValueChanged += delegate(float value)
            {
                ConfigBloodIntensity.Value = value;
            };

            var bloodSizer = TGAddons.CreateSetting(SettingsInstance.SettingsType.Slider, "Blood scale", "Modifies the scale of blood splatters.", "BUG", 1f, FGLauncher.ConfigBloodSize.Value, null, 0f, 10f);
            bloodSizer.OnSliderValueChanged += delegate(float value)
            {
                ConfigBloodSize.Value = value;
            };
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
