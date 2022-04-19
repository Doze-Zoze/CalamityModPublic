﻿using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;

namespace CalamityMod.NPCs.Astral
{
    public class BigSightseer : ModNPC
    {
        private static Texture2D glowmask;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Big Sightseer");
            Main.npcFrameCount[NPC.type] = 4;
            if (!Main.dedServ)
                glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/Astral/BigSightseerGlow", AssetRequestMode.ImmediateLoad).Value;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.7f,
                Velocity = 2f
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 56;
            NPC.damage = 50;
            NPC.defense = 20;
            NPC.DR_NERD(0.15f);
            NPC.lifeMax = 430;
            NPC.DeathSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCKilled/AstralEnemyDeath");
            NPC.noGravity = true;
            NPC.knockBackResist = 0.8f;
            NPC.value = Item.buyPrice(0, 0, 20, 0);
            NPC.aiStyle = -1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<BigSightseerBanner>();
            if (DownedBossSystem.downedAstrumAureus)
            {
                NPC.damage = 85;
                NPC.defense = 30;
                NPC.knockBackResist = 0.7f;
                NPC.lifeMax = 640;
            }
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				//BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.AstralSurface,

				// Will move to localization whenever that is cleaned up.
				new FlavorTextBestiaryInfoElement("Within their shells, the virus brews a potent chemical. When the seer approaches a foe, it spits this chemical out of its mandibles, hoping to melt the intruder’s flesh.")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.05f + NPC.velocity.Length() * 0.667f;
            if (NPC.frameCounter >= 8)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y > NPC.height * 3)
                {
                    NPC.frame.Y = 0;
                }
            }

            //DO DUST
            Dust d = CalamityGlobalNPC.SpawnDustOnNPC(NPC, 118, frameHeight, ModContent.DustType<AstralOrange>(), new Rectangle(70, 18, 48, 18), Vector2.Zero, 0.45f, true);
            if (d != null)
            {
                d.customData = 0.04f;
            }
        }

        public override void AI()
        {
            CalamityGlobalNPC.DoFlyingAI(NPC, 4f, 0.025f, 300f);

            NPC.ai[1]++;
            Player target = Main.player[NPC.target];

            if (NPC.justHit || target.dead)
            {
                //reset if hit
                NPC.ai[1] = 0;
            }

            //if can see target and waited long enough
            if (Collision.CanHit(target.position, target.width, target.height, NPC.position, NPC.width, NPC.height))
            {
                Vector2 vector = target.Center - NPC.Center;
                vector.Normalize();
                Vector2 spawnPoint = NPC.Center + vector * 42f;

                if (NPC.ai[1] >= (CalamityWorld.death ? 120f : 160f))
                {
                    NPC.ai[1] = 0f;

                    int n = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)spawnPoint.X, (int)spawnPoint.Y, ModContent.NPCType<AstralSeekerSpit>());
                    Main.npc[n].Center = spawnPoint;
                    Main.npc[n].velocity = vector * (CalamityWorld.death ? 12f : 10f);
                }
                else if (NPC.ai[1] >= 140f) //oozin dust at the "mouth"
                {
                    int dustType = Main.rand.NextBool(2) ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>();
                    int d = Dust.NewDust(spawnPoint - new Vector2(5), 10, 10, dustType);
                    Main.dust[d].velocity = NPC.velocity * 0.3f;
                    Main.dust[d].customData = true;
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 15;
                switch (Main.rand.Next(3))
                {
                    case 0:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AstralEnemyHit"), NPC.Center);
                        break;
                    case 1:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AstralEnemyHit2"), NPC.Center);
                        break;
                    case 2:
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AstralEnemyHit3"), NPC.Center);
                        break;
                }
            }

            CalamityGlobalNPC.DoHitDust(NPC, hitDirection, (Main.rand.Next(0, Math.Max(0, NPC.life)) == 0) ? 5 : ModContent.DustType<AstralEnemy>(), 1f, 4, 22);

            //if dead do gores
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        float rand = Main.rand.NextFloat(-0.18f, 0.18f);
                        Gore.NewGore(NPC.position + new Vector2(Main.rand.NextFloat(0f, NPC.width), Main.rand.NextFloat(0f, NPC.height)), NPC.velocity * rand, Mod.Find<ModGore>("BigSightseerGore" + i).Type);
                    }
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!NPC.IsABestiaryIconDummy)
                spriteBatch.Draw(glowmask, NPC.Center - screenPos + new Vector2(0, 4f), NPC.frame, Color.White * 0.75f, NPC.rotation, new Vector2(59f, 28f), NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (CalamityGlobalNPC.AnyEvents(spawnInfo.Player))
            {
                return 0f;
            }
            else if (spawnInfo.Player.InAstral(1))
            {
                return spawnInfo.Player.ZoneDesert ? 0.14f : 0.17f;
            }
            return 0f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<Stardust>(), 1, 2, 3, 3, 4));
        }
    }

    public class AstralSeekerSpit : ModNPC
    {
        public override void SetStaticDefaults()
        {
            this.HideFromBestiary();
            DisplayName.SetDefault("Seeker Spit");
            Main.npcFrameCount[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.width = 16;
            NPC.height = 16;
            NPC.damage = 45;
            NPC.defense = 0;
            NPC.lifeMax = 1;
            NPC.HitSound = null;
            NPC.DeathSound = SoundID.NPCDeath9;
            NPC.noGravity = true;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.alpha = 80;
            NPC.aiStyle = -1;
            if (DownedBossSystem.downedAstrumAureus)
            {
                NPC.damage = 75;
            }
        }

        public override void AI()
        {
            //DUST
            NPC.ai[0] += 0.18f;
            float angle = NPC.velocity.ToRotation() + MathHelper.PiOver2;
            float pulse = (float)Math.Sin(NPC.ai[0]);
            float radius = 5.8f;
            Vector2 offset = angle.ToRotationVector2() * pulse * radius;
            Dust.NewDustPerfect(NPC.Center + offset, ModContent.DustType<AstralOrange>(), Vector2.Zero);
            Dust.NewDustPerfect(NPC.Center - offset, ModContent.DustType<AstralBlue>(), Vector2.Zero);

            //kill on tile collide
            if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
            {
                NPC.StrikeNPCNoInteraction(9999, 0, 0);
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (damage > 0)
            {
                NPC.StrikeNPCNoInteraction(9999, 0, 0);
            }
        }

        public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
        {
            if (damage > 0)
            {
                NPC.StrikeNPCNoInteraction(9999, 0, 0);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            DoKillDust();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120, true);
        }

        private void DoKillDust()
        {
            int numDust = Main.rand.Next(17, 25);
            float rotPerIter = MathHelper.TwoPi / numDust;
            float angle = 0;
            for (int i = 0; i < numDust; i++)
            {
                Vector2 vel = (angle + Main.rand.NextFloat(-0.04f, 0.04f)).ToRotationVector2();
                int dustType = Main.rand.NextBool(2) ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>();
                Dust d = Dust.NewDustPerfect(NPC.Center, dustType, vel * Main.rand.NextFloat(1.8f, 2.2f));
                d.customData = NPC;

                angle += rotPerIter;
            }
        }
    }
}
