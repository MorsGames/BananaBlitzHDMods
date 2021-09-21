using System.Collections.Generic;
using Framework.UI;
using Smbunity;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace GraphicalTweaks
{
    public static class Main
    {
        /// <summary>
        ///     Field of View of the camera. -1 sets it to default.
        /// </summary>
        public static float BaseFOV { get; set; } = -1f;

        /// <summary>
        ///     Makes the FOV increase with the player's speed.
        /// </summary>
        public static bool DynamicFOV { get; set; } = false;

        /// <summary>
        ///     How strong the post-processing effects should be. 0 means none, 1 means full.
        /// </summary>
        public static float PostProcessAmount { get; set; } = 1f;

        /// <summary>
        ///     Changes the method used for anti aliasing. -1 is default (None or FXAA depending on your settings), 0 is none, 1 is FXAA, 2 is SMAA, 3 is TAA.
        /// </summary>
        public static int AntiAliasingMode { get; set; } = -1;

        /// <summary>
        ///     Disables the depth of field effect.
        /// </summary>
        public static bool DisableDOF { get; set; } = false;

        /// <summary>
        ///     Disables the bloom effect.
        /// </summary>
        public static bool DisableBloom { get; set; } = false;

        /// <summary>
        ///     Disables color grading, making the final image much less vibrant.
        /// </summary>
        public static bool DisableColorGrading { get; set; } = false;

        /// <summary>
        ///     Disables the ambient occlusion effect.
        /// </summary>
        public static bool DisableAmbientOcclusion { get; set; } = false;

        /// <summary>
        ///     Disables the chromatic aberration effect.
        /// </summary>
        public static bool DisableChromaticAberration { get; set; } = false;

        /// <summary>
        ///     The scale of the HUD, as a multiplier.
        /// </summary>
        public static float HUDScale { get; set; } = 1f;

        /// <summary>
        ///     Whether if the HUD should be hidden during gameplay or not.
        /// </summary>
        public static bool HideHUD { get; set; } = false;

        private static bool _canModifyPostProcess = true;

        /// <summary>
        ///     When the mod is loaded at the very start of the game.
        /// </summary>
        /// <param name="settings">Settings for the mod.</param>
        public static void OnModLoad(Dictionary<string, object> settings)
        {
            // Load the settings

            BaseFOV = (float)settings["BaseFOV"];
            DynamicFOV = (bool)settings["DynamicFOV"];

            PostProcessAmount = (float)settings["PostProcessAmount"];
            AntiAliasingMode = (int)(float)settings["AntiAliasingMode"];
            DisableDOF = (bool)settings["DisableDOF"];
            DisableBloom = (bool)settings["DisableBloom"];
            DisableColorGrading = (bool)settings["DisableColorGrading"];
            DisableAmbientOcclusion = (bool)settings["DisableAmbientOcclusion"];
            DisableChromaticAberration = (bool)settings["DisableChromaticAberration"];

            HUDScale = (float)settings["HUDScale"];
            HideHUD = (bool)settings["HideHUD"];
        }

        /// <summary>
        ///     Called every frame when the mod is active.
        /// </summary>
        public static void OnModUpdate()
        {
            var cameraController = Object.FindObjectOfType<CameraController>();
            if (cameraController != null)
            {
                // Change the FOV
                var player = Object.FindObjectOfType<Player>();
                if (DynamicFOV && player != null)
                {
                    cameraController.SetFieldOfView(Mathf.Lerp(cameraController.GetMainCamera().fieldOfView,
                        (BaseFOV > 0 ? BaseFOV : 60) + Mathf.Clamp(player.velocity - 12, 0, 20) * 2.0f, 0.0625f));
                }
                else 
                {
                    if (BaseFOV > 0)
                        cameraController.SetFieldOfView(Mathf.Lerp(cameraController.GetMainCamera().fieldOfView,
                            BaseFOV, 0.03125f));
                }
            }

            // Find the Post Process Volume
            var postProcessVolume = Object.FindObjectOfType<PostProcessVolume>();

            // Ignore the following the Post Process Volume doesn't exist
            if (postProcessVolume != null)
            {
                if (_canModifyPostProcess)
                {
                    // Apply the settings
                    if (PostProcessAmount != 1f)
                        postProcessVolume.weight = PostProcessAmount;

                    if (AntiAliasingMode > -1)
                    {
                        var postProcessLayer = Object.FindObjectOfType<PostProcessLayer>();
                        if (postProcessLayer != null)
                            postProcessLayer.antialiasingMode = (PostProcessLayer.Antialiasing) AntiAliasingMode;
                    }

                    var profile = postProcessVolume.profile;

                    if (DisableDOF)
                    {
                        var dof = profile.GetSetting<DepthOfField>();
                        dof.enabled.value = false;
                    }

                    if (DisableBloom)
                    {
                        var bloom = profile.GetSetting<Bloom>();
                        bloom.enabled.value = false;
                    }

                    if (DisableColorGrading)
                    {
                        var colorGrading = profile.GetSetting<ColorGrading>();
                        colorGrading.enabled.value = false;
                    }

                    if (DisableAmbientOcclusion)
                    {
                        var ambientOcclusion = profile.GetSetting<AmbientOcclusion>();
                        ambientOcclusion.enabled.value = false;
                    }

                    if (DisableChromaticAberration)
                    {
                        var chromaticAberration = profile.GetSetting<ChromaticAberration>();
                        chromaticAberration.enabled.value = false;
                    }

                    // Let's not run all this every single frame
                    _canModifyPostProcess = false;
                }
            }
            else
            {
                _canModifyPostProcess = true;
            }

            // Find the UI Manager
            var uiManager = Object.FindObjectOfType<UIManager>();

            // Ignore the following the UI Manager doesn't exist
            if (uiManager != null)
            {
                // Apply the settings
                if (HUDScale != 1f && uiManager.DisplayScale != HUDScale)
                    uiManager.DisplayScale = HUDScale;
                if (HideHUD && uiManager.DisplayHUD)
                    uiManager.DisplayHUD = false;
            }
        }
    }
}