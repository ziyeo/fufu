using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Fufu.FufuModCode.Character;

namespace Fufu.Scripts.Power;

public sealed class DualStateMasteryPower : CustomPowerModel
{
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerType Type => PowerType.Buff;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://fufu/images/powers/dual_state_mastery.png";
    public override string CustomBigIconPath => "res://fufu/images/powers/big/dual_state_mastery.png";
    /// <summary>
    /// 荒+2元素能量，芒+1气氛值。
    /// </summary>

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        await base.AfterTurnEnd(choiceContext, side);

        if (side != Owner.Side)
            return;

        Creature player = Owner;
        if (player.CombatState == null)
            return;

        // ====================== 双态逻辑 ======================
        if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Huang)
        {
            // 荒属性：+2 元素能量
            await PowerCmd.Apply<ElementEnergyPower>(choiceContext, new[] { player }, 2, player, null);
        }
        else if (FufuModCode.Character.FufuMod.CurrentAttr == FufuAttr.Mang)
        {
            // 芒属性：+1 气氛值
            await PowerCmd.Apply<AtmospherePower>(choiceContext, new[] { player }, 1, player, null);
        }
    }
}