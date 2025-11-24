using Unity.VisualScripting;
using UnityEngine;

public class PrefabSwitcher : MonoBehaviour
{
    [SerializeField] private MeshFilter meshObject;
    
    
    public enum WallSize
    {
        Small,
        Medium,
        Large,
        Pillar
    }

    public enum WallType
    {
        Flat,
        Devers,
        Pente,
        Double
    }

    public enum StepType
    {
        Single,
        Double
    }

    public enum ObjectType
    {
        Base,
        Wall,
        Transition,
        Top,
        Stal,
        Rock,
        Tunnel
    }

    public enum RockSize
    {
        Small,
        Big
    }

    public string IdentifyMesh(ObjectType objectType, WallType wallType, 
        StepType stepType, RockSize rockSize, WallSize wallSize)
    {
        string meshName = "";
        string typeName = "";
        string subtypeName = "";
        string size = "";
        if (objectType == ObjectType.Tunnel)
        {
            meshName = "Tunnel_2";
        }
        else
        {
            switch (objectType)
            {
                case ObjectType.Wall:
                    typeName = "Bloc_";
                    break;
                case ObjectType.Base:
                    typeName = "Base_";
                    break;
                case ObjectType.Top:
                    typeName = "Top_";
                    break;
                case ObjectType.Transition:
                    typeName = "Transition_";
                    break;
                case ObjectType.Rock:
                    typeName = "Rock_";
                    switch (rockSize)
                    {
                        case RockSize.Big:
                            typeName = "big";
                            break;
                        case RockSize.Small:
                            typeName = "small";
                            break;
                    }
                    break;
                case ObjectType.Stal:
                    typeName = "Stalac_";
                    break;
            }

            if (objectType == ObjectType.Base || objectType == ObjectType.Top)
            {
                switch (stepType)
                {
                    case StepType.Single:
                        subtypeName = "1s_";
                        break;
                    case StepType.Double:
                        subtypeName = "2s_";
                        break;
                }
            }

            if (objectType == ObjectType.Wall || objectType == ObjectType.Transition || objectType == ObjectType.Stal)
            {
                if (objectType != ObjectType.Stal)
                {
                    switch (wallType)
                    {
                        case WallType.Devers:
                            subtypeName = "devers_";
                            break;
                        case WallType.Pente:
                            subtypeName = "pente_";
                            break;
                        case WallType.Flat:
                            subtypeName = "flat_";
                            break;
                        case WallType.Double:
                            subtypeName = "double_";
                            break;
                    }
                }

                switch (wallSize)
                {
                    case WallSize.Small:
                        size = "1";
                        break;
                    case WallSize.Medium:
                        size = "2";
                        break;
                    case WallSize.Large:
                        size = "3";
                        break;
                    case WallSize.Pillar:
                        size = "round";
                        break;
                }
            }
        }

        meshName = typeName + subtypeName + size;
        return meshName;
    }

    public void SelectMesh(string meshName, MeshFilter meshFilter)
    {
        meshFilter.mesh.
    }
}
