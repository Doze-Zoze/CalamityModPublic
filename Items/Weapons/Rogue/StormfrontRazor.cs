﻿using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class StormfrontRazor : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormfront Razor");
            Tooltip.SetDefault("Throws a throwing knife that leaves sparks as it travels.\n" +
                               "Stealth Strike: The knife is faster and leaves a huge shower of sparks as it travels");
        }

        public override void SafeSetDefaults()
        {
            item.width = 40;
            item.height = 42;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.useStyle = 1;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = 5;

            item.useAnimation = 15;
            item.useTime = 15;
            item.damage = 50;
            item.crit += 8;
            item.knockBack = 7f;
            item.shoot = ModContent.ProjectileType<Projectiles.StormfrontRazorProjectile>();
            item.shootSpeed = 7f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "Cinquedea");
            recipe.AddIngredient(ItemID.HallowedBar, 6);
            recipe.AddIngredient(null, "EssenceofCinder", 4);
            recipe.AddIngredient(ItemID.Feather, 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(position, new Vector2(speedX * 1.6f, speedY * 1.6f), ModContent.ProjectileType<Projectiles.StormfrontRazorProjectile>(), damage, knockBack, player.whoAmI, 0, 40f);
                Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            else
            {
                Projectile.NewProjectile(position, new Vector2(speedX, speedY), ModContent.ProjectileType<Projectiles.StormfrontRazorProjectile>(), damage, knockBack, player.whoAmI, 0, 1);
                return false;
            }
        }
    }
}
