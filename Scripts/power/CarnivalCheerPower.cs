using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Fufu.Scripts.Power;

public sealed class CarnivalCheerPower : CustomPowerModel
{
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerType Type => PowerType.Buff;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://fufu/images/powers/carnival_cheer.png";
    public override string CustomBigIconPath => "res://fufu/images/powers/big/carnival_cheer.png";
    // "受伤或回血时获得气氛值。";

    private int _lastHealth;

   public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);
        if (side != Owner.Side) return;

        if (Owner.CurrentHp != _lastHealth)
        {
            await PowerCmd.Apply<AtmospherePower>(choiceContext, new[] { Owner }, Amount, Owner, null);
        }

        _lastHealth = Owner.CurrentHp;
    }
}