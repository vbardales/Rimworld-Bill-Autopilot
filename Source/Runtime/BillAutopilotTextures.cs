using UnityEngine;
using Verse;

namespace BillAutopilot
{
    [StaticConstructorOnStartup]
    internal static class BillAutopilotTextures
    {
        public static readonly Texture2D Gizmo =
            ContentFinder<Texture2D>.Get("BillAutopilot/Autopilot", reportFailure: false)
            ?? ContentFinder<Texture2D>.Get("UI/Commands/ForbidOff", reportFailure: false)
            ?? BaseContent.BadTex;
    }
}
