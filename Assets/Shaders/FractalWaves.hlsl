
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

void SumFractalWaves_float(float2 texCoords, float waveCount, float time, float directionality, float startDirection, float startSpeed, float startLength, float startAmplitude, out float height, out float3 normal)
{
    height = 0;
    normal = float3(0.0, 0.0, 0.0);

    float direction = startDirection * (3.1415926/180.0);
    float directionChange = directionality * (3.1415926/180.0);
    float speed = startSpeed;
    float waveLength = startLength;
    float amplitude = startAmplitude;

    for(float i = 0; i < waveCount; i++)
    {
        float2 waveAxis = RotateTexCoords(texCoords, direction);
        float k = 6.2831853 / waveLength;
        float input = k * (float2(dot(waveAxis, float2(1.0, 0.0)), dot(waveAxis, float2(0.0, 1.0))) + speed.xx * time);
        float wave = amplitude * exp(sin(input.x) - 1.0);
        float2 waveDerivative = normalize(waveAxis) * amplitude * exp(cos(input) - (1.0).xx);

        height += wave;
        normal += float3(waveDerivative.x, waveDerivative.y, sqrt(1.0 - dot(waveDerivative, waveDerivative)));

        direction += directionChange * (i * ((i % 2) * 2.0 - 1.0));
        speed *= 1.1;
        waveLength *= 0.9;
        amplitude *= 0.9;
    }

    normal = normalize(normal);
}