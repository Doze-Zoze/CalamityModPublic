using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class CoralCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coral Cannon");
            Tooltip.SetDefault("Has a chance to shoot a big coral that stuns enemies");
        }

        public override void SetDefaults()
        {
            item.damage = 20;
            item.ranged = true;
            item.width = 52;
            item.height = 40;
            item.crit += 10;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 7.5f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.UseSound = SoundID.Item61;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SmallCoral>();
            item.shootSpeed = 10f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (Main.rand.NextBool(5))
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<BigCoral>(), (int)((double)damage * 2.0), knockBack * 2f, player.whoAmI, 0.0f, 0.0f);
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            return false;
        }
    }
}
