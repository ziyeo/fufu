using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using System;

namespace Fufu.Scripts.Power;

public class ElementMasteryPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://fufu/images/powers/element_mastery.png";
    public override string CustomBigIconPath => "res://fufu/images/powers/big/element_mastery.png";

    //"元素能量不会损耗。消耗元素时，不会消耗元素能量。";
}