using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Keelhaul : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Keelhaul");
            Tooltip.SetDefault("Summons a geyser upon hitting an enemy\n" +
                               "Crumple 'em like paper");
        }

        public override void SetDefaults()
        {
            item.width = 42;
            item.height = 42;
            item.damage = 55;
            item.mana = 50;
            item.magic = true;
            item.noMelee = true;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useTurn = true;
            item.useStyle = 1;
            item.knockBack = 2f;
            item.UseSound = SoundID.Item102;
            item.autoReuse = true;
            item.rare = 8;
            item.Calamity().postMoonLordRarity = 21;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.shoot = ModContent.ProjectileType<Projectiles.KeelhaulBubble>();
            item.shootSpeed = 15f;
        }
    }
}
