using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Fufu.Scripts.Power;

public class HydroArchonPower : CustomPowerModel
{
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerType Type => PowerType.Buff;
    public override bool AllowNegative => false;


    public override string CustomPackedIconPath => "res://fufu/images/powers/hydro_archon.png";
    public override string CustomBigIconPath => "res://fufu/images/powers/big/hydro_archon.png";
    /// <summary>
    /// 回合结束获得1点元素能量。
    /// </summary>

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);

        // 只在自己回合结束触发
        if (side != Owner.Side)
            return;

        Creature player = Owner;
        if (player.CombatState == null)
            return;

        // 获得1点元素能量（参数完全正确，无类型错误）
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext, new[] { player }, 1, player, null);
    }
}