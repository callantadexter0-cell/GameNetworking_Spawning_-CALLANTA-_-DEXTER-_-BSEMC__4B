using Unity.Netcode;
using UnityEngine;

public class ObjectSpawner : NetworkBehaviour 
{
    [Header("Prefabs to Spawn")]
    public GameObject hostPrefab;   // SpawnableCube
    public GameObject clientPrefab; // SpawnableCapsule
    
    public Transform spawnLocation; 

    private GameObject hostSpawnedObject;
    private GameObject clientSpawnedObject;

    public void SpawnNetworkObject()
    {
        RequestSpawnServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    public void DespawnNetworkObject()
    {
        RequestDespawnServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    // Bagong syntax sa Unity 6 para mawala ang console warning
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestSpawnServerRpc(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId)
        {
            if (hostSpawnedObject == null)
            {
                hostSpawnedObject = Instantiate(hostPrefab, spawnLocation.position, Quaternion.identity);
                hostSpawnedObject.GetComponent<NetworkObject>().Spawn();
            }
        }
        else
        {
            if (clientSpawnedObject == null)
            {
                Vector3 clientPos = spawnLocation.position + new Vector3(2, 0, 0); 
                clientSpawnedObject = Instantiate(clientPrefab, clientPos, Quaternion.identity);
                clientSpawnedObject.GetComponent<NetworkObject>().Spawn();
            }
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestDespawnServerRpc(ulong clientId)
    {
        if (clientId == NetworkManager.ServerClientId)
        {
            if (hostSpawnedObject != null && hostSpawnedObject.GetComponent<NetworkObject>().IsSpawned)
            {
                hostSpawnedObject.GetComponent<NetworkObject>().Despawn();
            }
        }
        else
        {
            if (clientSpawnedObject != null && clientSpawnedObject.GetComponent<NetworkObject>().IsSpawned)
            {
                clientSpawnedObject.GetComponent<NetworkObject>().Despawn();
            }
        }
    }

    public void StartHostButton()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClientButton()
    {
        NetworkManager.Singleton.StartClient();
    }
}