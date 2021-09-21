using System.Collections.Generic;
using Smbunity;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace BBRemastered
{
    public static class Main
    {
        /// <summary>
        ///     The amount of bloom.
        /// </summary>
        public static float SwagLevel { get; set; } = 1f;

        /// <summary>
        ///     The drug mode.
        /// </summary>
        public static bool BananaPowder { get; set; } = false;

        private static bool _canModifyPostProcess = true;

        /// <summary>
        ///     When the mod is loaded at the very start of the game.
        /// </summary>
        /// <param name="settings">Settings for the mod.</param>
        public static void OnModLoad(Dictionary<string, object> settings)
        {
            // Load the settings
            SwagLevel = (float)settings["SwagLevel"];
            BananaPowder = (bool)settings["BananaPowder"];
        }

        /// <summary>
        ///     Called every frame when the mod is active.
        /// </summary>
        public static void OnModUpdate()
        {
            // Find the Post Process Volume
            var postProcessVolume = Object.FindObjectOfType<PostProcessVolume>();

            // Ignore the following the Post Process Volume doesn't exist
            if (postProcessVolume != null)
            {
                if (_canModifyPostProcess)
                {
                    // If the bloom settings are normal, change them
                    var profile = postProcessVolume.profile;

                    var chromaticAberration = profile.GetSetting<ChromaticAberration>();
                    if (BananaPowder)
                    {
                        chromaticAberration.intensity.value = SwagLevel * 6f;
                    }
                    else
                    {
                        chromaticAberration.intensity.value = 1f;
                        var bloom = profile.GetSetting<Bloom>();
                        bloom.intensity.value = SwagLevel * 2f;
                        bloom.threshold.value = 0f;
                        var colorGrading = profile.GetSetting<ColorGrading>();
                        colorGrading.contrast.value = 100f;
                        colorGrading.saturation.value = -50f;
                        colorGrading.colorFilter.value = new Color(0.4375f, 0.375f, 0.3125f);
                    }

                    var motionBlur = profile.AddSettings<MotionBlur>();

                    // Let's not run all this every single frame
                    _canModifyPostProcess = false;
                }
            }
            else
            {
                _canModifyPostProcess = true;
            }
        }
    }
}