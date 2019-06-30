using System;
using System.IO;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace acornplugin
{

    /// <summary>
    /// I will fucking beat you to death.
    /// </summary>

    [ApiVersion(2, 1)]
    public class acornplugin : TerrariaPlugin
    {
        /// <summary>
        /// The name of the plugin.
        /// </summary>
        public override string Name => "Acorn Plugin";

        /// <summary>
        /// The version of the plugin in its current state.
        /// </summary>
        public override Version Version => new Version(1, 0, 0);

        /// <summary>
        /// The author(s) of the plugin.
        /// </summary>
        public override string Author => "Nyanko";

        /// <summary>
        /// A short, one-line, description of the plugin's purpose.
        /// </summary>
        public override string Description => "Turns player-dropped items into something else. like an acorn";

        public acornplugin(Main game) : base(game)
        {

        }
        int itemid;
        public override void Initialize()
        {
            ServerApi.Hooks.NetGetData.Register(this, G);
            Commands.ChatCommands.Add(new Command("item", additem, "additem"));
        }
        
        void G(GetDataEventArgs args)
        {
            if (!args.Handled && args.MsgID == PacketTypes.ItemDrop)
            {
                using (var reader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
                {
                    int id = reader.ReadInt16();
                    if (id == 400)
                    {
                        reader.ReadSingle();   
                        reader.ReadSingle();   
                        reader.ReadSingle();   
                        reader.ReadSingle();    

                        short stack = reader.ReadInt16();

                        TShock.Players[args.Msg.whoAmI].SendData(PacketTypes.ItemDrop, "", id);
                        Item stuff = TShock.Utils.GetItemById(itemid);
                        TShock.Players[args.Msg.whoAmI].GiveItem(stuff.netID, stuff.Name, stuff.width, stuff.height, stack);
                        TShock.Players[args.Msg.whoAmI].SendSuccessMessage("God, I would viole");
                        args.Handled = true;
                    }
                    
                }
            }
        }
        void additem(CommandArgs args)
        {
            if (args.Parameters.Count != 1 && Int32.TryParse(args.Parameters[0], out itemid))
            {
                args.Player.SendErrorMessage("Incorrect syntax. Type /additem <id>");
            }
            else
            {
                itemid = Int32.Parse(args.Parameters[0]);
                Item t = TShock.Utils.GetItemById(itemid);
                args.Player.SendSuccessMessage($"Successfully changed to {t.Name}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.NetGetData.Deregister(this, G);
            }
            base.Dispose(disposing);
        }
    }
}
