﻿
using LogReaper.Net.Enums;

namespace LogReaper.Net.Dto;

internal class DictionaryRecord
{
    public OdinAssLogDictionaryType Dictionary { get; set; }
    public string Uid { get; set; }
    public string Representation { get; set; }
    public uint Id { get; set; }
}
