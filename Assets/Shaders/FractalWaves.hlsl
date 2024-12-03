
float2 RotateTexCoords(float2 texCoords, float angle)
{
    float s = sin(angle);
    float c = cos(angle);

    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix * 2.0 - 1.0;

    return mul(texCoords, rMatrix);
}

void SumFractalWaves_float(float2 texCoords, float waveCount, float time, float directionality, float endSpeed, float startDirection, float startSpeed, float startLength, float startAmplitude, out float height, out float3 normal)
{
    height = 0;
    normal = float3(0.0, 0.0, 1.0);

    float direction = startDirection * (3.1415926/180.0);
    float speed = startSpeed;
    float waveLength = startLength;
    float amplitude = startAmplitude;

    float deltaSpeed = (endSpeed - startSpeed) / trunc(waveCount);

    for(float i = 0; i < trunc(waveCount); i++)
    {
        float2 waveMovement = RotateTexCoords(texCoords, direction);
        float2 waveAxis = RotateTexCoords(float2(1.0, 0.0), direction) * float2(-1.0, 1.0);
        float k = 6.2831853 / waveLength;
        float input = k * (dot(waveMovement, float2(1.0, 0.0)) + speed.xx * time);
        float wave = amplitude * exp(sin(input) - 1.0);
        float2 waveDerivative = waveAxis * amplitude * exp(sin(input) - 1.0) * cos(input);

        height += wave;
        normal += float3(waveDerivative, 0.0);

        direction += (i * 1.23456) * directionality;
        speed += deltaSpeed;
        waveLength *= 0.9;
        amplitude *= 0.9;
    }

    height /= trunc(waveCount);
    normal /= trunc(waveCount);
}