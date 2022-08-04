﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Wikithis
{
	public partial class Wikithis : Mod
	{
		#region Fields and Field Init
		internal const string RickRoll = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";

		internal static bool AprilFools { get; private set; }
		internal static Dictionary<string, IWiki> Wikis { get; private set; }
		internal static Dictionary<(Mod, GameCulture.CultureName), string> ModToURL { get; private set; }
		internal static Dictionary<Mod, Asset<Texture2D>> ModToTexture { get; private set; }

		internal static Dictionary<(int, GameCulture.CultureName), string> ItemIdNameReplace { get; private set; }
		internal static Dictionary<(int, GameCulture.CultureName), string> NpcIdNameReplace { get; private set; }

		internal static GameCulture.CultureName CultureLoaded { get; private set; }
		#endregion

		#region Constructors
		public Wikithis()
		{
			Wikis = new();

			ModToURL = new();
			ModToTexture = new();

			ItemIdNameReplace = new();
			NpcIdNameReplace = new();

			AprilFools = DateTime.Now.Day == 1 && DateTime.Now.Month == 4;
		}
		#endregion

		#region Methods
		public override void Load()
		{
			CultureLoaded = (Language.ActiveCulture.Name == "en-US") ? GameCulture.CultureName.English :
				((Language.ActiveCulture.Name == "de-DE") ? GameCulture.CultureName.German :
				((Language.ActiveCulture.Name == "es-ES") ? GameCulture.CultureName.Spanish :
				((Language.ActiveCulture.Name == "fr-FR") ? GameCulture.CultureName.French :
				((Language.ActiveCulture.Name == "it-IT") ? GameCulture.CultureName.Italian :
				((Language.ActiveCulture.Name == "pl-PL") ? GameCulture.CultureName.Polish :
				((Language.ActiveCulture.Name == "pt-BR") ? GameCulture.CultureName.Portuguese :
				((Language.ActiveCulture.Name == "ru-RU") ? GameCulture.CultureName.Russian :
				((Language.ActiveCulture.Name == "zh-Hans") ? GameCulture.CultureName.Chinese : GameCulture.CultureName.English))))))));

			if (Main.dedServ)
				return;

			IL.Terraria.Main.DrawMouseOver += NPCURL;
		}

		internal static void SetupWikiPages()
		{
			foreach (IWiki wiki in Wikis.Values)
			{
				wiki.Initialize();
			}
		}

		public override void Unload()
		{
			Wikis = null;

			ModToURL = null;
			ModToTexture = null;

			ItemIdNameReplace = null;
			NpcIdNameReplace = null;

			AprilFools = false;
			CultureLoaded = 0;

			if (Main.dedServ)
				return;

			IL.Terraria.Main.DrawMouseOver -= NPCURL;
		}

		public override object Call(params object[] args)
		{
			Array.Resize(ref args, 4);
			const string success = "Success";

			try
			{
				string message = (args[0] as string)?.ToLower();
				int? messageOverload = args[0] as int?;
				const int index = 1;

				string[] first = new string[]
				{
					"AddModURL"
				};
				string[] second = new string[]
				{
					"ItemIdReplacement",
					"ItemIdReplacements",
					"ItemIdsReplacement",
					"ItemIdsReplacements",
					"ReplaceItemId",
					"ReplaceItemIds"
				};
				string[] third = new string[]
				{
					"NpcIdReplacement",
					"NpcIdReplacements",
					"NpcIdsReplacement",
					"NpcIdsReplacements",
					"ReplaceNpcId",
					"ReplaceNpcIds"
				};
				string[] fourth = new string[]
				{
					"AddWikiTexture",
					"WikiTexture",
					"AddWiki"
				};

				if (first.Any(x => x.ToLower() == message) || messageOverload.HasValue && messageOverload.Value == 0)
				{
					Mod mod = args[index + 0] as Mod;
					string domain = args[index + 1] as string;
					GameCulture.CultureName? culture = args[index + 2] as GameCulture.CultureName?;

					string nameOfArgument = string.Empty;
					if (mod == null)
						nameOfArgument = nameof(mod);
					if (domain == null)
						nameOfArgument = nameof(domain);
					if (culture == null)
						culture = GameCulture.CultureName.English;

					if (nameOfArgument != string.Empty)
						throw new ArgumentNullException($"Call Error: The {nameOfArgument} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned null.");

					if (culture != GameCulture.CultureName.English && ModToURL.ContainsKey(new(mod, GameCulture.CultureName.English)))
					{
						ModToURL.TryAdd((mod, culture.Value), domain);
					}
					else if (culture != GameCulture.CultureName.English && !ModToURL.ContainsKey(new(mod, GameCulture.CultureName.English)))
					{
						throw new Exception("English (default; main) key wasn't present in Dictionary, yet translations are being added!");
					}
					else
					{
						ModToURL.TryAdd((mod, culture.Value), domain);
					}
					return success;
				}
				else if (second.Any(x => x.ToLower() == message) || messageOverload.HasValue && messageOverload.Value == 1)
				{
					int? id2 = args[index + 0] as int?;
					List<int> id = args[index + 0] as List<int>;
					string name = args[index + 1] as string;
					GameCulture.CultureName? culture = args[index + 2] as GameCulture.CultureName?;

					/*if (id != null && id.Any(x => x <= 0 || x >= ItemLoader.ItemCount))
					{
						throw new IndexOutOfRangeException($"Call Error: The {id} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned one of its elements a less/equal to zero or bigger than/equal to maximum Item count.");
					}
					else if (id != null && id.Any(x => x > 0 || x < ItemID.Count))
					{
						throw new IndexOutOfRangeException($"Call Error: The {id} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned vanilla item ID.");
					}*/

					string nameOfArgument = string.Empty;
					if (id == null && id2 == null)
						nameOfArgument = nameof(id);
					if (name == null)
						nameOfArgument = nameof(name);
					if (culture == null)
						culture = GameCulture.CultureName.English;

					if (nameOfArgument != string.Empty)
						throw new ArgumentNullException($"Call Error: The {nameOfArgument} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned null.");

					if (id == null || id?.Count == 1)
					{
						ItemIdNameReplace.TryAdd((id2 ?? id[0], culture.Value), name);
					}
					else
					{
						foreach (int i in id)
						{
							ItemIdNameReplace.TryAdd((i, culture.Value), name);
						}
					}
					return success;
				}
				else if (third.Any(x => x.ToLower() == message) || messageOverload.HasValue && messageOverload.Value == 2)
				{
					int? id2 = args[index + 0] as int?;
					List<int> id = args[index + 0] as List<int>;
					string name = args[index + 1] as string;
					GameCulture.CultureName? culture = args[index + 2] as GameCulture.CultureName?;

					/*if (id != null && id.Any(x => x <= 0 || x >= NPCLoader.NPCCount))
					{
						throw new IndexOutOfRangeException($"Call Error: The {id} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned one of its elements a less/equal to zero or bigger than/equal to maximum NPC count.");
					}
					else if (id != null && id.Any(x => x >= NPCID.NegativeIDCount || x < NPCID.Count))
					{
						throw new IndexOutOfRangeException($"Call Error: The {id} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned vanilla NPC ID.");
					}*/

					string nameOfArgument = string.Empty;
					if (id == null && id2 == null)
						nameOfArgument = nameof(id);
					if (name == null)
						nameOfArgument = nameof(name);
					if (culture == null)
						culture = GameCulture.CultureName.English;

					if (nameOfArgument != string.Empty)
						throw new ArgumentNullException($"Call Error: The {nameOfArgument} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned null.");

					if (id == null || id?.Count == 1)
					{
						NpcIdNameReplace.TryAdd((id2 ?? id[0], culture.Value), name);
					}
					else
					{
						foreach (int i in id)
						{
							NpcIdNameReplace.TryAdd((i, culture.Value), name);
						}
					}
					return success;
				}
				else if (fourth.Any(x => x.ToLower() == message) || messageOverload.HasValue && messageOverload.Value == 3)
				{
					Mod mod = args[index + 0] as Mod;
					Asset<Texture2D> texture = args[index + 1] as Asset<Texture2D>;

					string nameOfArgument = string.Empty;
					if (mod == null)
						nameOfArgument = nameof(mod);
					if (texture == null)
						nameOfArgument = nameof(texture);

					if (nameOfArgument != string.Empty)
						throw new ArgumentNullException($"Call Error: The {nameOfArgument} argument for the attempted message, \"{message ?? messageOverload.ToString()}\" has returned null.");

					ModToTexture.TryAdd(mod, texture);
					return success;
				}
				else if (messageOverload.HasValue && messageOverload.Value == 4)
				{
				}
				else if (messageOverload.HasValue && messageOverload.Value == 5)
				{
				}
				else if (messageOverload.HasValue && messageOverload.Value == 6)
				{
				}
				else
				{
#pragma warning disable CA2208
					throw new ArgumentOutOfRangeException(nameof(messageOverload));
#pragma warning restore CA2208
				}
			}
			catch (Exception e)
			{
				Logger.Error($"Call Error: {e.StackTrace} {e.Message}");
			}

			return null;
		}
		#endregion

		#region Utilities
		internal static bool CheckURLValid(string s) => Uri.TryCreate(s, UriKind.Absolute, out Uri uriResult) && uriResult.Scheme == Uri.UriSchemeHttps;

		internal static string GetInternalName(int id, int num = 0) => num == 0 ? ItemID.Search.GetName(id) : NPCID.Search.GetName(id);

		public static string DefaultSearchStr(string name, Mod mod)
		{
			name = name.Replace(' ', '_');
			name = name.Replace("'", "%27");

			if (mod == null)
			{
				const int l = 25; // length of "https://terraria.wiki.gg/wiki/"

				string url = $"https://terraria.wiki.gg/wiki/{name}";
				if (CultureLoaded == GameCulture.CultureName.Polish)
					url = url.Insert(l, "pl/");
				else if (CultureLoaded == GameCulture.CultureName.Italian)
					url += "/it";
				else if (CultureLoaded == GameCulture.CultureName.French)
					url = url.Insert(l, "fr/");
				else if (CultureLoaded == GameCulture.CultureName.Spanish)
					url = url.Insert(l, "pl/");
				else if (CultureLoaded == GameCulture.CultureName.Russian)
					url = url.Insert(l, "ru/");
				else if (CultureLoaded == GameCulture.CultureName.German)
					url = url.Insert(l, "de/");
				else if (CultureLoaded == GameCulture.CultureName.Portuguese)
					url = url.Insert(l, "pt/");
				else if (CultureLoaded == GameCulture.CultureName.Chinese)
					url = url.Insert(l, "zh/");

				return url;
			}
			else
			{
				string url = string.Empty;
				bool success = false;

				GameCulture.CultureName culture = CultureLoaded;

				bool doesntContainsOthers = ModToURL.TryGetValue(new ValueTuple<Mod, GameCulture.CultureName>(mod, culture), out _);
				if (!doesntContainsOthers)
					culture = GameCulture.CultureName.English;

				if (ModToURL.TryGetValue((mod, culture), out string value))
				{
					success = true;
					url = value;
				}

				if (!success)
					return string.Empty;

				string[] urls = url.Split('$');
				string[] urls2 = url.Split('♛');
				string result = $"https://{urls[0]}/wiki";

				if (urls.Length >= 2)
				{
					foreach (string v in urls.AsSpan(1))
					{
						result += $"/{v}";
					}
				}

				result += $"/{name}";
				if (urls2.Length > 1)
					result += $"/{urls2[1]}";

				return CheckURLValid(result) ? result : string.Empty;
			}
		}

		internal static void OpenWikiPage(Mod mod, Item item)
		{
			IWiki<Item, int> wiki = Wikis[$"Wikithis/{nameof(ItemWiki)}"] as IWiki<Item, int>;

			if (wiki.HasEntryAndIsValid(item.type))
			{
				wiki.GetEntry(item.type).OpenWikiPage(false);
			}
			else
			{
				Main.NewText(Language.GetTextValue($"Mods.{mod.Name}.Error"), Color.OrangeRed);

				bool bl = ModToURL.ContainsKey((item.ModItem?.Mod, CultureLoaded));
				if (!bl)
					bl = ModToURL.ContainsKey((item.ModItem?.Mod, GameCulture.CultureName.English));

				mod.Logger.Error("Tried to get wiki page, but failed!");
				mod.Logger.Info("Type: " + item.type.ToString());
				mod.Logger.Info("Name: " + item.Name);
				mod.Logger.Info("Vanilla: " + (item.ModItem == null).ToString());
				mod.Logger.Info("Mod: " + item.ModItem?.Mod.Name);
				mod.Logger.Info("Domain in dictionary: " + (item.ModItem != null ? bl.ToString() : "False"));
			}
		}

		internal static void OpenWikiPage(Mod mod, NPC npc)
		{
			IWiki<Item, int> wiki = Wikis[$"Wikithis/{nameof(NPCWiki)}"] as IWiki<Item, int>;

			if (wiki.HasEntryAndIsValid(npc.type))
			{
				wiki.GetEntry(npc.type).OpenWikiPage(false);
			}
			else
			{
				Main.NewText(Language.GetTextValue($"Mods.{mod.Name}.Error"), Color.OrangeRed);

				bool bl = ModToURL.ContainsKey((npc.ModNPC?.Mod, CultureLoaded));
				if (!bl)
					bl = ModToURL.ContainsKey((npc.ModNPC?.Mod, GameCulture.CultureName.English));

				mod.Logger.Error("Tried to get wiki page, but failed!");
				mod.Logger.Info("Type: " + npc.type.ToString());
				mod.Logger.Info("Name: " + npc.GivenOrTypeName);
				mod.Logger.Info("Vanilla: " + (npc.ModNPC == null).ToString());
				mod.Logger.Info("Mod: " + npc.ModNPC?.Mod.Name);
				mod.Logger.Info("Domain in dictionary: " + (npc.ModNPC != null ? bl.ToString() : "False"));
			}
		}

		internal static string TooltipHotkeyString(ModKeybind keybind)
		{
			if (Main.dedServ || keybind == null)
				return string.Empty;

			List<string> assignedKeys = keybind.GetAssignedKeys();
			if (assignedKeys.Count == 0)
				return "[NONE]";

			StringBuilder stringBuilder = new(16);
			stringBuilder.Append(assignedKeys[0]);
			for (int index = 1; index < assignedKeys.Count; ++index)
				stringBuilder.Append(" / ").Append(assignedKeys[index]);

			return stringBuilder.ToString();
		}
		#endregion
	}
}