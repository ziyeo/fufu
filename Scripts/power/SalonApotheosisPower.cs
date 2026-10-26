using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fufu.Scripts.Power;

public class SalonApotheosisPower : CustomPowerModel
{
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerType Type => PowerType.Buff;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://fufu/images/powers/salon_apotheosis.png";
    public override string CustomBigIconPath => "res://fufu/images/powers/big/salon_apotheosis.png";
    // "回合结束时，每有2个召唤物，获得1点气氛值。";

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);
        if (side != Owner.Side) 
            return;

        Creature player = Owner;
        if (player.CombatState == null) 
            return;

        // 获取召唤物数量（你的沙龙属于宠物/召唤物）
        int summons = Owner.GetPowerAmount<BasicSingerPower>();
        int summons2 = Owner.GetPowerAmount<ShadowSalonPower>();
        if (summons <= 0 && summons2 <= 0)
            return;

        int summonCount = summons + summons2;

        // 每1个召唤物 = 获得 Amount 点气氛（基础1，升级2）
        int energyGain = summonCount / 2 ;
        if (energyGain <= 0)
            return;


        await PowerCmd.Apply<ElementEnergyPower>(choiceContext, new[] { player }, energyGain*Amount, player, null);
    }
}