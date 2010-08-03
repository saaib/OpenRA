﻿#region Copyright & License Information
/*
 * Copyright 2007-2010 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made 
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation. For more information,
 * see LICENSE.
 */
#endregion


using OpenRA.Traits;
using System.Linq;
using OpenRA.FileFormats;

namespace OpenRA.Mods.RA
{
	// a small hack to teach Production about Reservable.

	public class ReservableProductionInfo : ProductionInfo, ITraitPrerequisite<ReservableInfo>
	{
		public override object Create(ActorInitializer init) { return new ReservableProduction(this); }
	}

	class ReservableProduction : Production
	{
		public ReservableProduction(ReservableProductionInfo info) : base(info) {}

		public override bool Produce(Actor self, ActorInfo producee)
		{
			if (Reservable.IsReserved(self))
				return false;

			// Pick a spawn/exit point
			// Todo: Reorder in a synced random way
			foreach (var s in Spawns)
			{
				var exit = self.Location + s.Second;
				var spawn = self.CenterLocation + s.First;
				if (!self.World.WorldActor.traits.Get<UnitInfluence>().GetUnitsAt( exit ).Any())
				{
					var newUnit = self.World.CreateActor( producee.Name, new TypeDictionary
					{
						new LocationInit( exit ),
						new OwnerInit( self.Owner ),
					});
					newUnit.CenterLocation = spawn;
		        	
					var rp = self.traits.GetOrDefault<RallyPoint>();
					if( rp != null )
					{
						newUnit.QueueActivity( new Activities.HeliFly( Util.CenterOfCell(rp.rallyPoint)) );
					}
					
					foreach (var t in self.traits.WithInterface<INotifyProduction>())
						t.UnitProduced(self, newUnit, exit);
		
					Log.Write("debug", "{0} #{1} produced by {2} #{3}", newUnit.Info.Name, newUnit.ActorID, self.Info.Name, self.ActorID);
					return true;
				}
			}
			return false;
		}
	}
}
