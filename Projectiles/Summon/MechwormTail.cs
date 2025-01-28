using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MechwormTail : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public int segmentIndex = 1;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.netImportant = true;
            Projectile.timeLeft = 18000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 5;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Summon;
        }


        public override void AI()
        {
            if (Main.player[Projectile.owner].Calamity().mWorm)
            {
                Projectile.timeLeft = 10;
            } else
            {
                Projectile.Kill();
            }
        }


        //this works exactly the same as the MechwormBody SegmentMove, so check comments there
        internal void SegmentMove()
        {
            Player player = Main.player[Projectile.owner];
            var live = false;
            Projectile nextSegment = new Projectile();
            MechwormHead head = new MechwormHead();



            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.type == ModContent.ProjectileType<MechwormBody>() && projectile.owner == Projectile.owner && projectile.active)
                {
                    if (projectile.ModProjectile<MechwormBody>().segmentIndex == segmentIndex - 1)
                    {
                        live = true;
                        nextSegment = projectile;
                    }
                }
                if (projectile.type == ModContent.ProjectileType<MechwormHead>() && projectile.owner == Projectile.owner && projectile.active)
                {
                    if (segmentIndex == 1)
                    {
                        live = true;
                        nextSegment = projectile;
                    }
                    head = projectile.ModProjectile<MechwormHead>();
                }
            }
            if (!live) Projectile.Kill();
            if (Projectile.alpha <= 128)
                Lighting.AddLight(Projectile.Center, Color.DarkMagenta.ToVector3());
            if (head.Projectile.netUpdate)
            {
                Projectile.netUpdate = true;
                if (Projectile.netSpam > 59)
                    Projectile.netSpam = 59;
            }
            Projectile.extraUpdates = head.Projectile.extraUpdates;
            if (head.EndRiftGateUUID == -1)
            {
                Projectile.alpha = Utils.Clamp(Projectile.alpha - 16, 0, 255);
            }
            else if (Projectile.Hitbox.Intersects(Main.projectile[head.EndRiftGateUUID].Hitbox))
            {
                Projectile.alpha = 255;
            }
            Vector2 destinationOffset = nextSegment.Center - Projectile.Center;
            if (nextSegment.rotation != Projectile.rotation)
            {
                float angle = MathHelper.WrapAngle(nextSegment.rotation - Projectile.rotation);
                destinationOffset = destinationOffset.RotatedBy(angle * 0.1f);
            }
            Projectile.rotation = destinationOffset.ToRotation();
            if (destinationOffset != Vector2.Zero)
            {
                Projectile.Center = nextSegment.Center - destinationOffset.SafeNormalize(Vector2.Zero) * 20f;
            }
            Projectile.Center = Vector2.Clamp(Projectile.Center, MechwormHead.WorldTopLeft(10), MechwormHead.WorldBottomRight(10));
            Projectile.velocity = Vector2.Zero;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 90);

        public override bool PreDraw(ref Color lightColor)
        {
            //drawn by head
            return false;
        }

        public override bool? CanDamage() => Projectile.alpha == 0;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
    }
}
