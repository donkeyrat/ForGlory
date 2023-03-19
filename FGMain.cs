using Landfall.TABS;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using System.Collections;
using HarmonyLib;
using DM;
using TGCore;

namespace ForGlory 
{
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
            
            TGMain.newSounds.AddRange(dismember.LoadAllAssets<SoundBank>());
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
        
        public static AssetBundle dismember = AssetBundle.LoadFromMemory(Properties.Resources.dismemberment);

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
    }
}
