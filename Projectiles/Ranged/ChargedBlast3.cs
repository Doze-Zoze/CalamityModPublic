using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ChargedBlast3 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blast");
        }

        public override void SetDefaults()
        {
            projectile.width = 5;
            projectile.height = 5;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 4;
            projectile.extraUpdates = 3;
            projectile.alpha = 255;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 25;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0f, 0.3f, 0.7f);
            float num55 = 50f;
            float num56 = 1.5f;
            if (projectile.ai[1] == 0f)
            {
                projectile.localAI[0] += num56;
                if (projectile.localAI[0] > num55)
                {
                    projectile.localAI[0] = num55;
                }
            }
            else
            {
                projectile.localAI[0] -= num56;
                if (projectile.localAI[0] <= 0f)
                {
                    projectile.Kill();
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(100, 100, 255, 0);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Color color25 = Lighting.GetColor((int)((double)projectile.position.X + (double)projectile.width * 0.5) / 16, (int)(((double)projectile.position.Y + (double)projectile.height * 0.5) / 16.0));
            int num147 = 0;
            int num148 = 0;
            float num149 = (float)(Main.projectileTexture[projectile.type].Width - projectile.width) * 0.5f + (float)projectile.width * 0.5f;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Rectangle value6 = new Rectangle((int)Main.screenPosition.X - 500, (int)Main.screenPosition.Y - 500, Main.screenWidth + 1000, Main.screenHeight + 1000);
            if (projectile.getRect().Intersects(value6))
            {
                Vector2 value7 = new Vector2(projectile.position.X - Main.screenPosition.X + num149 + (float)num148, projectile.position.Y - Main.screenPosition.Y + (float)(projectile.height / 2) + projectile.gfxOffY);
                float num162 = 50f;
                float scaleFactor = 1.5f;
                if (projectile.ai[1] == 1f)
                {
                    num162 = (float)(int)projectile.localAI[0];
                }
                for (int num163 = 1; num163 <= (int)projectile.localAI[0]; num163++)
                {
                    Vector2 value8 = Vector2.Normalize(projectile.velocity) * (float)num163 * scaleFactor;
                    Color color29 = projectile.GetAlpha(color25);
                    color29 *= (num162 - (float)num163) / num162;
                    color29.A = 0;
                    Main.spriteBatch.Draw(Main.projectileTexture[projectile.type], value7 - value8, null, color29, projectile.rotation, new Vector2(num149, (float)(projectile.height / 2 + num147)), projectile.scale, spriteEffects, 0f);
                }
            }
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<ChargedDartExplosion>(), projectile.damage / 4, projectile.knockBack, projectile.owner, 0f, 0f);
            Main.PlaySound(SoundID.Item62, (int)projectile.position.X, (int)projectile.position.Y);
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                projectile.ai[0] += 0.1f;
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<ChargedDartExplosion>(), projectile.damage / 4, projectile.knockBack, projectile.owner, 1f, 0f);
            Main.PlaySound(SoundID.Item62, (int)projectile.position.X, (int)projectile.position.Y);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 2;
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<ChargedDartExplosion>(), projectile.damage / 4, projectile.knockBack, projectile.owner, 0f, 0f);
        }
    }
}
