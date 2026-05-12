using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Fufu.FufuModCode.Powers;

namespace Fufu.Scripts.Power;

public sealed class AtmospherePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;
    public override string CustomPackedIconPath => "res://fufu/images/powers/atmospherepower.png";
    public override string CustomBigIconPath => "res://fufu/images/powers/big/atmospherepower.png";
    

    // public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    // {
    //     if (dealer != Owner) return 0m;
    //     return Amount;
    // }

    // public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
    // {
    //     decimal finalAmount = Math.Clamp(Amount + amount, 0, 40);
    //     if (finalAmount != Amount)
    //     {
    //         await PowerCmd.SetAmount<AtmospherePower>(target, finalAmount, applier, cardSource);
    //     }
    // }
}