inline float2 voronoi_noise_randomVector (float2 UV, float offset){
    float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
    UV = frac(sin(mul(UV, m)) * 46839.32);
    return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
}

void VoronoiPrecise2D_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells) {
    float2 g = floor(UV * CellDensity);
    float2 f = frac(UV * CellDensity);
    float2 res = float2(8.0, 8.0);
    float2 ml = float2(0,0);
    float2 mv = float2(0,0);
 
    for(int y=-1; y<=1; y++){
        for(int x=-1; x<=1; x++){
            float2 lattice = float2(x, y);
            float2 offset = voronoi_noise_randomVector(g + lattice, AngleOffset);
            float2 v = lattice + offset - f;
            float d = dot(v, v);
 
            if(d < res.x){
                res.x = d;
                res.y = offset.x;
                mv = v;
                ml = lattice;
            }
        }
    }
 
    Cells = res.y;
 
    res = float2(8.0, 8.0);
    for(int y1=-2; y1<=2; y1++){
        for(int x1=-2; x1<=2; x1++){
            float2 lattice = ml + float2(x1, y1);
            float2 offset = voronoi_noise_randomVector(g + lattice, AngleOffset);
            float2 v = lattice + offset - f;
 
            float2 cellDifference = abs(ml - lattice);
            if (cellDifference.x + cellDifference.y > 0.1){
                float d = dot(0.5*(mv+v), normalize(v-mv));
                res.x = min(res.x, d);
            }
        }
    }
 
    Out = res.x;
}

void Voronoi2D_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells) {
    float2 g = floor(UV * CellDensity);
    float2 f = frac(UV * CellDensity);
    float3 res = float3(8.0, 8.0, 8.0);
 
    for(int y=-1; y<=1; y++){
        for(int x=-1; x<=1; x++){
            float2 lattice = float2(x, y);
            float2 offset = voronoi_noise_randomVector(g + lattice, AngleOffset);
            float2 v = lattice + offset - f;
            float d = dot(v, v);
             
            if(d < res.x){
                res.y = res.x;
                res.x = d;
                res.z = offset.x;
            }else if (d < res.y){
                res.y = d;
            }
        }
    }
 
    Out = res.x;
    Cells = res.z;
}

float rand(float2 seed)
{
    return frac(sin(dot(seed, float2(12.9898,78.233))) * 43758.5453);
}

float mix(float a, float b, float x)
{
    return b * x + a * (1.0-x);
}

void Voronoi2DBlender_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
{
    float2 cellPosition = floor(UV * CellDensity); //Truncate coordinates w/ density
    float2 localPosition = frac(UV * CellDensity); //Get Position within [0;1] w/ cell density

    float3 output = float3(8.0,8.0,8.0); //A big buffer value of the largest value allowed, idk why 8

    for (int y = -1; y <= 1; y++) //Create a bunch of points that will be offseted to form the cells
    {
        for (int x = -1; x <=1; x++) 
        {
            float2 cellOffset = float2(x,y);
            float2 pointPosition = cellOffset + voronoi_noise_randomVector(cellPosition + cellOffset, AngleOffset); //Randomly offset the original points w/ AngleOffset
            float distanceToPoint = distance(pointPosition, localPosition); //get distance between current local coordinates and point w/offset
            //float minDistance = min(output.x, distanceToPoint); //keep only the minimum
            //float2 tempPosition = cellOffset + pointPosition - localPosition;
            //float distanceToPoint = dot(tempPosition,tempPosition);

            if (distanceToPoint < output.x) //save the absolute minimum distance of the generated points
            {
                output.y = output.x; //keep the last maximum
                output.x = distanceToPoint; //change the current minimum
                output.z = pointPosition.x; //save the cell value
            }
            else if (distanceToPoint < output.y) //update the last maximum
            {
                output.y = distanceToPoint;
            }
        }
    }
    Out = output.x;
    Cells = output.z;
}

void Voronoi2DBlenderSmooth_float(float2 UV, float AngleOffset, float CellDensity, float Smoothness, out float Out, out float Cells)
{
    float2 cellPosition = floor(UV * CellDensity);
    float2 localPosition = frac(UV * CellDensity);

    float3 output = float3(8.0,8.0,8.0);
    
    for (int y = -2; y <= 2; y++)
    {
        for (int x = -2; x <=2; x++)
        {
            float2 cellOffset = float2(x,y);
            float2 pointPosition = cellOffset + voronoi_noise_randomVector(cellPosition + cellOffset, AngleOffset);
            float distanceToPoint = distance(pointPosition, localPosition);

            output.y = output.x; //Save the previous value
            
            float h = smoothstep(0.0, 1.0, 0.5 + 0.5 * (output.x - distanceToPoint) / Smoothness); //Cubic hermite spline
            output.x = mix(output.x, distanceToPoint, h) - Smoothness * h * (1.0-h); // Interpolate between the two values

            float hc = smoothstep(0.0, 1.0, 0.5 + 0.5 * (output.z - pointPosition.x) / Smoothness); // trying to interpolate the cells, currently not working
            output.z = mix(output.z, pointPosition.x, hc) - Smoothness * hc * (1.0-hc);
        }
    }
    Out = output.x;
    Cells = output.z;
}

void Voronoi2DSmooth_float(float2 UV, float AngleOffset, float CellDensity, float Smoothness, out float Out, out float Cells)
{
    float2 cellPosition = floor(UV * CellDensity);
    float2 localPosition = frac(UV * CellDensity);

    float3 output = float3(8.0,8.0,8.0);
    
    for (int y = -2; y <= 2; y++)
    {
        for (int x = -2; x <=2; x++)
        {
            float2 cellOffset = float2(x,y);
            float2 pointPosition = voronoi_noise_randomVector(cellPosition + cellOffset, AngleOffset);
            float2 tempPosition = cellOffset + pointPosition - localPosition;
            float distanceToPoint = dot(tempPosition, tempPosition);

            output.y = output.x; //Save the previous value
            
            float h = smoothstep(0.0, 1.0, 0.5 + 0.5 * (output.x - distanceToPoint) / Smoothness); //Cubic hermite spline
            output.x = mix(output.x, distanceToPoint, h) - Smoothness * h * (1.0-h); // Interpolate between the two values

            float hc = smoothstep(0.0, 1.0, 0.5 + 0.5 * (output.z - pointPosition.x) / Smoothness); // trying to interpolate the cells, currently not working
            output.z = mix(output.z, pointPosition.x, hc) - Smoothness * hc * (1.0-hc);
        }
    }
    Out = output.x;
    Cells = output.z;
}

