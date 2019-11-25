﻿using Tono;

namespace JitStreamDesigner
{
    public static class LAYER
    {
        public static readonly NamedId ActiveClass = NamedId.From("ActiveClass", 9011);
        public static readonly NamedId Clock = NamedId.From("Clock", 9010);    // Need to set upper with Tono.Gui.Uwp.uLayer.LogPanel
        public static readonly string DoubleBufferMap = "DoubleBufferMap";
        public static readonly NamedId ToolButtons = NamedId.From("ToolButtons", 5001);
        public static readonly NamedId ToolButtonBox = NamedId.From("ToolButtonBox", 5000);
        public static readonly NamedId EventQueue = NamedId.From("EventQueue", 400);
        public static readonly NamedId JitProcess = NamedId.From("JitProcess", 300);
    }
}
