using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Buffs.StatDebuffs
{
    public class ArmorCrunch : ModBuff
    {
        public static int DefenseReduction = 15;

        public override void SetDefaults()
        {
            DisplayName.SetDefault("Armor Crunch");
            Description.SetDefault("Your armor is shredded");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
			if (npc.Calamity().aCrunch < npc.buffTime[buffIndex])
				npc.Calamity().aCrunch = npc.buffTime[buffIndex];
			npc.DelBuff(buffIndex);
			buffIndex--;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Calamity().aCrunch = true;
        }
    }
}
