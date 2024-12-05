using UnityEngine;

public static class Perlin3D
{
    const int PermutationCount = 255;

    static readonly int[] permutations =
    {
        151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225,
        140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148,
        247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
        57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175,
        74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
        60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
        65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
        200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64,
        52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212,
        207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
        119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
        129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104,
        218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,
        81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157,
        184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
        222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,

        151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225,
        140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148,
        247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
        57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175,
        74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
        60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
        65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
        200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64,
        52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212,
        207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
        119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
        129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104,
        218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,
        81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157,
        184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
        222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
    };

    const int DirectionCount = 15;

    static readonly Vector3[] directions =
    {
        new(1f, 1f, 0f),
        new(-1f, 1f, 0f),
        new(1f, -1f, 0f),
        new(-1f, -1f, 0f),
        new(1f, 0f, 1f),
        new(-1f, 0f, 1f),
        new(1f, 0f, -1f),
        new(-1f, 0f, -1f),
        new(0f, 1f, 1f),
        new(0f, -1f, 1f),
        new(0f, 1f, -1f),
        new(0f, -1f, -1f),

        new(1f, 1f, 0f),
        new(-1f, 1f, 0f),
        new(0f, -1f, 1f),
        new(0f, -1f, -1f)
    };


    static float Scalar(Vector3 a, float bX, float bY, float bZ)
    {
        return a.x * bX + a.y * bY + a.z * bZ;
    }

    static float SmoothDistance(float d)
    {
        return d * d * d * (d * (d * 6f - 15f) + 10f);
    }

    // float direction000X;
    // float direction100X;
    // float direction010X;
    // float direction110X;
    // float direction001X;
    // float direction101X;
    // float direction011X;
    // float direction111X;
    //
    // float direction000Y;
    // float direction100Y;
    // float direction010Y;
    // float direction110Y;
    // float direction001Y;
    // float direction101Y;
    // float direction011Y;
    // float direction111Y;
    //
    // float direction000Z;
    // float direction100Z;
    // float direction010Z;
    // float direction110Z;
    // float direction001Z;
    // float direction101Z;
    // float direction011Z;
    // float direction111Z;

    // static int flooredPointX0;
    // static int flooredPointY0;
    // static int flooredPointZ0;
    //
    // static float distanceX0;
    // static float distanceY0;
    // static float distanceZ0;
    // static float distanceX1;
    // static float distanceY1;
    // static float distanceZ1;
    //
    // static int flooredPointX1;
    // static int flooredPointY1;
    // static int flooredPointZ1;
    //
    // static int permutationX0;
    // static int permutationX1;
    // static int permutationY00;
    // static int permutationY10;
    // static int permutationY01;
    // static int permutationY11;
    //
    // static Vector3 direction000;
    // static Vector3 direction100;
    // static Vector3 direction010;
    // static Vector3 direction110;
    // static Vector3 direction001;
    // static Vector3 direction101;
    // static Vector3 direction011;
    // static Vector3 direction111;
    //
    // static float value000;
    // static float value100;
    // static float value010;
    // static float value110;
    // static float value001;
    // static float value101;
    // static float value011;
    // static float value111;
    //
    // static float smoothDistanceX;
    // static float smoothDistanceY;
    // static float smoothDistanceZ;
    //
    // static float pointX;
    // static float pointY;
    // static float pointZ;

    public static float Get3DPerlinNoise(float x, float y, float z, float frequency)
    {
        x *= frequency;
        y *= frequency;
        z *= frequency;

        int flooredPointX0 = Mathf.FloorToInt(x);
        int flooredPointY0 = Mathf.FloorToInt(y);
        int flooredPointZ0 = Mathf.FloorToInt(z);

        float distanceX0 = x - flooredPointX0;
        float distanceY0 = y - flooredPointY0;
        float distanceZ0 = z - flooredPointZ0;

        float distanceX1 = distanceX0 - 1f;
        float distanceY1 = distanceY0 - 1f;
        float distanceZ1 = distanceZ0 - 1f;

        flooredPointX0 &= PermutationCount;
        flooredPointY0 &= PermutationCount;
        flooredPointZ0 &= PermutationCount;

        int flooredPointX1 = flooredPointX0 + 1;
        int flooredPointY1 = flooredPointY0 + 1;
        int flooredPointZ1 = flooredPointZ0 + 1;

        int permutationX0 = permutations[flooredPointX0];
        int permutationX1 = permutations[flooredPointX1];

        int permutationY00 = permutations[permutationX0 + flooredPointY0];
        int permutationY10 = permutations[permutationX1 + flooredPointY0];
        int permutationY01 = permutations[permutationX0 + flooredPointY1];
        int permutationY11 = permutations[permutationX1 + flooredPointY1];

        Vector3 direction000 = directions[permutations[permutationY00 + flooredPointZ0] & DirectionCount];
        Vector3 direction100 = directions[permutations[permutationY10 + flooredPointZ0] & DirectionCount];
        Vector3 direction010 = directions[permutations[permutationY01 + flooredPointZ0] & DirectionCount];
        Vector3 direction110 = directions[permutations[permutationY11 + flooredPointZ0] & DirectionCount];
        Vector3 direction001 = directions[permutations[permutationY00 + flooredPointZ1] & DirectionCount];
        Vector3 direction101 = directions[permutations[permutationY10 + flooredPointZ1] & DirectionCount];
        Vector3 direction011 = directions[permutations[permutationY01 + flooredPointZ1] & DirectionCount];
        Vector3 direction111 = directions[permutations[permutationY11 + flooredPointZ1] & DirectionCount];

        float value000 = Scalar(direction000, distanceX0, distanceY0, distanceZ0);
        float value100 = Scalar(direction100, distanceX1, distanceY0, distanceZ0);
        float value010 = Scalar(direction010, distanceX0, distanceY1, distanceZ0);
        float value110 = Scalar(direction110, distanceX1, distanceY1, distanceZ0);
        float value001 = Scalar(direction001, distanceX0, distanceY0, distanceZ1);
        float value101 = Scalar(direction101, distanceX1, distanceY0, distanceZ1);
        float value011 = Scalar(direction011, distanceX0, distanceY1, distanceZ1);
        float value111 = Scalar(direction111, distanceX1, distanceY1, distanceZ1);

        float smoothDistanceX = SmoothDistance(distanceX0);
        float smoothDistanceY = SmoothDistance(distanceY0);
        float smoothDistanceZ = SmoothDistance(distanceZ0);

        return Lerp(
            Lerp(Lerp(value000, value100, smoothDistanceX), Lerp(value010, value110, smoothDistanceX), smoothDistanceY),
            Lerp(Lerp(value001, value101, smoothDistanceX), Lerp(value011, value111, smoothDistanceX), smoothDistanceY),
            smoothDistanceZ);
    }

    static float Clamp(float value)
    {
        if (value < 0.0) return 0.0f;
        return value > 1.0 ? 1f : value;
    }

    static float Lerp(float a, float b, float t) => a + (b - a) * Clamp(t);
}