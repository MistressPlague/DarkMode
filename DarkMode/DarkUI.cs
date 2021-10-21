using MelonLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[assembly: MelonInfo(typeof(DarkMode.DarkUI), "Dark Mode", "1.0", "Plague")]
[assembly: MelonGame]
[assembly: MelonColor(ConsoleColor.Magenta)]

namespace DarkMode
{
    public class DarkUI : MelonMod
    {
        public override void OnApplicationStart()
        {
            var Category = MelonPreferences.CreateCategory("DarkMode", "Dark Mode");

            Category.CreateEntry("Enabled", true, "Enabled", false);

            Category.CreateEntry("ToggleRed", 0.25f, "Toggle Red", false);
            Category.CreateEntry("ToggleGreen", 0.25f, "Toggle Green", false);
            Category.CreateEntry("ToggleBlue", 0.25f, "Toggle Blue", false);

            Category.CreateEntry("ButtonRed", 0.75f, "Button Red", false);
            Category.CreateEntry("ButtonGreen", 0.75f, "Button Green", false);
            Category.CreateEntry("ButtonBlue", 0.75f, "Button Blue", false);

            Category.CreateEntry("SliderRed", 0.15f, "Slider Red", false);
            Category.CreateEntry("SliderGreen", 0.15f, "Slider Green", false);
            Category.CreateEntry("SliderBlue", 0.15f, "Slider Blue", false);

            Category.CreateEntry("MiscRed", 0.15f, "Misc Red", false);
            Category.CreateEntry("MiscGreen", 0.15f, "Misc Green", false);
            Category.CreateEntry("MiscBlue", 0.15f, "Misc Blue", false);

            OnPreferencesSaved(); // Using This To Load Too On Game Startup
        }

        private bool Enabled = true;

        private float ToggleRed = 0.15f;
        private float ToggleGreen = 0.15f;
        private float ToggleBlue = 0.15f;

        private float ButtonRed = 0.75f;
        private float ButtonGreen = 0.75f;
        private float ButtonBlue = 0.75f;

        private float SliderRed = 0.25f;
        private float SliderGreen = 0.25f;
        private float SliderBlue = 0.25f;

        private float MiscRed = 0.25f;
        private float MiscGreen = 0.25f;
        private float MiscBlue = 0.25f;

        public override void OnPreferencesSaved()
        {
            Enabled = MelonPreferences.GetEntryValue<bool>("DarkMode", "Enabled");

            ToggleRed = Mathf.Clamp(MelonPreferences.GetEntryValue<float>("DarkMode", "ToggleRed"), 0f, 1f);
            ToggleGreen = Mathf.Clamp(MelonPreferences.GetEntryValue<float>("DarkMode", "ToggleGreen"), 0f, 1f);
            ToggleBlue = Mathf.Clamp(MelonPreferences.GetEntryValue<float>("DarkMode", "ToggleBlue"), 0f, 1f);

            ButtonRed = Mathf.Clamp(MelonPreferences.GetEntryValue<float>("DarkMode", "ButtonRed"), 0f, 1f);
            ButtonGreen = Mathf.Clamp(MelonPreferences.GetEntryValue<float>("DarkMode", "ButtonGreen"), 0f, 1f);
            ButtonBlue = Mathf.Clamp(MelonPreferences.GetEntryValue<float>("DarkMode", "ButtonBlue"), 0f, 1f);

            SliderRed = Mathf.Clamp(MelonPreferences.GetEntryValue<float>("DarkMode", "SliderRed"), 0f, 1f);
            SliderGreen = Mathf.Clamp(MelonPreferences.GetEntryValue<float>("DarkMode", "SliderGreen"), 0f, 1f);
            SliderBlue = Mathf.Clamp(MelonPreferences.GetEntryValue<float>("DarkMode", "SliderBlue"), 0f, 1f);

            MiscRed = Mathf.Clamp(MelonPreferences.GetEntryValue<float>("DarkMode", "MiscRed"), 0f, 1f);
            MiscGreen = Mathf.Clamp(MelonPreferences.GetEntryValue<float>("DarkMode", "MiscGreen"), 0f, 1f);
            MiscBlue = Mathf.Clamp(MelonPreferences.GetEntryValue<float>("DarkMode", "MiscBlue"), 0f, 1f);

            if (UILoad)
            {
                ColourUI();
            }
        }

        private int AmountOfSceneLoads = 0;
        private bool UILoad = false;
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (HasDarkened)
            {
                return; // Just In Case
            }

            if (AmountOfSceneLoads != 2)
            {
                AmountOfSceneLoads++;
            }

            if (AmountOfSceneLoads == 2)
            {
                // Assume UI Was Loaded
                MelonLogger.Warning("UI Load!");
                MelonCoroutines.Start(OnUILoaded());
            }
        }

        private bool HasDarkened = false;
        internal IEnumerator OnUILoaded()
        {
            if (Enabled && !HasDarkened)
            {
                ColourSkyBox();
            }

            yield return new WaitForSeconds(10f);

            if (Enabled)
            {
                ColourUI();
            }

            UILoad = true;

            yield break;
        }

        private Dictionary<string, (Color, ColorBlock?)> OldColours = new Dictionary<string, (Color, ColorBlock?)>();
        private Color? SkyBoxColour = null;
        private Color? InitialSkyBoxColour = null;

        private void ColourSkyBox()
        {
            //VRChat Specific, Doesn't Effect The Mod Being Universal Due To Only Using Unity Methods And Null Propagation.
            var SkyBox = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/SkyCube_Baked")?.GetComponent<MeshRenderer>()?.material;
            var InitialSkyBox = GameObject.Find("LoadingBackground_TealGradient_Music/SkyCube_Baked")?.GetComponent<MeshRenderer>()?.material;

            //Reflection Probe
            GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_Lighting (1)/Reflection Probe")?.SetActive(!Enabled);

            //Point Light
            var LoadingScreenLighting = GameObject.Find("UserInterface/MenuContent/Popups/LoadingPopup/3DElements/LoadingBackground_TealGradient/_Lighting (1)/Point light");

            if (LoadingScreenLighting != null)
            {
                LoadingScreenLighting.GetComponent<Light>().intensity = Enabled ? 5f : 15.7f;
            }

            if (Enabled)
            {
                if (SkyBox != null)
                {
                    SkyBoxColour = SkyBox.GetColor("_Tint");
                    SkyBox.SetColor("_Tint", new Color(MiscRed, MiscGreen, MiscBlue, SkyBox.GetColor("_Tint").a));
                }

                if (InitialSkyBox != null)
                {
                    InitialSkyBoxColour = InitialSkyBox.GetColor("_Tint");
                    InitialSkyBox.SetColor("_Tint", new Color(MiscRed, MiscGreen, MiscBlue, InitialSkyBox.GetColor("_Tint").a));
                }

                OldColours.Clear();
            }
            else
            {
                if (SkyBox != null && SkyBoxColour != null)
                {
                    SkyBox.SetColor("_Tint", (Color)SkyBoxColour);
                }

                if (InitialSkyBox != null && InitialSkyBoxColour != null)
                {
                    InitialSkyBox.SetColor("_Tint", (Color)InitialSkyBoxColour);
                }
            }
        }

        private void ColourUI()
        {
            MelonLogger.Warning("Grabbing Images..");

            var ObjsWithColorsToInvert = Resources.FindObjectsOfTypeAll<Image>().Where(o => o != null).ToList();

            MelonLogger.Warning($"{(Enabled ? "Darkening" : "Reverting")} {ObjsWithColorsToInvert.Count} Image Colours..");

            if (HasDarkened)
            {
                ColourSkyBox();
            }

            foreach (var obj in ObjsWithColorsToInvert)
            {
                try
                {
                    if (Enabled)
                    {
                        var Comps = obj.transform.GetComponents<Component>().ToList();

                        if (obj.transform.parent != null)
                        {
                            Comps.AddRange(obj.transform.parent.GetComponents<Component>());
                        }

                        var PathToObj = obj.transform.GetPathToObject().ToLower();
                        var LowerCaseName = obj.name.ToLower();

                        OldColours[PathToObj] = (new Color(obj.color.r, obj.color.g, obj.color.b, obj.color.a), ((Comps.ToComponentList<Button>() is var buttons && buttons.Count > 0) ? buttons.First()?.colors : null));

                        if (!obj.name.ToLower().Contains("panel"))
                        {
                            if (Comps.ToComponentList<Toggle>() is var ToggleComps && (ToggleComps.Count > 0 || PathToObj.Contains("toggle")))
                            {
                                obj.color = (PathToObj.Contains("off") || LowerCaseName.Contains("check")) ? new Color(ToggleRed, ToggleGreen, ToggleBlue, obj.color.a) : new Color(Mathf.Clamp(ToggleRed + 0.20f, 0f, 1f), Mathf.Clamp(ToggleGreen + 0.20f, 0f, 1f), Mathf.Clamp(ToggleBlue + 0.20f, 0f, 1f), obj.color.a);
                            }
                            else if (Comps.ToComponentList<Button>() is var ButtonComps && (ButtonComps.Count > 0 || PathToObj.Contains("button")))
                            {
                                if (ButtonComps.Count > 0 && ButtonComps.First().colors != null)
                                {
                                    ButtonComps.First().colors = new ColorBlock
                                    {
                                        colorMultiplier = 1f,

                                        normalColor = new Color(ToggleRed, ToggleGreen, ToggleBlue, obj.color.a),
                                        highlightedColor = new Color(Mathf.Clamp(ToggleRed + 0.05f, 0f, 1f), Mathf.Clamp(ToggleGreen + 0.05f, 0f, 1f), Mathf.Clamp(ToggleBlue + 0.05f, 0f, 1f), obj.color.a),
                                        pressedColor = new Color(ToggleRed, ToggleGreen, ToggleBlue, obj.color.a),
                                        selectedColor = new Color(Mathf.Clamp(ToggleRed + 0.10f, 0f, 1f), Mathf.Clamp(ToggleGreen + 0.10f, 0f, 1f), Mathf.Clamp(ToggleBlue + 0.10f, 0f, 1f), obj.color.a),

                                        disabledColor = new Color(Mathf.Clamp(ToggleRed + 0.30f, 0f, 1f), Mathf.Clamp(ToggleGreen + 0.30f, 0f, 1f), Mathf.Clamp(ToggleBlue + 0.30f, 0f, 1f), obj.color.a)
                                    };
                                }

                                obj.color = new Color(ButtonRed, ButtonGreen, ButtonBlue, obj.color.a);

                            }
                            else if (Comps.ToComponentList<Slider>() is var SliderComps && (SliderComps.Count > 0 || PathToObj.Contains("slider")))
                            {
                                obj.color = PathToObj.Contains("fill") ? new Color(Mathf.Clamp(SliderRed + 0.20f, 0f, 1f), Mathf.Clamp(SliderGreen + 0.20f, 0f, 1f), Mathf.Clamp(SliderBlue + 0.20f, 0f, 1f), obj.color.a) : new Color(SliderRed, SliderGreen, SliderBlue, obj.color.a);
                            }
                            else
                            {
                                obj.color = new Color(MiscRed, MiscGreen, MiscBlue, obj.color.a);
                            }
                        }
                        else
                        {
                            obj.color = new Color(MiscRed, MiscGreen, MiscBlue, obj.color.a);
                        }
                    }
                    else
                    {
                        var PathToObj = obj.transform.GetPathToObject().ToLower();

                        if (OldColours.ContainsKey(PathToObj))
                        {
                            obj.color = OldColours[PathToObj].Item1;

                            if (OldColours[PathToObj].Item2 != null)
                            {
                                var Comps = obj.transform.GetComponents<Component>().ToList();

                                if (obj.transform.parent != null)
                                {
                                    Comps.AddRange(obj.transform.parent.GetComponents<Component>());
                                }

                                var button = Comps.ToComponentList<Button>().FirstOrDefault();

                                if (button != null)
                                {
                                    button.colors = (ColorBlock)OldColours[PathToObj].Item2;
                                }
                            }
                        }
                    }

                    //MelonLogger.Warning($"Darkened: {obj.gameObject.name} To {obj.color.r}, {obj.color.g}, {obj.color.b}!");
                }
                catch
                {
                    MelonLogger.Error($"Failed To {(Enabled ? "Darken" : "Revert")} A Object!");
                }
            }

            MelonLogger.Warning("Done!");

            HasDarkened = true;

            if (!Enabled)
            {
                OldColours.Clear();
            }
        }
    }

    internal static class Helpers
    {
        internal static string GetPathToObject(this Transform obj)
        {
            var Path = obj.name;

            if (obj.transform.parent != null)
            {
                Path = GetPathToObject(obj.transform.parent) + "/" + Path;
            }

            return Path;
        }

        internal static List<T> ToComponentList<T>(this IEnumerable<Component> list) where T : Component
        {
            return list.OfILCastedType<T>();
        }

        public static List<T> OfILCastedType<T>(this IEnumerable<Component> source) where T : Component
        {
            return source == null ? null : OfTypeIterator<T>(source).ToList();
        }

        private static IEnumerable<T> OfTypeIterator<T>(IEnumerable<Component> source) where T : Component
        {
            foreach (var obj in source)
            {
                if (obj?.TryCast<T>() is var result && result != null)
                {
                    yield return result;
                }
            }
        }
    }
}
