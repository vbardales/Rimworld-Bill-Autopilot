// Krafs.Publicizer publicises one field of the reference assembly, named in the csproj:
// Verse.PawnRenderer.pawn. That is what lets Patch_PawnRenderer_RenderPawnAt read __instance.pawn
// and find out whose renderer it is postfixing. In the real Assembly-CSharp the field is private:
// the compiler emits a plain cross-assembly ldfld either way, and the desktop CLR allows that
// instruction only when this assembly declares the waiver below.
//
// Publicizer defines the attribute type for us and normally applies it through the SDK's generated
// AssemblyInfo - which this project switches off with GenerateAssemblyInfo=false. The type was
// therefore embedded and the waiver was not, with nothing in the build or the log to say so.
//
// What that costs at runtime is NOT established. The Architect Studio session found a non-public
// call of its own, outside any try/catch, in an assembly with no waiver, working in a real game:
// RimWorld's Mono does not appear to enforce the check the way the desktop CLR does. So this file
// is a conformance fix - one line, no risk, and what Publicizer intends - not the repair of a mod
// anyone has seen dead. Nobody has produced a Player.log with the exception in it.
//
// Found without launching the game, by reading this assembly's own attribute list: the type name
// is present in the file whether or not the waiver was applied, so grepping the bytes proves
// nothing. Read GetCustomAttributesData(), not the strings.

[assembly: System.Runtime.CompilerServices.IgnoresAccessChecksTo("Assembly-CSharp")]
