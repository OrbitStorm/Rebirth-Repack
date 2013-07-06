using System;
using Server;

namespace Server.Items
{
	//<Seethe> so for the parchment it uses a rolled map image, but says "a parchment", double click and it opens a gump that reads "Thou'st holding a parchment from the box of darkness, thy surely are a survivor."
	public class DarkParchment : BaseItem
	{
		[Constructable]
		public DarkParchment() : base( 0x14EE )
		{
			Name = "a parchment";
		}

		public DarkParchment( Serial s ) : base( s )
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			object root = RootParent;
			if ( from.NetState != null && ( root == from || root == null ) )
				new Server.Menus.Questions.QuestionMenu( "Thou'st holding a parchment from the box of darkness, thy surely are a survivor.", new string[]{ "Close" } ).SendTo( from.NetState );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize (reader);
			int v = reader.ReadInt();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize (writer);
			writer.Write( (int)0 );
		}
	}
}
