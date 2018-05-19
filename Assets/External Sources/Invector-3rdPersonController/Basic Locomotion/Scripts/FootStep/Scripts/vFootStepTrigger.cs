using UnityEngine;
using System.Collections;
namespace Invector
{
    public class vFootStepTrigger : MonoBehaviour
    {
        protected Collider _trigger;
        protected vFootStep _fT;

        void OnDrawGizmos()
        {
            if (!trigger) return;
            Color color = Color.green;
            color.a = 0.5f;
            Gizmos.color = color;
            if (trigger is SphereCollider)
            {
                Gizmos.DrawSphere((trigger.bounds.center), (trigger as SphereCollider).radius);
            }
        }

        void Start()
        {
            _fT = GetComponentInParent<vFootStep>();
            if (_fT == null)
            {
                Debug.Log(gameObject.name + " can't find the FootStepFromTexture");
                gameObject.SetActive(false);
            }
        }

        public Collider trigger
        {
            get
            {
                if (_trigger == null) _trigger = GetComponent<Collider>();
                return _trigger;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (_fT == null) return;

            if (other.GetComponent<Terrain>() != null) //Check if trigger objet is a terrain
                _fT.StepOnTerrain(new FootStepObject(transform, other.transform));
            else
            {
                var stepHandle = other.GetComponent<vFootStepHandler>();
                var renderer = other.GetComponent<Renderer>();
                //Check renderer
                if (renderer != null && renderer.material != null)
                {
                    var index = 0;
                    var _name = string.Empty;
                    if (stepHandle != null && stepHandle.material_ID > 0)// if trigger contains a StepHandler to pass material ID. Default is (0)
                        index = stepHandle.material_ID;
                    if (stepHandle)
                    {
                        // check  stepHandlerType
                        switch (stepHandle.stepHandleType)
                        {
                            case vFootStepHandler.StepHandleType.materialName:
                                _name = renderer.materials[index].name;
                                break;
                            case vFootStepHandler.StepHandleType.textureName:
                                _name = renderer.materials[index].mainTexture.name;
                                break;
                        }
                    }
                    else
                        _name = renderer.materials[index].name;
                    _fT.StepOnMesh(new FootStepObject(transform, other.transform, _name));
                }
            }
        }
    }

    /// <summary>
    /// Foot step Object work with FootStepFromTexture
    /// </summary>
    public class FootStepObject
    {
        public string name;
        public Transform sender;
        public Transform ground;
        public FootStepObject(Transform sender, Transform ground, string name = "")
        {
            this.name = name;
            this.sender = sender;
            this.ground = ground;
        }
    }
}