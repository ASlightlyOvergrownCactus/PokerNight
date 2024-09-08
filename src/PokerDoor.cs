using System.Linq;
using On.MoreSlugcats;

namespace PokerNight;

using DevInterface;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using RWCustom;
using static Pom.Pom;

public static class PokerDoor
{
	public static readonly float spriteWidth = 53; // Change upon changing sprite for door.
	public static readonly float spriteheight = 62; // Change upon changing sprite for door.
    internal class PokerData : ManagedData
    {
        [StringField("room", "HI_C04")]
        internal string String = "HI_C04";

        public PokerData(PlacedObject owner) : base(owner, null)
        {
            
        }
    }

    internal static void __RegisterPokerDoor()
    {
	    // A different approach for registering objects, more classes involved but accessing your data is much nicer.
	    // Registers a self implemented Managed Object Type
	    // It handles spawning its object, data and representation 
	    RegisterManagedObject(new PokerDoorType());
	    // Could also be achieved with RegisterManagedObject(new ManagedObjectType("CuriousObject", typeof(CuriousObjectType.CuriousObject), typeof(CuriousObjectType.CuriousData), typeof(CuriousObjectType.CuriousRepresentation)));
	    // but at the expense of some extra reflection calls
    }
    
    // Some other objects, this time we're registering type, object, data and representation on our own
    public static class EnumExt_ManagedPlacedObjects
    {
	    public static PlacedObject.Type PokerDoorObject = new(nameof(PokerDoorObject), true);
	    public static PlacedObject.Type PokerDoorLocation = new(nameof(PokerDoorLocation), true);
    }
    
    // A very curious object, part managed part manual
	// Overriding the base class here was optional, you could have instantiated it passing all the types, but doing it like this saves some reflection calls.
	internal class PokerDoorType : ManagedObjectType
	{
		// Ignore the stuff in the baseclass and write your own if you want to
		public PokerDoorType() : base("PokerDoor", "POM examples", null, typeof(DoorData),
			typeof(PokerDoorRepresentation)) // this could have been (PlacedObjects.CuriousObject, typeof(CuriousObject), typeof(...)...)
		{
		}

		// Override at your own risk ? the default behaviour works just fine if you passed in a name to the constructor, but maybe you know what you're doign
		//public override PlacedObject.Type GetObjectType()
		//{
		//    return EnumExt_ManagedPlacedObjects.CuriousObject;
		//}

		public override UpdatableAndDeletable MakeObject(PlacedObject placedObject, Room room)
		{
			return new PokerDoorObject(placedObject, room);
		}

		// Maybe you need different parameters for these ? You do you...
		//public override PlacedObject.Data MakeEmptyData(PlacedObject pObj)
		//{
		//    return new CuriousData(pObj);
		//}

		//public override PlacedObjectRepresentation MakeRepresentation(PlacedObject pObj, ObjectsPage objPage)
		//{
		//    return new CuriousRepresentation(GetObjectType(), objPage, pObj);
		//}

		// Our curious and useful object
		class PokerDoorObject : CosmeticSprite, IDrawable
		{
			private readonly PlacedObject placedObject;
			private readonly List<PlacedObject> otherPlaces;

			public PokerDoorObject(PlacedObject placedObject, Room room)
			{
				this.placedObject = placedObject;
				this.room = room;
				otherPlaces = new List<PlacedObject>();

				// Finds aditional info from other objects
				foreach (PlacedObject pobj in room.roomSettings.placedObjects)
				{
					if (pobj.type == EnumExt_ManagedPlacedObjects.PokerDoorLocation && pobj.active)
						otherPlaces.Add(pobj);
				}

				Debug.Log("PokerDoorObject started and found " + otherPlaces.Count + " location");
			}

			// IDrawable stuff
			public override void AddToContainer(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, FContainer? newContainer)
			{
				base.AddToContainer(sLeaser, rCam, newContainer);
				if (newContainer == null) newContainer = rCam.ReturnFContainer("Midground");
				for (int i = 0; i < sLeaser.sprites.Length; i++)
				{
					newContainer.AddChild(sLeaser.sprites[i]);
				}
			}

			public override void ApplyPalette(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
			{
				base.ApplyPalette(sLeaser, rCam, palette);
				for (int i = 0; i < sLeaser.sprites.Length; i++)
				{
					sLeaser.sprites[i].color = Color.white;
				}
			}

			public override void DrawSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, float timeStacker, Vector2 camPos)
			{
				base.DrawSprites(sLeaser, rCam, timeStacker, camPos);
				for (int i = 0; i < sLeaser.sprites.Length; i++)
				{
					sLeaser.sprites[i]
						.SetPosition(this.placedObject.pos - camPos);
					sLeaser.sprites[i].scale = ((DoorData)this.placedObject.data).GetValue<float>("scale");
					Color clr = sLeaser.sprites[i].color;
					sLeaser.sprites[i].color = clr;
				}
			}

			public override void InitiateSprites(RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam)
			{
				base.InitiateSprites( sLeaser, rCam);
				sLeaser.sprites = new FSprite[1];
				for (int i = 0; i < sLeaser.sprites.Length; i++)
				{
					sLeaser.sprites[i] = new FSprite("Circle20");
					sLeaser.sprites[i].element = Plugin.elements[0];
				}

				AddToContainer(sLeaser, rCam, null);
			}

			public override void Update(bool eu)
			{
				base.Update(eu);
				if (room.PlayersInRoom.Count > 0)
				{
					if (AABBRectCheck(room.PlayersInRoom.First().bodyChunks[0].pos) && room.PlayersInRoom[0].input[0].jmp)
					{
						Debug.Log("teleport NOW!!!"); // Add room working with POM input
						Player player1 = room.PlayersInRoom[0];
						MoreSlugcats.MSCRoomSpecificScript.RoomWarp(room.PlayersInRoom[0], room, "SL_F01", default, false);
						
					}
				}
			}
			
			public bool AABBRectCheck(Vector2 point) // Change dimensions of poker door when we get actual door sprite
			{
				Vector2 topLeft = placedObject.pos - new Vector2(spriteWidth / 2f, spriteheight / -2f);
				Vector2 topRight = placedObject.pos - new Vector2(spriteWidth / -2f, spriteheight / -2f);
				Vector2 bottomLeft = placedObject.pos - new Vector2(spriteWidth / 2f, spriteheight / 2f);
				Vector2 bottomRight = placedObject.pos - new Vector2(spriteWidth / -2f, spriteheight / 2f);

				return (point.x >= topLeft.x && point.x <= topRight.x && point.y >= bottomLeft.y && point.y <= topRight.y);
			}
		}

		// The data for our poker door object
		// We declare a managed field called "scale" in the base constructor,
		// and another one called "red" that is tied to an actual field that can be accessed directly
		// some more managed fields that arent used for anything
		class DoorData : ManagedData
		{
#pragma warning disable 0649 // We're reflecting over these fields, stop worrying about it stupid compiler

			[StringField("RoomConnection", "HI_C04", "RoomConnection")]
			public string roomConnection = "HI_C04";

			// For certain types it's not possible to use the Attribute notation, and you'll have to pass the field to the constructor
			// but you can still link a field in your object to the managed field and they will stay in sync.
			[BackedByField("ev2")] public Vector2 extraPos;

			// Just make sure you pass all the expected fields to the ManagedData contructor
			[BackedByField("ev3")] public Vector2 extraPos2;

			// Until there is a better implementation, you'll have to do this for Vector2Field, IntVector2Field, EnumField and ColorField.
			[BackedByField("msid")] public SoundID mySound = SoundID.Bat_Afraid_Flying_Sounds;
#pragma warning restore 0649

			private static ManagedField[] customFields = new ManagedField[]
			{
				new FloatField("scale", 0.1f, 10f, 1f, displayName: "Scale"),
				new Vector2Field("ev2", new Vector2(-100, -40), Vector2Field.VectorReprType.line),
				//new Vector2Field("ev3", new Vector2(-100, -40), Vector2Field.VectorReprType.none),
				new DrivenVector2Field("ev3", "ev2",
					new Vector2(-100, -40)), // Combines two vector2s in one single constrained control
				new ExtEnumField<SoundID>("msid", SoundID.Bat_Afraid_Flying_Sounds,
					new SoundID[] { SoundID.Bat_Afraid_Flying_Sounds, SoundID.Bat_Attatch_To_Chain },
					displayName: "What sound a bat makes"),
			};

			public DoorData(PlacedObject owner) : base(owner, customFields)
			{

			}

			// Serialization has to include our manual field
			public override string ToString()
			{
				//Debug.Log("CuriousData serializing as " + base.ToString() + "~" + rotation);
				return base.ToString() + "~" + 30f; // placeholder
			}

			public override void FromString(string s)
			{
				//Debug.Log("CuriousData deserializing from "+ s);
				base.FromString(s);
				string[] arr = Regex.Split(s, "~");
				try
				{

				}
				catch
				{
				} // bad data, hopefully the default is fine :)
				//Debug.Log("CuriousData got rotation = " + rotation);
			}
		}

		// Representation... ManagedData takes care of creating controls for managed fields
		// but we have one unmanaged field to control
		class PokerDoorRepresentation : ManagedRepresentation
		{
			public PokerDoorRepresentation(PlacedObject.Type placedType, ObjectsPage objPage, PlacedObject pObj) : base(
				placedType, objPage, pObj)
			{
			}

			public override void Update()
			{
				base.Update();
			}

			
		}
	}
}