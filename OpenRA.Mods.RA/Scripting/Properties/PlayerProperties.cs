#region Copyright & License Information
/*
 * Copyright 2007-2015 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation. For more information,
 * see COPYING.
 */
#endregion

using System;
using System.Linq;
using Eluant;
using OpenRA.Mods.Common.Traits;
using OpenRA.Mods.RA.Traits;
using OpenRA.Scripting;

namespace OpenRA.Mods.RA.Scripting
{
	[ScriptPropertyGroup("Player")]
	public class PlayerProperties : ScriptPlayerProperties
	{
		public PlayerProperties(ScriptContext context, Player player)
			: base(context, player) { }

		[Desc("The player's name.")]
		public string Name { get { return Player.PlayerName; } }

		[Desc("Returns an array of actors representing all ground attack units of this player.")]
		public Actor[] GetGroundAttackers()
		{
			return Player.World.ActorsWithTrait<AttackBase>().Select(a => a.Actor)
				.Where(a => a.Owner == Player && !a.IsDead && a.IsInWorld && a.HasTrait<Mobile>())
				.ToArray();
		}
	}
}
