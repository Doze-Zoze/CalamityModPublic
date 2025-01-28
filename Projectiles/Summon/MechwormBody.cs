using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MechwormBody : ModProjectile, ILocalizedModType
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
            Projectile.minionSlots = 0.5f;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.hide = true;
            Projectile.DamageType = DamageClass.Summon;

        }

        public override void OnSpawn(IEntitySource source)
        {
            //Find an existing tail from the same owner, and take it's place while increasing it's index by one.
            foreach (var projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<MechwormTail>() && projectile.owner == Projectile.owner && projectile.active)
                {
                    segmentIndex = projectile.ModProjectile<MechwormTail>().segmentIndex;
                    projectile.ModProjectile<MechwormTail>().segmentIndex++;
                }
            }
        }
        public override void AI()
        {
            if (segmentIndex == -1)
            {
                //Find an existing tail from the same owner, and take it's place while increasing it's index by one.
                foreach (var projectile in Main.projectile)
                {
                    if (projectile.type == ModContent.ProjectileType<MechwormTail>() && projectile.owner == Projectile.owner && projectile.active)
                    {
                        segmentIndex = projectile.ModProjectile<MechwormTail>().segmentIndex;
                        projectile.ModProjectile<MechwormTail>().segmentIndex++;
                    }
                }
            }
            Player player = Main.player[Projectile.owner];
            if (Main.player[Projectile.owner].Calamity().mWorm)
            {
                Projectile.timeLeft = 10;
            }
            else
            {
                Projectile.Kill();
            }
        }

        // called by the head to allow us to update in order head -> tail, fixing segment spacing issues caused by inconsistent update ordering of the projectiles.
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

            // If the head is set to net update every body segment will also update.
            // This update cannot be blocked by netSpam.
            if (head.Projectile.netUpdate)
            {
                Projectile.netUpdate = true;
                if (Projectile.netSpam > 59)
                    Projectile.netSpam = 59;
            }

            Projectile.extraUpdates = head.Projectile.extraUpdates;

            if (head.EndRiftGateUUID == -1)
            {
                // Very rapidly fade-in.
                Projectile.alpha = Utils.Clamp(Projectile.alpha - 16, 0, 255);
            }
            else if (Projectile.Hitbox.Intersects(Main.projectile[head.EndRiftGateUUID].Hitbox))
            {
                // Disappear if touching the mechworm portal.
                // It will look like it's teleporting, when in reality, it's
                // just an invisible, uninteractable projectile for the time being.
                Projectile.alpha = 255;
            }

            //Movement code
            Vector2 destinationOffset = nextSegment.Center - Projectile.Center;
            if (nextSegment.rotation != Projectile.rotation)
            {
                float angle = MathHelper.WrapAngle(nextSegment.rotation - Projectile.rotation);
                destinationOffset = destinationOffset.RotatedBy(angle * 0.08f);
            }
            Projectile.rotation = destinationOffset.ToRotation();
            if (destinationOffset != Vector2.Zero)
            {
                Projectile.Center = nextSegment.Center - destinationOffset.SafeNormalize(Vector2.Zero) * 20f; //Moves the segment to a position offset from the prior segment. The factor at the end is the distance away.
            }
          
            Projectile.Center = Vector2.Clamp(Projectile.Center, MechwormHead.WorldTopLeft(10), MechwormHead.WorldBottomRight(10)); // Stops the mechworm from getting too close to the world boundary. Projectiles can instantly cause crashes when they cross the world boundary.
            Projectile.velocity = Vector2.Zero;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);

        public override bool PreDraw(ref Color lightColor)
        {
            //is drawn by the head
            return false;
        }

        public override bool? CanDamage() => Projectile.alpha == 0;

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
    }
}
