﻿using CalamityMod.Items.Placeables.Furniture.Trophies;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Furniture.BossTrophies
{
    public class ApolloTrophyTile : ModTile
    {
        public override void SetStaticDefaults() => this.SetUpTrophy();
    }
}
