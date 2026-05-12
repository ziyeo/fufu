using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;


namespace Fufu.Scripts.Power;

public sealed class SummonBasePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://fufu/images/powers/summon_base.png";
    public override string CustomBigIconPath => "res://fufu/images/powers/big/summon_base.png";

    // "召唤召唤物时，获得元素能量。";
    
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        await base.AfterTurnEnd(choiceContext, side);

        // 只在自己的回合结束触发
        if (side != Owner.Side)
            return;

        Creature player = base.Owner;
        if (player.CombatState == null)
            return;

        // 获取自己的存活召唤物
        int summons = Owner.GetPowerAmount<BasicSingerPower>();
        int summons2 = Owner.GetPowerAmount<ShadowSalonPower>();
        if (summons <= 0 && summons2 <= 0)
            return;

        int summonCount = summons + summons2;

        // 每3个召唤物 = 1元素能量
        int energyGain = summonCount / 3;
        if (energyGain <= 0)
            return;

        // 施加元素能量（官方写法）
        await PowerCmd.Apply<ElementEnergyPower>(choiceContext, new[] { player }, energyGain, player, null);
    }
}