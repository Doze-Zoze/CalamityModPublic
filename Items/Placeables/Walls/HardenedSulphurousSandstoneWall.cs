using Terraria.ID;
using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
namespace CalamityMod.Items.Placeables.Walls
{
    public class HardenedSulphurousSandstoneWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
            // DisplayName.SetDefault("Hardened Sulphurous Sandstone Wall");
        }

        public override void SetDefaults()
        {
            Item.createWall = ModContent.WallType<WallTiles.HardenedSulphurousSandstoneWall>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 9999;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4).AddTile(TileID.WorkBenches).AddIngredient(ModContent.ItemType<HardenedSulphurousSandstone>()).Register();
        }
    }
}
