using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using Fufu.FufuModCode.Powers;

namespace Fufu.Scripts.Power;
// 小图标（血条下显示）：res://fufu/images/powers/elementenergy.png
// 大图标（悬浮详情）：res://fufu/images/powers/big/elementenergy.png

public class ElementEnergyPower : CustomPowerModel
{
	public override PowerType Type => PowerType.Buff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override bool AllowNegative => false;
	public override string CustomPackedIconPath => "res://fufu/images/powers/elementenergypower.png";
	public override string CustomBigIconPath => "res://fufu/images/powers/big/elementenergypower.png";
	// 元素能量本身不加伤害，只做资源
	// public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	// {
	//     return 0m;
	// }

	// // 上限：用你 FufuMod 里的上限（默认无上限）
	// public override async Task BeforeApplied(Creature target, decimal amount, Creature? applier, CardModel? cardSource)
	// {
	//     decimal finalAmount = Math.Clamp(
	//         Amount + amount, 
	//         0, 
	//         Fufu.FufuModCode.Character.FufuMod.ElementEnergyMax
	//     );
	//     if (finalAmount != Amount)
	//     {
	//         await PowerCmd.SetAmount<ElementEnergyPower>(target, finalAmount, applier, cardSource);
	//     }
	//}
}
