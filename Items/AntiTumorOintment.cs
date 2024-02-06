﻿using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace CalamityMod.Items
{
    public class AntiTumorOintment : ModItem, ILocalizedModType
    {
        public static bool state = false;
        public new string LocalizationCategory => "Items.Misc";
        public override void SetDefaults()
        {
            Item.width = 44;
            Item.height = 34;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = (ContentSamples.CreativeHelper.ItemGroup)CalamityResearchSorting.SpawnPrevention;
		}

        public override void UpdateInventory(Player player)
        {
            state = player.Calamity().disableHiveCystSpawns; //So that any item of this type always has the proper flag for use in the tooltip
        }

        public override bool CanRightClick() => true;

        public override void RightClick(Player player)
        {
            if (player.Calamity().disableHiveCystSpawns == true)
                player.Calamity().disableHiveCystSpawns = false;
            else
                player.Calamity().disableHiveCystSpawns = true;

            bool favorited = Item.favorited;
            Item.SetDefaults(ModContent.ItemType<AntiTumorOintment>());
            Item.stack++;
            Item.favorited = favorited;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameI, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture;

            if (state)
            {
                //Replace with enabled texture
                texture = ModContent.Request<Texture2D>("CalamityMod/Items/AntiTumorOintment").Value;
                spriteBatch.Draw(texture, position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            }
            else
            {
                texture = ModContent.Request<Texture2D>("CalamityMod/Items/AntiTumorOintment").Value;
                spriteBatch.Draw(texture, position, null, Color.White, 0f, origin, scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D texture;

            if (state)
            {
                //Replace with enabled texture
                texture = ModContent.Request<Texture2D>("CalamityMod/Items/AntiTumorOintment").Value;
                spriteBatch.Draw(texture, Item.position - Main.screenPosition, null, lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            else
            {
                texture = ModContent.Request<Texture2D>("CalamityMod/Items/AntiTumorOintment").Value;
                spriteBatch.Draw(texture, Item.position - Main.screenPosition, null, lightColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            string text;
            if (state == true)
                text = "Currently On";
            else
                text = "Currently Off";
            tooltips.FindAndReplace("[STATE]", text);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DemoniteBar, 5).
                AddIngredient(ItemID.BottledWater).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
