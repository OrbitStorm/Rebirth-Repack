using System;

namespace Server.Items
{
	public enum CraftQuality
	{
		Low,
		Regular,
		Exceptional
	}

	public enum DurabilityLevel
	{
		Regular,
		Durable,
		Substantial,
		Massive,
		Fortified,
		Indestructible
	}

	public enum ArmorProtectionLevel
	{
		Regular,
		Defense,
		Guarding,
		Hardening,
		Fortification,
		Invulnerability,
	}

	public enum ArmorBodyType
	{
		Gorget,
		Gloves,
		Helmet,
		Arms,
		Legs, 
		Chest,
		Shield
	}

	public enum ArmorMaterialType
	{
		Cloth,
		Leather,
		Studded,
		Bone,
		Spined,
		Horned,
		Barbed,
		Ringmail,
		Chainmail,
		Plate
	}

	public enum ArmorMeditationAllowance
	{
		All,
		Half,
		None
	}
}
