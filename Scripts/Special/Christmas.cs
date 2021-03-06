using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Accounting;
using Server.Mobiles;
using Server.Commands;


namespace Server.Misc
{
	public class Coal : IronOre
	{
		[Constructable]
		public Coal() : this( 1 )
		{
		}

		[Constructable]
		public Coal( int amount ) : base( amount )
		{
			Name = "coal";
			Hue = 1109;
		}

		public override void AppendClickName(System.Text.StringBuilder sb, bool plural)
		{
			sb.Append( "coal" );
		}

		public Coal( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public static bool IsActive( Mobile from )
		{
			if ( from.AccessLevel > AccessLevel.Player )
				return true;
			else
				return ( DateTime.Now.Day == 17 || DateTime.Now.Day == 11 ) && Utility.RandomBool() && from.Karma < 0;
		}
	}

	public class GiftBag : Bag
	{
		[Constructable]
		public GiftBag( bool nice ) 
		{
			Item item = null;

			Hue = Utility.RandomList( 32, 64, 2301 );

			if ( nice )
			{
				Name = "Happy Holidays!";
				DropItem( MakeNewbie( new WristWatch() ) );
				if ( Utility.RandomBool() )
				{
					item = new Cake();
                    item.ItemID = 4164;
					item.Hue = 432;
					item.Name = "fruit cake";
					DropItem( MakeNewbie( item ) );
				}
				else
				{
					DropItem( MakeNewbie( new CheesePizza() ) );
				}

                if (Utility.RandomBool())
                    DropItem(MakeNewbie(new BeverageBottle(BeverageType.Champagne)));
                else
                    DropItem(MakeNewbie(new BeverageBottle(BeverageType.EggNog)));

				switch ( Utility.Random( 7 ) )
				{
					default:
					case 0:
						DropItem( MakeNewbie( new Apple() ) );
						break;
					case 1:
						DropItem( MakeNewbie( new Pear() ) );
						break;
					case 2:
						DropItem( MakeNewbie( new Bananas() ) );
						break;
					case 3:
						DropItem( MakeNewbie( new Dates() ) );
						break;
					case 4:
						DropItem( MakeNewbie( new Coconut() ) );
						break;
					case 5:
						DropItem( MakeNewbie( new Peach() ) );
						break;
					case 6:
						DropItem( MakeNewbie( new Grapes() ) );
						break;
				}

				item = new Goblet();
				item.Name = "a champagne glass";
				item.Hue = 71;
				DropItem( MakeNewbie( item ) );

				item = new Goblet();
				item.Name = "a champagne glass";
				item.Hue = 34;
				DropItem( MakeNewbie( item ) );

				DropItem( MakeNewbie( new FireworksWand( 100 ) ) );

				item = new Item( 5359 );
				item.Hue = Utility.RandomList( 32, 64, 2301 );
				item.Name = "Seasons Greetings";
				DropItem( MakeNewbie( item ) );
				
				//DropItem( new IrinaFlowers() );
			}
			else
			{
				Name = "You were naughty this year!";

                item = new Bacon();
                item.ItemID = 4164; // spam
				DropItem( MakeNewbie( item ) ); 

				DropItem( MakeNewbie( new Coal() ) );

				if ( Utility.RandomBool() )
				{
					item = new Lemon();
					item.Name = "Sour attitude";
					DropItem( item );
				}

				item = new Kindling();
				item.Name = "switches";
				DropItem( item ); // not newbied...

				item = new Item( 5359 );
				item.Hue = Utility.RandomList( 32, 64, 2301 );
				item.Name = "Maybe next year you will get a nicer gift.";
				DropItem( MakeNewbie( item ) );
			}
		}

		private static Item MakeNewbie( Item item )
		{
			item.LootType = LootType.Newbied;
			return item;
		}

		public GiftBag( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class ChristmasGifts
	{
		public static void Initialize()
		{
			CommandSystem.Register( "AddChristmasGifts", AccessLevel.Administrator, new CommandEventHandler( AddGifts ) );
		}

		private static void AddGifts( CommandEventArgs args )
		{
			args.Mobile.SendMessage( "Adding gifts...." );
			int good = 0, bad = 0;

			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( !m.Player || !(m.Account is Account) || m.AccessLevel > AccessLevel.Player )
					continue;
				Account acct = (Account)m.Account;

				if ( acct.AccessLevel > AccessLevel.Player || acct.LastLogin + TimeSpan.FromDays( 91 ) < DateTime.Now )
					continue;

				if ( m is PlayerMobile && ((PlayerMobile)m).GameTime < TimeSpan.FromHours( 1 ) )
					continue;

				m.AddToBackpack( new GiftBag( m.Karma > 0 ) );
				if ( m.Karma > 0 )
					good++;
				else
					bad++;
			}

			args.Mobile.SendMessage( "Done!" );
			args.Mobile.SendMessage( "Gave away {0} good and {1} bad gift bags ({2} total).", good, bad, good+bad );
		}
	}
}
