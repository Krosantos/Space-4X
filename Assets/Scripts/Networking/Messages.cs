using System.Collections.Generic;
using UnityEngine.Networking;

public class Messages{

    public const short MoveUnit = 100;
    public const short DestroyUnit = 101;
    public const short CreateUnit = 102;
    public const short UnlockTech = 103;
    public const short ChangeDiploStatus = 104;
    public const short TransmitMap = 105;
    public const short SendMessage = 106;
    public const short EndTurn = 107;
    public const short TakeTurn = 108;
    public const short EndGame = 109;
    public const short RegisterNewPlayer = 110;
    public const short UpdateResources = 111;
    public const short CheckForMap = 112;
}

public class MoveUnitMsg : MessageBase
{
    public int UnitId;
    public int HexTileId;

    public MoveUnitMsg() { }
    public MoveUnitMsg(int unitId, int hexTileId)
    {
        UnitId = unitId;
        HexTileId = hexTileId;
    }
}

public class RegisterNewPlayerMsg : MessageBase {
    public int PlayerId;

    public RegisterNewPlayerMsg() { }
    public RegisterNewPlayerMsg(int input) {
        PlayerId = input;
    }
}

public class RequestMapMsg : MessageBase
{
    public int Index;

    public RequestMapMsg() { }
    public RequestMapMsg(int index)
    {
        Index = index;
    }
}

public class TransmitMapMsg : MessageBase {
    public int[] SerializedMapChunk;
    public bool IsFinalPiece;

    public TransmitMapMsg() { }
    public TransmitMapMsg(int[] input, bool isFinal) {
        SerializedMapChunk = input;
        IsFinalPiece = isFinal;
    }
}

public class UpdateResourcesMsg : MessageBase {
    public float Amount;
    public int CargoId;

    public UpdateResourcesMsg() { }
    public UpdateResourcesMsg(float inputAmount, int inputCargoId) {
        Amount = inputAmount;
        CargoId = inputCargoId;
    }
}

public class CheckMapMsg : MessageBase
{
    public MapState MapExists;

    public CheckMapMsg() { }
    public CheckMapMsg(MapState mapExists)
    {
        MapExists = mapExists;
    }
}
