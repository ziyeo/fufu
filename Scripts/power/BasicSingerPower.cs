using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Fufu.Scripts.Power;

public class BasicSingerPower : CustomPowerModel
{
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerType Type => PowerType.Buff;
    public override bool AllowNegative => false;
    public override string CustomPackedIconPath => "res://fufu/images/powers/basic_singer.png";
    public override string CustomBigIconPath => "res://fufu/images/powers/big/basic_singer.png";


    // 回合结束触发（官方原版）
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);

        if (side != Owner.Side)
            return;

        // 基础治疗 2 点
        int heal = 2;

        // 获得 SingerHealBuff 加成（每层 +1）
        heal += Owner.GetPowerAmount<SingerHealBuffPower>();
        heal += Owner.GetPowerAmount<SingerTempHealBuffPower>();

        // 最终回血
        await CreatureCmd.Heal(Owner, heal*Amount);

        await PowerCmd.TickDownDuration(this);
    }
}