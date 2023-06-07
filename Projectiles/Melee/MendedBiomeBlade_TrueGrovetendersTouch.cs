﻿using CalamityMod.DataStructures;
using CalamityMod.Particles;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class TrueGrovetendersTouch : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        private NPC[] excludedTargets = new NPC[4];
        public override string Texture => "CalamityMod/Projectiles/Melee/MendedBiomeBlade_GrovetendersTouchBlade";
        private bool initialized = false;
        public Player Owner => Main.player[Projectile.owner];
        public float Timer => MaxTime - Projectile.timeLeft;
        public ref float HasSnapped => ref Projectile.ai[0];
        public ref float SnapCoyoteTime => ref Projectile.ai[1];
        public ref float Reach => ref Projectile.localAI[0];

        public float flipped;

        const float MaxTime = 90;
        const int coyoteTimeFrames = 15; //How many frames does the whip stay extended
        const int MaxReach = 400;
        const int MinReach = 300;
        const float SnappingPoint = 0.55f; //When does the snap occur.
        const float ReelBackStrenght = 14f;

        const float MaxTangleReach = 400f; //How long can tangling vines from crits be

        public BezierCurve curve;
        private Vector2 controlPoint1;
        private Vector2 controlPoint2;
        internal bool ReelingBack => Timer / MaxTime > SnappingPoint;

        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 80;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = TrueBiomeBlade.TropicalAttunement_LocalIFrames;
        }

        public override bool? CanCutTiles() => false; //Itd be quite counterproductive to make the whip cut the tiles it just grew

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            BezierCurve curve = new(new Vector2[] { Owner.MountedCenter, controlPoint1, controlPoint2, Projectile.Center });

            int numPoints = 32;
            Vector2[] chainPositions = curve.GetPoints(numPoints).ToArray();
            float collisionPoint = 0;
            for (int i = 1; i < numPoints; i++)
            {
                Vector2 position = chainPositions[i];
                Vector2 previousPosition = chainPositions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                    return true;
                if (i == numPoints - 1) //Extra length collision for the blade itself
                {
                    Vector2 projectileHalfLength = 85 * Projectile.rotation.ToRotationVector2();
                    return (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - projectileHalfLength, Projectile.Center + projectileHalfLength, 32, ref collisionPoint));
                }

            }

            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Vector2 projectileHalfLength = 85f * Projectile.rotation.ToRotationVector2();
            float collisionPoint = 0;
            //If you hit the enemy during the coyote time with the blade of the whip, guarantee a crit & get some bonus damage
            if (Collision.CheckAABBvLineCollision(target.Hitbox.TopLeft(), target.Hitbox.Size(), Projectile.Center - projectileHalfLength, Projectile.Center + projectileHalfLength, 32, ref collisionPoint))
            {
                if (SnapCoyoteTime > 0f)
                {
                    modifiers.SourceDamage *= TrueBiomeBlade.TropicalAttunement_SweetSpotDamageMultiplier;
                    modifiers.SetCrit();
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 sparkSpeed = Owner.DirectionTo(target.Center).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2)) * 9f;
                        Particle Spark = new CritSpark(target.Center, sparkSpeed, Color.White, Color.LimeGreen, 1f + Main.rand.NextFloat(0, 1f), 30, 0.4f, 0.6f);
                        GeneralParticleHandler.SpawnParticle(Spark);
                    }
                }
            }
            else
                modifiers.SourceDamage *= TrueBiomeBlade.TropicalAttunement_ChainDamageReduction; //If the enemy is hit with the chain of the whip, the damage gets reduced
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
            {
                bool boing = false;
                excludedTargets[0] = target;
                for (int i = 0; i < 3; i++)
                {
                    NPC potentialTarget = TargetNext(target.Center, i);
                    if (potentialTarget == null)
                        break;
                    if (!boing)
                    {
                        boing = true;
                        SoundEngine.PlaySound(SoundID.Item56);
                    }
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ProjectileType<GrovetendersEntanglingVines>(), (int)(damageDone * TrueBiomeBlade.TropicalAttunement_VineDamageReduction), 0, Owner.whoAmI, target.whoAmI, potentialTarget.whoAmI);
                }
                Array.Clear(excludedTargets, 0, 3);
            }
        }

        public NPC TargetNext(Vector2 hitFrom, int index)
        {
            float longestReach = MaxTangleReach;
            NPC target = null;
            for (int i = 0; i < Main.maxNPCs; ++i)
            {
                NPC npc = Main.npc[i];
                if (!excludedTargets.Contains(npc) && npc.CanBeChasedBy() && !npc.friendly && !npc.townNPC)
                {
                    float distance = Vector2.Distance(hitFrom, npc.Center);
                    if (distance < longestReach)
                    {
                        longestReach = distance;
                        target = npc;
                    }
                }
            }
            if (index < 3)
                excludedTargets[index + 1] = target;
            return target;
        }

        public override void AI()
        {
            if (!initialized) //Initialization. create control points & shit)
            {
                Projectile.velocity = Owner.SafeDirectionTo(Owner.Calamity().mouseWorld, Vector2.Zero);
                Reach = MathHelper.Clamp((Owner.Center - Owner.Calamity().mouseWorld).Length(), MinReach, MaxReach);
                SoundEngine.PlaySound(SoundID.DD2_OgreSpit, Projectile.Center);
                controlPoint1 = Projectile.Center;
                controlPoint2 = Projectile.Center;
                Projectile.timeLeft = (int)MaxTime;
                initialized = true;
                Projectile.netUpdate = true;
                Projectile.netSpam = 0;
            }

            if (ReelingBack && HasSnapped == 0f) //Snap & also small coyote time for the hook
            {
                SoundEngine.PlaySound(SoundID.Item41, Projectile.Center); //Snap
                HasSnapped = 1f;
                SnapCoyoteTime = coyoteTimeFrames;
            }

            if (SnapCoyoteTime > 0) //keep checking for the tile hook
            {
                Lighting.AddLight(Projectile.Center, 0.8f, 1f, 0.35f);
                HookToTile();
                SnapCoyoteTime--;
            }

            Owner.direction = Math.Sign(Projectile.velocity.X);
            Projectile.rotation = Projectile.AngleFrom(Owner.Center); //Point away from playah

            float ratio = GetSwingRatio();
            Projectile.Center = Owner.MountedCenter + SwingPosition(ratio);
            Projectile.direction = Projectile.spriteDirection = -Owner.direction * (int)flipped;

            //MessWithTiles();

            Owner.itemRotation = MathHelper.WrapAngle(Owner.AngleTo(Owner.Calamity().mouseWorld) - (Owner.direction < 0 ? MathHelper.Pi : 0));
        }
        public void HookToTile()
        {
            if (Main.myPlayer == Owner.whoAmI)
            {
                //Shmoove the player if a tile is hit. This movement always happens if the owner isnt on the ground, but will only happen if the projectile is above the player if they are standing on the ground)
                if (Collision.SolidCollision(Projectile.position, 32, 32) && (Owner.velocity.Y != 0 || Projectile.position.Y < Owner.position.Y))
                {
                    Owner.velocity = Owner.SafeDirectionTo(Projectile.Center, Vector2.Zero) * ReelBackStrenght;
                    SnapCoyoteTime = 0f;
                }
                SoundEngine.PlaySound(SoundID.Item65, Projectile.position);
            }
        }

        internal static float EaseInFunction(float progress) => progress == 0 ? 0f : (float)Math.Pow(2, 10 * progress - 10); //Potion seller i need your strongest easeIns

        private float GetSwingRatio()
        {
            float ratio = (Timer - SnappingPoint * MaxTime) / (MaxTime * (1 - SnappingPoint)); //The ratio for the last section of the snap is a new curve.
            if (!ReelingBack)
                ratio = EaseInFunction(Timer / (MaxTime * SnappingPoint));  //Watch this ratio get eased
            if (SnapCoyoteTime > 0)
                ratio = 0f;
            return ratio;
        }

        private Vector2 SwingPosition(float progress)
        {
            //Whip windup and snap part
            if (!ReelingBack)
            {
                float distance = Reach * MathHelper.Lerp((float)Math.Sin(progress * MathHelper.PiOver2), 1, 0.04f); //half arc
                distance = Math.Max(distance, 65); //Dont be too close to player

                float angleDeviation = MathHelper.Pi / 1.2f;
                float angleOffset = Owner.direction * flipped * MathHelper.Lerp(-angleDeviation, 0, progress); //Go from very angled to straight at the zenith of the attack
                if (flipped == -1)
                    distance *= MathHelper.Lerp(0.1f, 0.3f, (float)Math.Sin(progress * MathHelper.Pi));
                return Projectile.velocity.RotatedBy(angleOffset) * distance;
            }
            else
            {
                float distance = MathHelper.Lerp(Reach, 0f, progress); //Quickly zip back to the player . No angles or minimum distance from player.
                return Projectile.velocity * distance;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer == 0)
                return false;
            Texture2D handle = Request<Texture2D>("CalamityMod/Projectiles/Melee/MendedBiomeBlade_GrovetendersTouchBlade").Value;
            Texture2D blade = Request<Texture2D>("CalamityMod/Projectiles/Melee/MendedBiomeBlade_GrovetendersTouchGlow").Value;

            Vector2 projBottom = Projectile.Center + new Vector2(-handle.Width / 2, handle.Height / 2).RotatedBy(Projectile.rotation + MathHelper.PiOver4) * 0.75f;
            DrawChain(projBottom, out Vector2[] chainPositions);

            float drawRotation = (projBottom - chainPositions[^2]).ToRotation() + MathHelper.PiOver4; //Face away from the last point of the bezier curve
            drawRotation += SnapCoyoteTime > 0 ? MathHelper.Pi : 0; //During coyote time the blade flips for some reason. Prevent that from happening
            drawRotation += Projectile.spriteDirection < 0 ? 0f : 0f;

            if (ReelingBack)
                drawRotation = Utils.AngleLerp(drawRotation, (Projectile.Center - Owner.Center).ToRotation(), GetSwingRatio());

            Vector2 drawOrigin = new(0f, handle.Height);
            SpriteEffects flip = (Projectile.spriteDirection < 0) ? SpriteEffects.None : SpriteEffects.None;
            lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16f), (int)(Projectile.Center.Y / 16f));

            Vector2 nitpickCorrection = (flipped == -1 && (Timer / MaxTime < 0.35f)) ? drawRotation.ToRotationVector2() * 16f + (Owner.direction == -1f ? Vector2.UnitX * -12f : Vector2.Zero) : Vector2.Zero;

            Main.EntitySpriteDraw(handle, projBottom - nitpickCorrection - Main.screenPosition, null, lightColor, drawRotation, drawOrigin, Projectile.scale, flip, 0);

            if ((!ReelingBack || SnapCoyoteTime != 0f) && (Timer / MaxTime > 0.35f))
            {
                //Turn on additive blending
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                //Only update the origin for once
                drawOrigin = new Vector2(0f, blade.Height);
                Main.EntitySpriteDraw(blade, projBottom - nitpickCorrection - Main.screenPosition, null, Color.Lerp(Color.White, lightColor, 0.5f) * 0.9f, drawRotation, drawOrigin, Projectile.scale, flip, 0);
                //Back to normal
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            return false;
        }

        private void DrawChain(Vector2 projBottom, out Vector2[] chainPositions)
        {
            Texture2D chainTex = Request<Texture2D>("CalamityMod/Projectiles/Melee/BrokenBiomeBlade_GrovetendersTouchChain").Value;

            float ratio = GetSwingRatio();

            if (!ReelingBack) //Make the curve be formed from points slightly ahead of the projectile, but clamped to the max rotation (straight line ahead)
            {
                controlPoint1 = Owner.MountedCenter + SwingPosition(MathHelper.Clamp(ratio + 0.5f, 0f, 1f)) * 0.2f;
                controlPoint2 = Owner.MountedCenter + SwingPosition(MathHelper.Clamp(ratio + 0.2f, 0f, 1f)) * 0.5f;
            }
            else //After the whip snaps, make the curve be a wave
            {
                Vector2 perpendicular = SwingPosition(ratio).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2);
                controlPoint1 = Owner.MountedCenter + SwingPosition(MathHelper.Lerp(ratio, 1f, ratio)) + perpendicular * MathHelper.SmoothStep(0f, 1f, ratio) * 155f * Owner.direction;
                controlPoint2 = Owner.MountedCenter + SwingPosition(MathHelper.Lerp(ratio, 1f, ratio / 2)) + perpendicular * MathHelper.SmoothStep(0f, 1f, ratio) * -100f * Owner.direction;
            }

            BezierCurve curve = new(new Vector2[] { Owner.MountedCenter, controlPoint1, controlPoint2, projBottom });
            int numPoints = 30;
            chainPositions = curve.GetPoints(numPoints).ToArray();

            //Draw each chain segment bar the very first one
            for (int i = 1; i < numPoints; i++)
            {
                Vector2 position = chainPositions[i];
                float rotation = (chainPositions[i] - chainPositions[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(chainPositions[i], chainPositions[i - 1]) / chainTex.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points
                Vector2 scale = new(1, yScale);
                Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
                if (ReelingBack)
                    chainLightColor *= 1 - EaseInFunction(ratio); //Make the chain fade when reeling it back
                Vector2 origin = new(chainTex.Width / 2, chainTex.Height); //Draw from center bottom of texture
                Main.EntitySpriteDraw(chainTex, position - Main.screenPosition, null, chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(initialized);
            writer.Write(Reach);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            initialized = reader.ReadBoolean();
            Reach = reader.ReadSingle();
        }
    }
}
