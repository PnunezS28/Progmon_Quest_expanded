using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitPointDB
{
    // Script de c# plano que contiene la base de datos con los puntos de destino de las transiciones entre escenas
    public static void Init()
    {
        foreach(var tp in transitPointDB.Keys)
        {
            var transitionId = tp;
            var transitionPoint = transitPointDB[tp];
            transitionPoint.TransitId = transitionId;
        }
        Debug.Log("Initialized TransitPointDB");

    }

    //TransitDestinationName: nombre significativo
    //DestinationSceneId: id int de la escena de destino
    //destinationTransformTag: string con el tag asociado al transform de destino en la escena para
    // hacer aparecer al jugador

    public static Dictionary<int, TransitPoint> transitPointDB { get; set; } = new Dictionary<int, TransitPoint>() {
        {
            0,
            new TransitPoint()
            {
                TransitDestinationName="SampleScene_HealCenter",
                DestinationSceneId=0,//todo crear heal center
                destinationTransformTag="HealCenter"
            }
        },
        { //Transit desde inicio a laboratorio
            1,
            new TransitPoint()
            {
                TransitDestinationName="OneWayFromStartToLab",
                DestinationSceneId=1,
                destinationTransformTag="FromStartToLab"
            }
        },
        {
            2,
            new TransitPoint()
            {
                TransitDestinationName="FromBaseToLab",
                DestinationSceneId=1,
                destinationTransformTag="FromBase"
            }
        },
        { //Transit desde inicio a laboratorio
            3,
            new TransitPoint()
            {
                TransitDestinationName="FromLabToBase",
                DestinationSceneId=2,
                destinationTransformTag="FromLab"
            }
        },
        { //Transit directo a centro de curacion
            4,
            new TransitPoint()
            {
                TransitDestinationName="DirectToHealCenter",
                DestinationSceneId=3,
                destinationTransformTag="HealCenter"
            }
        },
        { //Transit a ciudad base desde centro de curacion
            5,
            new TransitPoint()
            {
                TransitDestinationName="FromBaseToHealCenter",
                DestinationSceneId=3,
                destinationTransformTag="FromBase"
            }
        },
        { //Transit a ciudad base desde centro de curacion
            6,
            new TransitPoint()
            {
                TransitDestinationName="FromHealCenterToBase",
                DestinationSceneId=2,
                destinationTransformTag="FromHealCenter"
            }
        },
        { //Transit desde base a warp center
            7,
            new TransitPoint()
            {
                TransitDestinationName="FromWarpCenterToBase",
                DestinationSceneId=2,
                destinationTransformTag="FromWarpCenter"
            }
        },
        { //Transit a ciudad base desde warp center
            8,
            new TransitPoint()
            {
                TransitDestinationName="FromBaseToWarpCenter",
                DestinationSceneId=4,
                destinationTransformTag="FromBase"
            }
        },
        { //Transit a warpcenter desde forest
            9,
            new TransitPoint()
            {
                TransitDestinationName="FromForestToWarpCenter",
                DestinationSceneId=4,
                destinationTransformTag="FromForest"
            }
        },
        { //Transit a forest desde warp center
            10,
            new TransitPoint()
            {
                TransitDestinationName="FromWarpCenterToForest",
                DestinationSceneId=5,
                destinationTransformTag="FromWarpCenter"
            }
        },
        //Forest area
        { //Transit a forest desde warp center
            11,
            new TransitPoint()
            {
                TransitDestinationName="FromForestEntranceToForestArea1",
                DestinationSceneId=6,
                destinationTransformTag="FromForestEntrance"
            }
        },
        { //Transit a forest desde warp center
            12,
            new TransitPoint()
            {
                TransitDestinationName="FromForestArea1ToEntrance",
                DestinationSceneId=5,
                destinationTransformTag="FromForestArea1"
            }
        },
        { //Transit a forest area 1 desde area2
            13,
            new TransitPoint()
            {
                TransitDestinationName="FromForestArea2ToForestArea1",
                DestinationSceneId=6,
                destinationTransformTag="FromForestArea2"
            }
        },
        { //Transit a forest area 1 desde Left2
            14,
            new TransitPoint()
            {
                TransitDestinationName="FromForestLeft2ToForestArea1",
                DestinationSceneId=6,
                destinationTransformTag="FromForestLeft2"
            }
        },
        { //Transit a forest area 2 desde area1
            15,
            new TransitPoint()
            {
                TransitDestinationName="FromForestArea1ToForestLeft",
                DestinationSceneId=7,
                destinationTransformTag="FromForestArea1"
            }
        },
        { //Transit a forest area2 desde area 3
            16,
            new TransitPoint()
            {
                TransitDestinationName="FromForestArea3ToArea2",
                DestinationSceneId=7,
                destinationTransformTag="FromForestArea3"
            }
        },
        { //Transit a forest area2 desde area 3
            17,
            new TransitPoint()
            {
                TransitDestinationName="FromForestArea2ToArea3",
                DestinationSceneId=8,
                destinationTransformTag="FromForestArea2"
            }
        },
        { //Transit a forest area3 desde exit
            18,
            new TransitPoint()
            {
                TransitDestinationName="FromForestExitRoomToArea3",
                DestinationSceneId=8,
                destinationTransformTag="FromExitRoom"
            }
        },
        { //Transit a forest exit desde area 3
            19,
            new TransitPoint()
            {
                TransitDestinationName="FromForestExitRoomToArea3",
                DestinationSceneId=9,
                destinationTransformTag="FromForestArea3"
            }
        },
        { //Transit a forest left1 desde area 1
            20,
            new TransitPoint()
            {
                TransitDestinationName="FromForestArea1ToLeft2",
                DestinationSceneId=10,
                destinationTransformTag="FromForestArea1"
            }
        },
        { //Transit a forest exit desde area 3
            21,
            new TransitPoint()
            {
                TransitDestinationName="FromCaveToForestLeft2",
                DestinationSceneId=10,
                destinationTransformTag="FromForestCave"
            }
        },
        { //Transit a forest exit desde area 3
            22,
            new TransitPoint()
            {
                TransitDestinationName="FromForestLeft2ToCave",
                DestinationSceneId=11,
                destinationTransformTag="FromForestLeft2"
            }
        },

        //MountainArea
        { //Transit a MountainEntrance desde WarpCenter
            23,
            new TransitPoint()
            {
                TransitDestinationName="FromWarpToMountain",
                DestinationSceneId=12,
                destinationTransformTag="FromWarpCenter"
            }
        },
        { //Transit a MountainEntrance desde WarpCenter
            24,
            new TransitPoint()
            {
                TransitDestinationName="FromMountainToWarp",
                DestinationSceneId=4,
                destinationTransformTag="FromMountain"
            }
        },
        { //Transit a MountainEntrance desde area1
            25,
            new TransitPoint()
            {
                TransitDestinationName="FromMountain1ToMountainEntrance",
                DestinationSceneId=12,
                destinationTransformTag="FromMountainArea1"
            }
        },
        { //Transit a Mountain1 desde Entrance
            26,
            new TransitPoint()
            {
                TransitDestinationName="FromMountainEntranceToMountain1",
                DestinationSceneId=13,
                destinationTransformTag="FromEntrance"
            }
        },
        { //Transit a Mountain1 desde Mountain2
            27,
            new TransitPoint()
            {
                TransitDestinationName="FromMountain2ToMountain1",
                DestinationSceneId=13,
                destinationTransformTag="FromArea2"
            }
        },
        { //Transit a Mountain1 desde Mountain2
            28,
            new TransitPoint()
            {
                TransitDestinationName="FromMountain1ToMountain2",
                DestinationSceneId=14,
                destinationTransformTag="FromArea1"
            }
        },
        { //Transit a Mountain1 desde Mountain2
            29,
            new TransitPoint()
            {
                TransitDestinationName="FromMountain1ToMountain3",
                DestinationSceneId=15,
                destinationTransformTag="FromArea1"
            }
        },
        { //Transit a Mountain1 desde Mountain2
            30,
            new TransitPoint()
            {
                TransitDestinationName="FromMountain3ToMountain1",
                DestinationSceneId=13,
                destinationTransformTag="FromArea3"
            }
        },
        { //Transit a Mountain1 desde Mountain2
            31,
            new TransitPoint()
            {
                TransitDestinationName="FromMountain4ToMountain2",
                DestinationSceneId=14,
                destinationTransformTag="FromArea4"
            }
        },
        { //Transit a Mountain1 desde Mountain2
            32,
            new TransitPoint()
            {
                TransitDestinationName="FromMountain2ToMountain4",
                DestinationSceneId=16,
                destinationTransformTag="FromArea2"
            }
        },
        { //Transit a Mountain1 desde Mountain2
            33,
            new TransitPoint()
            {
                TransitDestinationName="FromMountain3ToMountainCave",
                DestinationSceneId=17,
                destinationTransformTag="FromMountain3"
            }
        },
        { //Transit a Mountain1 desde Mountain2
            34,
            new TransitPoint()
            {
                TransitDestinationName="FromMountainCaveToMountain3",
                DestinationSceneId=15,
                destinationTransformTag="FromMountainCave"
            }
        },
        { //Transit a Mountain1 desde Mountain2
            35,
            new TransitPoint()
            {
                TransitDestinationName="FromMountain4ToMountain5",
                DestinationSceneId=16,
                destinationTransformTag="FromArea5"
            }
        },
        { //Transit a Mountain1 desde Mountain2
            36,
            new TransitPoint()
            {
                TransitDestinationName="FromMountain5ToMountain4",
                DestinationSceneId=18,
                destinationTransformTag="FromArea4"
            }
        },
        { //Transit a Mountain1 desde Mountain2
            37,
            new TransitPoint()
            {
                TransitDestinationName="FromMountain5ToMountainExit",
                DestinationSceneId=19,
                destinationTransformTag="FromArea5"
            }
        },
        { //Transit a Mountain1 desde Mountain2
            38,
            new TransitPoint()
            {
                TransitDestinationName="FromMountainExitToMountain5",
                DestinationSceneId=18,
                destinationTransformTag="FromAreaExit"
            }
        },
        //CastleArea
        { //Transit a CastleEntrance desde WarpCenter
            39,
            new TransitPoint()
            {
                TransitDestinationName="FromWarpCenterToCastleEntrance",
                DestinationSceneId=20,
                destinationTransformTag="FromWarpArea"
            }
        },
        { //Transit a Warpcenter desde CastleEntrance
            40,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleEntranceToWarpCenter",
                DestinationSceneId=4,
                destinationTransformTag="FromCastleEntrance"
            }
        },
        { //Transit a Castle1 desde CastleEntrance
            41,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleEntranceToCastle1",
                DestinationSceneId=21,
                destinationTransformTag="FromEntrance"
            }
        },
        { //Transit a CastleEntrance desde CastleArea1
            42,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleArea1ToCastleEntrance",
                DestinationSceneId=20,
                destinationTransformTag="FromCastleArea1"
            }
        },
        { //Transit a CastleArea2 desde CastleArea1
            43,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleArea1ToCastleArea2",
                DestinationSceneId=22,
                destinationTransformTag="FromArea1"
            }
        },
        { //Transit a CastleArea1 desde CastleArea2
            44,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleArea2ToCastleArea1",
                DestinationSceneId=21,
                destinationTransformTag="FromArea2"
            }
        },
        { //Transit a CastleArea3 desde CastleArea2
            45,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleArea2ToCastleArea3",
                DestinationSceneId=23,
                destinationTransformTag="FromArea2"
            }
        },
        { //Transit a CastleArea2 desde CastleArea3
            46,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleArea3ToCastleArea2",
                DestinationSceneId=22,
                destinationTransformTag="FromArea3"
            }
        },
        { //Transit a CastleArea4 desde CastleArea3
            47,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleArea3ToCastleArea4",
                DestinationSceneId=24,
                destinationTransformTag="FromArea3"
            }
        },
        { //Transit a CastleArea3 desde CastleArea4
            48,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleArea4ToCastleArea3",
                DestinationSceneId=23,
                destinationTransformTag="FromArea4"
            }
        },
        { //Transit a CastleArea5 desde CastleArea4
            49,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleArea4ToCastleArea5",
                DestinationSceneId=25,
                destinationTransformTag="FromArea4"
            }
        },
        { //Transit a CastleArea4 desde CastleArea5
            50,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleArea4ToCastleArea5",
                DestinationSceneId=24,
                destinationTransformTag="FromArea5"
            }
        },
        { //Transit a CastleArea6 desde CastleArea5
            51,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleArea5ToCastleArea6",
                DestinationSceneId=26,
                destinationTransformTag="FromArea5"
            }
        },
        { //Transit a CastleArea5 desde CastleArea6
            52,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleArea6ToCastleArea5",
                DestinationSceneId=25,
                destinationTransformTag="FromArea6"
            }
        },
        { //Transit a CastleAreaExit desde CastleArea6
            53,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleArea6ToCastleAreaExit",
                DestinationSceneId=27,
                destinationTransformTag="FromArea6"
            }
        },
        { //Transit a CastleAreaExit desde CastleArea6
            54,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleAreaExitToCastleArea6",
                DestinationSceneId=26,
                destinationTransformTag="FromAreaExit"
            }
        },
        { //Transit a CastleAreaSide1 desde CastleArea4
            55,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleArea4ToCastleAreaSide1",
                DestinationSceneId=28,
                destinationTransformTag="FromArea4"
            }
        },
        { //Transit a CastleAreaSide1 desde CastleArea4
            56,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleArea4ToCastleAreaSide1",
                DestinationSceneId=24,
                destinationTransformTag="FromSideArea1"
            }
        },
        { //Transit a CastleAreaSide1 desde CastleArea4
            57,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleAreaSide1ToCastleAreaExtra1",
                DestinationSceneId=29,
                destinationTransformTag="FromAreaSide1"
            }
        },
        { //Transit a CastleAreaSide1 desde CastleArea4
            58,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleAreaExtra1ToCastleAreaSide1",
                DestinationSceneId=28,
                destinationTransformTag="FromExtraArea1"
            }
        },
        { //Transit a CastleAreaSide1 desde CastleArea4
            59,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleAreaSide1ToCastleAreaExtra2",
                DestinationSceneId=30,
                destinationTransformTag="FromSideArea1"
            }
        },
        { //Transit a CastleAreaSide1 desde CastleArea4
            60,
            new TransitPoint()
            {
                TransitDestinationName="FromCastleAreaExtra2ToCastleAreaSide1",
                DestinationSceneId=30,
                destinationTransformTag="FromExtraArea2"
            }
        }

    };
}
