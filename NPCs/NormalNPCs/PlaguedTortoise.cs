﻿using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles;  

namespace CalamityMod.NPCs
{
    public class PlaguedTortoise : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plagueshell");
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 2f;
            npc.damage = 80;
            npc.aiStyle = 39;
            npc.width = 46;
            npc.height = 32;
            npc.defense = 32;
            npc.lifeMax = 800;
            npc.knockBackResist = 0.2f;
            animationType = 153;
            npc.value = Item.buyPrice(0, 0, 20, 0);
            npc.HitSound = SoundID.NPCHit24;
            npc.DeathSound = SoundID.NPCDeath27;
            npc.noGravity = false;
            npc.buffImmune[189] = true;
            npc.buffImmune[153] = true;
            npc.buffImmune[70] = true;
            npc.buffImmune[69] = true;
            npc.buffImmune[44] = true;
            npc.buffImmune[39] = true;
            npc.buffImmune[24] = true;
            npc.buffImmune[20] = true;
            npc.buffImmune[ModContent.BuffType<BrimstoneFlames>()] = true;
            npc.buffImmune[ModContent.BuffType<HolyFlames>()] = true;
            npc.buffImmune[ModContent.BuffType<Plague>()] = true;
            banner = npc.type;
            bannerItem = ModContent.ItemType<Items.PlagueshellBanner>();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 46, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 46, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !NPC.downedGolemBoss || spawnInfo.player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.HardmodeJungle.Chance * 0.09f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<Plague>(), 300, true);
        }

        public override void NPCLoot()
        {
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ModContent.ItemType<Items.PlagueCellCluster>(), Main.rand.Next(3, 5));
        }
    }
}
