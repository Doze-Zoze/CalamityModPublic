﻿using System;
using System.Collections.Generic;
using System.IO;
using CalamityMod.Events;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SlimeGod
{
    [AutoloadBossHead]
    public class SplitEbonianPaladin : ModNPC
    {
        private float bossLife;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.6f,
                PortraitScale = 1f,
                PortraitPositionYOverride = 0
            };
            value.Position.Y += 10;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.LifeMaxNERB(2000, 2400, 110000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.GetNPCDamage();
            NPC.width = 150;
            NPC.height = 92;
            NPC.scale = 0.8f;
            NPC.defense = 8;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            AnimationType = NPCID.KingSlime;
            NPC.value = Item.buyPrice(0, 1, 0, 0);
            NPC.Opacity = 0.8f;
            NPC.lavaImmune = false;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.SlimeGodPaladin")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (CalamityGlobalNPC.slimeGodPurple < 0 || !Main.npc[CalamityGlobalNPC.slimeGodPurple].active)
                CalamityGlobalNPC.slimeGodPurple = NPC.whoAmI;

            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || NPC.localAI[1] == 1f || bossRush;

            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            NPC.defense = NPC.defDefense;
            int setDamage = NPC.defDamage;
            if (NPC.localAI[1] == 1f)
            {
                NPC.defense = NPC.defDefense + 16;
                setDamage += 20;
            }

            float scale = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 0.6f : Main.getGoodWorld ? 0.8f : 1f;
            NPC.aiAction = 0;
            NPC.noTileCollide = false;
            NPC.noGravity = false;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (NPC.ai[0] != 4f)
            {
                if (player.dead || !player.active)
                {
                    NPC.TargetClosest();
                    player = Main.player[NPC.target];
                    if (player.dead || !player.active)
                    {
                        NPC.ai[0] = 4f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                    }
                }
                else if (NPC.timeLeft < 1800)
                    NPC.timeLeft = 1800;
            }

            bool enraged = true;
            bool hyperMode = NPC.localAI[1] == 1f;
            if (CalamityGlobalNPC.slimeGodRed != -1)
            {
                if (Main.npc[CalamityGlobalNPC.slimeGodRed].active)
                    enraged = false;
            }

            if (bossRush)
            {
                enraged = true;
                hyperMode = true;
            }

            // Teleport
            float teleportGateValue = 720f;
            if (!player.dead && NPC.timeLeft > 10 && NPC.ai[3] >= teleportGateValue && NPC.ai[0] == 0f && NPC.velocity.Y == 0f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                // Slow down dramatically
                NPC.velocity.X *= 0.5f;

                NPC.ai[0] = 6f;
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.netUpdate = true;
                    NPC.TargetClosest(false);
                    player = Main.player[NPC.target];

                    float distanceAhead = 960f;
                    Vector2 randomDefault = Main.rand.NextBool() ? Vector2.UnitX : -Vector2.UnitX;
                    Vector2 vectorAimedAheadOfTarget = player.Center + new Vector2((float)Math.Round(player.velocity.X), 0f).SafeNormalize(randomDefault) * distanceAhead;
                    Point predictiveTeleportPoint = vectorAimedAheadOfTarget.ToTileCoordinates();
                    int randomPredictiveTeleportOffset = 5;
                    int teleportTries = 0;
                    while (teleportTries < 100)
                    {
                        teleportTries++;
                        int teleportTileX = Main.rand.Next(predictiveTeleportPoint.X - randomPredictiveTeleportOffset, predictiveTeleportPoint.X + randomPredictiveTeleportOffset + 1);
                        int teleportTileY = Main.rand.Next(predictiveTeleportPoint.Y - randomPredictiveTeleportOffset, predictiveTeleportPoint.Y);

                        if (!Main.tile[teleportTileX, teleportTileY].HasUnactuatedTile)
                        {
                            bool canTeleportToTile = true;
                            if (canTeleportToTile && Main.tile[teleportTileX, teleportTileY].LiquidType == LiquidID.Lava)
                                canTeleportToTile = false;
                            if (canTeleportToTile && !Collision.CanHitLine(NPC.Center, 0, 0, predictiveTeleportPoint.ToVector2() * 16, 0, 0))
                                canTeleportToTile = false;

                            if (canTeleportToTile)
                            {
                                NPC.localAI[0] = teleportTileX * 16 + 8;
                                NPC.localAI[3] = teleportTileY * 16 + 16;
                                NPC.ai[3] = 0f;
                                break;
                            }
                            else
                                predictiveTeleportPoint.X += predictiveTeleportPoint.X < 0f ? 1 : -1;
                        }
                        else
                            predictiveTeleportPoint.X += predictiveTeleportPoint.X < 0f ? 1 : -1;
                    }

                    // Default teleport if the above conditions aren't met in 100 iterations
                    if (teleportTries >= 100)
                    {
                        Vector2 bottom = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)].Bottom;
                        NPC.localAI[0] = bottom.X;
                        NPC.localAI[3] = bottom.Y;
                        NPC.ai[3] = 0f;
                    }
                }
            }

            // Get ready to teleport by increasing ai[3]
            // This only occurs in Rev and Death for the split Slime Gods
            if (NPC.ai[3] < teleportGateValue && revenge)
            {
                if (!Collision.CanHitLine(NPC.Center, 0, 0, player.Center, 0, 0) || Math.Abs(NPC.Top.Y - player.Bottom.Y) > 320f)
                    NPC.ai[3] += death ? 3f : 2f;
                else
                    NPC.ai[3] += 1f;
            }

            float distanceSpeedBoost = Vector2.Distance(player.Center, NPC.Center) * (bossRush ? 0.008f : 0.005f);

            if (NPC.ai[0] == 0f)
            {
                bool phaseThroughTilesToReachTarget = Vector2.Distance(player.Center, NPC.Center) > 2400f || !Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
                if (Main.netMode != NetmodeID.MultiplayerClient && phaseThroughTilesToReachTarget)
                {
                    // Set damage
                    NPC.damage = setDamage;

                    NPC.ai[0] = 5f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                }

                if (NPC.velocity.Y == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.TargetClosest();
                    NPC.velocity.X *= 0.8f;
                    NPC.ai[1] += hyperMode ? 2f : 1f;

                    float jumpGateValue = 40f;
                    float velocityX = death ? 10f : revenge ? 9f : expertMode ? 8f : 6f;
                    if (revenge)
                    {
                        float moveBoost = death ? 15f * (1f - lifeRatio) : 10f * (1f - lifeRatio);
                        float speedBoost = death ? 3f * (1f - lifeRatio) : 2f * (1f - lifeRatio);
                        jumpGateValue -= moveBoost;
                        velocityX += speedBoost;
                    }

                    float distanceBelowTarget = NPC.position.Y - (player.position.Y + 80f);
                    float speedMult = 1f;
                    if (distanceBelowTarget > 0f)
                        speedMult += distanceBelowTarget * 0.002f;

                    if (speedMult > 2f)
                        speedMult = 2f;

                    float velocityY = 5f;
                    if (!Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1))
                        velocityY += 2f;

                    if (NPC.ai[1] > jumpGateValue)
                    {
                        // Set damage
                        NPC.damage = setDamage;

                        velocityY *= 1.25f;
                        NPC.ai[1] = 0f;
                        NPC.velocity.Y -= velocityY * speedMult;
                        NPC.velocity.X = (velocityX + distanceSpeedBoost) * NPC.direction;
                        NPC.noTileCollide = true;
                        NPC.netUpdate = true;
                    }
                    else if (NPC.ai[1] >= jumpGateValue - 30f)
                        NPC.aiAction = 1;
                }
                else
                {
                    // Set damage
                    NPC.damage = setDamage;

                    NPC.velocity.X *= 0.99f;
                    if (NPC.direction < 0 && NPC.velocity.X > -1f)
                        NPC.velocity.X = -1f;
                    if (NPC.direction > 0 && NPC.velocity.X < 1f)
                        NPC.velocity.X = 1f;

                    if (!player.dead)
                    {
                        if (NPC.velocity.Y > 0f && NPC.Bottom.Y > player.Top.Y)
                            NPC.noTileCollide = false;
                        else if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                            NPC.noTileCollide = false;
                        else
                            NPC.noTileCollide = true;
                    }
                }

                NPC.ai[2] += 1f;
                if (revenge)
                    NPC.ai[2] += death ? 1f * (1f - lifeRatio) : 0.5f * (1f - lifeRatio);

                if (NPC.velocity.Y == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    float phaseSwitchGateValue = 210f;
                    bool switchPhase = NPC.ai[2] >= phaseSwitchGateValue;
                    if (switchPhase)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            switch ((int)NPC.localAI[2])
                            {
                                default:
                                case 1:
                                    NPC.ai[0] = Main.rand.NextBool() ? 2f : 3f;
                                    break;

                                case 2:
                                    NPC.ai[0] = Main.rand.NextBool() ? 3f : (hyperMode ? 2f : 1f);
                                    break;

                                case 3:
                                    NPC.ai[0] = Main.rand.NextBool() ? (hyperMode ? 3f : 1f) : 2f;
                                    break;
                            }

                            if (NPC.ai[0] == 2f)
                            {
                                // Set damage
                                NPC.damage = setDamage;

                                NPC.noTileCollide = true;
                                NPC.velocity.Y = death ? -10f : revenge ? -9f : expertMode ? -8f : -7f;
                            }

                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                            NPC.netUpdate = true;
                        }
                    }
                    else if (NPC.ai[1] >= phaseSwitchGateValue - 30f)
                        NPC.aiAction = 1;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.velocity.X *= 0.8f;

                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= 15f)
                {
                    if (revenge)
                    {
                        float slimeShotVelocity = 6f;
                        Vector2 projectileVelocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * slimeShotVelocity;
                        float minFiringDistance = 160f; // 10 tile distance
                        if (Vector2.Distance(NPC.Center, player.Center) > minFiringDistance)
                        {
                            SoundEngine.PlaySound(SlimeGodCore.BigShotSound, NPC.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ModContent.ProjectileType<UnstableEbonianGlob>();
                                int damage = NPC.GetProjectileDamage(type);
                                int numProj = death ? 5 : 3;
                                int spread = death ? 12 : 8;
                                float rotation = MathHelper.ToRadians(spread);
                                for (int j = 0; j < numProj; j++)
                                {
                                    Vector2 randomVelocity = Main.rand.NextVector2CircularEdge(3f, 3f);
                                    Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, j / (float)(numProj - 1))) + randomVelocity;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + perturbedSpeed.SafeNormalize(Vector2.UnitY) * 30f * NPC.scale, perturbedSpeed, type, damage, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }

                    NPC.localAI[2] = NPC.ai[0];
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 2f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.noTileCollide = true;
                NPC.noGravity = true;

                if (NPC.velocity.X < 0f)
                    NPC.direction = -1;
                else
                    NPC.direction = 1;

                NPC.spriteDirection = NPC.direction;
                NPC.TargetClosest();
                Vector2 targetCenter = player.Center;
                targetCenter.Y -= 350f;
                Vector2 targetDist = targetCenter - NPC.Center;

                if (NPC.ai[2] == 1f)
                {
                    NPC.ai[1] += 1f;
                    targetDist = player.Center - NPC.Center;
                    targetDist.Normalize();
                    targetDist *= death ? 10f : revenge ? 9f : expertMode ? 8f : 7f;
                    NPC.velocity = (NPC.velocity * 4f + targetDist) / 5f;

                    if (NPC.ai[1] > 12f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[0] = 2.1f;
                        NPC.ai[2] = 0f;
                        NPC.velocity = targetDist;
                    }
                }
                else
                {
                    if (Math.Abs(NPC.Center.X - player.Center.X) < 40f && NPC.Center.Y < player.Center.Y - 300f)
                    {
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 1f;
                        return;
                    }

                    targetDist.Normalize();
                    targetDist *= (death ? 12f : revenge ? 11f : expertMode ? 10f : 9f) + distanceSpeedBoost;
                    NPC.velocity = (NPC.velocity * 5f + targetDist) / 6f;
                }
            }
            else if (NPC.ai[0] == 2.1f)
            {
                // Set damage
                NPC.damage = setDamage;

                bool atTargetPosition = NPC.position.Y + NPC.height >= player.position.Y;
                if (NPC.ai[2] == 0f && atTargetPosition && Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.ai[2] = 1f;
                    NPC.netUpdate = true;
                }

                if (atTargetPosition || NPC.velocity.Y <= 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] > 10f)
                    {
                        SoundEngine.PlaySound(SlimeGodCore.BigShotSound, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            // Eruption of slime balls
                            float projectileVelocity = 4f;
                            int type = ModContent.ProjectileType<UnstableEbonianGlob>();
                            int damage = NPC.GetProjectileDamage(type);
                            Vector2 destination = new Vector2(NPC.Center.X, NPC.Center.Y - 100f) - NPC.Center;
                            destination.Normalize();
                            destination *= projectileVelocity;
                            int numProj = 9;
                            float rotation = MathHelper.ToRadians(90);
                            for (int i = 0; i < numProj; i++)
                            {
                                // Spawn projectiles 0, 1, 2, 6, 7 and 8
                                if (i < 3 || i > 5)
                                {
                                    Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitY * 30f * NPC.scale + Vector2.Normalize(perturbedSpeed) * 30f * NPC.scale, perturbedSpeed, type, damage, 0f, Main.myPlayer);
                                }
                            }

                            // Fire slime balls directly at players with a max of 2
                            if (enraged && expertMode)
                            {
                                List<int> targets = new List<int>();
                                foreach (Player plr in Main.ActivePlayers)
                                {
                                    if (!plr.dead)
                                        targets.Add(plr.whoAmI);

                                    if (targets.Count > 1)
                                        break;
                                }
                                foreach (int t in targets)
                                {
                                    Vector2 projFireDirection = Vector2.Normalize(Main.player[t].Center - NPC.Center) * projectileVelocity;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(projFireDirection) * 30f * NPC.scale, projFireDirection, type, damage, 0f, Main.myPlayer);
                                }
                            }
                        }

                        NPC.localAI[2] = NPC.ai[0] - 0.1f;
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.netUpdate = true;
                    }
                }
                else if (NPC.ai[2] == 0f)
                    NPC.noTileCollide = true;

                NPC.noGravity = true;

                NPC.velocity.Y += 0.5f;
                float velocityLimit = bossRush ? 20f : death ? 15f : revenge ? 14f : expertMode ? 13f : 12f;
                if (NPC.velocity.Y > velocityLimit)
                    NPC.velocity.Y = velocityLimit;
            }
            else if (NPC.ai[0] == 3f)
            {
                if (NPC.velocity.Y == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.TargetClosest();
                    NPC.velocity.X *= 0.8f;

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] > 15f)
                    {
                        // Set damage
                        NPC.damage = setDamage;

                        NPC.ai[1] = 0f;

                        NPC.velocity.Y -= 6f;
                        if (player.position.Y + player.height < NPC.Center.Y)
                            NPC.velocity.Y -= 1.2f;
                        if (player.position.Y + player.height < NPC.Center.Y - 40f)
                            NPC.velocity.Y -= 1.4f;
                        if (player.position.Y + player.height < NPC.Center.Y - 80f)
                            NPC.velocity.Y -= 1.7f;
                        if (player.position.Y + player.height < NPC.Center.Y - 120f)
                            NPC.velocity.Y -= 2f;
                        if (player.position.Y + player.height < NPC.Center.Y - 160f)
                            NPC.velocity.Y -= 2.2f;
                        if (player.position.Y + player.height < NPC.Center.Y - 200f)
                            NPC.velocity.Y -= 2.4f;
                        if (!Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1))
                            NPC.velocity.Y -= 2f;

                        NPC.velocity.X = ((death ? 11f : revenge ? 10f : expertMode ? 9f : 8f) + distanceSpeedBoost) * NPC.direction;
                        NPC.ai[2] += 1f;
                    }
                    else
                        NPC.aiAction = 1;
                }
                else
                {
                    // Set damage
                    NPC.damage = setDamage;

                    NPC.velocity.X *= 0.98f;
                    float velocityLimit = (death ? 6.5f : revenge ? 6f : expertMode ? 5.5f : 5f);
                    if (NPC.direction < 0 && NPC.velocity.X > -velocityLimit)
                        NPC.velocity.X = -velocityLimit;
                    if (NPC.direction > 0 && NPC.velocity.X < velocityLimit)
                        NPC.velocity.X = velocityLimit;
                }

                if (NPC.ai[2] >= 2f && NPC.velocity.Y == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.localAI[2] = NPC.ai[0];
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                }
            }

            // Despawn
            else if (NPC.ai[0] == 4f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.noTileCollide = true;
                NPC.Opacity -= 0.03f;

                if (NPC.timeLeft > 10)
                    NPC.timeLeft = 10;

                if (NPC.Opacity < 0f)
                    NPC.Opacity = 0f;

                NPC.velocity.X *= 0.98f;
            }

            else if (NPC.ai[0] == 5f)
            {
                // Set damage
                NPC.damage = setDamage;

                if (NPC.velocity.X > 0f)
                    NPC.direction = 1;
                else
                    NPC.direction = -1;

                NPC.spriteDirection = NPC.direction;
                NPC.noTileCollide = true;
                NPC.noGravity = true;
                NPC.knockBackResist = 0f;
                Vector2 distanceFromTarget = player.Center - NPC.Center;
                distanceFromTarget.Y -= 16f;
                if (Main.netMode != NetmodeID.MultiplayerClient && distanceFromTarget.Length() < 500f && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height) &&
                    Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                }

                if (distanceFromTarget.Length() > 12f)
                {
                    distanceFromTarget.Normalize();
                    distanceFromTarget *= 12f;
                }

                NPC.velocity = (NPC.velocity * 4f + distanceFromTarget) / 5f;
            }
            else if (NPC.ai[0] == 6f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.aiAction = 1;
                NPC.ai[1] += 1f;
                float teleportTime = bossRush ? 20f : death ? 30f : 40f;
                scale = MathHelper.Clamp((teleportTime - NPC.ai[1]) / teleportTime, 0f, 1f);
                scale = 0.5f + scale * 0.5f;
                if (NPC.ai[1] >= teleportTime && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.Bottom = new Vector2(NPC.localAI[0], NPC.localAI[3]);
                    NPC.ai[0] = 7f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                }

                if (Main.netMode == NetmodeID.MultiplayerClient && NPC.ai[1] >= teleportTime * 2f)
                {
                    NPC.ai[0] = 7f;
                    NPC.ai[1] = 0f;
                }

                // Emit teleport dust
                Color dustColor = Color.Lavender;
                dustColor.A = 150;
                for (int i = 0; i < 10; i++)
                {
                    int corruptDust = Dust.NewDust(NPC.position + Vector2.UnitX * -20f, NPC.width + 40, NPC.height, DustID.TintableDust, NPC.velocity.X, NPC.velocity.Y, NPC.alpha, dustColor, 2f);
                    Main.dust[corruptDust].noGravity = true;
                    Main.dust[corruptDust].velocity *= 0.5f;
                }
            }
            else if (NPC.ai[0] == 7f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.ai[1] += 1f;
                float teleportEndTime = bossRush ? 10f : death ? 15f : 20f;
                scale = MathHelper.Clamp(NPC.ai[1] / teleportEndTime, 0f, 1f);
                scale = 0.5f + scale * 0.5f;
                if (NPC.ai[1] >= teleportEndTime && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                    NPC.TargetClosest();
                }

                if (Main.netMode == NetmodeID.MultiplayerClient && NPC.ai[1] >= teleportEndTime * 2f)
                {
                    NPC.ai[0] = 0f;
                    NPC.ai[1] = 0f;
                    NPC.TargetClosest();
                }

                // Emit teleport dust
                Color dustColor = Color.Lavender;
                dustColor.A = 150;
                for (int i = 0; i < 10; i++)
                {
                    int corruptDust = Dust.NewDust(NPC.position + Vector2.UnitX * -20f, NPC.width + 40, NPC.height, DustID.TintableDust, NPC.velocity.X, NPC.velocity.Y, NPC.alpha, dustColor, 2f);
                    Main.dust[corruptDust].noGravity = true;
                    Main.dust[corruptDust].velocity *= 0.5f;
                }
            }

            if (bossLife == 0f && NPC.life > 0)
                bossLife = NPC.lifeMax;

            if (NPC.life > 0)
            {
                float scaleRatio = lifeRatio;
                scaleRatio = scaleRatio * 0.5f + 0.75f;
                scaleRatio *= scale;

                if (scaleRatio != NPC.scale)
                {
                    NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                    NPC.position.Y = NPC.position.Y + (float)NPC.height;
                    NPC.scale = scaleRatio * 0.75f;
                    NPC.width = (int)(150f * NPC.scale);
                    NPC.height = (int)(92f * NPC.scale);
                    NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                    NPC.position.Y = NPC.position.Y - (float)NPC.height;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int slimeSpawnThreshold = (int)((double)NPC.lifeMax * 0.2);
                    if ((float)(NPC.life + slimeSpawnThreshold) < bossLife)
                    {
                        bossLife = (float)NPC.life;
                        int x = (int)(NPC.position.X + (float)Main.rand.Next(NPC.width - 32));
                        int y = (int)(NPC.position.Y + (float)Main.rand.Next(NPC.height - 32));
                        int slimeType = ModContent.NPCType<CorruptSlimeSpawn>();
                        int slimeSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), x, y, slimeType);
                        Main.npc[slimeSpawn].SetDefaults(slimeType);
                        Main.npc[slimeSpawn].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                        Main.npc[slimeSpawn].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                        Main.npc[slimeSpawn].ai[0] = (float)(-1000 * Main.rand.Next(3));
                        Main.npc[slimeSpawn].ai[1] = 0f;
                        if (Main.netMode == NetmodeID.Server && slimeSpawn < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, slimeSpawn, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            Color lightColor = new Color(200, 150, Main.DiscoB, NPC.alpha);
            Color newColor = NPC.localAI[1] == 1f ? lightColor : drawColor;
            return newColor * NPC.Opacity;
        }

        public override void OnKill()
        {
            int heartAmt = Main.rand.Next(3) + 3;
            for (int i = 0; i < heartAmt; i++)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Every Slime God piece drops Gel, even if it's not the last one.
            npcLoot.Add(ItemID.Gel, 1, 32, 48);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            Color dustColor = Color.Lavender;
            dustColor.A = 150;
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TintableDust, hit.HitDirection, -1f, NPC.alpha, dustColor, 1f);
            }
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 50;
                NPC.height = 50;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int i = 0; i < 40; i++)
                {
                    int corruptionDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TintableDust, 0f, 0f, NPC.alpha, dustColor, 2f);
                    Main.dust[corruptionDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[corruptionDust].scale = 0.5f;
                        Main.dust[corruptionDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 70; j++)
                {
                    int corruptionDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TintableDust, 0f, 0f, NPC.alpha, dustColor, 3f);
                    Main.dust[corruptionDust2].noGravity = true;
                    Main.dust[corruptionDust2].velocity *= 5f;
                    corruptionDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TintableDust, 0f, 0f, NPC.alpha, dustColor, 2f);
                    Main.dust[corruptionDust2].velocity *= 2f;
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Weak, 240, true);
        }
    }
}
