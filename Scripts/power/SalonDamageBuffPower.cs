using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Fufu.Scripts.Power;

public class SalonDamageBuffPower : CustomPowerModel
{
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerType Type => PowerType.Buff;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://fufu/images/powers/salon_buff.png";
    public override string CustomBigIconPath => "res://fufu/images/powers/big/salon_buff.png";

    public int Duration { get; set; }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);
        if (side == Owner.Side && Duration > 0)
        {
            Duration--;
            if (Duration <= 0)
            {
                await PowerCmd.Remove(this);
            }
        }
    }

}