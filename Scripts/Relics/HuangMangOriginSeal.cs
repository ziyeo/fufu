using BaseLib.Abstracts;
using BaseLib.Utils;
using Fufu.FufuModCode.Character;
using Fufu.FufuModCode.Character.RelicPool;
using Fufu.FufuModCode.Relics;
using Fufu.Scripts.Power;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Fufu.Scripts.Relics;

[Pool(typeof(FufuRelicPool))]
public sealed class HuangMangOriginSeal : FufuModRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
    public override string PackedIconPath => "res://fufu/images/relics/HuangMangOriginSeal.png";
    protected override string BigIconPath => "res://fufu/images/relics/big/HuangMangOriginSeal.png";
    public static async Task SwitchToHuangAttribute(Creature player, PlayerChoiceContext choiceContext)
    {

    // 主动给奖励
    await PowerCmd.Apply<ElementEnergyPower>(choiceContext, player, 3, player, null);
    await PowerCmd.Apply<AtmospherePower>(choiceContext, player, 1, player, null);
    }
    // 放在HuangMangOriginSeal.cs里
    public static async Task RefreshStateIcon(Creature player, PlayerChoiceContext choiceContext)
    {
    // 先移除旧的Power
    await PowerCmd.Remove<HuangMangStatePower>(player);
    // 重新施加，强制刷新图标
    await PowerCmd.Apply<HuangMangStatePower>(choiceContext, player, 1, player, null);
    }
    public override async Task AfterRoomEntered(AbstractRoom room)
	{
		if (room is CombatRoom)
		{
			Flash();
            Creature player = base.Owner.Creature;
            var context = new ThrowingPlayerChoiceContext();

            // ========== 设置元素能量（修复空引用） ==========
            var elementPower = player.GetPower<ElementEnergyPower>();
            if (elementPower != null)
            {
                elementPower.SetAmount(FufuModCode.Character.FufuMod.ElementEnergyValue, true);
            }
            else
            {
                await PowerCmd.Apply<ElementEnergyPower>(context, player, FufuModCode.Character.FufuMod.ElementEnergyValue, player, null);
            }

            // ========== 设置气氛值（修复空引用） ==========
            var atmospherePower = player.GetPower<AtmospherePower>();
            if (atmospherePower != null)
            {
                atmospherePower.SetAmount(FufuModCode.Character.FufuMod.AtmosphereValue, true);
            }
            else
            {
                await PowerCmd.Apply<AtmospherePower>(context, player, FufuModCode.Character.FufuMod.AtmosphereValue, player, null);
            }
		}
	}

}