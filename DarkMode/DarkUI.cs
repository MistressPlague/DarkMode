using MelonLoader;
using System;
using System.Collections;
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
        private int AmountOfSceneLoads = 0;
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
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

        internal IEnumerator OnUILoaded()
        {
            yield return new WaitForSeconds(10f);

            MelonLogger.Warning("Grabbing Images..");
            var ObjsWithColorsToInvert = Resources.FindObjectsOfTypeAll<Image>().Where(o => o != null).ToList();
            MelonLogger.Warning($"Darkening {ObjsWithColorsToInvert.Count} Colours..");

            foreach (var obj in ObjsWithColorsToInvert)
            {
                try
                {
                    if (obj.GetComponent<Button>() || obj.GetComponentInChildren<Button>(true) || (obj.transform.parent != null && obj.transform.parent.GetComponent<Button>()))
                    {
                        obj.color = new Color(obj.color.r / 1.15f, obj.color.g / 1.15f, obj.color.b / 1.15f, obj.color.a);
                    }
                    else if (obj.GetComponent<Toggle>() || obj.GetComponentInChildren<Toggle>(true) || (obj.transform.parent != null && obj.transform.parent.GetComponent<Toggle>()))
                    {
                        obj.color = new Color(obj.color.r / 1.5f, obj.color.g / 1.5f, obj.color.b / 1.5f, obj.color.a);
                    }
                    else
                    {
                        obj.color = new Color(obj.color.r / 2f, obj.color.g / 2f, obj.color.b / 2f, obj.color.a);
                    }

                    MelonLogger.Warning($"Darkened: {obj.gameObject.name} To {obj.color.r}, {obj.color.g}, {obj.color.b}!");
                }
                catch
                {
                    MelonLogger.Error($"Failed To Darken A Object!");
                }
            }

            MelonLogger.Warning("Done!");

            yield break;
        }
    }
}
