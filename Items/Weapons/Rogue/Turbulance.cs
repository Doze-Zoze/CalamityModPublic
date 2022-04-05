using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Turbulance : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Turbulance");
            Tooltip.SetDefault(@"Fires a cloudy javelin that bursts into wind slashes on hit
Wind slashes home if the javelin crits
Stealth strikes are trailed by homing wind slashes");
        }

        public override void SafeSetDefaults()
        {
            Item.width = 14;
            Item.damage = 18;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 18;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 14;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<TurbulanceProjectile>();
            Item.shootSpeed = 12f;
            Item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable()) //setting the stealth strike
            {
                int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
                if (stealth.WithinBounds(Main.maxProjectiles))
                    Main.projectile[stealth].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<AerialiteBar>(), 5).AddIngredient(ItemID.SunplateBlock, 3).AddIngredient(ItemID.Cloud, 4).AddTile(TileID.SkyMill).Register();
        }
    }
}
