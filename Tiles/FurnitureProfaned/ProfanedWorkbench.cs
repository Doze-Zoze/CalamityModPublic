﻿using CalamityMod.Dusts.Furniture;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.FurnitureProfaned
{
    public class ProfanedWorkbench : ModTile
    {
        public override void SetStaticDefaults() => this.SetUpWorkBench(ModContent.ItemType<Items.Placeables.FurnitureProfaned.ProfanedWorkBench>(), true);

        public override bool CreateDust(int i, int j, ref int type)
        {
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, 246, 0f, 0f, 1, new Color(255, 255, 255), 1f);
            Dust.NewDust(new Vector2(i, j) * 16f, 16, 16, ModContent.DustType<ProfanedTileRock>(), 0f, 0f, 1, new Color(255, 255, 255), 1f);
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
