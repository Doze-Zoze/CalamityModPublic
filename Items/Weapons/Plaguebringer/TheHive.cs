using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class TheHive : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Hive");
            Tooltip.SetDefault("Launches a variety of rockets that explode into bees on death");
        }

        public override void SetDefaults()
        {
            item.damage = 70;
            item.ranged = true;
            item.width = 62;
            item.height = 30;
            item.useTime = 21;
            item.useAnimation = 21;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 3.5f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
            item.UseSound = SoundID.Item61;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Projectiles.BeeRPG>();
            item.shootSpeed = 13f;
            item.useAmmo = 771;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            switch (Main.rand.Next(4))
            {
                case 0:
                    type = ModContent.ProjectileType<Projectiles.GoliathRocket>();
                    break;
                case 1:
                    type = ModContent.ProjectileType<Projectiles.HiveMissile>();
                    break;
                case 2:
                    type = ModContent.ProjectileType<Projectiles.HiveBomb>();
                    break;
                case 3:
                    type = ModContent.ProjectileType<Projectiles.BeeRPG>();
                    break;
                default:
                    break;
            }
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
