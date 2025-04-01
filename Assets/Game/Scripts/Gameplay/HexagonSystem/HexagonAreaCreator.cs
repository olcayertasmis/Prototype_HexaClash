using System.Collections.Generic;
using UnityEngine;

namespace HexaClash.Game.Scripts.Gameplay.HexagonSystem
{
    public class HexagonAreaCreator : MonoBehaviour
    {
        [Header("Hexagon Area Prefab")]
        [SerializeField] private GameObject hexagonAreaPrefab;

        [Header("Hexagon Grid Settings")]
        [SerializeField] private int ringCount; // Kac katmanli hexagon alani olacagini belirler
        [SerializeField] private float hexagonSize; // Hexagon boyutu

        private readonly List<HexagonArea> _hexagonAreas = new();

        private static readonly Vector2[] HexDirections =
        {
            new(1, 0), new(0.5f, -0.87f), new(-0.5f, -0.87f),
            new(-1, 0), new(-0.5f, 0.87f), new(0.5f, 0.87f)
        };

        private void Awake()
        {
            if (!ValidatePrefab()) return;

            CreateHexagonGrid();
            AssignAllNeighbors();
        }

        private bool ValidatePrefab()
        {
            if (!hexagonAreaPrefab)
            {
                Debug.LogError("Hexagon Area Prefab is not assigned!");
                return false;
            }

            if (!hexagonAreaPrefab.GetComponent<HexagonArea>())
            {
                Debug.LogError("HexagonArea component is missing on the prefab!");
                return false;
            }

            return true;
        }

        private void CreateHexagonGrid()
        {
            _hexagonAreas.Clear();

            CreateHexagon(transform.position + new Vector3(1f, 0, 0)); // Merkez Hexagon

            for (int ring = 1; ring <= ringCount; ring++)
            {
                Vector3 startPos = transform.position + new Vector3(0, 0, ring * hexagonSize * 1.75f);
                Vector3 currentPos = startPos;

                for (int dir = 0; dir < 6; dir++)
                {
                    for (int i = 0; i < ring; i++)
                    {
                        CreateHexagon(currentPos);
                        currentPos += new Vector3(HexDirections[dir].x * hexagonSize * 2f, 0, HexDirections[dir].y * hexagonSize * 2f);
                    }
                }
            }
        }

        private HexagonArea CreateHexagon(Vector3 currentPos)
        {
            GameObject hexagonObj = Instantiate(hexagonAreaPrefab, currentPos, Quaternion.identity, transform);
            HexagonArea hexagonArea = hexagonObj.GetComponent<HexagonArea>();

            hexagonArea.Initialize(_hexagonAreas.Count == 0);
            _hexagonAreas.Add(hexagonArea);

            return hexagonArea;
        }

        private void AssignAllNeighbors()
        {
            foreach (var hexagon in _hexagonAreas)
            {
                List<HexagonArea> neighbors = new List<HexagonArea>();
                foreach (var otherHex in _hexagonAreas)
                {
                    if (hexagon == otherHex) continue;

                    // Komsuluk kontrolu
                    if (Vector3.Distance(hexagon.transform.position, otherHex.transform.position) < hexagonSize * 2f + 0.1f)
                    {
                        neighbors.Add(otherHex);
                    }
                }

                hexagon.SetNeighbors(neighbors);
            }
        }
    }
}