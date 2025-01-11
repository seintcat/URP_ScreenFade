using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine
{
    public static class ScreenFade
    {
        private const int rendererDataIndex = 0;

        public static bool playing { get { return coroutine != null; } }
        public static bool shutdown { get; private set; }

        private static Material _fadeMaterial;
        public static Material fadeMaterial
        {
            get
            {
                if (_fadeMaterial == null)
                {
                    Initialize();
                }
                return _fadeMaterial;
            }
        }

        private static Coroutine coroutine;

        public static void Initialize()
        {
            shutdown = true;
            ReadOnlySpan<ScriptableRendererData> list = UniversalRenderPipeline.asset.rendererDataList;
            if (list.Length > rendererDataIndex && list[rendererDataIndex] != null)
            {
                ScriptableRendererFeature targetFeature = list[rendererDataIndex].rendererFeatures.Find(
                    (feature) =>
                    {
                        return feature.name == "ScreenFade";
                    });
                if (((FullScreenPassRendererFeature)targetFeature) != null)
                {
                    _fadeMaterial = ((FullScreenPassRendererFeature)targetFeature).passMaterial;
                    shutdown = false;
                }
                else
                {
                    Debug.LogError("Setting reset failed(RendererFeature type error, check Quality setting and make sure WebGL2.0)");
                }
            }
            else
            {
                Debug.LogError("Setting reset failed(Can't find RendererData)");
            }
        }

        public static Coroutine FadeIn(this MonoBehaviour component, float fadeDuration)
        {
            if(shutdown)
            {
                return null;
            }

            if (coroutine != null)
            {
                component.StopCoroutine(coroutine);
            }
            coroutine = component.StartCoroutine(Fade(1, 0, fadeDuration));
            return coroutine;
        }
        public static Coroutine FadeOut(this MonoBehaviour component, float fadeDuration)
        {
            if (shutdown)
            {
                return null;
            }

            if (coroutine != null)
            {
                component.StopCoroutine(coroutine);
            }
            coroutine = component.StartCoroutine(Fade(0, 1, fadeDuration));
            return coroutine;
        }
        private static IEnumerator Fade(float startValue, float endValue, float fadeDuration)
        {
            if(!shutdown)
            {
                float elapsed = 0;

                while (elapsed < fadeDuration)
                {
                    elapsed += Time.deltaTime;
                    fadeMaterial.SetFloat("_Fade", Mathf.Lerp(startValue, endValue, elapsed / fadeDuration));
                    yield return null;
                }

                fadeMaterial.SetFloat("_Fade", endValue);
                coroutine = null;
            }
        }

        public static void FadeReset(this MonoBehaviour component)
        {
            if (!shutdown)
            {
                fadeMaterial.SetFloat("_Fade", 0);
            }
        }
    }
}
