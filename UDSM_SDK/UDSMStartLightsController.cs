// ============================================================
//  UDSM SDK — UDSMStartLightsController
//
//  Place this on the ROOT GameObject of your start-lights lamp post.
//  In Unity, it auto-renames the root to "start_lights" and each
//  assigned panel renderer's GameObject to "panel_1".."panel_4".
//
//  At runtime the mod finds the renamed GOs by name and drives
//  emission on each panel's material directly. The component itself
//  is stripped when CarX loads the map asset bundle (assemblies don't
//  match between the SDK Unity project and the mod DLL), so it has
//  NO runtime logic — naming is the only thing that has to survive.
//
//  Setup:
//    1. Build a lamp post with 4 panel meshes as children.
//    2. Add this component to the root.
//    3. Drag each panel's MeshRenderer into "Light Panels" top-to-bottom.
//    4. Make sure each panel has its OWN material instance and the
//       material's Emission is enabled (HDRP/Lit: tick "Emission Inputs").
//    5. Re-export the map. Done.
// ============================================================
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("UDSM/UDSM Start Lights Controller")]
public class UDSMStartLightsController : MonoBehaviour
{
    [Header("Panel renderers — assign top-to-bottom (panels 1, 2, 3, 4)")]
    [Tooltip("Drag each panel's Renderer here. The script renames the GameObject of each one to 'panel_1'..'panel_4' so the mod can find them by name at runtime.")]
    public Renderer[] LightPanels = new Renderer[4];

    public const string RootName = "start_lights";

    private void OnValidate()
    {
        // Force the root GO to be named "start_lights" — the mod looks for it by name.
        if (gameObject.name != RootName)
            gameObject.name = RootName;

        // Rename each assigned panel renderer's GameObject to panel_1..panel_4.
        for (int i = 0; i < LightPanels.Length && i < 4; i++)
        {
            var r = LightPanels[i];
            if (r == null) continue;
            string desired = $"panel_{i + 1}";
            if (r.gameObject.name != desired)
                r.gameObject.name = desired;
        }
    }
}
