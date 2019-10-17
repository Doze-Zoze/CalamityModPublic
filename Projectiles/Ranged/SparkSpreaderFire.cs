using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader; using CalamityMod.Dusts;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Dusts; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Projectiles
{
    public class SparkSpreaderFire : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 3;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 60;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.05f / 255f, (255 - projectile.alpha) * 0.45f / 255f, (255 - projectile.alpha) * 0.05f / 255f);
            if (projectile.timeLeft > 60)
            {
                projectile.timeLeft = 60;
            }
            if (projectile.wet && !projectile.lavaWet)
            {
                projectile.Kill();
            }
            if (Main.rand.Next(3) == 0)
            {
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 35, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 80, default, 0.75f);
                Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 55, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 50, default, 0.75f);
            }
            if (projectile.ai[0] > 7f)
            {
                float num296 = 1f;
                if (projectile.ai[0] == 8f)
                {
                    num296 = 0.25f;
                }
                else if (projectile.ai[0] == 9f)
                {
                    num296 = 0.5f;
                }
                else if (projectile.ai[0] == 10f)
                {
                    num296 = 0.75f;
                }
                projectile.ai[0] += 1f;
                int num297 = 6;
                if (Main.rand.Next(1) == 0)
                {
                    for (int num298 = 0; num298 < 2; num298++)
                    {
                        int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 10, default, 0.75f);
                        if (Main.rand.Next(3) == 0)
                        {
                            Main.dust[num299].noGravity = true;
                            Main.dust[num299].scale *= 1.75f;
                            Dust expr_DBEF_cp_0 = Main.dust[num299];
                            expr_DBEF_cp_0.velocity.X *= 2f;
                            Dust expr_DC0F_cp_0 = Main.dust[num299];
                            expr_DC0F_cp_0.velocity.Y *= 2f;
                        }
                        else
                        {
                            Main.dust[num299].noGravity = true;
                            Main.dust[num299].scale *= 0.5f;
                        }
                        Dust expr_DC74_cp_0 = Main.dust[num299];
                        expr_DC74_cp_0.velocity.X *= 1.2f;
                        Dust expr_DC94_cp_0 = Main.dust[num299];
                        expr_DC94_cp_0.velocity.Y *= 1.2f;
                        Main.dust[num299].scale *= num296;
                        Main.dust[num299].velocity += projectile.velocity;
                    }
                }
            }
            else
            {
                projectile.ai[0] += 1f;
            }
            projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 60 * Main.rand.Next(1, 4));
        }
    }
}
