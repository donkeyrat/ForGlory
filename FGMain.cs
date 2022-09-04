using Landfall.TABS;
using UnityEngine;
using Landfall.TABS.Workshop;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using HarmonyLib;
using System.IO;

namespace ForGlory {

	public class FGMain {

        public FGMain() {

            db = LandfallUnitDatabase.GetDatabase();

            blueColor = dismember.LoadAsset<GameObject>("E_BloodDamage").GetComponent<ParticleTeamColor>().blueColor;
            redColor = dismember.LoadAsset<GameObject>("E_BloodDamage").GetComponent<ParticleTeamColor>().redColor;
            foreach (var b in db.UnitBaseList) {
                if (!b.GetComponent<ParticleTeamColor>()) {
                    var co = b.AddComponent<ParticleTeamColor>();
                    co.redColor = redColor;
                    co.blueColor = blueColor;
                    co.playSystem = false;
                    co.useColor = false;
                    co.useMaterial = false;
                    co.enabled = false;
                    if (b.name.Contains("Stiffy")) {
                        co.redColor = Color.black;
                        co.blueColor = Color.black;
                    }
                }
                if ((b.name.Contains("Humanoid") || b.name.Contains("Stiffy") || b.name.Contains("Blackbeard") || b.name.Contains("Halfling")) && b.GetComponentInChildren<SkinnedMeshRenderer>() && b.GetComponent<Unit>().data) {
                    DismembermentMethodTwo(b);
                }
            }
            foreach (var expl in Resources.FindObjectsOfTypeAll<Explosion>()) {
                expl.gameObject.AddComponent<ExplosionBloodEffect>();
            }
            var harmony = new Harmony("DerulanWasHitByATruckSorryGuys");
            harmony.PatchAll();
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
                m_settingsKey = "Team Color Blood",
                settingName = "Team Color Blood",
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
                m_settingsKey = "Skeleton Blood",
                settingName = "Skeleton Blood",
                settingsType = SettingsInstance.SettingsType.Options,
                toolTip = "Enables/disables skeleton blood."
            };
            skeletonBlood.OnValueChanged += SkeletonBlood;
            list.Add(skeletonBlood);

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

        public void TeamColor(int value)  {
            if (value == 0) {
                TeamColorEnabled = true;
            }
            else {
                TeamColorEnabled = false;
            }
        }

        public void SkeletonBlood(int value)  {
            if (value == 0) {
                SkeletonBloodEnabled = true;
            }
            else {
                SkeletonBloodEnabled = false;
            }
        }

        public void BloodIntensifier(float value) {
            BloodIntensity = value;
        }

        public void BloodSizer(float value) {
            BloodSize = value;
        }


        public void DismembermentMethodTwo(GameObject b) {

            var parts = b.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < parts.Length; i++) {
                try {
                    if (parts[i] != null && !parts[i].GetComponent<Torso>() && !parts[i].GetComponent<Hip>()) {

                        if (!parts[i].GetComponent<KneeLeft>() && !parts[i].GetComponent<KneeRight>() && !parts[i].GetComponent<HandLeft>() && !parts[i].GetComponent<HandRight>()) {

                            parts[i].gameObject.AddComponent<DismemberablePart>();
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

        public static AssetBundle dismember = AssetBundle.LoadFromMemory(Properties.Resources.dismemberment);

        public LandfallUnitDatabase db;
    }
}
