using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SeafoamBomb : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seafoam Bomb");
            Tooltip.SetDefault("Throws a bomb that explodes into a bubble which deals extra damage to enemies");
        }

        public override void SafeSetDefaults()
        {
            item.width = 26;
            item.height = 44;
            item.damage = 16;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 22;
            item.useStyle = 1;
            item.useTime = 22;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.shoot = ModContent.ProjectileType<SeafoamBomb>();
            item.shootSpeed = 8f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Bomb, 25);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
