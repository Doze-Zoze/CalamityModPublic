using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Healing
{
    public class SilvaOrb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silva Orb");
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 22;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            projectile.alpha -= 2;
            if (projectile.localAI[0] == 0f)
            {
                projectile.scale += 0.05f;
                if (projectile.scale > 1.2f)
                {
                    projectile.localAI[0] = 1f;
                }
            }
            else
            {
                projectile.scale -= 0.05f;
                if (projectile.scale < 0.8f)
                {
                    projectile.localAI[0] = 0f;
                }
            }

			CalamityGlobalProjectile.HealingProjectile(projectile, (int)projectile.ai[1], (int)projectile.ai[0], 6f, 15f);
            return;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, 203, 103, projectile.alpha);
        }

        public override void Kill(int timeLeft)
        {
            for (int num407 = 0; num407 < 5; num407++)
            {
                int num408 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 157, 0f, 0f, 0, new Color(Main.DiscoR, 203, 103), 1f);
                Main.dust[num408].noGravity = true;
                Main.dust[num408].velocity *= 1.5f;
                Main.dust[num408].scale = 1.5f;
            }
        }
    }
}
