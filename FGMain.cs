using Landfall.TABS;
using UnityEngine;
using Landfall.TABS.Workshop;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using HarmonyLib;
using System.IO;
using DM;

namespace ForGlory {

	public class FGMain {

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
            
            GlobalSettingsHandler service = ServiceLocator.GetService<GlobalSettingsHandler>();
            var list = service.BugsSettings.ToList();

            var teamColorBlood = new SettingsInstance
            {
                currentValue = 0,
                defaultValue = 0,
                options = new string[]
                {
                    "Enabled",
                    "Disabled"
                },
                m_hideSetting = false,
                m_platform = SettingsInstance.Platform.All,
                m_settingsKey = "Toggle Team Color Blood",
                settingName = "Toggle Team Color Blood",
                settingsType = SettingsInstance.SettingsType.Options,
                toolTip = "Enables/disables team colored blood."
            };
            teamColorBlood.OnValueChanged += TeamColor;
            list.Add(teamColorBlood);

            var skeletonBlood = new SettingsInstance
            {
                currentValue = 0,
                defaultValue = 0,
                options = new string[]
                {
                    "Enabled",
                    "Disabled"
                },
                m_hideSetting = false,
                m_platform = SettingsInstance.Platform.All,
                m_settingsKey = "Toggle Skeleton Blood",
                settingName = "Toggle Skeleton Blood",
                settingsType = SettingsInstance.SettingsType.Options,
                toolTip = "Enables/disables skeleton blood."
            };
            skeletonBlood.OnValueChanged += SkeletonBlood;
            list.Add(skeletonBlood);
            
            var killAfterDecapitate = new SettingsInstance
            {
                currentValue = 0,
                defaultValue = 0,
                options = new string[]
                {
                    "Enabled",
                    "Disabled"
                },
                m_hideSetting = false,
                m_platform = SettingsInstance.Platform.All,
                m_settingsKey = "Kill Units After Decapitation",
                settingName = "Kill Units After Decapitation",
                settingsType = SettingsInstance.SettingsType.Options,
                toolTip = "Enables/disables units dying after decapitation."
            };
            killAfterDecapitate.OnValueChanged += KillUnitsAfterDecapitate;
            list.Add(killAfterDecapitate);
            
            var dismemberment = new SettingsInstance
            {
                currentSliderValue = 60f,
                defaultSliderValue = 60f,
                min = 0f,
                max = 100f,
                m_hideSetting = false,
                m_platform = SettingsInstance.Platform.All,
                m_settingsKey = "Dismemberment Chance",
                settingName = "Dismemberment Chance",
                settingsType = SettingsInstance.SettingsType.Slider,
                toolTip = "Percent chance of dismemberment every time a unt takes damage."
            };
            dismemberment.OnSliderValueChanged += Dismemberment;
            list.Add(dismemberment);
            
            var decapitation = new SettingsInstance
            {
                currentSliderValue = 60f,
                defaultSliderValue = 60f,
                min = 0f,
                max = 100f,
                m_hideSetting = false,
                m_platform = SettingsInstance.Platform.All,
                m_settingsKey = "Decapitation Chance",
                settingName = "Decapitation Chance",
                settingsType = SettingsInstance.SettingsType.Slider,
                toolTip = "Percent chance of decapitation for every dismemberment."
            };
            decapitation.OnSliderValueChanged += Decapitation;
            list.Add(decapitation);

            var bloodIntensity = new SettingsInstance
            {
                currentSliderValue = 1f,
                defaultSliderValue = 1f,
                min = 0.1f,
                max = 10f,
                m_hideSetting = false,
                m_platform = SettingsInstance.Platform.All,
                m_settingsKey = "Blood Intensity",
                settingName = "Blood Intensity",
                settingsType = SettingsInstance.SettingsType.Slider,
                toolTip = "Modifies the intensity of blood splatters."
            };
            bloodIntensity.OnSliderValueChanged += BloodIntensifier;
            list.Add(bloodIntensity);

            var bloodSizer = new SettingsInstance
            {
                currentSliderValue = 1f,
                defaultSliderValue = 1f,
                min = 0.1f,
                max = 10f,
                m_hideSetting = false,
                m_platform = SettingsInstance.Platform.All,
                m_settingsKey = "Blood Scale",
                settingName = "Blood Scale",
                settingsType = SettingsInstance.SettingsType.Slider,
                toolTip = "Modifies the scale of blood splatters."
            };
            bloodSizer.OnSliderValueChanged += BloodSizer;
            list.Add(bloodSizer);

            service.SetField("m_bugsSettings", list.ToArray());

            foreach (var sb in dismember.LoadAllAssets<SoundBank>()) {
                if (sb.name.Contains("Sound")) {
                    var vsb = ServiceLocator.GetService<SoundPlayer>().soundBank;
                    var cat = vsb.Categories.ToList();
                    cat.AddRange(sb.Categories);
                    vsb.Categories = cat.ToArray();
                }
            }
        }

        public void TeamColor(int value)  
        {
            if (value == 0) TeamColorEnabled = true;
            
            else TeamColorEnabled = false;
        }

        public void SkeletonBlood(int value)  
        {
            if (value == 0) SkeletonBloodEnabled = true;
            
            else SkeletonBloodEnabled = false;
        }
        
        public void KillUnitsAfterDecapitate(int value)  
        {
            if (value == 0) KillUnitsAfterDecapitateEnabled = true;
            
            else KillUnitsAfterDecapitateEnabled = false;
        }
        
        public void Dismemberment(float value)
        {
            DismembermentChance = value / 100;
        }
        
        public void Decapitation(float value)
        {
            DecapitationChance = value / 100;
        }

        public void BloodIntensifier(float value) 
        {
            BloodIntensity = value;
        }

        public void BloodSizer(float value) 
        {
            BloodSize = value;
        }


        public void DoDismembermentCheck(GameObject b) 
        {
            var unit = b.GetComponent<Unit>();;
            var root = b.AddComponent<RootDismemberment>();
            var parts = b.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < parts.Length; i++) 
            {
                try 
                {
                    if (parts[i] != null && !parts[i].GetComponent<Torso>() && !parts[i].GetComponent<Hip>()) 
                    {
                        if (!parts[i].GetComponent<KneeLeft>() && !parts[i].GetComponent<KneeRight>() && !parts[i].GetComponent<HandLeft>() && !parts[i].GetComponent<HandRight>()) 
                        {
                            var part = parts[i].gameObject.AddComponent<DismemberablePart>();
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
        }

        public static Color blueColor;

        public static Color redColor;

        public static float BloodIntensity = 1f;

        public static float BloodSize = 1f;

        public static bool TeamColorEnabled = true;

        public static bool SkeletonBloodEnabled = true;
        
        public static bool KillUnitsAfterDecapitateEnabled = true;

        public static float DismembermentChance = 0.6f;
        
        public static float DecapitationChance = 0.6f;

        public static AssetBundle dismember = AssetBundle.LoadFromMemory(Properties.Resources.dismemberment);
    }
}
