using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Класс для отправки запросов на сервер при изменении инвентаря.
/// Подписывается на события добавления и удаления предметов, отправляет данные на сервер.
/// </summary>
public class InventoryServerCommunicator : MonoBehaviour
{
    // Токен для авторизации на сервере
    private const string Token = "uREthYeRfX46xaSy8CEzencsAgmMR4Nr7SNMd08qPq6c7StMTGxzhxPTnRNaP";

    /// <summary>
    /// Подписывается на события при включении объекта.
    /// </summary>
    private void OnEnable()
    {
        InventoryEvents.OnItemAdded += HandleItemAdded;
        InventoryEvents.OnItemRemoved += HandleItemRemoved;
    }

    /// <summary>
    /// Отписывается от событий при отключении объекта.
    /// </summary>
    private void OnDisable()
    {
        InventoryEvents.OnItemAdded -= HandleItemAdded;
        InventoryEvents.OnItemRemoved -= HandleItemRemoved;
    }

    /// <summary>
    /// Обрабатывает событие добавления предмета в инвентарь.
    /// </summary>
    /// <param name="itemId">Идентификатор добавленного предмета.</param>
    private void HandleItemAdded(string itemId)
    {
        Debug.Log($"Item added: {itemId}");
        StartCoroutine(SendRequestToServer(itemId, "add"));
    }

    /// <summary>
    /// Обрабатывает событие удаления предмета из инвентаря.
    /// </summary>
    /// <param name="itemId">Идентификатор удаленного предмета.</param>
    private void HandleItemRemoved(string itemId)
    {
        Debug.Log($"Item removed: {itemId}");
        StartCoroutine(SendRequestToServer(itemId, "remove"));
    }

    /// <summary>
    /// Отправляет запрос на сервер с данными о предмете и действии.
    /// </summary>
    /// <param name="itemId">Идентификатор предмета.</param>
    /// <param name="action">Действие (add или remove).</param>
    /// <returns>Корутина для асинхронной отправки запроса.</returns>
    private IEnumerator SendRequestToServer(string itemId, string action)
    {
        // Формируем JSON-данные для отправки
        string jsonData = $"{{\"item_id\": \"{itemId}\", \"action\": \"{action}\"}}";

        // Создаем POST-запрос
        using (UnityWebRequest request = new UnityWebRequest("https://madahub.amnerat.com/api/inventory/status", "POST"))
        {
            // Устанавливаем заголовки
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Token {Token}");

            // Добавляем тело запроса
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            // Отправляем запрос и ждем ответа
            yield return request.SendWebRequest();

            // Обрабатываем ответ
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Server response: " + request.downloadHandler.text);

                // Парсим JSON-ответ
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
    /// Класс для парсинга JSON-ответа от сервера.
    /// </summary>
    [System.Serializable]
    private class ServerResponse
    {
        public string response;
        public string status;
        public string data_submitted;
    }
}