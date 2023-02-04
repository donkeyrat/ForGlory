using Landfall.TABS;
using UnityEngine;
using Landfall.TABS.Workshop;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using HarmonyLib;
using System.IO;
using BepInEx.Configuration;
using DM;
using Sony.NP;

namespace ForGlory {

	public class FGMain 
    {
        public FGMain()
        {
            var db = ContentDatabase.Instance().LandfallContentDatabase;
            
            blueColor = dismember.LoadAsset<GameObject>("E_BloodDamage").GetComponent<ParticleTeamColor>().blueColor;
            redColor = dismember.LoadAsset<GameObject>("E_BloodDamage").GetComponent<ParticleTeamColor>().redColor;
            
            foreach (var b in db.GetUnitBases().ToList()) 
            {
                if (!b.GetComponent<ParticleTeamColor>()) 
                {
                    var co = b.AddComponent<ParticleTeamColor>();
                    co.redColor = redColor;
                    co.blueColor = blueColor;
                    co.playSystem = false;
                    co.useColor = false;
                    co.useMaterial = false;
                    co.enabled = false;
                    if (b.name.Contains("Stiffy")) 
                    {
                        co.redColor = Color.black;
                        co.blueColor = Color.black;
                    }
                }
                if ((b.name.Contains("Humanoid") || b.name.Contains("Stiffy") || b.name.Contains("Blackbeard") || b.name.Contains("Halfling")) && b.GetComponentInChildren<SkinnedMeshRenderer>() && b.GetComponent<Unit>().data) 
                {
                    DoDismembermentCheck(b);
                }
            }

            foreach (var expl in Resources.FindObjectsOfTypeAll<Explosion>())
            {
                expl.gameObject.AddComponent<ExplosionBloodEffect>().explosion = expl;
            }
            
            foreach (var weapon in db.GetWeapons())
            {
                if (weapon.GetComponent<RangeWeapon>() && weapon.GetComponent<RangeWeapon>().ObjectToSpawn && weapon.GetComponent<RangeWeapon>().ObjectToSpawn.GetComponent<ProjectileHit>())
                {
                    weapon.GetComponent<RangeWeapon>().ObjectToSpawn.AddComponent<ProjectileHitBloodEffect>().projectileHit = weapon.GetComponent<RangeWeapon>().ObjectToSpawn.GetComponent<ProjectileHit>();
                }
            }
            
            Dictionary<DatabaseID, GameObject> projectiles = (Dictionary<DatabaseID, GameObject>)typeof(LandfallContentDatabase).GetField("m_projectiles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
            foreach (var proj in projectiles.Values)
            {
                if (proj.GetComponent<ProjectileHit>() && !proj.GetComponent<ProjectileHitBloodEffect>())
                {
                    proj.AddComponent<ProjectileHitBloodEffect>().projectileHit = proj.GetComponent<ProjectileHit>();
                }
            }
            typeof(LandfallContentDatabase).GetField("m_projectiles", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(db, projectiles);
            
            foreach (var weapon in Resources.FindObjectsOfTypeAll<CollisionWeapon>())
            {
                weapon.gameObject.AddComponent<CollisionBloodEffect>().collisionWeapon = weapon;
            }
            
            
            new Harmony("DerulanWasHitByATruckSorryGuys").PatchAll();
            
            var toggleWeaponBlood = CreateSetting(SettingsInstance.SettingsType.Options, "Toggle melee blood", "Enables/disables blood from melee weapons.", "GAMEPLAY", 0f, FGLauncher.ConfigWeaponBloodEnabled.Value ? 0 : 1, new[] { "Enabled", "Disabled" } );
            toggleWeaponBlood.OnValueChanged += delegate(int value)
            {
                FGLauncher.ConfigWeaponBloodEnabled.Value = value == 0;
            };
            
            var toggleProjectileBlood = CreateSetting(SettingsInstance.SettingsType.Options, "Toggle projectile blood", "Enables/disables blood from projectiles.", "GAMEPLAY", 0f, FGLauncher.ConfigProjectileBloodEnabled.Value ? 0 : 1, new[] { "Enabled", "Disabled" } );
            toggleProjectileBlood.OnValueChanged += delegate(int value)
            {
                FGLauncher.ConfigProjectileBloodEnabled.Value = value == 0;
            };
            
            var toggleExplosionBlood = CreateSetting(SettingsInstance.SettingsType.Options, "Toggle explosion blood", "Enables/disables blood from explosions.", "GAMEPLAY", 0f, FGLauncher.ConfigExplosionBloodEnabled.Value ? 0 : 1, new[] { "Disabled", "Enabled" } );
            toggleExplosionBlood.OnValueChanged += delegate(int value)
            {
                FGLauncher.ConfigExplosionBloodEnabled.Value = value == 1;
            };

            var dismemberment = CreateSetting(SettingsInstance.SettingsType.Slider, "Dismemberment chance", "Percent chance of dismemberment every time a unt takes damage.", "GAMEPLAY", 10f, FGLauncher.ConfigDismembermentChance.Value, null, 0f, 100f);
            dismemberment.OnSliderValueChanged += delegate(float value)
            {
                FGLauncher.ConfigDismembermentChance.Value = value;
            };
            
            var decapitation = CreateSetting(SettingsInstance.SettingsType.Slider, "Decapitation chance", "Percent chance of decapitation for every dismemberment.", "GAMEPLAY", 30f, FGLauncher.ConfigDecapitationChance.Value, null, 0f, 100f);
            decapitation.OnSliderValueChanged += delegate(float value)
            {
                FGLauncher.ConfigDecapitationChance.Value = value;
            };

            var teamColorBlood = CreateSetting(SettingsInstance.SettingsType.Options, "Toggle team color blood", "Enables/disables team colored blood.", "BUG", 0f, FGLauncher.ConfigTeamColorEnabled.Value ? 0 : 1, new[] { "Enabled", "Disabled" } );
            teamColorBlood.OnValueChanged += delegate(int value)
            {
                FGLauncher.ConfigTeamColorEnabled.Value = value == 0;
            };

            var skeletonBlood = CreateSetting(SettingsInstance.SettingsType.Options, "Toggle skeleton blood", "Enables/disables skeleton blood.", "BUG", 0f, FGLauncher.ConfigSkeletonBloodEnabled.Value ? 0 : 1, new[] { "Enabled", "Disabled" } );
            skeletonBlood.OnValueChanged += delegate(int value)
            {
                FGLauncher.ConfigSkeletonBloodEnabled.Value = value == 0;
            };
            
            var killAfterDecapitate = CreateSetting(SettingsInstance.SettingsType.Options, "Kill units after decapitation", "Enables/disables units dying after decapitation.", "BUG", 0f, FGLauncher.ConfigKillUnitsAfterDecapitateEnabled.Value ? 0 : 1, new[] { "Enabled", "Disabled" } );
            killAfterDecapitate.OnValueChanged += delegate(int value)
            {
                FGLauncher.ConfigKillUnitsAfterDecapitateEnabled.Value = value == 0;
            };
            
            var bloodAmount = CreateSetting(SettingsInstance.SettingsType.Slider, "Blood amount", "Modifies the blood amount of blood splatters.", "BUG", 1f, FGLauncher.ConfigBloodAmount.Value, null, 0f, 10f);
            bloodAmount.OnSliderValueChanged += delegate(float value)
            {
                FGLauncher.ConfigBloodAmount.Value = value;
            };
            
            var bloodIntensity = CreateSetting(SettingsInstance.SettingsType.Slider, "Blood intensity", "Modifies the intensity of blood splatters.", "BUG", 1f, FGLauncher.ConfigBloodIntensity.Value, null, 0f, 10f);
            bloodIntensity.OnSliderValueChanged += delegate(float value)
            {
                FGLauncher.ConfigBloodIntensity.Value = value;
            };

            var bloodSizer = CreateSetting(SettingsInstance.SettingsType.Slider, "Blood scale", "Modifies the scale of blood splatters.", "BUG", 1f, FGLauncher.ConfigBloodSize.Value, null, 0f, 10f);
            bloodSizer.OnSliderValueChanged += delegate(float value)
            {
                FGLauncher.ConfigBloodSize.Value = value;
            };

            foreach (var sb in dismember.LoadAllAssets<SoundBank>()) 
            {
                if (sb.name.Contains("Sound")) 
                {
                    var vsb = ServiceLocator.GetService<SoundPlayer>().soundBank;
                    var cat = vsb.Categories.ToList();
                    cat.AddRange(sb.Categories);
                    vsb.Categories = cat.ToArray();
                }
            }
        }

        private void DoDismembermentCheck(GameObject b) 
        {
            var unit = b.GetComponent<Unit>();;
            var root = b.AddComponent<RootDismemberment>();
            var parts = b.GetComponentsInChildren<Rigidbody>();
            foreach (var bodyPart in parts)
            {
                try 
                {
                    if (bodyPart != null && !bodyPart.GetComponent<Torso>() && !bodyPart.GetComponent<Hip>()) 
                    {
                        if (!bodyPart.GetComponent<KneeLeft>() && !bodyPart.GetComponent<KneeRight>() && !bodyPart.GetComponent<HandLeft>() && !bodyPart.GetComponent<HandRight>()) 
                        {
                            var part = bodyPart.gameObject.AddComponent<DismemberablePart>();
                            part.unit = unit;
                            part.root = root;
                        }
                    }
                }
                catch (Exception exc)
                {
                    Debug.Log(exc);
                }
            }

            root.dismemberableParts = root.GetComponentsInChildren<DismemberablePart>().ToList();
        }

        private SettingsInstance CreateSetting(SettingsInstance.SettingsType settingsType, string settingName, string toolTip, string settingListToAddTo, float defaultValue, float currentValue, string[] options = null, float min = 0f, float max = 1f) 
        {
            var setting = new SettingsInstance
            {
                settingName = settingName,
                toolTip = toolTip,
                m_settingsKey = settingName,
                settingsType = settingsType,
                options = options,
                min = min,
                max = max,
                defaultValue = (int)defaultValue,
                currentValue = (int)currentValue,
                defaultSliderValue = defaultValue,
                currentSliderValue = currentValue
            };

            var global = ServiceLocator.GetService<GlobalSettingsHandler>();
            SettingsInstance[] listToAdd;
            if (settingListToAddTo == "BUG") listToAdd = global.BugsSettings;
            else if (settingListToAddTo == "VIDEO") listToAdd = global.VideoSettings;
            else if (settingListToAddTo == "AUDIO") listToAdd = global.AudioSettings;
            else if (settingListToAddTo == "CONTROLS") listToAdd = global.ControlSettings;
            else listToAdd = global.GameplaySettings;

            var list = listToAdd.ToList();
            list.Add(setting);

            switch (settingListToAddTo)
            {
                case "BUG":
                    typeof(GlobalSettingsHandler).GetField("m_bugsSettings", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(global, list.ToArray());
                    break;
                case "VIDEO":
                    typeof(GlobalSettingsHandler).GetField("m_videoSettings", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(global, list.ToArray());
                    break;
                case "AUDIO":
                    typeof(GlobalSettingsHandler).GetField("m_audioSettings", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(global, list.ToArray());
                    break;
                case "CONTROLS":
                    typeof(GlobalSettingsHandler).GetField("m_controlSettings", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(global, list.ToArray());
                    break;
                default:
                    typeof(GlobalSettingsHandler).GetField("m_gameplaySettings", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(global, list.ToArray());
                    break;
            }

            return setting;
        }

        public static Color blueColor;

        public static Color redColor;

        public static bool WeaponBloodEnabled => FGLauncher.ConfigWeaponBloodEnabled.Value;

        public static bool ProjectileBloodEnabled => FGLauncher.ConfigProjectileBloodEnabled.Value;
        
        public static bool ExplosionBloodEnabled => FGLauncher.ConfigExplosionBloodEnabled.Value;

        public static float DismembermentChance => FGLauncher.ConfigDismembermentChance.Value / 100;
        
        public static float DecapitationChance => FGLauncher.ConfigDecapitationChance.Value / 100;
        
        public static bool TeamColorEnabled => FGLauncher.ConfigTeamColorEnabled.Value;

        public static bool SkeletonBloodEnabled => FGLauncher.ConfigSkeletonBloodEnabled.Value;
        
        public static bool KillUnitsAfterDecapitateEnabled => FGLauncher.ConfigKillUnitsAfterDecapitateEnabled.Value;
        
        public static float BloodAmount => FGLauncher.ConfigBloodAmount.Value * 100;
        
        public static float BloodIntensity => FGLauncher.ConfigBloodIntensity.Value;

        public static float BloodSize => FGLauncher.ConfigBloodSize.Value;

        public static AssetBundle dismember = AssetBundle.LoadFromMemory(Properties.Resources.dismemberment);
    }
}
