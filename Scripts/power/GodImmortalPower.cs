using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Fufu.Scripts.Power;

public class GodlmmortalPower : CustomPowerModel
{
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerType Type => PowerType.Buff;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://fufu/images/powers/god_immortal.png";
    public override string CustomBigIconPath => "res://fufu/images/powers/big/god_immortal.png";
    //"每次受到伤害或恢复生命时，获得1点力量。";

    // 受到伤害时
    private int _lastHealth;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);
        if (side != Owner.Side) return;

        if (Owner.CurrentHp != _lastHealth)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, Amount, Owner, null);
        }

        _lastHealth = Owner.CurrentHp;
    }
}