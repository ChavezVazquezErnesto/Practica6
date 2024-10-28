using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Vuforia;

public class Move : MonoBehaviour
{
    public GameObject model;
    public ObserverBehaviour[] ImageTargets; // Asegúrate de que los marcadores estén en el orden correcto en el array (0, 1, 2, 3, 4)
    public float speed = 1.0f;
    private bool isMoving = false;
    private int currentTarget = 0; // Comienza desde el primer marcador (índice 0)
    public bool useSpecialSequence = false; // Activa o desactiva el modo de secuencia especial

    // Esta función se llama al presionar el botón de "mover" para el recorrido estándar
    public void moveToNextMarker(){
        if(!isMoving){
            if (useSpecialSequence)
            {
                StartCoroutine(SpecialMoveModel()); // Usa la secuencia especial
            }
            else
            {
                StartCoroutine(MoveModel()); // Usa el movimiento en orden
            }
        }
    }
    
    private IEnumerator MoveModel(){
        isMoving = true;

        // Selecciona el siguiente marcador en orden
        ObserverBehaviour target = ImageTargets[currentTarget];
        if(target == null || (target.TargetStatus.Status != Status.TRACKED && target.TargetStatus.Status != Status.EXTENDED_TRACKED)){
            isMoving = false;
            yield break;
        }

        Vector3 startPosition = model.transform.position;
        Vector3 endPosition = target.transform.position;
        
        float journey = 0.0f;

        while(journey <= 1.0f){
            journey += Time.deltaTime * speed;
            model.transform.position = Vector3.Lerp(startPosition, endPosition, journey);
            yield return null;
        }

        // Desactiva el marcador actual después de llegar a él, excepto si es el marcador 2
        if (currentTarget != 0) // Asumiendo que el índice del marcador 2 en el array es 1
        {
            target.gameObject.SetActive(false);
        }

        // Actualiza el índice al siguiente marcador en el array
        currentTarget = (currentTarget + 1) % ImageTargets.Length;
        isMoving = false;
    }

    // Movimiento especial (1 -> 2 -> 4 -> regresa a 1)
    private IEnumerator SpecialMoveModel(){
        isMoving = true;

        // Patrones específicos para el movimiento especial: índice de los marcadores (1 -> 2 -> 4 -> regresa a 1)
        int[] specialPattern = { 0, 1, 3, 0 }; // Indices en el array: [marcador 1, marcador 2, marcador 4, regresar a marcador 1]
        
        foreach (int targetIndex in specialPattern)
        {
            ObserverBehaviour target = ImageTargets[targetIndex];
            if(target == null || (target.TargetStatus.Status != Status.TRACKED && target.TargetStatus.Status != Status.EXTENDED_TRACKED)){
                isMoving = false;
                yield break;
            }

            Vector3 startPosition = model.transform.position;
            Vector3 endPosition = target.transform.position;
            
            float journey = 0.0f;

            while(journey <= 1.0f){
                journey += Time.deltaTime * speed;
                model.transform.position = Vector3.Lerp(startPosition, endPosition, journey);
                yield return null;
            }

            // Desactiva el marcador actual después de llegar a él, excepto si es el marcador 2
            if (targetIndex != 0) // Mantiene visible el marcador 2
            {
                target.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(1.0f); // Pausa breve entre movimientos
        }

        isMoving = false; // Termina la secuencia especial
    }

    // Función para reiniciar el recorrido
    public void ResetJourney(){
        StopAllCoroutines(); // Detiene cualquier movimiento en progreso
        isMoving = false;
        currentTarget = 0; // Reinicia el recorrido desde el primer marcador (marcador 1)
        
        // Reactiva todos los marcadores desactivados
        foreach (ObserverBehaviour marker in ImageTargets)
        {
            if(marker != null)
                marker.gameObject.SetActive(true);
        }

        // Lleva el modelo de vuelta al marcador inicial (marcador 1)
        if (ImageTargets.Length > 0 && ImageTargets[0] != null) {
            model.transform.position = ImageTargets[0].transform.position;
        }
    }

    // Nuevo recorrido específico (1 -> 2 -> 4 -> regresa a 1)
    public void CustomJourney(){
        if(!isMoving){
            StartCoroutine(CustomMoveModel());
        }
    }

    private IEnumerator CustomMoveModel(){
        isMoving = true;

        // Define el patrón específico: índice de los marcadores (1 -> 2 -> 4 -> regresa a 1)
        int[] customPattern = { 0, 1, 3, 0 }; // Indices en el array: [marcador 1, marcador 2, marcador 4, regresar a marcador 1]
        
        foreach (int targetIndex in customPattern)
        {
            ObserverBehaviour target = ImageTargets[targetIndex];
            if(target == null || (target.TargetStatus.Status != Status.TRACKED && target.TargetStatus.Status != Status.EXTENDED_TRACKED)){
                isMoving = false;
                yield break;
            }

            Vector3 startPosition = model.transform.position;
            Vector3 endPosition = target.transform.position;
            
            float journey = 0.0f;

            while(journey <= 1.0f){
                journey += Time.deltaTime * speed;
                model.transform.position = Vector3.Lerp(startPosition, endPosition, journey);
                yield return null;
            }

            yield return new WaitForSeconds(1.0f); // Pausa breve entre movimientos
        }

        isMoving = false; // Termina la secuencia personalizada
    }

    // Nuevo recorrido completo (0 -> 1 -> 2 -> 3 -> 4 -> regresa a 0)
    public void FullSequenceJourney(){
        if(!isMoving){
            StartCoroutine(FullSequenceMoveModel());
        }
    }

    private IEnumerator FullSequenceMoveModel(){
        isMoving = true;

        // Define el patrón completo: índice de los marcadores (0 -> 1 -> 2 -> 3 -> 4 -> regresa a 0)
        int[] fullPattern = { 0, 1, 2, 4, 0 }; // Indices en el array

        foreach (int targetIndex in fullPattern)
        {
            ObserverBehaviour target = ImageTargets[targetIndex];
            if(target == null || (target.TargetStatus.Status != Status.TRACKED && target.TargetStatus.Status != Status.EXTENDED_TRACKED)){
                isMoving = false;
                yield break;
            }

            Vector3 startPosition = model.transform.position;
            Vector3 endPosition = target.transform.position;

            float journey = 0.0f;

            while(journey <= 1.0f){
                journey += Time.deltaTime * speed;
                model.transform.position = Vector3.Lerp(startPosition, endPosition, journey);
                yield return null;
            }

            yield return new WaitForSeconds(1.0f); // Pausa breve entre movimientos
        }

        isMoving = false; // Termina la secuencia completa
    }

    // Update se deja vacío para que el modelo solo se mueva cuando se presione el botón
    void Update()
    {
        
    }
}
