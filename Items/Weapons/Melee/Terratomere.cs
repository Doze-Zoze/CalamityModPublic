using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Terratomere : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terratomere");
            Tooltip.SetDefault("Linked to the essence of Terraria\n" +
                               "Heals the player on true melee hits\n" +
                               "Fires a barrage of homing beams that inflict several debuffs");
        }

        public override void SetDefaults()
        {
            item.width = 64;
            item.damage = 125;
            item.melee = true;
            item.useAnimation = 21;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 21;
            item.useTurn = true;
            item.knockBack = 7f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 64;
            item.value = Item.buyPrice(1, 0, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<TerratomereProjectile>();
            item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int num6 = Main.rand.Next(4, 6);
            for (int index = 0; index < num6; ++index)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-40, 41) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-40, 41) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)((double)damage * 0.5), knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<XerocsGreatsword>());
            recipe.AddIngredient(ModContent.ItemType<Floodtide>());
            recipe.AddIngredient(ModContent.ItemType<Hellkite>());
            recipe.AddIngredient(ModContent.ItemType<TemporalFloeSword>());
            recipe.AddIngredient(ItemID.TerraBlade);
            recipe.AddIngredient(ModContent.ItemType<AstralBar>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<XerocsGreatsword>());
            recipe.AddIngredient(ModContent.ItemType<Floodtide>());
            recipe.AddIngredient(ModContent.ItemType<Hellkite>());
            recipe.AddIngredient(ModContent.ItemType<TemporalFloeSword>());
            recipe.AddIngredient(ModContent.ItemType<TerraEdge>());
            recipe.AddIngredient(ModContent.ItemType<AstralBar>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 107);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(ModContent.BuffType<GlacialState>(), 300);
            }
            target.AddBuff(BuffID.CursedInferno, 600);
            target.AddBuff(BuffID.Frostburn, 600);
            target.AddBuff(BuffID.OnFire, 600);
            target.AddBuff(BuffID.Ichor, 300);
            if (target.type == NPCID.TargetDummy || !target.canGhostHeal || player.moonLeech)
            {
                return;
            }
            int healAmount = Main.rand.Next(3) + 2;
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(ModContent.BuffType<GlacialState>(), 300);
            }
            target.AddBuff(BuffID.CursedInferno, 600);
            target.AddBuff(BuffID.Frostburn, 600);
            target.AddBuff(BuffID.OnFire, 600);
            target.AddBuff(BuffID.Ichor, 300);
			if (player.moonLeech)
				return;
            int healAmount = Main.rand.Next(3) + 2;
            player.statLife += healAmount;
            player.HealEffect(healAmount);
        }
    }
}
