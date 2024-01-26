using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionList : MonoBehaviour
{
    public static bool IsAppropriateResolution(int width, int height)
    {
        for (int i = 0; i < widths.Length; i++)
        {
            if (widths[i] == width && heights[i] == height)
            {
                return true;
            }
        }
        return false;
    }

    public static int GetAppropriateHeight(int width)
    {
        for (int i = 0; i < widths.Length; i++)
        {
            if (widths[i] == width)
            {
                return heights[i];
            }
        }
        return 0;
    }

    public static int[] widths = new int[] {640,
                                            800,
                                            864,
                                            960,
                                            1024,
                                            1152,
                                            1280,
                                            1366,
                                            1600,
                                            1920,
                                            2048,
                                            2560,
                                            2880,
                                            3200,
                                            3840,
                                            4096,
                                            5120,
                                            7680,
                                            15360};

    public static int[] heights = new int[] {360,
                                            450,
                                            486,
                                            540,
                                            576,
                                            648,
                                            720,
                                            768,
                                            900,
                                            1080,
                                            1152,
                                            1440,
                                            1620,
                                            1800,
                                            2160,
                                            2304,
                                            2880,
                                            4320,
                                            8640};
}
