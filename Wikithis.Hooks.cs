﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Wikithis
{
	partial class Wikithis
	{
		private void NPCURL(ILContext il)
		{
			ILCursor c = new(il);
			try
			{
				c.GotoNext(i => i.MatchCall(typeof(NPCLoader).GetMethod(nameof(NPCLoader.ModifyHoverBoundingBox), BindingFlags.Public | BindingFlags.Static)))
					.GotoNext(i => i.MatchStloc(14));

				c.Index++;
				c.Emit(OpCodes.Ldsfld, typeof(Main).GetField(nameof(Main.npc)))
					.Emit(OpCodes.Ldloc, 10)
					.Emit(OpCodes.Ldelem_Ref);
				c.Emit(OpCodes.Ldloc, 14);
				c.EmitDelegate<Action<NPC, bool>>((npc, hovers) =>
				{
					if (WikithisConfig.Config.CanWikiNPCs && hovers && WikithisSystem.WikiKeybind.JustPressed)
					{
						OpenWikiPage(this, npc);
					}
				});
			}
			catch (Exception e)
			{
				Logger.Error($"IL Error: {e.Message} {e.StackTrace}");
			}
		}
	}
}
