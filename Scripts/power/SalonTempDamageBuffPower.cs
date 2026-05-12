using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Power;

// 【临时】沙龙增伤（只持续1回合，和永久Buff不冲突）
public class SalonTempDamageBuffPower : CustomPowerModel
{
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerType Type => PowerType.Buff;
    public override bool AllowNegative => false;
    public int Duration { get; set; }

    public override string CustomPackedIconPath => "res://fufu/images/powers/salon_temp_damage_buff.png";
    public override string CustomBigIconPath => "res://fufu/images/powers/big/salon_temp_damage_buff.png";

    // 回合结束移除（1回合）
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        await base.AfterTurnEnd(choiceContext, side);
        if (side == Owner.Side)
            Duration--;

        if (Duration <= 0)
            await PowerCmd.Remove(this);
    }
}