using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class AbyssLamp : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.value = 0;
            item.createTile = ModContent.TileType<Tiles.AbyssLamp>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "SmoothAbyssGravel");
            recipe.AddIngredient(mod.GetItem("Lumenite"), 3);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "VoidCondenser");
            recipe.AddRecipe();
        }
    }
}
