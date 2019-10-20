using CalamityMod.Items.Materials;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureAshen
{
    public class AshenChandelier : ModItem
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
            item.rare = 3;
            item.consumable = true;
            item.value = 0;
            item.createTile = ModContent.TileType<Tiles.FurnitureAshen.AshenChandelier>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SmoothBrimstoneSlag>(), 4);
            recipe.AddIngredient(ModContent.ItemType<UnholyCore>());
            recipe.AddIngredient(ItemID.Chain);
            recipe.SetResult(this, 1);
            recipe.AddTile(ModContent.TileType<Tiles.FurnitureAshen.AshenAltar>());
            recipe.AddRecipe();
        }
    }
}
