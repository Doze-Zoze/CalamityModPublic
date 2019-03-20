using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
	public class TrashmanTrashcan : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Trash Can");
			Tooltip.SetDefault("Summons the trash man");
		}
		public override void SetDefaults()
		{
            item.damage = 0;
			item.useStyle = 1;
			item.useAnimation = 20;
			item.useTime = 20;
			item.noMelee = true;
			item.width = 30;
            item.height = 30;
            item.value = Item.sellPrice(0, 1, 0, 0);
            item.shoot = mod.ProjectileType("DannyDevito");
            item.buffType = mod.BuffType("DannyDevito");
			item.rare = 5;
			item.UseSound = SoundID.NPCDeath13;
		}

        public override void UseStyle(Player player)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(item.buffType, 15, true);
            }
        }
	}
}
