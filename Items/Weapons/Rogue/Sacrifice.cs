using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class Sacrifice : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sacrifice");
            Tooltip.SetDefault("Throws sacrificial daggers that lodge themselves in enemies\n" +
                "Right click causes all stuck daggers to fly back at you and give you life\n" +
                "Daggers stuck to enemies release bloodsplosions over time\n" +
                "Stealth strikes provide much more life when returning to you");
        }

        public override void SafeSetDefaults()
        {
            item.damage = 215;
            item.width = item.height = 68;
            item.useAnimation = item.useTime = 9;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SacrificeProjectile>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;

            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }

        public override bool AltFunctionUse(Player player) => player.ownedProjectileCounts[item.shoot] > 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type != type || !Main.projectile[i].active || Main.projectile[i].owner != player.whoAmI)
                        continue;

                    if (Main.projectile[i].ai[0] != 1f)
                        continue;

                    NPC attachedNPC = Main.npc[(int)Main.projectile[i].ai[1]];
                    Main.projectile[i].ai[0] = 2f;
                    Main.projectile[i].ModProjectile<SacrificeProjectile>().AbleToHealOwner = attachedNPC.type != NPCID.TargetDummy && attachedNPC.type != ModContent.NPCType<SuperDummyNPC>();
                    Main.projectile[i].netUpdate = true;
                }
                return false;
            }

            Vector2 velocity = new Vector2(speedX, speedY);
            position += velocity * 3f;
            int proj = Projectile.NewProjectile(position, velocity, type, damage, knockBack, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && Main.projectile.IndexInRange(proj))
                Main.projectile[proj].Calamity().stealthStrike = true;
            return false;
        }
    }
}
