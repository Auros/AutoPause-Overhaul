using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace AutoPause.Utilities
{
    public static class Sprites
    {
        public static Material NoGlowMat
        {
            get
            {
                if (noGlowMat == null)
                {
                    noGlowMat = new Material(Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").First());
                    noGlowMat.name = "UINoGlowCustom";
                }
                return noGlowMat;
            }
        }
        private static Material noGlowMat;
    }

    public class LoadScripts
    {
        static public Dictionary<string, Sprite> _cachedSprites = new Dictionary<string, Sprite>();

        public static IEnumerator LoadSpriteCoroutine(string spritePath, Action<Sprite> done)
        {
            Texture2D tex;

            if (_cachedSprites.ContainsKey(spritePath))
            {
                done?.Invoke(_cachedSprites[spritePath]);
                yield break;
            }

            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(spritePath))
            {
                yield return www.SendWebRequest();

                if (www.isHttpError || www.isNetworkError)
                {
                    Log.Error("Connection Error!");
                }
                else
                {
                    tex = DownloadHandlerTexture.GetContent(www);
                    yield return new WaitForSeconds(.05f);
                    var newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f, 100, 1);
                    _cachedSprites.Add(spritePath, newSprite);
                    done?.Invoke(newSprite);
                }
            }
        }
    }

    internal static class AutoPauseConfig
    {
        public static bool modEnabled = true;
        public static bool lowSense = false;
        public static bool senseiMain = true;
        public static bool senseiRed = false;

        public static void Load()
        {
            modEnabled = Plugin.AutoPauseConfig.GetBool("Main", "Enabled", true, true);
            lowSense = Plugin.AutoPauseConfig.GetBool("Main", "LowSense", false, true);
            lowSense = Plugin.AutoPauseConfig.GetBool("Main", "SensiMain", true, true);
            lowSense = Plugin.AutoPauseConfig.GetBool("Main", "SensiRed", false, true);
        }
    }
}
