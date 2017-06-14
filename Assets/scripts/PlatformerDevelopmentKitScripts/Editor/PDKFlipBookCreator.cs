using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PDKFlipBookCreator : EditorWindow
{
    // TODO: Comment this later
    private static PDKEditorUtil editorUtil;
    private static string animName;
    private static string outputPath;
    private static Texture2D[] frames;
    private static Sprite[] framesSprites;
    private static int frameRate = 4;
    private static bool shouldLoop = true;
    private Vector2 framesScrollPosition = Vector2.zero;
    private static PDKFlipBookCreator window;

    // TODO: Comment this later
    private void OnEnable()
    {
        editorUtil = ScriptableObject.CreateInstance<PDKEditorUtil>();
    }


    // This will overide unity's standard GUI
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        // TODO: Comment this later
        GUILayout.Label("Create Flipbook Animation", EditorStyles.boldLabel);
        // TODO: Comment this later
        editorUtil.Field("Animation Name:", ref animName);
        // TODO: Comment this later
        editorUtil.Field("Frame Rate:", ref frameRate);
        framesScrollPosition = EditorGUILayout.BeginScrollView(framesScrollPosition, GUILayout.Height(140));
        for (int indexOfSpriteToDisplay = 0; indexOfSpriteToDisplay < frames.Length; indexOfSpriteToDisplay++)
        {
            // TODO: Comment this later
            editorUtil.Field("Frame " + indexOfSpriteToDisplay + ":");
            // TODO: Comment this later
            editorUtil.Field(null, ref frames[indexOfSpriteToDisplay]);
        }
        EditorGUILayout.EndScrollView();

        // TODO: Comment this later
        editorUtil.Field("Output Location:", ref outputPath);
        // TODO: Comment this later
        editorUtil.Field("Loop Animation:", ref shouldLoop);
        // TODO: Comment this later
        if (editorUtil.Button("Create Animation"))
        {
            CreateFilpbookAnim();
            window.Close();
        }
        EditorGUILayout.EndVertical();
    }

    [UnityEditor.MenuItem("Assets/Create Flipbook Animation(PDK)")]
    public static void OpenWindow()
    {
        //Texture2D[] textures = Selection.GetFiltered(typeof(object), SelectionMode.DeepAssets) as Texture2D[];
        object[] ssprites = Selection.GetFiltered(typeof(SparseTexture), SelectionMode.DeepAssets);
        object[] objects = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
        frames = new Texture2D[objects.Length];
        framesSprites = new Sprite[objects.Length];
        for (int i = 0; i < objects.Length; i++)
        {
            frames[i] = (Texture2D)objects[i];
            framesSprites[i] = Sprite.Create(frames[i], new Rect(0, 0, frames[i].width, frames[i].height), new Vector2(0.5f, 0.5f));
        }
        // Get existing open window or if none, make a new one:
        window = (PDKFlipBookCreator)EditorWindow.GetWindow(typeof(PDKFlipBookCreator));
        window.Show();
    }


    private void GetSelectedTextures()
    {
        //Texture2D[] textures = Selection.GetFiltered(typeof(object), SelectionMode.DeepAssets) as Texture2D[];
        //Sprite[] ssprites = Selection.GetFiltered(typeof(Sprite), SelectionMode.DeepAssets) as Sprite[];
        object[] objects = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);
        for (int i = 0; i < objects.Length; i++)
        {
            frames[i] = (Texture2D)objects[i];
        }
    }


    private void CreateFilpbookAnim()
    {
        AnimationClip newFlipbook = new AnimationClip();
        newFlipbook.frameRate = frameRate;
        EditorCurveBinding spriteBinding = new EditorCurveBinding();

        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";
        ObjectReferenceKeyframe[] spriteKeyFrames = new ObjectReferenceKeyframe[frames.Length];
        for (int i = 0; i < (frames.Length); i++)
        {
            SpriteRenderer spriteRenderer = GameObject.Find("Main Camera").GetComponent<SpriteRenderer>();
            Sprite newSprite = Sprite.Create(frames[i], new Rect(0, 0, frames[i].width, frames[i].height), new Vector2(0.5f, 0.5f));
            spriteKeyFrames[i] = new ObjectReferenceKeyframe();
            spriteKeyFrames[i].time = i;
            spriteKeyFrames[i].value = newSprite;
            spriteRenderer.sprite = (Sprite)spriteKeyFrames[i].value;
            Debug.Log(newSprite);
        }
        if (shouldLoop)
        {
            newFlipbook.wrapMode = WrapMode.Loop;
        }
        else
        {
            newFlipbook.wrapMode = WrapMode.Default;
        }
        AnimationUtility.SetObjectReferenceCurve(newFlipbook, spriteBinding, spriteKeyFrames);
        Resources.UnloadUnusedAssets();
        Debug.Log(outputPath + "\\" + animName + ".anim");
        AssetDatabase.CreateAsset(newFlipbook, outputPath + "\\" + animName + ".anim");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
