using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
	public class StaffoftheMechworm : ModItem
    {
        // This value is also referenced by the God Slayer and Auric summoner helmets.
        public const int BaseDamage = 240; // originally 325
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Staff of the Mechworm");
            Tooltip.SetDefault("Summons an aerial mechworm to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = BaseDamage;
            item.mana = 15;
            item.width = 58;
            item.height = 58;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(platinum: 1, gold: 40);
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item113;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<MechwormHead>();
            item.shootSpeed = 10f;
            item.summon = true;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool CanUseItem(Player player)
        {
            float neededSlots = 1;
            float foundSlotsCount = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.minion && p.owner == player.whoAmI)
                {
                    foundSlotsCount += p.minionSlots;
                    if (foundSlotsCount + neededSlots > player.maxMinions)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int head = -1;
            int tail = -1;
            for (int num187 = 0; num187 < Main.projectile.Length; num187++)
            {
                if (Main.projectile[num187].active && Main.projectile[num187].owner == Main.myPlayer)
                {
                    if (head == -1 && Main.projectile[num187].type == ModContent.ProjectileType<MechwormHead>())
                    {
                        head = num187;
                    }
                    if (tail == -1 && Main.projectile[num187].type == ModContent.ProjectileType<MechwormTail>())
                    {
                        tail = num187;
                    }
                    if (head != -1 && tail != -1)
                    {
                        break;
                    }
                }
            }
            if (head == -1 && tail == -1)
            {
                int curr = Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI, 0f, 0f);
                curr = Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormBody>(), damage, knockBack, player.whoAmI, curr, 0f);
                int head2 = curr;
                curr = Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormBody>(), damage, knockBack, player.whoAmI, curr, 0f);
                Main.projectile[head2].localAI[1] = curr;
                head2 = curr;
                curr = Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormTail>(), damage, knockBack, player.whoAmI, curr, 0f);
                Main.projectile[head2].localAI[1] = curr;
            }
            else if (head != -1 && tail != -1)
            {
                position = Main.projectile[tail].Center;
                int body = Projectile.NewProjectile(position, Vector2.Zero, ModContent.ProjectileType<MechwormBody>(), damage, knockBack, player.whoAmI, Projectile.GetByUUID(Main.myPlayer, Main.projectile[tail].ai[0]), 0f);
                int body2 = body;

                body = Projectile.NewProjectile(position, Vector2.Zero, ModContent.ProjectileType<MechwormBody>(), damage, knockBack, player.whoAmI, body, 0f);

                Main.projectile[body2].localAI[1] = body;
                Main.projectile[body2].netUpdate = true;
                Main.projectile[body].localAI[1] = tail;
                Main.projectile[body].netUpdate = true;
                Main.projectile[tail].ai[0] = Main.projectile[body].projUUID;
                Main.projectile[tail].netUpdate = true;
            }
            return false;
        }
    }
}
