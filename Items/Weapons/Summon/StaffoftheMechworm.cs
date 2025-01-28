using CalamityMod.Projectiles.Summon;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class StaffoftheMechworm : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Summon";
        public override void SetDefaults()
        {
            Item.width = 68;
            Item.height = 68;
            Item.damage = 100;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 10; // 9 because of useStyle 1
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.UseSound = SoundID.Item113;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<MechwormHead>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool Exists = false;
            int tailID = 0;
            //checks to see if a mechworm already exists owned by the same player
            foreach (var projectile in Main.projectile)
            {
                if (projectile.type == type && projectile.owner == player.whoAmI && projectile.active)
                {
                    Exists = true;
                }

                if (projectile.type == ModContent.ProjectileType<MechwormTail>() && projectile.owner == player.whoAmI && projectile.active)
                {
                    tailID = projectile.whoAmI;
                }
            }

            //calculates avaliable minion slots
            float foundSlotsCount = 0f;
            foreach (var p in Main.projectile)
            {
                if (p.active && p.minion && p.owner == player.whoAmI)
                {
                    foundSlotsCount += p.minionSlots;
                }
            }
            if (Exists) //If there is a mechworm, just add one segment
            {
                if (!(foundSlotsCount + 0.5f > (float)player.maxMinions)) // If there's at least half a slot, spawn a segment
                {
                    Projectile.NewProjectile(source, Main.projectile[tailID].Center, Vector2.Zero, ModContent.ProjectileType<MechwormBody>(), damage, knockback, player.whoAmI);
                }
            }
            else //spawn a new mechworm
            {
                if (!(foundSlotsCount + 2f > (float)player.maxMinions)) // If there's at least 2 open slots, spawn the head, 2 segments, and the tail.
                {
                    Projectile.NewProjectile(source, Main.MouseWorld, player.DirectionTo(Main.MouseWorld) * 1000, ModContent.ProjectileType<MechwormHead>(), damage, knockback, player.whoAmI);
                    Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormTail>(), damage, knockback, player.whoAmI);
                    Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormBody>(), damage, knockback, player.whoAmI);
                    Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormBody>(), damage, knockback, player.whoAmI);
                    Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormTeleportRift>(), 0, 0f, player.whoAmI); //spawn a cute lil portal

                } else if (!(foundSlotsCount + 1f > (float)player.maxMinions)) //If there's at least 1 open slot, spawn the head and tail
                {
                    Projectile.NewProjectile(source, Main.MouseWorld, player.DirectionTo(Main.MouseWorld) * 1000, ModContent.ProjectileType<MechwormHead>(), damage, knockback, player.whoAmI);
                    Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormTail>(), damage, knockback, player.whoAmI);
                    Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<MechwormTeleportRift>(), 0, 0f, player.whoAmI);
                }
            }
            return false;
        }
    }
}
