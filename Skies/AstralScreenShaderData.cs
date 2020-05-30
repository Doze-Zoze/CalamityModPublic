using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;

namespace CalamityMod.Skies
{
    public class AstralScreenShaderData : ScreenShaderData
    {
        public AstralScreenShaderData(Ref<Effect> shader, string passName) : base(shader, passName) { }

        public override void Apply()
        {
            Vector3 vec = Main.bgColor.ToVector3();
            vec *= (CalamityWorld.death ? 0.7f : 0.4f);
            base.UseOpacity(Math.Max(vec.X, Math.Max(vec.Y, vec.Z)));
            base.Apply();
        }
    }
}
