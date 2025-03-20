using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// ����� ��� �������� �������� �� ������ ��� ��������� ���������.
/// ������������� �� ������� ���������� � �������� ���������, ���������� ������ �� ������.
/// </summary>
public class InventoryServerCommunicator : MonoBehaviour
{
    // ����� ��� ����������� �� �������
    private const string Token = "uREthYeRfX46xaSy8CEzencsAgmMR4Nr7SNMd08qPq6c7StMTGxzhxPTnRNaP";

    /// <summary>
    /// ������������� �� ������� ��� ��������� �������.
    /// </summary>
    private void OnEnable()
    {
        InventoryEvents.OnItemAdded += HandleItemAdded;
        InventoryEvents.OnItemRemoved += HandleItemRemoved;
    }

    /// <summary>
    /// ������������ �� ������� ��� ���������� �������.
    /// </summary>
    private void OnDisable()
    {
        InventoryEvents.OnItemAdded -= HandleItemAdded;
        InventoryEvents.OnItemRemoved -= HandleItemRemoved;
    }

    /// <summary>
    /// ������������ ������� ���������� �������� � ���������.
    /// </summary>
    /// <param name="itemId">������������� ������������ ��������.</param>
    private void HandleItemAdded(string itemId)
    {
        Debug.Log($"Item added: {itemId}");
        StartCoroutine(SendRequestToServer(itemId, "add"));
    }

    /// <summary>
    /// ������������ ������� �������� �������� �� ���������.
    /// </summary>
    /// <param name="itemId">������������� ���������� ��������.</param>
    private void HandleItemRemoved(string itemId)
    {
        Debug.Log($"Item removed: {itemId}");
        StartCoroutine(SendRequestToServer(itemId, "remove"));
    }

    /// <summary>
    /// ���������� ������ �� ������ � ������� � �������� � ��������.
    /// </summary>
    /// <param name="itemId">������������� ��������.</param>
    /// <param name="action">�������� (add ��� remove).</param>
    /// <returns>�������� ��� ����������� �������� �������.</returns>
    private IEnumerator SendRequestToServer(string itemId, string action)
    {
        // ��������� JSON-������ ��� ��������
        string jsonData = $"{{\"item_id\": \"{itemId}\", \"action\": \"{action}\"}}";

        // ������� POST-������
        using (UnityWebRequest request = new UnityWebRequest("https://madahub.amnerat.com/api/inventory/status", "POST"))
        {
            // ������������� ���������
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Token {Token}");

            // ��������� ���� �������
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // ���������� ������ � ���� ������
            yield return request.SendWebRequest();

            // ������������ �����
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Server response: " + request.downloadHandler.text);

                // ������ JSON-�����
                ServerResponse response = JsonUtility.FromJson<ServerResponse>(request.downloadHandler.text);
                if (response.response == "success")
                {
                    Debug.Log("Server received data successfully.");
                }
                else
                {
                    Debug.LogError("Server response indicates failure.");
                }
            }
            else
            {
                Debug.LogError("Request failed: " + request.error);
            }
        }
    }

    /// <summary>
    /// ����� ��� �������� JSON-������ �� �������.
    /// </summary>
    [System.Serializable]
    private class ServerResponse
    {
        public string response;
        public string status;
        public string data_submitted;
    }
}