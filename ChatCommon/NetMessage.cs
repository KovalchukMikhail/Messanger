﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatCommon
{
    public class NetMessage
    {
        public int? Id { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public string? NickNameFrom { get; set; }
        public string? NickNameTo { get; set; }
        public string IpUser { get; set; }
        public string PortUser { get; set; }

        public Command Command { get; set; }

        public string SerialazeMessageToJSON() => JsonSerializer.Serialize(this);

        public static NetMessage? DeserializeMessgeFromJSON(string message) => JsonSerializer.Deserialize<NetMessage>(message);

        public void PrintGetMessageFrom()
        {
            Console.WriteLine(ToString());
        }

        public override string ToString()
        {
            return $"{DateTime} \n Получено сообщение {Text} \n от {NickNameFrom}  ";
        }
    }
}
