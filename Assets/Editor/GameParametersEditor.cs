using System.Reflection;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameParameters), true)]
public class GameParametersEditor : Editor
{
    private delegate void DDrawPrefix(Object obj, FieldInfo field);

    public override void OnInspectorGUI()
    {
        GameParameters parameters = (GameParameters)target;

        EditorGUILayout.LabelField($"Name: {parameters.GetParametersName()}", EditorStyles.boldLabel);

        System.Type ParametersType = parameters.GetType();
        FieldInfo[] fields = ParametersType.GetFields();
        DDrawPrefix drawPrefix = (Object obj, FieldInfo field) =>
        {
            GameParameters p = (GameParameters)obj;
            if (GUILayout.Button(p.ShowsField(field.Name) ? "-" : "+", GUILayout.Width(20f)))
            {
                p.ToggleShowField(field.Name);
                EditorUtility.SetDirty(p);
                AssetDatabase.SaveAssets();
            }
        };
        for (int fieldIndex = 0; fieldIndex < fields.Length; fieldIndex++)
            _DrawField(parameters, fields[fieldIndex], drawPrefix);
    }

    void _DrawField(Object obj, FieldInfo field, DDrawPrefix drawPrefix = null)
    {
        if (System.Attribute.IsDefined(field, typeof(HideInInspector), false))
            return;

        EditorGUILayout.BeginHorizontal();

        if (drawPrefix != null)
            drawPrefix(obj, field);

        EditorGUILayout.LabelField(field.Name);

        if (field.FieldType == typeof(string))
            field.SetValue(
                obj,
                EditorGUILayout.TextField((string)field.GetValue(obj))
            );
        else if (field.FieldType == typeof(bool))
            field.SetValue(
                obj,
                EditorGUILayout.Toggle((bool)field.GetValue(obj))
            );
        else if (field.FieldType == typeof(int))
            field.SetValue(
                obj,
                EditorGUILayout.IntField((int)field.GetValue(obj))
            );
        else if (field.FieldType == typeof(float))
            field.SetValue(
                obj,
                EditorGUILayout.FloatField((float)field.GetValue(obj))
            );
        else if (field.FieldType == typeof(double))
            field.SetValue(
                obj,
                EditorGUILayout.DoubleField((double)field.GetValue(obj))
            );
        else if (field.FieldType == typeof(long))
            field.SetValue(
                obj,
                EditorGUILayout.LongField((long)field.GetValue(obj))
            );
        else if (field.FieldType == typeof(Rect))
            field.SetValue(
                obj,
                EditorGUILayout.RectField((Rect)field.GetValue(obj))
            );
        else if (field.FieldType == typeof(Vector2))
            field.SetValue(
                obj,
                EditorGUILayout.Vector2Field("", (Vector2)field.GetValue(obj))
            );
        else if (field.FieldType == typeof(Vector3))
            field.SetValue(
                obj,
                EditorGUILayout.Vector3Field("", (Vector3)field.GetValue(obj))
            );
        else if (field.FieldType == typeof(Vector4))
            field.SetValue(
                obj,
                EditorGUILayout.Vector4Field("", (Vector4)field.GetValue(obj))
            );
        else if (field.FieldType == typeof(Color))
            field.SetValue(
                obj,
                EditorGUILayout.ColorField("", (Color)field.GetValue(obj))
            );
        else if (field.FieldType == typeof(int))
        {
            bool isRange = System.Attribute.IsDefined(field, typeof(RangeAttribute), false);
            if (isRange)
            {
                RangeAttribute attr = (RangeAttribute)System.Attribute.GetCustomAttribute(field, typeof(RangeAttribute));
                field.SetValue(
                    obj,
                    EditorGUILayout.IntSlider((int)field.GetValue(obj), (int)attr.min, (int)attr.max)
                );
            }
            else
                  field.SetValue(
                    obj,
                    EditorGUILayout.IntField((int)field.GetValue(obj))
                );
        }
        else if (field.FieldType == typeof(float))
        {
            bool isRange = System.Attribute.IsDefined(field, typeof(RangeAttribute), false);
            if (isRange)
            {
                RangeAttribute attr = (RangeAttribute)System.Attribute.GetCustomAttribute(field, typeof(RangeAttribute));
                field.SetValue(
                    obj,
                    EditorGUILayout.Slider((float)field.GetValue(obj), attr.min, attr.max)
                );
            }
            else
                field.SetValue(
                    obj,
                    EditorGUILayout.FloatField((float)field.GetValue(obj))
                );
        }
        else
            field.SetValue(
                obj,
                EditorGUILayout.ObjectField((Object)field.GetValue(obj), field.FieldType, true)
            );

        EditorGUILayout.EndHorizontal();
    }
}
