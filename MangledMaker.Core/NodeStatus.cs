namespace MangledMaker.Core
{
    using System;

    [Flags]
    public enum NodeStatus
    {
        None,
        Invalid,
        Truncated,
        Error = Invalid | Truncated,
        BasicStatus = Error,
        PointerReference = 0x10,
        //PublicStatus = Invalid | Missing | PointerReference,
        Udc = 0x20,
        UdtThunk = 0x40,
        Array = 0x80,
        ExtendedStatus = PointerReference | Udc | UdtThunk | Array,
        NoTypeEncoding = 0x100,
        PinPointer = 0x200,
        ComArray = 0x400,
        VirtualCallThunk = 0x800,
        SpecialStatus = NoTypeEncoding | PinPointer | ComArray | VirtualCallThunk,
        SpecialAndExtendedStatus = SpecialStatus | ExtendedStatus,
        TypeStatus = ExtendedStatus | VirtualCallThunk,
        FullTypeStatus = TypeStatus | BasicStatus
    }
}