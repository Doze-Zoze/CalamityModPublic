using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class FlashBullet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flash Round");
            Tooltip.SetDefault("Gives off a concussive blast that confuses enemies in a large area for a short time");
        }

        public override void SetDefaults()
        {
            item.damage = 7;
            item.ranged = true;
            item.width = 8;
            item.height = 8;
            item.maxStack = 999;
            item.consumable = true;
            item.knockBack = 1.15f;
            item.value = 250;
            item.rare = 1;
            item.shoot = ModContent.ProjectileType<Projectiles.FlashBullet>();
            item.shootSpeed = 12f;
            item.ammo = 97;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Glass, 3);
            recipe.AddIngredient(ItemID.Grenade);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 10);
            recipe.AddRecipe();
        }
    }
}
