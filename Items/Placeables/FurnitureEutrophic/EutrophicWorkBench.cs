using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureEutrophic
{
    public class EutrophicWorkBench : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.FurnitureEutrophic.EutrophicWorkBench>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Navystone>(), 10);
            recipe.SetResult(this, 1);
            recipe.AddTile(ModContent.TileType<EutrophicCrafting>());
            recipe.AddRecipe();
        }
    }
}
