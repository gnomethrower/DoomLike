using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Sprites;

namespace EightDirectionalSpriteSystem
{
    [CustomEditor(typeof(ActorAnimation))]
    public class ActorAnimationEditor : Editor
    {
        private SerializedProperty propName;
        private SerializedProperty propAnimType;
        private SerializedProperty propAnimMode;
        private SerializedProperty propFrameList;
        private SerializedProperty propSpeed;

        private ReorderableList frameList;
        private SerializedProperty propSelectedFrame;

        private SerializedProperty propSelectedFrameSprites;
        private string[] dirNames = { "Front", "Front-left", "Left", "Back-left", "Back", "Back-right", "Right", "Front-right" };

        private int currFrame;
        private int currDirection;
        private float nextFrameChangeTime;
        private bool isAnimationPlaying = false;

        private GUIContent animTypeLabel = new GUIContent("Anim Type");
        private GUIContent animModeLabel = new GUIContent("Anim Mode");
        private GUIContent animationSpeedLabel = new GUIContent("Speed (fps)");

        private Texture2D arrowsIcon;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(propName);
            EditorGUILayout.PropertyField(propAnimType, animTypeLabel);
            EditorGUILayout.PropertyField(propAnimMode, animModeLabel);
            EditorGUILayout.Slider(propSpeed, 0.01f, 60.0f, animationSpeedLabel);

            frameList.DoLayoutList();

            if (propSelectedFrame != null && propSelectedFrameSprites != null)
            {
                InspectorFrameGUI();
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override GUIContent GetPreviewTitle()
        {
            ActorAnimation targetAnimation = serializedObject.targetObject as ActorAnimation;
            if (targetAnimation != null)
            {
                return new GUIContent(targetAnimation.name);
            }
            return new GUIContent("No Title");
        }

        public override void OnPreviewSettings()
        {
            ActorAnimation.AnimDirType animDir = (ActorAnimation.AnimDirType)propAnimType.enumValueIndex;

            isAnimationPlaying = GUILayout.Toggle(isAnimationPlaying, "P", EditorStyles.miniButton);

            if (animDir == ActorAnimation.AnimDirType.EightDirections)
            {
                GUILayout.Space(8);
                if (GUILayout.Button("<", EditorStyles.toolbarButton))
                {
                    currDirection--;
                    if (currDirection < 0)
                        currDirection += 8;
                }

                if (GUILayout.Button(">", EditorStyles.toolbarButton))
                {
                    currDirection++;
                    if (currDirection >= 8)
                        currDirection -= 8;
                }
            }
            else
            {
                currDirection = 0;
            }
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            base.OnPreviewGUI(r, background);
                
            ActorAnimation targetAnimation = serializedObject.targetObject as ActorAnimation;
            Sprite currSprite = targetAnimation.GetSprite(currFrame, currDirection);
            if (currSprite != null)
            {
                Texture2D previewTexture = AssetPreview.GetAssetPreview(currSprite);

                if (previewTexture != null)
                {
                    previewTexture.filterMode = currSprite.texture.filterMode;
                    EditorGUI.DrawTextureTransparent(r, previewTexture, ScaleMode.ScaleToFit);
                }
            }
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        private void OnEnable()
        {
            propName = serializedObject.FindProperty("animName");
            propAnimType = serializedObject.FindProperty("animType");
            propAnimMode = serializedObject.FindProperty("animMode");
            propSpeed = serializedObject.FindProperty("speed");
            propFrameList = serializedObject.FindProperty("frameList");

            frameList = new ReorderableList(serializedObject, propFrameList, true, true, true, true);

            frameList.drawHeaderCallback = this.OnPhaseListDrawHeader;
            frameList.drawElementCallback = this.OnPhaseListDrawElement;
            frameList.onAddCallback = this.OnPhaseListAddElement;
            frameList.onRemoveCallback = this.OnPhaseListRemoveElement;
            frameList.onSelectCallback = this.OnPhaseListSelectElement;
            frameList.onChangedCallback = this.OnPhaseListChanged;

            propSelectedFrame = null;
            propSelectedFrameSprites = null;

            currFrame = 0;
            currDirection = 0;
            if (propSpeed.floatValue > 0.0f)
                nextFrameChangeTime = (float)EditorApplication.timeSinceStartup + 1.0f / propSpeed.floatValue;
            else
                nextFrameChangeTime = 0.0f;

            string[] assets = AssetDatabase.FindAssets("edss-8-dir-arrows");
            if (assets.Length > 0)
            {
                arrowsIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(assets[0]));
            }
            else
            {
                arrowsIcon = null;
            }

            EditorApplication.update += Update;

            frameList.index = 0;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        private void Update()
        {
            if (isAnimationPlaying && EditorApplication.timeSinceStartup >= nextFrameChangeTime)
            {
                currFrame++;
                if (currFrame >= propFrameList.arraySize)
                {
                    currFrame -= propFrameList.arraySize;
                }

                if (propSpeed.floatValue > 0.0f)
                    nextFrameChangeTime = (float)EditorApplication.timeSinceStartup + 1.0f / propSpeed.floatValue;
                else
                    nextFrameChangeTime = 0.0f;

            }
        }

        private void InspectorFrameGUI()
        {
            ActorAnimation.AnimDirType animType = (ActorAnimation.AnimDirType) propAnimType.enumValueIndex;

            EditorGUILayout.LabelField("Selected frame: " + string.Format("{0:00}", frameList.index + 1));

            if (animType == ActorAnimation.AnimDirType.EightDirections)
            {
                SerializedProperty spritePropBL = propSelectedFrameSprites.GetArrayElementAtIndex(3);
                SerializedProperty spritePropB = propSelectedFrameSprites.GetArrayElementAtIndex(4);
                SerializedProperty spritePropBR = propSelectedFrameSprites.GetArrayElementAtIndex(5);

                SerializedProperty spritePropL = propSelectedFrameSprites.GetArrayElementAtIndex(2);
                SerializedProperty spritePropR = propSelectedFrameSprites.GetArrayElementAtIndex(6);

                SerializedProperty spritePropFL = propSelectedFrameSprites.GetArrayElementAtIndex(1);
                SerializedProperty spritePropF = propSelectedFrameSprites.GetArrayElementAtIndex(0);
                SerializedProperty spritePropFR = propSelectedFrameSprites.GetArrayElementAtIndex(7);

                Rect r;

                EditorGUILayout.BeginHorizontal();
                r = EditorGUILayout.GetControlRect(GUILayout.Height(64.0f), GUILayout.Width(64.0f));
                spritePropBL.objectReferenceValue = EditorGUI.ObjectField(r, spritePropBL.objectReferenceValue, typeof(Sprite), false);
                r = EditorGUILayout.GetControlRect(GUILayout.Height(64.0f), GUILayout.Width(64.0f));
                spritePropB.objectReferenceValue = EditorGUI.ObjectField(r, spritePropB.objectReferenceValue, typeof(Sprite), false);
                r = EditorGUILayout.GetControlRect(GUILayout.Height(64.0f), GUILayout.Width(64.0f));
                spritePropBR.objectReferenceValue = EditorGUI.ObjectField(r, spritePropBR.objectReferenceValue, typeof(Sprite), false);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                r = EditorGUILayout.GetControlRect(GUILayout.Height(64.0f), GUILayout.Width(64.0f));
                spritePropL.objectReferenceValue = EditorGUI.ObjectField(r, spritePropL.objectReferenceValue, typeof(Sprite), false);
                r = EditorGUILayout.GetControlRect(GUILayout.Height(64.0f), GUILayout.Width(64.0f));
                if (arrowsIcon != null)
                    EditorGUI.DrawTextureTransparent(r, arrowsIcon);
                    //EditorGUI.DrawPreviewTexture(r, m_ArrowsIcon);
                else
                    EditorGUI.DrawRect(r, Color.white);
                r = EditorGUILayout.GetControlRect(GUILayout.Height(64.0f), GUILayout.Width(64.0f));
                spritePropR.objectReferenceValue = EditorGUI.ObjectField(r, spritePropR.objectReferenceValue, typeof(Sprite), false);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                r = EditorGUILayout.GetControlRect(GUILayout.Height(64.0f), GUILayout.Width(64.0f));
                spritePropFL.objectReferenceValue = EditorGUI.ObjectField(r, spritePropFL.objectReferenceValue, typeof(Sprite), false);
                r = EditorGUILayout.GetControlRect(GUILayout.Height(64.0f), GUILayout.Width(64.0f));
                spritePropF.objectReferenceValue = EditorGUI.ObjectField(r, spritePropF.objectReferenceValue, typeof(Sprite), false);
                r = EditorGUILayout.GetControlRect(GUILayout.Height(64.0f), GUILayout.Width(64.0f));
                spritePropFR.objectReferenceValue = EditorGUI.ObjectField(r, spritePropFR.objectReferenceValue, typeof(Sprite), false);
                EditorGUILayout.EndHorizontal();

            }
            else
            {
                SerializedProperty spriteProp = propSelectedFrameSprites.GetArrayElementAtIndex(0);
                Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(64.0f));
                spriteProp.objectReferenceValue = EditorGUI.ObjectField(r, new GUIContent(dirNames[0]), spriteProp.objectReferenceValue, typeof(Sprite), true);
            }
        }

        // Frame list callback for drawing header
        private void OnPhaseListDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Animation Frames");
        }

        // Frame list callback for drawing single row
        private void OnPhaseListDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            //var element = frameList.serializedProperty.GetArrayElementAtIndex(index);
            float frameLength = 1.0f / propSpeed.floatValue;

            rect.y += 2;
            EditorGUI.LabelField(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), string.Format("Frame #{0:00}", index + 1));
            EditorGUI.LabelField(new Rect(rect.x + 100, rect.y, 100, EditorGUIUtility.singleLineHeight), string.Format("len: {0:0.00}s", frameLength));
        }

        // Frame list callback for adding new element
        private void OnPhaseListAddElement(ReorderableList list)
        {
            ActorAnimation animation = target as ActorAnimation;
            animation.FrameList.Add(new ActorFrame());
            if (list.index < 0 )
            {
                list.index = 0;
            }
            list.serializedProperty.serializedObject.UpdateIfRequiredOrScript();
            //EditorUtility.SetDirty(target);
        }

        // Frame list callback for adding new element
        private void OnPhaseListRemoveElement(ReorderableList list)
        {
            ActorAnimation animation = target as ActorAnimation;
            animation.FrameList.RemoveAt(list.index);
            if (list.index >= animation.FrameCount)
            {
                list.index = animation.FrameCount - 1;
            }
            list.serializedProperty.serializedObject.UpdateIfRequiredOrScript();
            //EditorUtility.SetDirty(target);
        }

        // Frame list callback for selcting element
        private void OnPhaseListSelectElement(ReorderableList list)
        {
            propSelectedFrame = list.serializedProperty.GetArrayElementAtIndex(list.index);
            if (propSelectedFrame != null)
            {
                propSelectedFrameSprites = propSelectedFrame.FindPropertyRelative("sprites");
            }
            else
            {
                propSelectedFrameSprites = null;
            }
        }

        // Frame list callback for any changes on the list
        private void OnPhaseListChanged(ReorderableList list)
        {
            if (list.index >= 0)
            {
                propSelectedFrame = list.serializedProperty.GetArrayElementAtIndex(list.index);
                if (propSelectedFrame != null)
                {
                    propSelectedFrameSprites = propSelectedFrame.FindPropertyRelative("sprites");
                }
                else
                {
                    propSelectedFrameSprites = null;
                }
            }
            else
            {
                propSelectedFrame = null;
                propSelectedFrameSprites = null;
            }
        }

    }
}
