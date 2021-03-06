precision mediump float;

varying mediump vec2 v_texcoord0;
uniform sampler2D aTexture0;

uniform mediump float radius;

uniform mediump float aspectRatio;

uniform int arraySize;
uniform float leftContourPoints[16];
uniform float rightContourPoints[16];
uniform float deltaArray[8];

uniform float left2ndContourPoints[8];
uniform float right2ndContourPoints[8];
uniform float deltaShrinkArray[4];

uniform float maxX;
uniform float minX;
uniform float maxY;
uniform float minY;

uniform float maxShrinkX;
uniform float minShrinkX;
uniform float maxShrinkY;
uniform float minShrinkY;

uniform int isSmall;

mediump vec2 warpPositionToUse(vec2 currentPoint, vec2 contourPointA,  vec2 contourPointB, float radius, float delta, float aspectRatio,int isSmall)
{
    vec2 positionToUse = currentPoint;
    vec2 currentPointToUse = vec2(currentPoint.x, currentPoint.y * aspectRatio + 0.5 - 0.5 * aspectRatio);
    vec2 contourPointAToUse = vec2(contourPointA.x, contourPointA.y * aspectRatio + 0.5 - 0.5 * aspectRatio);

    float r = distance(currentPointToUse, contourPointAToUse);
    if(r < radius)
    {
        vec2 dir = 1.0 * normalize(contourPointB - contourPointA);
        if(isSmall == 0)
        dir = -dir;
        float dist = radius * radius - r * r;
        float alpha = dist / (dist + (r-delta) * (r-delta));
        alpha = alpha * alpha;
        positionToUse = positionToUse - alpha * delta * dir ;
    }
    return positionToUse;
}

void main()
{
    vec2 positionToUse = v_texcoord0;
    if(positionToUse.x < maxX && positionToUse.x > minX && positionToUse.y < maxY && positionToUse.y > minY)
    {
        for(int i = 0; i < 8; i++)
        {
            positionToUse = warpPositionToUse(positionToUse, vec2(rightContourPoints[i * 2], rightContourPoints[i * 2 + 1]), vec2(leftContourPoints[i * 2], leftContourPoints[i * 2 + 1]), radius, deltaArray[i], aspectRatio,isSmall);
            positionToUse = warpPositionToUse(positionToUse, vec2(leftContourPoints[i * 2], leftContourPoints[i * 2 + 1]), vec2(rightContourPoints[i * 2], rightContourPoints[i * 2 + 1]), radius, deltaArray[i], aspectRatio,isSmall);
        }
    }
    if(positionToUse.x < maxShrinkX && positionToUse.x > minShrinkX && positionToUse.y < maxShrinkY && positionToUse.y > minShrinkY)
    {
        for(int i = 0; i < 4; i++)
        {
              positionToUse = warpPositionToUse(positionToUse, vec2(right2ndContourPoints[i * 2], right2ndContourPoints[i * 2 + 1]), vec2(left2ndContourPoints[i * 2], left2ndContourPoints[i * 2 + 1]), radius, deltaShrinkArray[i], aspectRatio,isSmall);
            positionToUse = warpPositionToUse(positionToUse, vec2(left2ndContourPoints[i * 2], left2ndContourPoints[i * 2 + 1]), vec2(right2ndContourPoints[i * 2], right2ndContourPoints[i * 2 + 1]), radius, deltaShrinkArray[i], aspectRatio,isSmall);
        }

    }
    gl_FragColor = texture2D(aTexture0, positionToUse);
}
