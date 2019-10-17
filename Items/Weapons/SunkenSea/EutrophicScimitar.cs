using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class EutrophicScimitar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eutrophic Scimitar");
            Tooltip.SetDefault("Fires a beam upon swing that stuns enemies");
        }

        public override void SetDefaults()
        {
            item.damage = 130;
            item.melee = true;
            item.width = 46;
            item.height = 76;
            item.useTime = 38;
            item.useAnimation = 38;
            item.useStyle = 1;
            item.knockBack = 2;
            item.shoot = ModContent.ProjectileType<Projectiles.EutrophicScimitarProj>();
            item.shootSpeed = 17;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.useTurn = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i <= 21; i++)
            {
                Dust dust;
                dust = Main.dust[Terraria.Dust.NewDust(new Vector2(position.X - 58 / 2, position.Y - 58 / 2), 58, 58, 226, 0f, 0f, 0, new Color(255, 255, 255), 0.4605263f)];
                dust.noGravity = true;
                dust.fadeIn = 0.9473684f;
            }
            return true;
        }
    }
}
