// Custom Action by DumbGameDev
// www.dumbgamedev.com
// Eric Vander Wal

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{

    [ActionCategory("Custom")]
    [Tooltip ("Enables a bubble canvas above NPC when using the canvas")]
	public class BubbleCanvas : FsmStateAction
    {

	    [ActionSection("Canvas")]
	    
	    [RequiredField]
	    [CheckForComponent(typeof(Canvas))]
	    [Title("Canvas")]
	    [Tooltip("Select the root canvas for the speech bubble.")]
	    public FsmOwnerDefault gameObject;

	    [RequiredField]
	    [Title("Canvas Child Gameobject")]
	    [Tooltip("Make a child gameobject of the canvas. This game object will be activated and deactived in order to hide the speech bubble.")]
	    public FsmGameObject objectToActivate;
	    
	    [RequiredField]
	    [CheckForComponent(typeof(CanvasGroup))]
	    [Title("Canvas Group Object")]
	    [Tooltip("A parent game object to the speech bubble must have a canvas group component. This can be the root Canvas.")]
	    public FsmGameObject canvasImage1;
	    
	    public FsmBool centerJustifyCanvas;
      
	    [ActionSection("Target")]
	    
	    [RequiredField]
	    [Tooltip("Target Object that has speech bubble over it..")]
	    public FsmGameObject targetObject;
	    
	    [Title("Bubble Placement Offset")]
	    [Tooltip("Manually offset the canvas.")]
	    public FsmVector3 canvasOffset = new Vector3(0, 0, 0);
	    
		[ActionSection("Bubble")]

	    [Title("Bubble Display Delay")]
	    [Tooltip("Delay before bubble appears. Set to 0 for no delay")]
	    public FsmFloat canvasOnDelay;
	    
	    [Title("Bubble Display Time")]
	    [Tooltip("Length of time before the bubble disappears.")]
	    public FsmFloat canvasDisplayTime;
	    
	    [Tooltip("Bubble will not end.")]
	    public FsmBool neverEnd;

	    [Tooltip("Bubble will fade in.")]
	    public FsmBool enableFadeIn;
	    
	    [Tooltip("Bubble will fade out.")]
	    public FsmBool enableFadeOut;
	    
	    [Title("Bubble Fadeout Speed")]
	    [Tooltip("Bubble Fade Out Speed.")]
	    public FsmFloat fadeoutTime = 1f;
	    
	    [Title("Bubble Fadein Speed")]
	    [Tooltip("Bubble Fade In Speed.")]
	    public FsmFloat fadeinTime = 1f;

		[ActionSection("Events")]
	    
	    [Title("Fade Out Finish Event")]
	    [Tooltip("Event to send after fade out is finished.")]
	    public FsmEvent sendEvent;
	    
	    [Title("Bubble No End Event")]
	    [Tooltip("Event to once bubble is activated for forever.")]
	    public FsmEvent sendEventFinished;
	    
	    [ActionSection("Canvas Rotation (Req EveryFrame")]
	    
	    [Tooltip("Enables bubble/canvas rotation options below.")]
	    public FsmBool enableRotation;
	    
	    [Title("Rotate Towards")]
	    [Tooltip("Face the canvas towards which game object. Ie the player or camera.")]
	    public FsmGameObject playerGO;
	    
	    [Title("Rotate Speed")]
	    public FsmFloat turnSpeedGO;
	    
	    public FsmBool ReverseDirection;
	    
	    private GameObject player;
	    
	    [ActionSection("Everyframe")]
 
	    [Tooltip("Run this action everframe. This is nessesary if you want the bubble to follow the target object or bubble to rotate to face the camera.")]
	    public FsmBool everyFrame;
	    
	    
	    public override void Reset()
	    {
		    
		    gameObject = null;
		    objectToActivate = null;
		    canvasImage1 = null;
		    canvasOnDelay = 1f;
		    neverEnd = false;
		    canvasDisplayTime = 10f;
		    targetObject = null;
		    canvasOffset = null;
		    enableFadeIn = true;
		    enableFadeOut = true;
		    fadeinTime = 1f;
		    fadeoutTime = 1f;
		    sendEvent = null;
		    everyFrame = false;
		    turnSpeedGO = null;
		    playerGO = null;
		    ReverseDirection = null;
		    enableRotation = true;
		    centerJustifyCanvas = false;

	    }

	    public override void OnEnter()
	    {

            StartCoroutine(ActivationRoutine());
		    
		    if (!everyFrame.Value)
		    {
			    findCanvasCorner();
			    rotateCanvas();
		    }
		    
		    
	    }
	    
	    
	    public override void OnUpdate()
	    {
		    if (everyFrame.Value)
		    {
			    findCanvasCorner();
			    rotateCanvas();
		    }
	    }
	    
	    

        private IEnumerator ActivationRoutine()
        {

            //Wait for X secs before fade in canvas.
	        yield return new WaitForSeconds(canvasOnDelay.Value);

            //     -- FADE IN HERE --

            //Enable my canvas
	        objectToActivate.Value.SetActive(true);
	        var canvasImage1_canvas = canvasImage1.Value.GetComponent<CanvasGroup>();


            // Check to see if the fade in option is toggled
	        if (enableFadeIn.Value)
            {

                canvasImage1_canvas.alpha = 0.1f;
                while (canvasImage1_canvas.alpha < 1)
                {
	                canvasImage1_canvas.alpha += Time.deltaTime / fadeinTime.Value;
                    yield return null;

                }

            }

            else
            {

                // Dont fade in, just set alpha to 1 immediately
                canvasImage1_canvas.alpha = 1f;

            }

            // -- FADE OUT HERE ---
	        if (!neverEnd.Value)
            {

                //Turn the Game Oject back off after x sec.
		        yield return new WaitForSeconds(canvasDisplayTime.Value);

		        // Check to see if the fade out option is toggled
		        if (enableFadeOut.Value)
		        {
			        
			        canvasImage1_canvas = canvasImage1.Value.GetComponent<CanvasGroup>();
			        while (canvasImage1_canvas.alpha > 0)
			        {
				        canvasImage1_canvas.alpha -= Time.deltaTime / fadeoutTime.Value;
				        yield return null;
			        }
			        
			        objectToActivate.Value.SetActive(false);
			        Fsm.Event(sendEvent);
			      
		        }
		        
		        else
		        {
			        
			        objectToActivate.Value.SetActive(false);
			        
		        }
		        
            }

            // Never Fade Out
            else
            {
	            Fsm.Event(sendEventFinished);
            }

        }



        void findCanvasCorner()
	    {
		    
		    var go = Fsm.GetOwnerDefaultTarget(gameObject);
		    

            // Get the four corners of the canvas and store in variables
		    var _bubbleCanvas = go.GetComponent<RectTransform>();
            Vector3[] fourCornersArray = new Vector3[4];
            _bubbleCanvas.GetWorldCorners(fourCornersArray);
            Vector3 bottomRight = (Vector3)fourCornersArray[3];
            Vector3 bottomLeft = (Vector3)fourCornersArray[0];
            Vector3 topRight = (Vector3)fourCornersArray[2];

            // Get offset for bottom
            Vector3 bottomDistance = bottomLeft - bottomRight;
            bottomDistance = bottomDistance / 2;

            // Get offset for the right side
            Vector3 rightDistance = topRight - bottomRight;
            rightDistance = rightDistance / 2;

            // Combine Offsets into a single vector
            Vector3 cornerOffset = bottomDistance + rightDistance;

            // Get taget objects position
		    Vector3 TO = targetObject.Value.transform.position;

            // Get target object height and set by half
		    var TO_collider = targetObject.Value.GetComponent<Collider>();
            Vector3 TO_size = new Vector3(0, 0, 0);
            TO_size.y = TO_collider.bounds.size.y;
		    TO_size = TO_size / 2;
		    
		    if(centerJustifyCanvas.Value){

            // Move game objects position to the target object plus canvas corner offset
			    go.transform.position = TO + rightDistance + TO_size + canvasOffset.Value;
			    
		    }
		    
		    else{
		    	
	            // Move game objects position to the target object plus canvas corner offset	    
			    go.transform.position = TO + cornerOffset + TO_size + canvasOffset.Value;
			    
		    }
		    
		    
	    }
	    
	    void rotateCanvas()
	    {
	    	
	    	if(enableRotation.Value){
		    var go = Fsm.GetOwnerDefaultTarget(gameObject);
		    if (go == null)
		    {
			    return;
		    }
		    
		    player = playerGO.Value;
		    var playerPosition = player.transform;
		    var turnSpeed = turnSpeedGO.Value;
		    
		    if(!ReverseDirection.Value){
			    
			    var newPosition = Quaternion.LookRotation(go.transform.position - playerPosition.position);
			    go.transform.rotation = Quaternion.Slerp(go.transform.rotation, newPosition, turnSpeed* Time.deltaTime);
			    
		    }
		    
		    else {
			    
			    Vector3 direction = playerPosition.position - go.transform.position;
			    go.transform.rotation = Quaternion.Slerp(go.transform.rotation, Quaternion.LookRotation(direction), turnSpeed* Time.deltaTime);
			    
		    }
	    	}
	    	
	    	else {
	    		
	    	}
		    
		    
	    }

    }
}
