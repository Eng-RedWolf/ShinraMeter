﻿using System;

namespace Tera.Game.Messages
{
   
    public class SDespawnNpc : ParsedMessage
    {
        internal SDespawnNpc(TeraMessageReader reader) : base(reader)
        {

            Npc = reader.ReadEntityId();
        }
        public EntityId Npc { get;}

    }
}