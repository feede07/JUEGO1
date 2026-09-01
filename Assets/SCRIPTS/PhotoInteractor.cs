using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameInput))]
public class PhotoInteractor : MonoBehaviour
{
    private readonly HashSet<PhotoCollectible> nearbyPhotos = new();
    private GameInput gameInput;

    private void Awake()
    {
        gameInput = GetComponent<GameInput>();
    }

    private void OnEnable()
    {
        gameInput.InteractPressed += CollectClosestPhoto;
    }

    private void OnDisable()
    {
        gameInput.InteractPressed -= CollectClosestPhoto;
        nearbyPhotos.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        PhotoCollectible collectible = other.GetComponentInParent<PhotoCollectible>();

        if (collectible != null && collectible.IsAvailable)
        {
            nearbyPhotos.Add(collectible);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PhotoCollectible collectible = other.GetComponentInParent<PhotoCollectible>();

        if (collectible != null)
        {
            nearbyPhotos.Remove(collectible);
        }
    }

    private void CollectClosestPhoto()
    {
        PhotoCollectible closestPhoto = null;
        float closestDistance = float.PositiveInfinity;

        nearbyPhotos.RemoveWhere(photo => photo == null || !photo.IsAvailable);

        foreach (PhotoCollectible photo in nearbyPhotos)
        {
            float distance = (photo.transform.position - transform.position).sqrMagnitude;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPhoto = photo;
            }
        }

        if (closestPhoto != null && closestPhoto.Collect())
        {
            nearbyPhotos.Remove(closestPhoto);
        }
    }
}
