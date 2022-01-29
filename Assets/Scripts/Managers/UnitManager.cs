using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class UnitManager : MonoBehaviour
{
    public GameObject selectionCircle;

    private bool _hovered = false;
    private Transform _canvas;
    private GameObject _healthbar;
    protected BoxCollider _collider;
    public virtual Unit Unit { get; set; }
    public GameObject fov;
    public AudioSource contextualSource;
    public int ownerMaterialSlotIndex = 0;

    public void SetOwnerMaterial(int owner)
    {
        Color playerColor = GameManager.instance.gamePlayersParameters.players[owner].color;
        Material[] materials = transform.Find("Mesh").GetComponent<Renderer>().materials;
        materials[ownerMaterialSlotIndex].color = playerColor;
        transform.Find("Mesh").GetComponent<Renderer>().materials = materials;
    }

    public void EnableFOV()
    {
        fov.SetActive(true);
    }

    public void Initialize(Unit unit)
    {
        _collider = GetComponent<BoxCollider>();
        Unit = unit;
    }

    private void Awake()
    {
        _canvas = GameObject.Find("Canvas").transform;
    }

    private void OnMouseEnter()
    {
        _hovered = true;
    }

    private void OnMouseExit()
    {
        _hovered = false;
    }

    private void Update()
    {
        if (_hovered && Input.GetMouseButtonDown(0) && IsActive() && _IsMyUnit())
        Select(
            true,
            Input.GetKey(KeyCode.LeftShift) ||
            Input.GetKey(KeyCode.RightShift)
        );
    }

    private bool _IsMyUnit()
    {
        return Unit.Owner == GameManager.instance.gamePlayersParameters.myPlayerId;
    }

    protected virtual bool IsActive()
    {
        return true;
    }

    private void _SelectUtil()
    {
        Globals.SELECTED_UNITS.Add(this);
        selectionCircle.SetActive(true);
        if (_healthbar == null)
        {
            _healthbar = GameObject.Instantiate(Resources.Load("Prefabs/UI/Healthbar")) as GameObject;
            _healthbar.transform.SetParent(_canvas);
            Healthbar h = _healthbar.GetComponent<Healthbar>();
            Rect boundingBox = Utils.GetBoundingBoxOnScreen(
                transform.Find("Mesh").GetComponent<Renderer>().bounds,
                Camera.main
            );
            h.Initialize(transform, boundingBox.height);
            h.SetPosition();
        }
        EventManager.TriggerEvent("SelectUnit", Unit);
        contextualSource.PlayOneShot(Unit.Data.onSelectSound);
    }

    public void Select() { Select(false, false); }
    public void Select(bool singleClick, bool holdingShift)
    {
        // basic case: using the selection box
        if (!singleClick)
        {
            _SelectUtil();
            return;
        }

        // single click: check for shift key
        if (!holdingShift)
        {
            List<UnitManager> selectedUnits = new List<UnitManager>(Globals.SELECTED_UNITS);
            foreach (UnitManager um in selectedUnits)
                um.Deselect();
            _SelectUtil();
        }
        else
        {
            if (!Globals.SELECTED_UNITS.Contains(this))
                _SelectUtil();
            else
                Deselect();
        }
    }

    public void Deselect()
    {
        Globals.SELECTED_UNITS.Remove(this);
        selectionCircle.SetActive(false);
        Destroy(_healthbar);
        _healthbar = null;
        EventManager.TriggerEvent("DeselectUnit", Unit);
    }

    public void EnableFOV(float size)
    {
        fov.SetActive(true);
        MeshRenderer mr = fov.GetComponent<MeshRenderer>();
        mr.material = new Material(mr.material);
        StartCoroutine(_ScalingFOV(size));
    }

    private IEnumerator _ScalingFOV(float size)
    {
        float r = 0f, t = 0f, step = 0.05f;
        float scaleUpTime = 0.35f;
        Vector3 _startScale = fov.transform.localScale;
        Vector3 _endScale = size * Vector3.one;
        _endScale.z = 1f;
        do
        {
            fov.transform.localScale = Vector3.Lerp(_startScale, _endScale, r);
            t += step;
            r = t / scaleUpTime;
            yield return new WaitForSecondsRealtime(step);
        } while (r < 1f);
    }
}
